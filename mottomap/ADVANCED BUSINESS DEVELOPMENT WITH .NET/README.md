# 🏍️ MotoMap - Módulo de Movimentações e Histórico (.NET)

O MotoMap é um sistema desenvolvido para auxiliar na organização e gestão dos pátios de motos da empresa Mottu. Este módulo específico, desenvolvido em ASP.NET Core (C#), é o **Dono das Ações** relacionadas às operações temporais e integrações externas do sistema.

## 👨‍💻 Equipe

- Caike Dametto – RM: 558614
- Guilherme Janunzzi – RM: 558461

## 🏛️ Justificativa da Arquitetura

* **Tecnologia (.NET e C#):** A escolha pelo ASP.NET Core se deu pela sua alta performance, natureza open-source e ecossistema robusto, ideal para a construção de APIs RESTful escaláveis. A linguagem C# oferece segurança de tipo e recursos modernos que agilizam o desenvolvimento.
* **Padrão de API RESTful:** Optamos por uma arquitetura RESTful para garantir a interoperabilidade com outros módulos (como o de Java). O uso de verbos HTTP (`GET`, `POST`, `PUT`, `DELETE`) e status codes padronizados torna a comunicação clara e previsível.
* **Divisão de Responsabilidades (Microsserviços):** Este módulo funciona como um microsserviço focado exclusivamente no domínio de movimentações e histórico. Essa abordagem facilita a manutenção, a implantação e a escalabilidade, permitindo que cada parte do sistema evolua de forma independente.
* **Entity Framework Core:** Utilizamos o EF Core como ORM para abstrair o acesso ao banco de dados, aumentando a produtividade. O uso do provedor SQLite torna o ambiente de desenvolvimento extremamente leve e rápido, sem a necessidade de um servidor de banco de dados externo.

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core
- C#
- Entity Framework Core
- SQLite

## ✅ Pré-requisitos

* .NET SDK (versão 6.0 ou superior recomendada)
* Um ambiente de desenvolvimento integrado (IDE) como Visual Studio, JetBrains Rider ou Visual Studio Code.

## 🚀 Como Executar a API

1.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_SEU_REPOSITORIO_DOTNET>
    cd <NOME_DA_PASTA_DO_PROJETO_DOTNET>
    ```

2.  **Crie o Banco de Dados (SQLite):**
    O banco de dados será criado automaticamente. Basta executar o comando abaixo para aplicar as configurações no banco de dados local.
    ```bash
    dotnet ef database update
    ```

3.  **Execute o projeto:**
    ```bash
    dotnet run
    ```

4.  **Acesse a documentação interativa (Swagger):**
    A API estará rodando e a documentação Swagger estará disponível, geralmente em: `http://localhost:5001/swagger` (a porta pode variar).

## 🧪 Como Rodar os Testes

Para executar os testes automatizados do projeto (se houver), navegue até a pasta do projeto de testes e execute o seguinte comando:

```bash
# Navegue para a pasta de testes (ex: cd MotoMap.Tests)
dotnet test

## 🔗 Exemplos de Uso dos Endpoints

Abaixo estão exemplos de como interagir com os principais endpoints da API.

### 1. Registrar uma nova moto

**Requisição:** `POST /api/motos`

**Corpo (Body):**
```json
{
  "placa": "ABC1D23",
  "modelo": "Yamaha Fazer 250",
  "ano": 2023
}

### 2. Registrar a entrada de uma moto em um pátio

**Requisição:** `POST /movimentacoes/entrada`

**Corpo (Body):**
```json
{
  "motoId": 1,
  "posicaoId": 101,
  "usuarioId": 55,
  "observacoes": "Entrada para manutenção."
}

### 3. Registrar a entrada de uma moto em um pátio

**Requisição:** `GET /historico/posicoes/atuais`

📅 **Licença**

*MotoMap © 2025 - FIAP*

Todos os direitos reservados.