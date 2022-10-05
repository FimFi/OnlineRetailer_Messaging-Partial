using CustomerApi.Data;
using CustomerApi.Infrastructure;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using SharedModels;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ connection string (I use CloudAMQP as a RabbitMQ server).
// Remember to replace this connectionstring with your own.
string cloudAMQPConnectionString =
   "host=hawk-01.rmq.cloudamqp.com;virtualHost=dqslqjpf;username=dqslqjpf;password=T31Zdro1hILZQtaYuk1VBAUDC7ISp6Ec";

// Add services to the container.
builder.Services.AddDbContext<CustomerApiContext>(opt => opt.UseInMemoryDatabase("CustomersDb"));

builder.Services.AddScoped<IRepository<Customer>, CustomerRepository>();

builder.Services.AddTransient<IDbInitializer, DbInitializer>();

builder.Services.AddSingleton<IConverter<Customer, CustomerDto>, CustomerConverter>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetService<CustomerApiContext>();
    var dbInitializer = services.GetService<IDbInitializer>();
    dbInitializer.Initialize(dbContext);
}

// Create a message listener in a separate thread.
Task.Factory.StartNew(() =>
    new Listener(app.Services, cloudAMQPConnectionString).Start());

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
