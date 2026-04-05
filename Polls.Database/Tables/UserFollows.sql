CREATE TABLE [dbo].[UserFollows]
(
    [FollowerId] UNIQUEIDENTIFIER NOT NULL, 
    [AuthorId] UNIQUEIDENTIFIER NOT NULL, 
    [StartFollowAt] DATE NULL DEFAULT GETDATE(), 
    
    CONSTRAINT [PK_UserFollows] PRIMARY KEY ([FollowerId], [AuthorId]),
    CONSTRAINT [FK_UserFollows_Follower] FOREIGN KEY ([FollowerId]) REFERENCES [Users]([Id]),
    CONSTRAINT [FK_UserFollows_Author]   FOREIGN KEY ([AuthorId])   REFERENCES [Users]([Id])
)
