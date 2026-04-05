CREATE PROCEDURE [dbo].[CreatePollWithVariants]
    @Id          UNIQUEIDENTIFIER,
    @Title       NVARCHAR(50),
    @Description NVARCHAR(250) = NULL,
    @OwnerUserId UNIQUEIDENTIFIER,
    @Algorithm   INT,
    @StartDate   DATETIME,
    @EndDate     DATETIME = NULL,
    @IsAnonymous BIT = 0,
    @Variants    VariantList READONLY
AS
BEGIN
    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Polls (Id, Title, Description, OwnerUserId, Algorithm, StartDate, EndDate, IsAnonymous)
        VALUES (@Id, @Title, @Description, @OwnerUserId, @Algorithm, @StartDate, @EndDate, @IsAnonymous)

        INSERT INTO Variants (Id, PollId, Text)
        SELECT Id, @Id, Text FROM @Variants

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
    END CATCH
END