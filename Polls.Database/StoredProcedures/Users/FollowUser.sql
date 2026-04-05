CREATE PROCEDURE [dbo].[FollowUser]
    @FollowerId   UNIQUEIDENTIFIER,
    @AuthorId     UNIQUEIDENTIFIER,
    @StartFollowAt DATE
AS
BEGIN
    INSERT INTO UserFollows (FollowerId, AuthorId, StartFollowAt)
    VALUES (@FollowerId, @AuthorId, @StartFollowAt)
END