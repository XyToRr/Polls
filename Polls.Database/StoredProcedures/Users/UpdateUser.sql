CREATE PROCEDURE [dbo].[UpdateUser]
    @Id       UNIQUEIDENTIFIER,
    @Name     NVARCHAR(50),
    @LastName NVARCHAR(50) = NULL,
    @Login    NVARCHAR(50)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Login = @Login AND Id != @Id)  
        RETURN

    UPDATE Users
    SET Name     = @Name,
        LastName = @LastName,
        Login    = @Login
    WHERE Id = @Id
END