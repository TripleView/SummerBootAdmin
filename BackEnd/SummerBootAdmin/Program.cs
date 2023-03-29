using MySql.Data.MySqlClient;
using SummerBoot.Core;
using SummerBoot.Repository.Generator;

namespace SummerBootAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://**:5000");
            var configuration = builder.Configuration;
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSummerBoot();

            builder.Services.AddSummerBootRepository(it =>
            {
                var connectionString = configuration.GetValue("");
                it.AddDatabaseUnit<MySqlConnection,IUnitOfWork1>(connectionString, x =>
                {
                    x.BindDbGeneratorType<IDbGenerator1>();
                    x.BindRepositorysWithAttribute<>();
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