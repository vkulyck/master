CREATE TABLE [carma].[Comments]
(
    [CommentID]         UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [ThreadID]          UNIQUEIDENTIFIER NOT NULL REFERENCES [carma].[Threads](ThreadID) ON DELETE CASCADE,
    [ParentCommentID]   UNIQUEIDENTIFIER NULL REFERENCES [carma].[Comments](CommentID),
    [AuthorID]          INT NOT NULL REFERENCES [carma].[Users](UserID),
    [AgencyID]          INT NOT NULL REFERENCES [carma].[Agencies](AgencyID),
    [Created]           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [ContentHistory]    NVARCHAR(MAX) NOT NULL DEFAULT '{}'
)
