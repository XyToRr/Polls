CREATE PROCEDURE [dbo].[GetPollById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT Id, Title, Description, OwnerUserId, Algorithm, 
           StartDate, EndDate, IsAnonymous, ClosedManually, CreatedAt
    FROM Polls
    WHERE Id = @Id
END