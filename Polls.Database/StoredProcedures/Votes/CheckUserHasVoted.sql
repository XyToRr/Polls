CREATE PROCEDURE [dbo].[CheckUserHasVoted]
    @PollId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT CAST(CASE 
        WHEN EXISTS(
            SELECT 1 FROM dbo.Votes 
            WHERE PollId = @PollId 
            AND UserId = @UserId 
            AND IsValid = 1
        ) THEN 1
        ELSE 0
    END AS BIT)
END
