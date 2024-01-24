:setvar IIS_USR "IIS APPPOOL\DefaultAppPool"

USE [master]
GO

IF EXISTS(SELECT * FROM [master].dbo.syslogins WHERE LoginName = '$(IIS_USR)')
  RETURN
CREATE LOGIN [IIS APPPOOL\.NET v4.5] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
if exists(select * from sys.database_principals where name = 'foo')
--select * from sys.database_principals where name like  '%DefaultAppPool%'
select loginname from master.dbo.syslogins where name = '$(IIS_USR)'
return
/*
USE [master]
GO
CREATE LOGIN [IIS APPPOOL\.NET v4.5] FROM WINDOWS WITH DEFAULT_DATABASE=[master]
GO
use [GMLIVE];
GO
use [master];
GO
USE [GMLIVE]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [GMPLAY];
GO
use [GMLIVE];
GO
USE [GMPLAY]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [HumanResources];
GO
use [GMPLAY];
GO
USE [HumanResources]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
USE [HumanResources]
GO
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\.NET v4.5]
GO
USE [HumanResources]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\.NET v4.5]
GO
USE [HumanResources]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [IIS APPPOOL\.NET v4.5]
GO
use [master];
GO
use [HumanResources];
GO
USE [master]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [MOCD];
GO
use [master];
GO
USE [MOCD]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
USE [MOCD]
GO
ALTER ROLE [db_owner] ADD MEMBER [IIS APPPOOL\.NET v4.5]
GO
use [NPCOM];
GO
use [MOCD];
GO
USE [NPCOM]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [NPCOM.Profile];
GO
use [NPCOM];
GO
USE [NPCOM.Profile]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [TGSEWD];
GO
use [NPCOM.Profile];
GO
USE [TGSEWD]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [TGSEWDTST];
GO
use [TGSEWD];
GO
USE [TGSEWDTST]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
use [TGSSFO];
GO
use [TGSEWDTST];
GO
USE [TGSSFO]
GO
CREATE USER [IIS APPPOOL\.NET v4.5] FOR LOGIN [IIS APPPOOL\.NET v4.5]
GO
*/