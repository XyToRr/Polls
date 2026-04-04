CREATE PROCEDURE [dbo].[CreatePoll]
    @Id          UNIQUEIDENTIFIER,
    @Title       NVARCHAR(50),
    @Description NVARCHAR(250) = NULL,
    @OwnerUserId UNIQUEIDENTIFIER,
    @Algorithm   INT,
    @StartDate   DATETIME,
    @EndDate     DATETIME = NULL,
    @IsAnonymous BIT = 0
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = @OwnerUserId)
        RETURN

    INSERT INTO Polls (Id, Title, Description, OwnerUserId, Algorithm, StartDate, EndDate, IsAnonymous)
    VALUES (@Id, @Title, @Description, @OwnerUserId, @Algorithm, @StartDate, @EndDate, @IsAnonymous)
END