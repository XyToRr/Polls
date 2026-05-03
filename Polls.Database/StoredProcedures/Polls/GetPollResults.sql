CREATE PROCEDURE [dbo].[GetPollResults]
    @PollId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT 
        var.Id    AS VariantId,
        var.Text  AS VariantText,
        vs.VoteId,
        vs.Rank
    FROM Variants var
    LEFT JOIN VoteSelections vs ON vs.VariantId = var.Id
    LEFT JOIN Votes v ON vs.VoteId = v.Id AND v.IsValid = 1
    WHERE var.PollId = @PollId
END