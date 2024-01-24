/*
Title: .NET CLR Assembly Import Script for SQL Server 2017

Description:

	This script performs *most* of the necessary steps to import an existing DLL
	into a SQL Server database for general function/procedure integration.

	Review the following notes before running this script:
	
	CREATE ASYMMETRIC KEY GmWebSigningKey 
	FROM EXECUTABLE FILE = '$(DLL_PATH)';
GO
	

	To avoid a stream of errors when you run this script, check/confirm the following:

		1. Go to the Query menu in SSMS and make sure "SQLCMD Mode" is enabled.
		2. Build *and sign* the assembly you wish to import in Visual Studio.
		3. Fill in the SQLCMD variables below (defined at each `:set` command)
			2a. SQL Server may not have access to some paths on your system by default. 
				When file access is denied you will see the following error:

					-----------------------------------------------------------------
					Msg 15208, Level 16, State 45, Line 20
					The certificate, asymmetric key, or private key file is not valid
					or does not exist; or you do not have permissions for it.
					-----------------------------------------------------------------
				
				You can retrieve the list of service accounts with the following query:

					-----------------------------------
					SELECT servicename, service_account
					FROM sys.dm_server_services
					GO
					-----------------------------------

		3. Set CATALOG to the catalog that will contain the assembly.
		4. Execute the script below. The process can be reversed with
		   the following commands:

				------------------------------------
				DROP ASSEMBLY [$(KEY_NAME)]
				DROP USER [$(KEY_LOGIN)]
				DROP LOGIN [$(KEY_LOGIN)]
				DROP ASYMMETRIC KEY [$(KEY_NAME)]
				------------------------------------
*/

:setvar DLL_NAME GmWeb.Common
:setvar DLL_DIRECTORY C:\Users\Jake\code\goodmojo\projects\GmWeb\GmWeb.Common\bin\Framework\Debug
:setvar CATALOG GMPLAY
:setvar KEY_NAME GmWebSigningKey
:setvar KEY_LOGIN GmWebSigningKeyLogin

USE master;
GO
EXEC sp_configure 'show advanced options', 1
RECONFIGURE
GO
EXEC sp_configure 'clr enabled', 1
RECONFIGURE
GO
EXEC sp_configure 'show advanced options', 0
RECONFIGURE
GO

PRINT 'Creating key/login for DLL trust'
PRINT '$(DLL_DIRECTORY)\$(DLL_NAME).dll'



USE master;
GO
CREATE ASYMMETRIC KEY $(KEY_NAME)
	FROM EXECUTABLE FILE = '$(DLL_DIRECTORY)\$(DLL_NAME).dll';
GO


CREATE LOGIN [$(KEY_LOGIN)] FROM ASYMMETRIC KEY [$(KEY_NAME)]
GO
GRANT UNSAFE ASSEMBLY TO [$(KEY_LOGIN)]
GO


USE [$(CATALOG)]
GO
CREATE USER [$(KEY_LOGIN)] FOR LOGIN [$(KEY_LOGIN)]
GO

CREATE ASSEMBLY [$(DLL_NAME)]
	FROM '$(DLL_DIRECTORY)\$(DLL_NAME).dll'
	WITH PERMISSION_SET = SAFE
GO

-- Symmetric Encryption
CREATE FUNCTION AesEncrypt(@Data NVARCHAR(MAX), @Key NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].AesEncrypt
GO

CREATE FUNCTION AesDecrypt(@Data NVARCHAR(MAX), @Key NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].AesDecrypt
GO

CREATE FUNCTION GenerateKey(@Seed NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].GenerateKey
GO

CREATE FUNCTION GenerateRandomKey()
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].GenerateRandomKey
GO

-- Base64 Encoding
CREATE FUNCTION BytesToBase64(@Bytes VARBINARY(MAX))
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].BytesToBase64
GO

CREATE FUNCTION Base64ToBytes(@Encoding NVARCHAR(MAX))
RETURNS VARBINARY(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].Base64ToBytes
GO

-- One-Way Hasing
CREATE FUNCTION ComputeDataHash(@Data NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].ComputeDataHash
GO

CREATE FUNCTION ValidateHashData(@Data NVARCHAR(MAX), @SaltHash NVARCHAR(MAX))
RETURNS BIT
AS
	EXTERNAL NAME [$(DLL_NAME)].[$(DLL_NAME).CLR.SqlExports].ValidateHashData
GO
