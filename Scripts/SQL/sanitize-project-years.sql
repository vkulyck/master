USE TGSSFO
GO

ALTER TABLE [tblProjects]
ALTER COLUMN [ProjectYear] CHAR(4) NULL
GO

UPDATE [TGSSFO].[dbo].[tblProjects] SET [ProjectYear] = NULL WHERE [ProjectYear] = 'NONE'
GO

ALTER TABLE [TGSSFO].[dbo].[tblProjects] 
ALTER COLUMN [ProjectYear] INT NULL
GO