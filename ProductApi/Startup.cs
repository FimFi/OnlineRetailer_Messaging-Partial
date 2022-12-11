// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ProductApi
{
    using global::ProductApi.Data;
    using global::ProductApi.Models;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.ServiceFabric.Module;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SharedModels;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            

            services.AddMvc();
            services.AddDbContext<ProductApiContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));
            services.AddScoped<IRepository<Product>, ProductRepository>();
            services.AddTransient<IDbInitializer, DbInitializer>();
            services.AddSingleton<IConverter<Product, ProductDto>, ProductConverter>();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();


            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc();
            app.UseRouting();
            app.UseAuthorization();
        }

        /* var builder = WebApplication.CreateBuilder(args);

         // RabbitMQ connection string (I use CloudAMQP as a RabbitMQ server).
         // Remember to replace this connectionstring with your own.
         string cloudAMQPConnectionString =
            "host=hawk-01.rmq.cloudamqp.com;virtualHost=dqslqjpf;username=dqslqjpf;password=T31Zdro1hILZQtaYuk1VBAUDC7ISp6Ec";

         // Add services to the container.

         builder.Services.AddDbContext<ProductApiContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));

     // Register repositories for dependency injection
     builder.Services.AddScoped<IRepository<Product>, ProductRepository>();

     // Register database initializer for dependency injection
     builder.Services.AddTransient<IDbInitializer, DbInitializer>();

     // Register ProductConverter for dependency injection
     builder.Services.AddSingleton<IConverter<Product, ProductDto>, ProductConverter>();


     builder.Services.AddControllers();
     // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
     builder.Services.AddEndpointsApiExplorer();
     builder.Services.AddSwaggerGen();

     var app = builder.Build();

     // Configure the HTTP request pipeline.
     if (app.Environment.IsDevelopment())
     {
         app.UseSwagger();
         app.UseSwaggerUI();
     }

     // Initialize the database.
     using (var scope = app.Services.CreateScope())
     {
         var services = scope.ServiceProvider;
         var dbContext = services.GetService<ProductApiContext>();
         var dbInitializer = services.GetService<IDbInitializer>();
         dbInitializer.Initialize(dbContext);
     }

     // Create a message listener in a separate thread.
     Task.Factory.StartNew(() =>
         new MessageListener(app.Services, cloudAMQPConnectionString).Start());

     //app.UseHttpsRedirection();


    
     */
    }
}