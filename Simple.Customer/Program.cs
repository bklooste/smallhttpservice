using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Marten;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddControllers();
builder.Services.AddSingleton<IDocumentStore>(DocumentStore.For(storeOptions =>
{
    storeOptions.Connection(connectionString);
    storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
    storeOptions.CreateDatabasesForTenants(c =>
            c.ForTenant()
                .CheckAgainstPgDatabase()
                .WithOwner("postgres")
                .WithEncoding("UTF-8")
                .ConnectionLimit(-1)
        );
}));
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer API", Version = "v1" });
            });
var app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c =>      //Swagger UI should be served from static container not service
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseAuthorization();
//with attribute app.MapGet("/", [Authorize] (DbContext db) => db.Todos.ToListAsync();
app.MapGet("/HealthCheck", () => return Results.Ok(); );


app.Run();

