using System.Text;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.Generator;
using SummerBootAdmin.Dto;
using SummerBootAdmin.Dto.Hangfire;
using SummerBootAdmin.Dto.Login;
using SummerBootAdmin.Model;
using SummerBootAdmin.Model.Department;
using SummerBootAdmin.Model.Dictionary;
using SummerBootAdmin.Model.Menu;
using SummerBootAdmin.Model.Role;
using SummerBootAdmin.Model.User;
using SummerBootAdmin.Repository.Department;
using SummerBootAdmin.Repository.Role;
using SummerBootAdmin.Repository.User;
using SummerBootAdmin.Service;

namespace SummerBootAdmin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;
            var url = configuration.GetValue<string>("url");
            builder.WebHost.UseUrls(url);
            builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson(it =>
            {
                //it.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                it.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                it.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
            builder.Services.AddSummerBootMvcExtension(it =>
            {
                //�Ƿ�����ȫ�ִ�����
                it.UseGlobalExceptionHandle = true;
                //�Ƿ����ò���У�鴦��
                it.UseValidateParameterHandle = true;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT授权token前面需要加上字段Bearer与一个空格,如Bearer token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

            });
            builder.Services.AddAutoMapper(it =>
            {
                it.AddProfile<SummerbootProfile>();
            });

            builder.Services.AddCors(it =>
                it.AddPolicy("all", x => x.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials()));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSummerBoot();
            builder.Services.AddSummerBootRepository(it =>
            {
                var connectionString = configuration.GetValue<string>("mysqlDbConnectionString");
                it.AddDatabaseUnit<MySqlConnection, IUnitOfWork1>(connectionString, x =>
                {
                    //ͨ���Զ���ע�������󶨲ִ�
                    x.BindRepositoriesWithAttribute<AutoRepository1Attribute>();

                    //�����ݿ����ɽӿ�
                    x.BindDbGeneratorType<IDbGenerator1>();
                    //实体类在插入或更新前进行预处理，比如给创建人,更新人,创建时间，更新时间赋值
                    x.BindEntityClassHandlerType(typeof(MyEntityClassHandler));
                });
            });
            var redisDbString = configuration.GetValue<string>("redisDbString");
            builder.Services.AddSummerBootCache(x => x.UseRedis(redisDbString));
            builder.Services.AddHangfire(x => x.UseRedisStorage(redisDbString, new RedisStorageOptions()
            {
                Db = 8
            }));
            //添加hangfire认证
            builder.Services.AddSingleton<IDashboardAuthorizationFilter, HangfireAuthorizationFilter>();

            // 添加认证服务
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.GetValue<string>("jwtDomain"),
                        ValidAudience = configuration.GetValue<string>("jwtDomain"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("jwtSecurityKey")))
                    };
                });

            builder.Services.AddAuthorization(x => x.AddPolicy("urlPolicy", policy => policy.Requirements.Add(new SummerBootRequirement())));
            builder.Services.AddTransient<IAuthorizationHandler, SummerBootAuthorizationHandler>();
            var app = builder.Build();
            app.UseCors("all");
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            //hangfire控制仪表盘的访问路径和授权配置
            var hanfireDashboardAuthorizationFilters = app.Services.GetService<IDashboardAuthorizationFilter>();
            app.UseHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>() { hanfireDashboardAuthorizationFilters }
            });
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
            app.UseHangfireServer(new BackgroundJobServerOptions()
            {
                Queues = new[] { env }
            });

            await InitDatabase(app.Services);

            app.Run();
        }

        /// <summary>
        /// Initialize the database
        /// 初始化数据库
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private static async Task InitDatabase(IServiceProvider service)
        {
            var scopeServiceProvider = service.CreateScope().ServiceProvider;
            var dbGenerator = scopeServiceProvider.GetService<IDbGenerator1>();

            if (dbGenerator.GetAllTableNames().Count == 0)
            {
                var sqls = dbGenerator.GenerateSql(new List<Type>()
                {
                    typeof(Department), typeof(Dictionary), typeof(DictionaryItem),
                    typeof(Role), typeof(RoleAssignMenu),typeof(UserRole),
                    typeof(User), typeof(MenuMeta), typeof(Menu),typeof(MenuApiMapping)
                });
                foreach (var generateDatabaseSqlResult in sqls)
                {
                    dbGenerator.ExecuteGenerateSql(generateDatabaseSqlResult);
                }
            }

            var userRepository = scopeServiceProvider.GetService<IUserRepository>();
            var roleRepository = scopeServiceProvider.GetService<IRoleRepository>();
            var userRoleRepository = scopeServiceProvider.GetService<IUserRoleRepository>();
            var departmentRepository = scopeServiceProvider.GetService<IDepartmentRepository>();
            var adminUser = await userRepository.FirstOrDefaultAsync(x => x.Account == "admin");
            if (adminUser == null)
            {
                #region 管理员角色

                var adminRole = await roleRepository.FirstOrDefaultAsync(x => x.Name == "admin");
                if (adminRole == null)
                {
                    adminRole = new Role()
                    {
                        Name = "admin",
                        Remark = "admin"
                    };
                    await roleRepository.InsertAsync(adminRole);
                }

                #endregion

                #region 管理员部门


                var systemDept = await departmentRepository.FirstOrDefaultAsync(x => x.Name == "System");
                if (systemDept == null)
                {
                    systemDept = new Department()
                    {
                        Name = "System",
                        ParentId = null
                    };
                    await departmentRepository.InsertAsync(systemDept);
                }
                #endregion

                adminUser = new User()
                {
                    Account = "admin",
                    Name = "admin",
                    DepartmentId = systemDept.Id,
                    Password = BCrypt.Net.BCrypt.HashPassword("admin")
                };
                await userRepository.InsertAsync(adminUser);

                var userRole = new UserRole()
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };
                await userRoleRepository.InsertAsync(userRole);
            }
        }
    }
}