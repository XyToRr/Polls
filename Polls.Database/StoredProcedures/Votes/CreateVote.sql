CREATE PROCEDURE [dbo].[CreateVote]
    @Id         UNIQUEIDENTIFIER,
    @PollId     UNIQUEIDENTIFIER,
    @UserId     UNIQUEIDENTIFIER,
    @CreatedAt  DATETIME,
    @Selections VoteSelectionList READONLY
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Votes (Id, PollId, UserId, IsValid, CreatedAt)
        VALUES (@Id, @PollId, @UserId, 1, @CreatedAt)

        INSERT INTO VoteSelections (VariantId, VoteId, Rank)
        SELECT VariantId, @Id, Rank FROM @Selections

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
    END CATCH
END