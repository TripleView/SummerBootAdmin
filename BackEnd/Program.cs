using MySqlConnector;
using SummerBoot.Core;
using SummerBoot.Repository;
using SummerBoot.Repository.Generator;

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
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSummerBoot();
            builder.Services.AddSummerBootRepository(it =>
            {
                var connectionString = configuration.GetValue<string>("mysqlDbConnectionString");
                it.AddDatabaseUnit<MySqlConnection,IUnitOfWork1>(connectionString, x =>
                {
                    x.BindRepositorysWithAttribute<AutoRepository1Attribute>();
                    x.BindDbGeneratorType<IDbGenerator1>();
                });
            });

            var app = builder.Build();

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