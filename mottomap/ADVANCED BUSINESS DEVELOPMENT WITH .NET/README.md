🏍️ MotoMap - API de Gestão de Pátios (.NET) - Sprint 4

API RESTful completa para o sistema MotoMap, responsável pela gestão de motos, pátios, usuários e movimentações. Este projeto foi desenvolvido para a disciplina "Advanced Business Development with .NET" da FIAP.

👨‍💻 Equipe

Caike Dametto – RM: 558614

Guilherme Janunzzi – RM: 558461

🏛️ Arquitetura

A arquitetura desta API foi refatorada para seguir as melhores práticas, corrigindo os débitos do feedback da Sprint 3 [cite: image_412d07.png].

API (Controllers): Camada de entrada da aplicação. Responsável apenas por receber requisições HTTP, validar DTOs e chamar a camada de serviço.

Services (Camada de Serviço): Nova camada que contém toda a lógica de negócio. Removemos a lógica dos "Fat Controllers", tratando aqui as regras de validação, orquestração de dados e comunicação com o DbContext [cite: image_412d07.png].

Domain (Models): Contém as entidades principais do sistema (Moto, Movimentacao, HistoricoPosicao, Usuario, Patio).

Data (Repositórios): Camada de acesso a dados, utilizando o Entity Framework Core e o padrão de Repositório (implementado dentro dos Serviços).

✨ Features (Sprint 4)

Este projeto implementa todas as funcionalidades avançadas exigidas na 4ª Sprint [cite: image_412c4e.png]:

Segurança (JWT): Endpoints [Authorize] protegidos usando autenticação e autorização baseadas em JSON Web Tokens (JWT).

Versionamento de API: A API suporta versionamento (/api/v1/...).

Health Checks: Endpoint /health disponível para monitoramento da saúde da API e do banco de dados (SQLite).

Testes Unitários (xUnit): Cobertura de testes para a lógica de negócio (ex: UsuarioService).

Testes de Integração (WebApplicationFactory): Testes que simulam requisições HTTP reais para validar a segurança (401) e o Health Check (200 OK).

Machine Learning (ML.NET): Endpoint POST /api/v1/Previsao/tempo-estadia que prevê o tempo de estadia de uma moto, treinado com o histórico do banco de dados.

🛠️ Tecnologias Utilizadas

.NET 8

ASP.NET Core

Entity Framework Core

SQLite (Banco de dados local)

xUnit (Testes Unitários e de Integração)

Moq (Mocking para Testes Unitários)

ML.NET (FastTree Regression)

Swagger/OpenAPI (Documentação)

✅ Pré-requisitos

.NET SDK 8.0

Um editor de código (VS Code, Visual Studio 2022)

🚀 Como Executar a API

Clone o repositório:

git clone <URL_DO_SEU_REPOSITORIO>
cd mottoMap_aspNet


Restaure os Pacotes:
(Pode ser necessário após a clonagem)

dotnet restore


Crie o Banco de Dados (SQLite):
Este comando cria o banco motomap.db com todas as tabelas.

dotnet ef database update


Execute o projeto:

dotnet run


A API estará disponível em http://localhost:5171 (ou a porta configurada no launchSettings.json).

Acesse a Documentação (Swagger):
Abra o navegador em: http://localhost:5171/swagger

🧪 Como Rodar os Testes

Este projeto cumpre o requisito de testes unitários e de integração [cite: image_412c4e.png].

Para executar todos os testes (3 no total), navegue até a pasta do projeto de testes e execute o comando:

# A partir da pasta raiz (onde está o .sln)
cd MotoMap.Api.DotNet.Tests

# Execute o comando de teste
dotnet test


Resultado Esperado:
Resumo do teste: total: 3; falhou: 0; bem-sucedido: 3;

🔐 Segurança (Como usar a API no Swagger)

A maioria dos endpoints (Motos, Patios, etc.) são protegidos por JWT [cite: image_412c4e.png]. Para usá-los:

Execute o endpoint POST /api/v1/Auth/register para criar um usuário.

Execute o endpoint POST /api/v1/Auth/login com as suas credenciais.

Copie o token da resposta (ex: eyJhbGci...).

Clique no botão "Authorize" no topo do Swagger.

Na caixa "Value", cole o token no formato: Bearer {SEU_TOKEN_AQUI}.

Clique em "Authorize". Agora você pode testar todos os endpoints!
