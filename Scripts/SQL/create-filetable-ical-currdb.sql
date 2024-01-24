USE [GMFR]
GO

---------------------------------------------------------------------------------

ALTER DATABASE [GMFR]
ADD FILEGROUP [GMFR-FG] CONTAINS FILESTREAM;
GO

---------------------------------------------------------------------------------

DECLARE @GMFR_DATA_PATH VARCHAR(MAX) = (
	SELECT physical_name 
	FROM sys.database_files
	WHERE type_desc = 'ROWS'
)

DECLARE @GMFR_DIR_SEP_INDEX INT = LEN(@GMFR_DATA_PATH) - CHARINDEX(N'\',REVERSE(@GMFR_DATA_PATH))
DECLARE @GMFR_DATA_DIR NVARCHAR(MAX) = LEFT(@GMFR_DATA_PATH, @GMFR_DIR_SEP_INDEX)
DECLARE @GMFR_FS_PATH NVARCHAR(MAX) = CONCAT(@GMFR_DATA_DIR, N'\', N'GMFR-FS')

DECLARE @ADD_FILE_CMD VARCHAR(MAX) = '
	ALTER DATABASE [GMFR]
	ADD FILE ( NAME = ''GMFR-FS'', FILENAME = ' + QUOTENAME(@GMFR_FS_PATH, '''')
	+ ') TO FILEGROUP [GMFR-FG]
'
EXEC(@ADD_FILE_CMD)
GO

---------------------------------------------------------------------------------

ALTER DATABASE [GMFR]
SET FILESTREAM ( NON_TRANSACTED_ACCESS = FULL, DIRECTORY_NAME = 'GMFR-FS' )

---------------------------------------------------------------------------------

CREATE TABLE [agency].[EventCalendars]
AS FILETABLE
WITH 
(
    FileTable_Directory = 'EventCalendars',
    FileTable_Collate_Filename = database_default
);
GO

CREATE TABLE [agency].[ActivityCalendars]
AS FILETABLE
WITH 
(
    FileTable_Directory = 'ActivityCalendars',
    FileTable_Collate_Filename = database_default
);
GO

DECLARE @SampleCalendarFile VARCHAR(MAX) = '
BEGIN:VCALENDAR
BEGIN:VEVENT
SUMMARY:LA->NYC
DTSTAMP:20210521T190638Z
DTSTART;TZID=America/Los_Angeles:20210527T054500
DTEND;TZID=America/New_York:20210527T064500
LOCATION:Statue of Liberty
SEQUENCE:0
BEGIN:VALARM
TRIGGER:-PT1H
DESCRIPTION:Event reminder
ACTION:DISPLAY
END:VALARM
END:VEVENT
END:VCALENDAR
'
DECLARE @UserId VARCHAR(MAX) = (SELECT Id FROM [GmIdentity].[dbo].[AspNetUsers] WHERE Email  = 'A857478@greatersouthwest.org')
DECLARE @CalendarName VARCHAR(MAX) = CONCAT(@UserId, '.ics')
DECLARE @CalendarData VARBINARY(MAX) = CONVERT(VARBINARY(MAX), @SampleCalendarFile)

TRUNCATE TABLE [sched].[UserCalendars]

INSERT INTO [sched].[UserCalendars] (
	[name],
	[file_stream]
)
VALUES (@CalendarName, @CalendarData);
GO
DECLARE @UserId VARCHAR(MAX) = (SELECT Id FROM [GmIdentity].[dbo].[AspNetUsers] WHERE Email  = 'A857478@greatersouthwest.org')
DECLARE @InsertedCalendarFile VARCHAR(MAX) = (SELECT CONVERT(VARCHAR(MAX), [file_stream]) FROM [sched].[UserCalendars] WHERE [name] = @UserId + '.ics')

PRINT 'Inserted UserId:'
PRINT @UserId
PRINT ''
PRINT 'Inserted Calendar File:'
PRINT @InsertedCalendarFile
GO

