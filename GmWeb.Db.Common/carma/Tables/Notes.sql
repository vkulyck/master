CREATE TABLE [carma].[Notes]
(
    [NoteID]            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [AuthorID]          INT NOT NULL REFERENCES [carma].[Users](UserID),
    [SubjectID]         INT NULL REFERENCES [carma].[Users](UserID),
    [ModifiedByID]      INT NULL REFERENCES [carma].[Users](UserID),
    [Created]           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [Modified]          DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),
    [Title]             NVARCHAR(1024) NOT NULL,
    [Message]           VARCHAR(MAX) NOT NULL, 
    [Status]            TINYINT NOT NULL DEFAULT 0, 
    [IsFlagged]         AS CONVERT(BIT, [Status] & 0x01)
)
