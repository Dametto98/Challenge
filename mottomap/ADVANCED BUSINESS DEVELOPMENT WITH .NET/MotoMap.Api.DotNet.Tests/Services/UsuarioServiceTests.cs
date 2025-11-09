public class UsuarioServiceTests
{
    private readonly MotoMapDbContext _dbContext;
    private readonly IConfiguration _mockConfiguration;
    private readonly UsuarioService _usuarioService;

    // O construtor do teste é executado ANTES de cada teste
    public UsuarioServiceTests()
    {
        // 1. Configurar o Banco de Dados em Memória
        var options = new DbContextOptionsBuilder<MotoMapDbContext>()
            // Usamos Guid.NewGuid() para garantir que cada teste use um banco de dados limpo e novo
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new MotoMapDbContext(options);

        // 2. Simular (Mock) o IConfiguration para ler a chave secreta do JWT
        var mockConfigSection = new Mock<IConfigurationSection>();
        mockConfigSection.Setup(s => s.Value).Returns("SuaChaveSecretaSuperLongaParaTestesQueNinguemSabe123");

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c.GetSection("Jwt:Key")).Returns(mockConfigSection.Object);
        
        _mockConfiguration = mockConfig.Object;

        // 3. Criar a instância do serviço que vamos testar
        _usuarioService = new UsuarioService(_dbContext, _mockConfiguration);
    }

    [Fact]
    public async Task RegisterAsync_EmailJaExistente_DeveRetornarNulo()
    {
        // --- Arrange (Arrumar) ---
        
        // 1. Primeiro, adicionamos um usuário "existente" ao nosso banco em memória
        var usuarioExistente = new Usuario
        {
            Nome = "Usuario Antigo",
            Email = "email@teste.com",
            PasswordHash = "hashqualquer", // O hash não importa para este teste
            Perfil = "Admin"
        };
        _dbContext.Usuarios.Add(usuarioExistente);
        await _dbContext.SaveChangesAsync();

        // 2. Criamos um DTO (objeto de registo) que tenta usar o MESMO email
        var dto = new UsuarioRegisterDto
        {
            Nome = "Usuario Novo",
            Email = "email@teste.com", // Email repetido
            Password = "Password123!",
            Perfil = "Operador"
        };

        // --- Act (Agir) ---
        
        // 3. Chamamos o método que queremos testar
        var resultado = await _usuarioService.RegisterAsync(dto);

        // --- Assert (Verificar) ---
        
        // 4. Verificamos se o resultado é nulo (como esperado)
        Assert.Null(resultado);

        // 5. (Bonus) Verificamos se o número de usuários no banco de dados ainda é 1
        //    Isso prova que o novo usuário NÃO foi adicionado.
        var totalUsuarios = await _dbContext.Usuarios.CountAsync();
        Assert.Equal(1, totalUsuarios);
    }
}