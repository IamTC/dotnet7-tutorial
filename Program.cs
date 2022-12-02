using System.Net.Mime;
using System.Text.Json;
using Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Repositories;
using Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

var mongoDbSetttings = builder.Configuration.GetSection(nameof(MONGODBSETTINGS)).Get<MONGODBSETTINGS>();

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient(mongoDbSetttings.ConnectionString);
});

builder.Services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();
builder.Services.AddHealthChecks().AddMongoDb(
    mongoDbSetttings.ConnectionString,
    name: "mongodb",
    timeout: TimeSpan.FromSeconds(3),
    tags: new[] { "ready" }
);

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

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = async (constext, report) =>
    {
        var result = JsonSerializer.Serialize(
            new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(rep => new
                {
                    name = rep.Key,
                    status = rep.Value.Status.ToString(),
                    exception = rep.Value.Exception != null ? rep.Value.Exception.Message : null,
                    duration = rep.Value.Duration.ToString()
                })
            });

        constext.Response.ContentType = MediaTypeNames.Application.Json;
        await constext.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/life", new HealthCheckOptions
{
    Predicate = (_) => false
});

app.Run();
