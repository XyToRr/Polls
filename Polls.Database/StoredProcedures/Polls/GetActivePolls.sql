CREATE PROCEDURE [dbo].[GetActivePolls]
AS
BEGIN
    SELECT Id, Title, Description, OwnerUserId, Algorithm,
           StartDate, EndDate, IsAnonymous, CreatedAt
    FROM Polls
    WHERE EndDate > GETDATE()
      AND ClosedManually = 0
END