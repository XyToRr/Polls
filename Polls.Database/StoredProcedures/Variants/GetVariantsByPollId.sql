CREATE PROCEDURE [dbo].[GetVariantsByPollId]
    @PollId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT Id, PollId, Text
    FROM Variants
    WHERE PollId = @PollId
END