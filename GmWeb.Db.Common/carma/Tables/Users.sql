CREATE TABLE [carma].[Users] (
    [UserID]                INT IDENTITY(1,1)   NOT NULL PRIMARY KEY, 
    [AgencyID]              INT              	NOT NULL REFERENCES [carma].[Agencies]([AgencyID]),
    [AccountID]             UNIQUEIDENTIFIER 	NULL,
    [LookupID]              UNIQUEIDENTIFIER 	DEFAULT NEWSEQUENTIALID() NOT NULL,
    [Email]                 NVARCHAR (50)   	NULL,
    [Title]                 NVARCHAR (50)    	NULL,
    [FirstName]             NVARCHAR (50)    	NULL,
    [LastName]              NVARCHAR (50)    	NOT NULL,
    [Phone]                 CHAR (15)      	    NULL,
    [UserRole]              INT                 NOT NULL,
    [Gender]                INT                	NOT NULL,
    [LanguageCode]          CHAR (3)      	    NOT NULL,
    [Profile]    NVARCHAR(MAX)       NOT NULL DEFAULT '{}', 
    [BirthDate] DATE NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [Idx_AccountID_UserID.LookupID]
    ON [carma].[Users]([AccountID] ASC)
    INCLUDE([UserID], [LookupID]);
