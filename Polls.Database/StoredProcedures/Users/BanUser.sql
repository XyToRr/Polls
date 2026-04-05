CREATE PROCEDURE [dbo].[BanUser]
    @PollId         UNIQUEIDENTIFIER,
    @UserId         UNIQUEIDENTIFIER,
    @BannedByUserId UNIQUEIDENTIFIER,
    @BanReason      NVARCHAR(150) = NULL,
    @BannedAt       DATETIME
AS
BEGIN
    INSERT INTO PollBannedUsers (PollId, UserId, BannedByUserId, BanReason, BannedAt)
    VALUES (@PollId, @UserId, @BannedByUserId, @BanReason, @BannedAt)
END