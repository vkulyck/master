CREATE TABLE [carma].[Threads]
(
    [ThreadID]          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    [AuthorID]          INT NOT NULL REFERENCES [carma].[Users](UserID),
    [SubjectID]         INT NULL REFERENCES [carma].[Users](UserID),
    [AgencyID]          INT NOT NULL REFERENCES [carma].[Agencies](AgencyID),
    [Created]           DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [TitleHistory]      NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [TitleModified]     AS (coalesce(CONVERT([datetimeoffset],Trim(json_value([TitleHistory],'$.Timestamps[0]'))),[Created])),
    [Title]             AS (json_value([TitleHistory],'$.CurrentValue')),
    [ContentHistory]    NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [ContentModified]   AS (coalesce(CONVERT([datetimeoffset],Trim(json_value([ContentHistory],'$.Timestamps[0]'))),[Created])),
    [Content]           AS (json_value([ContentHistory],'$.CurrentValue')),
    [Status]            TINYINT NOT NULL DEFAULT 0, 
    [IsFlagged]         AS CONVERT(BIT, [Status] & 0x01)
)
