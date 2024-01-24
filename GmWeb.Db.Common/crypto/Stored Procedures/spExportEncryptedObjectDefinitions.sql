CREATE PROCEDURE [crypto].[spExportEncryptedObjectDefinitions]
AS
BEGIN
	DECLARE 
		@ObjectName VARCHAR(MAX), 
		@ObjectType VARCHAR(MAX)
 
	DECLARE ObjectCursor CURSOR FOR
	SELECT 
		name, 
		CASE type_desc
			WHEN 'SQL_SCALAR_FUNCTION' THEN 'Function'
			WHEN 'SQL_STORED_PROCEDURE' THEN 'StoredProcedure'
			ELSE 'None'
		END 
	FROM sys.objects
	WHERE OBJECTPROPERTY([object_id], 'IsEncrypted') = 1

	OPEN ObjectCursor
 
	FETCH NEXT FROM ObjectCursor INTO @ObjectName, @ObjectType;
 
	WHILE @@FETCH_STATUS = 0
	BEGIN
		PRINT CONCAT('<NAME START>', @ObjectName, '<NAME END>')
		PRINT CONCAT('<OBJECT TYPE START>', @ObjectType, '<OBJECT TYPE END>')

		DECLARE @Definition NVARCHAR(MAX) = [crypto].[fn_DecryptObjectDefinition](@ObjectName)
		PRINT '<DEFINITION START>'

        ----------------------------------------------------------------------------------------------------------
		-- Replaced: PRINT @Definition
		-- PRINT only prints out the first 8000 characters, but this function is necessary for preserving 
        -- whitespace. To remedy the character limit, we instead break longer strings into chunks and print 
        -- the chunks individually. 
        -- Original source: 
        --  Question: https://stackoverflow.com/questions/7850477/how-to-print-varcharmax-using-print-statement
        --  Answer:   https://stackoverflow.com/a/14611173/258809
		----------------------------------------------------------------------------------------------------------
			DECLARE @CurrentEnd BIGINT; /* The ending position of the current chunk */
			DECLARE @Offset TINYINT;    /* Offset amount based on newline */
			DECLARE @OutputString NVARCHAR(MAX) = REPLACE(
                REPLACE(
                    @Definition,
                    char(13) + char(10), 
                    char(10)
                ), 
                char(13), 
                char(10)
            )

			WHILE LEN(@OutputString) > 1
			BEGIN
				IF CHARINDEX(CHAR(10), @OutputString) between 1 AND 4000
				BEGIN
					   SET @CurrentEnd =  CHARINDEX(char(10), @OutputString) -1
					   SET @Offset = 2
				END
				ELSE
				BEGIN
					   SET @CurrentEnd = 4000
					   SET @Offset = 1
				END   
				PRINT SUBSTRING(@OutputString, 1, @CurrentEnd) 
				SET @OutputString = SUBSTRING(@OutputString, @CurrentEnd+@Offset, LEN(@OutputString))   
			END
		----------------------------------------------------------------------------------------------------------

		PRINT '<DEFINITION END>'
		FETCH NEXT FROM ObjectCursor INTO @ObjectName, @ObjectType;
	END
	CLOSE ObjectCursor;
 
	DEALLOCATE ObjectCursor;
END