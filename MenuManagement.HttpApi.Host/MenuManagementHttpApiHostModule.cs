using MenuManagement.Application;
using MenuManagement.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace MenuManagement.HttpApi.Host;

[DependsOn(
    typeof(MenuManagementHttpApiModule),
    typeof(MenuManagementApplicationModule),
    typeof(MenuManagementEntityFrameworkCoreModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAutofacModule)
)]
public class MenuManagementHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(MenuManagementHttpApiModule).Assembly);
        });

        ConfigureAuthentication(context, configuration);
        ConfigureSwaggerServices(context, configuration);
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        // 使用 SecretKey 验证 token（与 TokenService 生成的 token 格式匹配）
        // 注意：SecretKey必须与token来源项目完全一致
        // token来源项目配置的SecretKey: "HxAbpGeo_DefaultSecretKey_For_JWT_Token_Generation_Must_Be_At_Least_32_Characters_Long"
        // token来源项目配置的Authority: "https://localhost:44307" (但实际运行在44359端口)
        var secretKey = configuration["AuthServer:SecretKey"] 
            ?? "HxAbpGeo_DefaultSecretKey_For_JWT_Token_Generation_Must_Be_At_Least_32_Characters_Long";
        var issuer = configuration["AuthServer:Authority"] 
            ?? "https://localhost:44307";
        var audience = configuration["AuthServer:Audience"] 
            ?? "HxAbpGeo";
        
        // 记录配置信息（用于调试）- 显示SecretKey的前10个字符和长度，不显示完整值
        var logger = context.Services.BuildServiceProvider()
            .GetRequiredService<ILogger<MenuManagementHttpApiHostModule>>();
        var secretKeyPreview = secretKey?.Length > 10 
            ? secretKey.Substring(0, 10) + "..." 
            : secretKey;
        logger.LogInformation("JWT认证配置 - Issuer: {Issuer}, Audience: {Audience}, SecretKey预览: {SecretKeyPreview}, SecretKey长度: {SecretKeyLength}", 
            issuer, audience, secretKeyPreview, secretKey?.Length ?? 0);
        
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // 构建可能的Issuer列表（兼容http和https，以及不同的端口）
                // token来源项目配置的Authority是44307，但实际运行在44359端口
                var possibleIssuers = new List<string> { issuer };
                
                // 添加http和https版本
                if (issuer.StartsWith("http://"))
                {
                    possibleIssuers.Add(issuer.Replace("http://", "https://"));
                }
                else if (issuer.StartsWith("https://"))
                {
                    possibleIssuers.Add(issuer.Replace("https://", "http://"));
                }
                
                // 添加token来源项目的配置值（44307）和实际运行端口（44359）
                possibleIssuers.Add("https://localhost:44307");
                possibleIssuers.Add("http://localhost:44307");
                possibleIssuers.Add("https://localhost:44359");
                possibleIssuers.Add("http://localhost:44359");
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    // 支持多个可能的Issuer值（兼容http和https，以及不同的端口配置）
                    ValidIssuers = possibleIssuers.Distinct().ToArray(),
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) // 允许5分钟的时间差异，避免时间同步问题
                };
                
                // 配置事件处理（用于调试和日志记录）
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = authContext =>
                    {
                        var eventLogger = authContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MenuManagementHttpApiHostModule>>();
                        
                        var exception = authContext.Exception;
                        eventLogger.LogError(exception, 
                            "JWT 认证失败 - 错误类型: {ExceptionType}, 消息: {Message}, Token长度: {TokenLength}",
                            exception.GetType().Name,
                            exception.Message,
                            authContext.Request.Headers["Authorization"].ToString().Length);
                        
                        // 如果是签名验证失败，记录更详细的信息
                        if (exception is SecurityTokenSignatureKeyNotFoundException)
                        {
                            var config = authContext.HttpContext.RequestServices
                                .GetRequiredService<IConfiguration>();
                            var currentSecretKey = config["AuthServer:SecretKey"] 
                                ?? "HxAbpGeo_DefaultSecretKey_For_JWT_Token_Generation";
                            var secretKeyPreview = currentSecretKey.Length > 20 
                                ? currentSecretKey.Substring(0, 20) + "..." 
                                : currentSecretKey;
                            eventLogger.LogError("签名验证失败 - 当前使用的SecretKey预览: {SecretKeyPreview}, 长度: {SecretKeyLength}", 
                                secretKeyPreview, currentSecretKey.Length);
                            eventLogger.LogError("请检查token来源项目(C:\\work\\docs\\lincao\\成果文件\\源代码\\后端\\src\\abpvnext)的appsettings.json中AuthServer:SecretKey的值");
                            eventLogger.LogError("TokenService生成token时默认使用: 'HxAbpGeo_DefaultSecretKey_For_JWT_Token_Generation'");
                            eventLogger.LogError("如果token来源项目没有配置SecretKey，请确保当前项目也使用相同的默认值");
                        }
                        else if (exception is SecurityTokenInvalidIssuerException)
                        {
                            eventLogger.LogError("Issuer验证失败 - 期望: {ExpectedIssuers}, 请检查token中的iss声明",
                                string.Join(", ", options.TokenValidationParameters.ValidIssuers ?? new[] { issuer }));
                        }
                        else if (exception is SecurityTokenInvalidAudienceException)
                        {
                            eventLogger.LogError("Audience验证失败 - 期望: {ExpectedAudience}, 请检查token中的aud声明",
                                audience);
                        }
                        else if (exception is SecurityTokenExpiredException)
                        {
                            eventLogger.LogWarning("Token已过期");
                        }
                        
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = validatedContext =>
                    {
                        var eventLogger = validatedContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MenuManagementHttpApiHostModule>>();
                        
                        var userId = validatedContext.Principal?.FindFirst(Volo.Abp.Security.Claims.AbpClaimTypes.UserId)?.Value;
                        var userName = validatedContext.Principal?.FindFirst(Volo.Abp.Security.Claims.AbpClaimTypes.UserName)?.Value;
                        var issuer = validatedContext.Principal?.FindFirst("iss")?.Value;
                        var audience = validatedContext.Principal?.FindFirst("aud")?.Value;
                        
                        eventLogger.LogInformation("JWT Token 验证成功 - UserId: {UserId}, UserName: {UserName}, Issuer: {Issuer}, Audience: {Audience}",
                            userId, userName, issuer, audience);
                        return Task.CompletedTask;
                    },
                    OnChallenge = challengeContext =>
                    {
                        var eventLogger = challengeContext.HttpContext.RequestServices
                            .GetRequiredService<ILogger<MenuManagementHttpApiHostModule>>();
                        eventLogger.LogWarning("JWT 认证挑战 - 错误: {Error}, 错误描述: {ErrorDescription}",
                            challengeContext.Error, challengeContext.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"] ?? throw new InvalidOperationException("AuthServer:Authority is not configured"),
            new Dictionary<string, string>
            {
                { "MenuManagement", "MenuManagement API" }
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "菜单管理 API",
                    Version = "v1",
                    Description = "菜单管理系统 API 文档，提供菜单的创建、查询、更新、删除以及菜单与角色、组织的关联管理功能。",
                    Contact = new OpenApiContact
                    {
                        Name = "菜单管理系统",
                        Email = "support@menumanagement.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License"
                    }
                });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                
                // 启用 XML 注释
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpClaimsMap();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "MenuManagement API");
            var configuration = context.GetConfiguration();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:ClientSecret"]);
            options.OAuthScopes("MenuManagement");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }
}
