CREATE TABLE [carma].[UserConfigs] (
    [UserConfigID]          INT IDENTITY(1,1)   NOT NULL PRIMARY KEY, 
    [OwnerID]               INT              	NOT NULL REFERENCES [carma].[Users]([UserID]),
    [SubjectID]             INT              	NOT NULL REFERENCES [carma].[Users]([UserID]),
    [Status]                TINYINT             NOT NULL DEFAULT 0, 
    [IsStarred]             AS CONVERT(BIT, [Status] & 0x01),
    CONSTRAINT [UK_UserConfigs] UNIQUE NONCLUSTERED ([OwnerID],[SubjectID])
);
GO