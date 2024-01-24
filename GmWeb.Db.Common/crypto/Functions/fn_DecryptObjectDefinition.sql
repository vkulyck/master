/*
** RC4 functions
** Published at https://sqlperformance.com/2016/05/sql-performance/the-internals-of-with-encryption
** Based on http://www.sqlteam.com/forums/topic.asp?TOPIC_ID=76258
** by Peter Larsson (SwePeso)
*/

CREATE FUNCTION [crypto].[fn_DecryptObjectDefinition] (@ObjectName VARCHAR(MAX))  
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE
		-- Note: OBJECT_ID only works for schema-scoped objects
		@ObjectID integer = OBJECT_ID(@ObjectName),
		@FamilyGuid binary(16),
		@BinObjectID binary(4),
		@SubObjectID binary(2),
		@ImageVal varbinary(MAX),
		@RC4Key binary(20);
 
	-- Find the database family GUID
	SELECT @FamilyGuid = CONVERT(binary(16), DRS.family_guid)
	FROM sys.database_recovery_status AS DRS
	WHERE DRS.database_id = DB_ID();
 
	-- Convert object ID to little-endian binary(4)
	SET @BinObjectID = CONVERT(binary(4), REVERSE(CONVERT(binary(4), @ObjectID)));
 
	SELECT
		-- Read the encrypted value
		@ImageVal = SOV.imageval,
		-- Get the subobjid and convert to little-endian binary
		@SubObjectID = CONVERT(binary(2), REVERSE(CONVERT(binary(2), SOV.subobjid)))
	FROM sys.sysobjvalues AS SOV
	WHERE 
		SOV.[objid] = @ObjectID
		AND SOV.valclass = 1;
 
	-- Compute the RC4 initialization key
	SET @RC4Key = HASHBYTES('SHA1', @FamilyGuid + @BinObjectID + @SubObjectID);
 
	-- Apply the standard RC4 algorithm and
	-- convert the result back to nvarchar
	DECLARE @Definition NVARCHAR(MAX) = CONVERT
		(
			nvarchar(MAX),
			[crypto].[fn_Cipher]
			(
				@RC4Key,
				@ImageVal
			)
		);

	RETURN @Definition
END

