CREATE TABLE [dbo].[ServiceClient] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ClientId]     NVARCHAR (50)  NOT NULL,
    [ClientSecret] NVARCHAR (MAX) NOT NULL,
    [Description]  NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[ServiceUser] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [username] NVARCHAR (50)  NOT NULL,
    [password] NVARCHAR (MAX) NOT NULL,
    [role]     NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
