/*
** RC4 functions
** Published at https://sqlperformance.com/2016/05/sql-performance/the-internals-of-with-encryption
** Based on http://www.sqlteam.com/forums/topic.asp?TOPIC_ID=76258
** by Peter Larsson (SwePeso)
*/



CREATE FUNCTION [crypto].[fn_Cipher]
(
    @Pwd varbinary(256),
    @Text varbinary(MAX)
)
RETURNS varbinary(MAX)
WITH 
    SCHEMABINDING, 
    RETURNS NULL ON NULL INPUT
AS
BEGIN
    DECLARE @Box AS table 
    (
        i tinyint PRIMARY KEY, 
        v tinyint NOT NULL
    );
 
    INSERT @Box
        (i, v)
    SELECT
        FIR.i, FIR.v
    FROM [crypto].[fn_InitializeRC4](@Pwd) AS FIR;
 
    DECLARE
        @Index integer = 1,
        @i smallint = 0,
        @j smallint = 0,
        @t tinyint = NULL,
        @k smallint = NULL,
        @CipherBy tinyint = NULL,
        @Cipher varbinary(MAX) = 0x;
 
    WHILE @Index <= DATALENGTH(@Text)
    BEGIN
        SET @i = (@i + 1) % 256;
 
        SELECT
            @j = (@j + b.v) % 256,
            @t = b.v
        FROM @Box AS b
        WHERE b.i = @i;
 
        UPDATE b
        SET b.v = (SELECT w.v FROM @Box AS w WHERE w.i = @j)
        FROM @Box AS b
        WHERE b.i = @i;
 
        UPDATE @Box
        SET v = @t
        WHERE i = @j;
 
        SELECT @k = b.v
        FROM @Box AS b
        WHERE b.i = @i;
 
        SELECT @k = (@k + b.v) % 256
        FROM @Box AS b
        WHERE b.i = @j;
 
        SELECT @k = b.v
        FROM @Box AS b
        WHERE b.i = @k;
 
        SELECT
            @CipherBy = CONVERT(tinyint, SUBSTRING(@Text, @Index, 1)) ^ @k,
            @Cipher = @Cipher + CONVERT(binary(1), @CipherBy);
 
        SET @Index += 1;
    END;
 
    RETURN @Cipher;
END;