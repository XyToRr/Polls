CREATE TABLE [dbo].[Polls]
(
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [Title]           NVARCHAR(50)     NOT NULL, 
    [Description]     NVARCHAR(250)    NULL, 
    [OwnerUserId]     UNIQUEIDENTIFIER NOT NULL, 
    [Algorithm]       INT              NOT NULL, 
    [StartDate]       DATETIME         NOT NULL, 
    [EndDate]         DATETIME         NULL, 
    [IsAnonymous]     BIT              NOT NULL DEFAULT 0, 
    [ClosedManually]  BIT              NULL DEFAULT 0,
    [CreatedAt]       DATETIME         NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_Polls] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Polls_Owner] FOREIGN KEY ([OwnerUserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_Polls_Dates] CHECK ([EndDate] IS NULL OR [EndDate] > [StartDate])
)