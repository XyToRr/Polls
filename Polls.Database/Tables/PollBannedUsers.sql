CREATE TABLE [dbo].[PollBannedUsers]
(
    [PollId]    UNIQUEIDENTIFIER NOT NULL, 
    [UserId]    UNIQUEIDENTIFIER NOT NULL, 
    [BanReason] NVARCHAR(150) NULL, 
    [BannedAt]  DATETIME NOT NULL DEFAULT GETDATE(),
    [BannedByUserId] UNIQUEIDENTIFIER NOT NULL,

    CONSTRAINT [PK_PollBannedUsers] PRIMARY KEY ([PollId], [UserId]),
    CONSTRAINT [FK_PollBannedUsers_Poll]     FOREIGN KEY ([PollId])        REFERENCES [Polls]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PollBannedUsers_User]     FOREIGN KEY ([UserId])        REFERENCES [Users]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PollBannedUsers_BannedBy] FOREIGN KEY ([BannedByUserId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION
)

GO

CREATE TRIGGER [dbo].[TR_PollBannedUsers_InvalidateVotes]
ON [dbo].[PollBannedUsers]
AFTER INSERT
AS
BEGIN
    UPDATE Votes
    SET IsValid = 0
    WHERE UserId IN (SELECT UserId FROM inserted)
      AND PollId IN (SELECT PollId FROM inserted)
END


GO

CREATE TRIGGER [dbo].[TR_PollBannedUsers_RestoreVotes]
ON [dbo].[PollBannedUsers]
AFTER DELETE
AS
BEGIN
    UPDATE Votes
    SET IsValid = 1
    WHERE UserId IN (SELECT UserId FROM deleted)
      AND PollId IN (SELECT PollId FROM deleted)
END