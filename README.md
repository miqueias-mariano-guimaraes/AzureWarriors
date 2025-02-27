# AzureWarriors

AzureWarriors é um sistema serverless desenvolvido em **.NET 8** utilizando **Azure Functions** para expor endpoints HTTP, **Dapper** para acesso performático ao **Azure SQL Database** e uma arquitetura em camadas para organizar as responsabilidades do projeto. Este sistema gerencia comunidades, clans e convites, com regras de negócio como:

- Cada usuário pode pertencer a apenas uma comunidade e a um único clan por vez.
- Se o usuário trocar de comunidade/clan, seus pontos são reiniciados.
- Clans possuem um líder definido.

---

## Sumário

- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Pré-requisitos](#pré-requisitos)
- [Estrutura da Solução](#estrutura-da-solução)
- [Configuração do Ambiente](#configuração-do-ambiente)
- [Como Executar Localmente](#como-executar-localmente)
- [Publicação no Azure](#publicação-no-azure)
- [Endpoints da API (Postman Collection)](#endpoints-da-api-postman-collection)
- [Licença](#licença)

---

## Arquitetura do Projeto

O projeto está organizado em **quatro camadas** principais:

- **AzureWarriors.Domain:**  
  Contém as entidades (Community, Clan, User, Invitation), enums (como InvitationStatus) e, opcionalmente, interfaces para identificação de agregados. Essa camada representa o núcleo do domínio.

- **AzureWarriors.Application:**  
  Contém as interfaces de repositório (definidas para serem implementadas na camada de Infrastructure), os serviços (CommunityService, ClanService, UserService, InvitationService) que orquestram as regras de negócio e os DTOs para transferência de dados.

- **AzureWarriors.Infrastructure:**  
  Responsável pelo acesso ao banco de dados usando Dapper. Inclui a implementação dos repositórios (CommunityRepository, ClanRepository, UserRepository, InvitationRepository), a fábrica de conexões (IDbConnectionFactory/DbConnectionFactory) e os scripts SQL para criação do schema.

- **AzureWarriors.Functions:**  
  Camada que expõe os endpoints HTTP por meio de Azure Functions (modelo .NET Isolated Worker). Essa camada injeta os serviços da camada Application e é o ponto de entrada do sistema.

---

## Pré-requisitos

- **Visual Studio 2022** ou superior com suporte ao .NET 8 e Azure Functions.
- **.NET 8 SDK** instalado.
- **Azure Functions Core Tools** (para execução e publicação local).
- Conta e Subscription no **Azure**.
- Instância do **Azure SQL Database** ou outra instância SQL para testes.
- **Dapper** (via NuGet) para acesso ao banco.
- Conhecimento básico de **injeção de dependências** e **configuração de Application Settings** no Azure.

---

## Estrutura da Solução

A solução é organizada da seguinte forma:

```
AzureWarriors (Solution)
│
├── AzureWarriors.Domain
│   ├── Entities
│   │   ├── Community.cs
│   │   ├── Clan.cs
│   │   ├── User.cs
│   │   └── Invitation.cs
│   ├── Enums
│   │   └── InvitationStatus.cs
│   └── Interfaces  (opcional)
│
├── AzureWarriors.Application
│   ├── Interfaces
│   │   ├── ICommunityRepository.cs
│   │   ├── IClanRepository.cs
│   │   ├── IUserRepository.cs
│   │   └── IInvitationRepository.cs
│   ├── DTOs
│   │   └── CreateCommunityDto.cs
│   └── Services
│       ├── CommunityService.cs
│       ├── ClanService.cs
│       ├── UserService.cs
│       └── InvitationService.cs
│
├── AzureWarriors.Infrastructure
│   ├── Data
│   │   └── DbConnectionFactory.cs
│   ├── Repositories
│   │   ├── CommunityRepository.cs
│   │   ├── ClanRepository.cs
│   │   ├── UserRepository.cs
│   │   └── InvitationRepository.cs
│   └── Scripts
│       └── CreateTables.sql
│
└── AzureWarriors.Functions
    ├── Http
    │   ├── CommunityFunctions.cs
    │   ├── ClanFunctions.cs
    │   ├── InvitationFunctions.cs
    │   └── UserFunctions.cs
    ├── Configurations
    └── Program.cs
```

---

## Configuração do Ambiente

### Variáveis de Ambiente Locais

No ambiente de desenvolvimento, utilize o arquivo `local.settings.json` para configurar suas variáveis de ambiente. Exemplo:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "ConnectionStrings": {
    "SqlConnection": "Server=tcp:SEU_SERVIDOR.database.windows.net,1433;Database=AzureWarriorsDB;User ID=SeuUsuario;Password=SuaSenha;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### Configuração no Azure

1. **Function App:**  
   No Portal do Azure, abra seu Function App e acesse **Configuration**.  
   - Adicione a connection string com o nome `SqlConnection` na seção **Connection Strings** ou **Application Settings**.

2. **Firewall e Acesso:**  
   Certifique-se de que o IP do seu ambiente (ou a opção "Allow Azure services") esteja habilitado no firewall do seu Azure SQL Server.

---

## Como Executar Localmente

1. **Abra a solução** no Visual Studio.
2. Certifique-se de que o projeto **AzureWarriors.Functions** esteja configurado como **Startup Project**.
3. Verifique o arquivo `local.settings.json` com as configurações corretas.
4. Pressione **F5** ou **Start Debugging** para iniciar as Azure Functions localmente.
5. Utilize ferramentas como o **Postman** para testar os endpoints na URL:  
   `http://localhost:7071/api/{nomeDoEndpoint}`

---

## Publicação no Azure

1. **Configure** seu Function App no Portal do Azure.
2. Utilize a funcionalidade de **Publish** do Visual Studio:
   - Clique com o botão direito no projeto **AzureWarriors.Functions** e selecione **Publish**.
   - Siga o assistente para publicar diretamente no Azure.
3. **Atualize** as Application Settings no Portal para incluir a connection string e quaisquer outras variáveis necessárias.
4. Após a publicação, teste os endpoints utilizando a URL do seu Function App, por exemplo:  
   `https://seuapp.azurewebsites.net/api/CreateCommunity?code=YOUR_FUNCTION_KEY`

---

## Endpoints da API (Postman Collection)

Uma collection para o Postman foi fornecida para testar os endpoints. Importe o arquivo JSON da collection no Postman e ajuste a URL base conforme seu ambiente (local ou publicado).

---

## Contato

Para dúvidas ou sugestões, entre em contato com miqueias.dev@gmail.com .

---

Com essa estrutura e instruções, o **AzureWarriors** está pronto para ser desenvolvido, testado e publicado, funcionando tanto localmente quanto no ambiente Azure.
