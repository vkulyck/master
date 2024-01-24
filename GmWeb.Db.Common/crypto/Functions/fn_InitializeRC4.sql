/*
** RC4 functions
** Published at https://sqlperformance.com/2016/05/sql-performance/the-internals-of-with-encryption
** Based on http://www.sqlteam.com/forums/topic.asp?TOPIC_ID=76258
** by Peter Larsson (SwePeso)
*/

CREATE FUNCTION [crypto].[fn_InitializeRC4]
(
    @Pwd varbinary(256)
)
RETURNS @Box table
(
    i tinyint PRIMARY KEY, 
    v tinyint NOT NULL
)
WITH SCHEMABINDING
AS
BEGIN
    DECLARE @Key table
    (
        i tinyint PRIMARY KEY,
        v tinyint NOT NULL
    );
 
    DECLARE
        @Index smallint = 0,
        @PwdLen tinyint = DATALENGTH(@Pwd);
 
    WHILE @Index <= 255
    BEGIN
        INSERT @Key
            (i, v)
        VALUES
            (@Index, CONVERT(tinyint, SUBSTRING(@Pwd, @Index % @PwdLen + 1, 1)));
 
        INSERT @Box (i, v)
        VALUES (@Index, @Index);
 
        SET @Index += 1;
    END;
 
    DECLARE
        @t tinyint = NULL,
        @b smallint = 0;
 
    SET @Index = 0;
 
    WHILE @Index <= 255
    BEGIN
        SELECT @b = (@b + b.v + k.v) % 256
        FROM @Box AS b
        JOIN @Key AS k
            ON k.i = b.i
        WHERE b.i = @Index;
 
        SELECT @t = b.v
        FROM @Box AS b
        WHERE b.i = @Index;
 
        UPDATE b1
        SET b1.v = (SELECT b2.v FROM @Box AS b2 WHERE b2.i = @b)
        FROM @Box AS b1
        WHERE b1.i = @Index;
 
        UPDATE @Box
        SET v = @t
        WHERE i = @b;
 
        SET @Index += 1;
    END;
 
    RETURN;
END;