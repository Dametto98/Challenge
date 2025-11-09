# 🏍️ MotoMap - API de Gestão de Pátios (.NET) — Sprint 4

API RESTful completa para o sistema **MotoMap**, responsável pela gestão de motos, pátios, usuários e movimentações. Projeto desenvolvido para a disciplina **Advanced Business Development with .NET** da FIAP.

## 👨‍💻 Equipe
- **Caike Dametto** – RM: 558614  
- **Guilherme Janunzzi** – RM: 558461  

## 🏛️ Arquitetura
A arquitetura foi refatorada para aplicar boas práticas e corrigir os débitos apontados na Sprint 3.

### Camadas da Aplicação
- **API (Controllers):** Recebe requisições HTTP, valida DTOs e aciona os serviços.  
- **Services:** Contém toda a lógica de negócio, removendo os antigos *Fat Controllers*. Inclui validações, orquestração de dados e uso do DbContext.  
- **Domain (Models):** Define as entidades principais: Moto, Movimentacao, HistoricoPosicao, Usuario e Patio.  
- **Data (Repositórios):** Acesso a dados utilizando Entity Framework Core com padrão de repositório (implementado nos Services).

## ✨ Funcionalidades — Sprint 4
Este projeto entrega todas as funcionalidades avançadas exigidas:

- **Segurança (JWT):** Endpoints protegidos com `[Authorize]` utilizando autenticação e autorização via JWT.  
- **Versionamento de API:** Estrutura `/api/v1/...`.  
- **Health Checks:** Endpoint `/health` monitora API e banco SQLite.  
- **Testes Unitários (xUnit):** Cobrem regras de negócio como `UsuarioService`.  
- **Testes de Integração (WebApplicationFactory):** Validam autenticação (401) e Health Check (200).  
- **Machine Learning (ML.NET):** Endpoint `POST /api/v1/Previsao/tempo-estadia` prevê tempo de estadia de uma moto usando FastTree Regression.

## 🛠️ Tecnologias Utilizadas
- .NET 8  
- ASP.NET Core  
- Entity Framework Core  
- SQLite  
- xUnit  
- Moq  
- ML.NET  
- Swagger / OpenAPI  

## ✅ Pré-requisitos
- **.NET SDK 8.0**  
- **VS Code** ou **Visual Studio 2022**

## 🚀 Como Executar a API

### 1. Clone o repositório:

git clone https://github.com/Dametto98/Challenge/tree/main/mottomap/ADVANCED%20BUSINESS%20DEVELOPMENT%20WITH%20.NET
cd mottoMap_aspNet


### 2. Restaure os pacotes:
```bash
dotnet restore

### 3. Crie o banco de dados (SQLite):
```bash
dotnet ef database update

### 4. Execute o projeto:
```bash
dotnet run

### A API estará disponível em:
http://localhost:5171

### 5. Acesse o Swagger:
```bash
http://localhost:5171/swagger

### 1. Clone o repositório:
```bash

## 🧪 Como Rodar os Testes
```bash
cd MotoMap.Api.DotNet.Tests
dotnet test

### Resultado esperado:
Resumo do teste: total: 3; falhou: 0; bem-sucedido: 3;

## 🔐 Usando JWT no Swagger
Execute POST /api/v1/Auth/register para criar um usuário.

Execute POST /api/v1/Auth/login.

Copie o token retornado.

No Swagger, clique em Authorize.

Cole assim:

Bearer {SEU_TOKEN_AQUI}

Agora você pode acessar os endpoints protegidos.

📅 **Licença**

*MotoMap © 2025 - FIAP*

Todos os direitos reservados.
