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
                    //ͨ���Զ���ע�������󶨲ִ�
                    x.BindRepositoriesWithAttribute<AutoRepository1Attribute>();
                    
                    //�����ݿ����ɽӿ�
                    x.BindDbGeneratorType<IDbGenerator1>();

                    //�󶨲���ǰ�ص�
                    x.BeforeInsert += entity =>
                    {
                        if (entity is BaseEntity baseEntity)
                        {
                            baseEntity.CreateOn = DateTime.Now;
                        }
                    };

                    //�󶨸���ǰ�ص�
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

        /// <summary>
        /// Initialize the database
        /// 初始化数据库
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private static async Task InitDatabase(IServiceProvider service)
        {
            var dbGenerator= service.CreateScope().ServiceProvider.GetService<IDbGenerator1>();
        
            if (dbGenerator.GetAllTableNames().Count == 0)
            {
                var sqls = dbGenerator.GenerateSql(new List<Type>()
                {
                    typeof(Department), typeof(Dictionary), typeof(DictionaryItem),
                    typeof(Role), typeof(RoleAssignMenu),
                    typeof(User), typeof(MenuMeta), typeof(Menu),
                });
                foreach (var generateDatabaseSqlResult in sqls)
                {
                    dbGenerator.ExecuteGenerateSql(generateDatabaseSqlResult);
                }
            }
        
        }
    }
}