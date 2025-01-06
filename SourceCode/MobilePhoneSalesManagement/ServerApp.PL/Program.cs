
using Microsoft.EntityFrameworkCore;
using ServerApp.BLL.Services;
using ServerApp.BLL.Services.InterfaceServices;
using ServerApp.DAL.Data;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using ServerApp.DAL.Repositories;
using ServerApp.DAL.Repositories.Generic;
using System;

namespace ServerApp.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register DbContext
            builder.Services.AddDbContext<ShopDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DuongConnection"),
                    sqlOptions => sqlOptions.MigrationsAssembly("ServerApp.DAL"));
            });


            // Đăng ký IUnitOfWork và UnitOfWork
            builder.Services.AddScoped<IGenericRepository<User>, UserRepository>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserService, UserService>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
