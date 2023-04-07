using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.Generator;
using SummerBootAdmin.Dto;

namespace SummerBootAdmin
{
    public class Program
    {
        public static void Main(string[] args)
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
            });
            builder.Services.AddSummerBootMvcExtension(it =>
            {
                //是否启用全局错误处理
                it.UseGlobalExceptionHandle = true;
                //是否启用参数校验处理
                it.UseValidateParameterHandle = true;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(it =>
            {
                it.AddProfile<SummerbootProfile>();
            });
            builder.Services.AddCors(it =>
                it.AddPolicy("all", x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            builder.Services.AddSummerBoot();
            builder.Services.AddSummerBootRepository(it =>
            {
                var connectionString = configuration.GetValue<string>("mysqlDbConnectionString");
                it.AddDatabaseUnit<MySqlConnection, IUnitOfWork1>(connectionString, x =>
                {
                    //通过自定义注解批量绑定仓储
                    x.BindRepositorysWithAttribute<AutoRepository1Attribute>();
                    
                    //绑定数据库生成接口
                    x.BindDbGeneratorType<IDbGenerator1>();

                    //绑定插入前回调
                    x.BeforeInsert += entity =>
                    {
                        if (entity is BaseEntity baseEntity)
                        {
                            baseEntity.CreateOn = DateTime.Now;
                        }
                    };

                    //绑定更新前回调
                    x.BeforeUpdate += entity =>
                    {
                        if (entity is BaseEntity baseEntity)
                        {
                            baseEntity.LastUpdateOn = DateTime.Now;
                        }
                    };
                });
            });

            var app = builder.Build();
            app.UseCors("all");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}