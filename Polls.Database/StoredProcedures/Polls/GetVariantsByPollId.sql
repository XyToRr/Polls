CREATE PROCEDURE [dbo].[GetVariantsByPollId]
    @PollId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        PollId,
        Text
    FROM dbo.Variants
    WHERE PollId = @PollId;
END
