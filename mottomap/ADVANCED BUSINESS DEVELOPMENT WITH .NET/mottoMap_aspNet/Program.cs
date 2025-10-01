using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;

var builder = WebApplication.CreateBuilder(args);

// Alterado para ler a nova string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Indo de UseOracle para UseSqlite
builder.Services.AddDbContext<MotoMapDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();