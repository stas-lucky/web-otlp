using MyLab.Client.Web.Clients;
using MyLab.Common;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services
    .AddRefitClient<IMyLabApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://localhost:7143");
    });

OtlpConfiguration.AddOTLP(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
