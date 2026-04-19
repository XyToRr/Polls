CREATE PROCEDURE [dbo].[GetUserByLogin]
    @Login NVARCHAR(50)
AS
BEGIN
    SELECT Id, Name, LastName, Login, CreatedAt, Password
    FROM Users
    WHERE Login = @Login
END