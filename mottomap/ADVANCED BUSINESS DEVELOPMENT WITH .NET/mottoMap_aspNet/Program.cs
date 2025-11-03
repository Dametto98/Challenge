using Microsoft.EntityFrameworkCore;
using MotoMap.Api.DotNet.Data;
using MotoMap.Api.DotNet.Services; // Importa a pasta de serviços

var builder = WebApplication.CreateBuilder(args);

// --- Configuração dos Serviços (DI) ---

// 1. DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MotoMapDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. Nossos Serviços (Lógica de Negócio)
// Serviços do Passo 1 (Refatoração)
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<IHistoricoService, HistoricoService>();

// Serviços do Passo 2 (Novas Entidades) - LINHAS ADICIONADAS
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPatioService, PatioService>();


// 3. Serviços do ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// --- Configuração do Pipeline HTTP ---

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

