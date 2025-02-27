-- 1. Criação das Tabelas (sem FKs)

-- Tabela Communities
CREATE TABLE [dbo].[Communities] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]        VARCHAR(100)     NOT NULL,
    [Description] TEXT             NULL,
    [CreatedAt]   DATETIME         NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Communities] PRIMARY KEY ([Id])
);

-- Unique Index em Name
CREATE UNIQUE INDEX [IX_Communities_Name]
    ON [dbo].[Communities] ([Name]);

------------------------------------------------------------------

-- Tabela Users
CREATE TABLE [dbo].[Users] (
    [Id]          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Username]    VARCHAR(100)     NOT NULL,
    [CommunityId] UNIQUEIDENTIFIER NULL,
    [ClanId]      UNIQUEIDENTIFIER NULL,
    [Points]      INT              NOT NULL DEFAULT (0),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

-- Unique Index em Username
CREATE UNIQUE INDEX [IX_Users_Username]
    ON [dbo].[Users] ([Username]);

------------------------------------------------------------------

-- Tabela Clans
CREATE TABLE [dbo].[Clans] (
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CommunityId]   UNIQUEIDENTIFIER NOT NULL,
    [LeaderUserId]  UNIQUEIDENTIFIER NOT NULL, -- FK para quem é o líder
    [Name]          VARCHAR(100)     NOT NULL,
    [CreatedAt]     DATETIME         NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Clans] PRIMARY KEY ([Id])
);

-- Unique Index em Name
CREATE UNIQUE INDEX [UQ_Clans_CommunityId_Name]
    ON [dbo].[Clans] ([Name]);

------------------------------------------------------------------

-- Tabela Invitations
CREATE TABLE [dbo].[Invitations] (
    [Id]        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ClanId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]    UNIQUEIDENTIFIER NOT NULL,
    [Status]    VARCHAR(20)      NOT NULL,  -- 'Pending', 'Accepted', 'Declined'
    [CreatedAt] DATETIME         NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Invitations] PRIMARY KEY ([Id])
);

-- 2. Adicionando as FKs após a criação das tabelas

-- FK Users -> Communities
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Communities]
FOREIGN KEY ([CommunityId])
REFERENCES [dbo].[Communities]([Id]);

-- FK Users -> Clans
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Clans]
FOREIGN KEY ([ClanId])
REFERENCES [dbo].[Clans]([Id]);

-- FK Clans -> Communities
ALTER TABLE [dbo].[Clans]
ADD CONSTRAINT [FK_Clans_Communities]
FOREIGN KEY ([CommunityId])
REFERENCES [dbo].[Communities]([Id]);

-- FK Clans -> Users (líder)
ALTER TABLE [dbo].[Clans]
ADD CONSTRAINT [FK_Clans_Users_Leader]
FOREIGN KEY ([LeaderUserId])
REFERENCES [dbo].[Users]([Id]);

-- FK Invitations -> Clans
ALTER TABLE [dbo].[Invitations]
ADD CONSTRAINT [FK_Invitations_Clans]
FOREIGN KEY ([ClanId])
REFERENCES [dbo].[Clans]([Id]);

-- FK Invitations -> Users
ALTER TABLE [dbo].[Invitations]
ADD CONSTRAINT [FK_Invitations_Users]
FOREIGN KEY ([UserId])
REFERENCES [dbo].[Users]([Id]);

-- FIM DO SCRIPT