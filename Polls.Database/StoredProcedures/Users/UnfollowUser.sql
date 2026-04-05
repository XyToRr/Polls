CREATE PROCEDURE [dbo].[UnfollowUser]
    @FollowerId UNIQUEIDENTIFIER,
    @AuthorId   UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM UserFollows
    WHERE FollowerId = @FollowerId
      AND AuthorId = @AuthorId
END