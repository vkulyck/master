DECLARE 
	@id INT = 1,
	@Email VARCHAR(MAX)

DECLARE @UserCursor as CURSOR;
 
SET @UserCursor = CURSOR FOR
SELECT UserEmailAddress FROM dbo.tblUser
 
OPEN @UserCursor;
FETCH NEXT FROM @UserCursor INTO @Email
 
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE dbo.tblUser 
	SET UserKeyID = @id
	WHERE UserEmailAddress = @Email

	SET @id = @id + 1

	FETCH NEXT FROM @UserCursor INTO @Email
END
 
CLOSE @UserCursor;
DEALLOCATE @UserCursor;

SELECT MAX(UserKeyID) FROM dbo.tblUser