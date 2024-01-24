USE master;
GO

:setvar DLL_NAME GmWeb.Common
:setvar DLL_DIRECTORY C:\Users\Jake\code\goodmojo\projects\GmWeb\GmWeb.Common\bin\Framework\Debug
:setvar CATALOG GMPLAY
:setvar KEY_NAME GmWebSigningKey
:setvar KEY_LOGIN GmWebSigningKeyLogin

USE [$(CATALOG)]
GO

PRINT 'Dropping objects'
DROP FUNCTION ValidateHashData
DROP FUNCTION ComputeDataHash
DROP FUNCTION Base64ToBytes
DROP FUNCTION BytesToBase64
DROP FUNCTION GenerateRandomKey
DROP FUNCTION GenerateKey
DROP FUNCTION AesDecrypt
DROP FUNCTION AesEncrypt

PRINT 'Dropping assembly'
DROP ASSEMBLY [$(DLL_NAME)]
GO

PRINT 'Dropping user'
DROP USER $(KEY_LOGIN)
GO

USE master
GO

PRINT 'Dropping login'
DROP LOGIN $(KEY_LOGIN)
GO

PRINT 'Dropping key'
DROP ASYMMETRIC KEY [$(KEY_NAME)]
GO

/*
PRINT 'Disabling CLR'

EXEC sp_configure 'show advanced options', 1
RECONFIGURE
GO
EXEC sp_configure 'clr enabled', 0
RECONFIGURE
GO
EXEC sp_configure 'show advanced options', 0
RECONFIGURE
GO
*/
