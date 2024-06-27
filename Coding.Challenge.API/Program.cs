using Coding.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

builder.Services.AddRazorPages();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();

app.Run();