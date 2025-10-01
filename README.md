# 🛵 MotoMap – Sistema Inteligente de Gestão de Pátios Mottu

> Solução multiplataforma para gestão de localização de motos nos pátios da Mottu, com suporte Web e Mobile, e integração entre APIs .NET e Java.

---

## 📌 Visão Geral

**MotoMap** é uma plataforma desenvolvida para resolver o problema de controle manual da localização de motos nos pátios da Mottu. Com operações em mais de 100 filiais no Brasil e México, a solução visa aumentar a produtividade, segurança e organização da frota.

A aplicação é composta por:

- 🔗 **Duas APIs RESTful**: uma desenvolvida em ASP.NET Core e outra em Spring Boot
- 💻 **Frontend Web**: para supervisores e administradores gerenciarem as motos e pátios
- 📱 **App Mobile**: para operadores realizarem check-in/check-out das motos e visualização rápida

---

## ⚙️ Tecnologias Utilizadas

| Camada           | Tecnologia                     |
|------------------|--------------------------------|
| API Principal    | ASP.NET Core + EF Core + Oracle|
| API Secundária   | Spring Boot + Spring Data JPA  |
| Banco de Dados   | Oracle                         |
| Web Frontend     | React (Next.js)                |
| Mobile           | React Native + Expo            |
| Hospedagem       | Azure VM + Docker              |

---

## 🧱 Arquitetura da Solução

A solução é dividida em dois domínios de responsabilidade, com APIs distintas:

### 📦 API .NET – `api-dotnet.mottumap.com`

> Responsável pelo cadastro e gerenciamento de motos e filiais.

**Principais rotas:**
- `GET /motos`
- `POST /motos`
- `PUT /motos/{id}`
- `DELETE /motos/{id}`
- `GET /filiais`

### ⚙️ API Java – `api-java.mottumap.com`

> Responsável pela movimentação de motos no pátio e histórico de posições.

**Principais rotas:**
- `GET /movimentacoes`
- `POST /movimentacoes`
- `GET /posicoes`
- `GET /filiais/{id}/mapa`

---

## 🧩 Funcionamento Integrado

A plataforma web e mobile se comunica com ambas as APIs conforme o contexto:

| Ação no Sistema | API Utilizada |
|-----------------|---------------|
| Cadastrar nova moto | .NET        |
| Consultar filiais   | .NET        |
| Visualizar mapa do pátio | Java  |
| Registrar movimentação de moto | Java |
| Histórico de motos por posição | Java |

Ambas as APIs seguem o mesmo padrão de DTOs e retornos REST para facilitar o consumo no frontend.

---

## 🌐 Web App (Administração)

Funcionalidades:
- Cadastro de motos e filiais
- Visualização do layout do pátio com posições ocupadas
- Relatórios simples por filial
- Gerenciamento de usuários e permissões

---

## 📱 Mobile App (Operadores)

Funcionalidades:
- Login de operador
- Visualização do pátio e motos por posição
- Registro de entrada/saída de motos
- Histórico local via `AsyncStorage`

---

## ✅ Pré-requisitos

Antes de começar, garanta que você tenha as seguintes ferramentas instaladas:

-   [.NET SDK 8.0](https://dotnet.microsoft.com/download) ou superior
-   [Java JDK 17](https://www.oracle.com/java/technologies/javase/jdk17-archive-downloads.html) ou superior
-   [Node.js 18.x](https://nodejs.org/) ou superior
-   [Docker](https://www.docker.com/products/docker-desktop/) (para o banco de dados)
-   Um cliente Git para clonar o repositório

---

## 🧪 Como Rodar Localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/seuusuario/mottomap
cd mottomap
```

### 2. Iniciar APIs

#### .NET API
1.  Navegue até a pasta `backend-dotnet`.
2.  Crie uma cópia do arquivo `appsettings.example.json` e renomeie para `appsettings.Development.json`.
3.  Preencha a string de conexão do Oracle no novo arquivo.
4.  Execute os comandos:

```bash
cd backend-dotnet
dotnet ef database update
dotnet run

#### Java API
```bash
cd backend-java
./mvnw spring-boot:run
```

> Certifique-se de ter o Oracle ou H2 configurado

### 3. Iniciar Web App

```bash
cd frontend-web
npm install
npm run dev
```

### 4. Iniciar Mobile App

```bash
cd app-mobile
npm install
npx expo start
```

---

## 📊 Organização dos Diretórios

```
mottomap/
├── backend-dotnet/         # API em ASP.NET Core
├── backend-java/           # API em Spring Boot
├── frontend-web/           # Aplicação Web (React)
├── app-mobile/             # Aplicação Mobile (React Native)
├── database/               # Modelos DER, MER e scripts SQL
└── docs/                   # Documentação, wireframes, diagramas
```

---

## 🚧 Futuras Integrações

A arquitetura do MotoMap foi preparada para futuras adições de:

- Leitura automática via sensores (IoT)
- Detecção por câmera (Visão Computacional)
- Alertas automatizados via sensores de movimento e status

Essas tecnologias serão implementadas no segundo semestre, em conjunto com as disciplinas específicas.

---

## 👥 Equipe

- Caike Dametto – RM558614 
- Guilherme Jannunzi – RM558461

---

## 📎 Licença

Projeto desenvolvido exclusivamente para fins acadêmicos na FIAP – 2º Ano de Análise e Desenvolvimento de Sistemas (Challenge 2025).
