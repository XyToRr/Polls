CREATE PROCEDURE [dbo].[ClosePoll]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    UPDATE Polls
    SET ClosedManually = 1
    WHERE Id = @Id
      AND EndDate > GETDATE()
      AND ClosedManually = 0
END