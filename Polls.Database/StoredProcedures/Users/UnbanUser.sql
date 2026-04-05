CREATE PROCEDURE [dbo].[UnbanUser]
    @PollId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM PollBannedUsers 
    WHERE PollId = @PollId 
      AND UserId = @UserId
END