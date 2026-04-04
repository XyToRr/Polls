CREATE PROCEDURE [dbo].[GetUserById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SELECT Id, Name, LastName, Login, CreatedAt
    FROM Users
    WHERE Id = @Id
END