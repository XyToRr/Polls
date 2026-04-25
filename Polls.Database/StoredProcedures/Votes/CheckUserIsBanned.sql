CREATE PROCEDURE [dbo].[CheckUserIsBanned]
    @PollId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT CAST(CASE 
        WHEN EXISTS(
            SELECT 1 FROM dbo.PollBannedUsers 
            WHERE PollId = @PollId 
            AND UserId = @UserId
        ) THEN 1
        ELSE 0
    END AS BIT)
END
