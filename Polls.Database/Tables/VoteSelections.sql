CREATE TABLE [dbo].[VoteSelections]
(
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [VariantId] UNIQUEIDENTIFIER NOT NULL,
    [VoteId]    UNIQUEIDENTIFIER NOT NULL,
    [Rank]      INT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_VoteSelections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VoteSelections_Variant] FOREIGN KEY ([VariantId]) REFERENCES [Variants]([Id]) ON DELETE CASCADE,  
    CONSTRAINT [FK_VoteSelections_Vote] FOREIGN KEY ([VoteId]) REFERENCES [Votes]([Id]) ON DELETE CASCADE
)