CREATE TABLE [dbo].[Users]
(
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Name]      NVARCHAR(50)     NOT NULL,
    [LastName]  NVARCHAR(50)     NULL,
    [Password]  NVARCHAR(256)    NOT NULL,
    [Login]     NVARCHAR(50)     NOT NULL, 
    [CreatedAt] DATETIME         NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_Users]       PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Users_Login] UNIQUE ([Login]),
)