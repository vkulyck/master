CREATE TABLE [carma].[UserActivities] (
    [UserActivityID]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ActivityCalendarID]        UNIQUEIDENTIFIER NOT NULL REFERENCES [carma].[ActivityCalendars] ([stream_id]),
    [ActivityEventID]           UNIQUEIDENTIFIER NOT NULL,
    [ActivityID]                UNIQUEIDENTIFIER NOT NULL,
    [RegistrantID]              INT NOT NULL REFERENCES [carma].[Users] ([UserID]),
    [RegistrarID]               INT NOT NULL REFERENCES [carma].[Users] ([UserID]),
    [ActivityStart]             DATETIME NOT NULL,
    [DateRegistered]            DATETIME NOT NULL,
    [DateConfirmed]             DATETIME NULL,
    [Status]                    TINYINT NOT NULL DEFAULT(0),
    CONSTRAINT [UK_UserActivities] UNIQUE NONCLUSTERED ([ActivityID],[RegistrantID])
)