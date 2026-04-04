CREATE PROCEDURE [dbo].[DeleteUser]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM UserFollows 
    WHERE FollowerId = @UserId OR AuthorId = @UserId

    DELETE FROM PollBannedUsers WHERE UserId = @UserId

    DELETE vt FROM [dbo].[VOTES] vt
    INNER JOIN POLLS pl ON vt.PollId = pl.Id
    WHERE vt.[UserId] = @UserId
    AND pl.EndDate > GETDATE()
    AND pl.ClosedManually = 0

    DELETE FROM Users WHERE Id = @UserId
END