using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot-aggregation.json");

builder.Services.AddOcelot();

var app = builder.Build();

app.UseHttpsRedirection();



//app.MapMetrics(); //Doesn't work here for some unknown reason.
// Do it the old way instead:
app.UseRouting();
app.UseAuthorization();
app.UseHttpMetrics();
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello world!!");
    });
});

app.UseOcelot().Wait();

app.Run();


