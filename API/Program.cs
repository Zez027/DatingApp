using API;
using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configura o pipeline de requisições HTTP.
app.UseMiddleware<ExceptionMiddleware>();

// Configuração do CORS para permitir credenciais e a origem específica do frontend (Angular)
app.UseCors(builder => builder
    .AllowAnyHeader()                  // Permitir qualquer cabeçalho
    .AllowAnyMethod()                  // Permitir qualquer método (GET, POST, etc.)
    .WithOrigins("https://localhost:4200")  // Permitir a origem do seu frontend (Angular)
    .AllowCredentials());             // Permitir credenciais (como cookies ou tokens)

// Ativa a autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Redireciona para HTTPS (caso a requisição não seja HTTPS)
app.UseHttpsRedirection();

// Mapeia os controladores para o roteamento
app.MapControllers();

// Aplica a migração e popula o banco de dados
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();  // Aplica as migrações pendentes ao banco de dados
    await Seed.SeedUsers(context);  // Semeia o banco com dados iniciais, se necessário
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "Ocorreu um erro durante a migration");
}

app.Run();  // Inicia a aplicação
