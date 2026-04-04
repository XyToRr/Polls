CREATE PROCEDURE [dbo].[CreateUser]
    @Id        UNIQUEIDENTIFIER,
    @Name      NVARCHAR(50),
    @LastName  NVARCHAR(50) = NULL,
    @Password  NVARCHAR(256),
    @Login     NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE [Login] = @Login)
        RETURN

    INSERT INTO Users (Id, Name, LastName, Password, Login)
    VALUES (@Id, @Name, @LastName, @Password, @Login)
END