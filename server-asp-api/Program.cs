using server_asp_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//  чтобы можно было с клиента стучаться в API
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();