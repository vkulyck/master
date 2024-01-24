USE GMPLAY
GO

DECLARE 
	@ResetUsers BIT = 0,
	@ResetClients BIT = 0,
	@SanitizeUsers BIT = 0,
	@SanitizeClients BIT = 0,
	@ReprocessUsers BIT = 0,
	@ReprocessClients BIT = 0,
	@SelectTestUsers BIT = 0,
	@SelectTestClients BIT = 0

DECLARE @Keyphrase NVARCHAR(MAX), @Key NVARCHAR(MAX)
SELECT @Keyphrase = N'test encryption key'
SELECT @Key = dbo.GenerateKey(@keyphrase)

PRINT 'Re-encrypting data with encryption key;'
PRINT 'Keyphrase: "' + @Keyphrase + '"'
PRINT 'Key: "' + @Key + '"'

IF @ResetUsers = 1
BEGIN
	PRINT 'Resettning tblUser'
	UPDATE dbo.tblUser
	SET
		UserFName = orig.UserFName,
		UserLName = orig.UserLName,
		UserPassword = orig.UserPassword
	FROM
		dbo.tblUser curr JOIN [GMPLAY-Original].dbo.tblUser orig ON curr.UserEmailAddress = orig.UserEmailAddress
END

IF @ResetClients = 1
BEGIN
	PRINT 'Resetting tblClient'
	UPDATE dbo.tblClient
	SET
		FirstName = orig.FirstName,
		LastName = orig.LastName,
		Birthday = orig.Birthday,
		Password = orig.Password
	FROM
		dbo.tblClient curr JOIN [GMPLAY-Original].dbo.tblClient orig ON curr.ClientID = orig.ClientID
END

IF @SanitizeUsers = 1
BEGIN
	PRINT 'Sanitizing tblUser'
	UPDATE dbo.tblUser
	SET
		UserPassword = CASE 
			WHEN dbo.fn_ClearText(UserPassword) IS NULL THEN dbo.fn_CipherText(' ')
			ELSE UserPassword
		END
END

IF @SanitizeClients = 1
BEGIN
	PRINT 'Sanitizing tblClient' -- Nothing to do
END

IF @ReprocessUsers = 1
BEGIN
	PRINT 'Re-encrypting/hashing tblUser'

	UPDATE dbo.tblUser
	SET
		UserFName = dbo.AesEncrypt(dbo.fn_ClearText(UserFName), @Key),
		UserLName = dbo.AesEncrypt(dbo.fn_ClearText(UserLName), @Key),
		UserPassword = dbo.ComputeDataHash(dbo.fn_ClearText(UserPassword))
END

IF @ReprocessClients = 1
BEGIN
	PRINT 'Re-encrypting/hashing tblClient'

	UPDATE dbo.tblClient
	SET
		FirstName = dbo.AesEncrypt(dbo.fn_ClearText(FirstName), @Key),
		LastName = dbo.AesEncrypt(dbo.fn_ClearText(LastName), @Key),
		Birthday = dbo.AesEncrypt(dbo.fn_ClearDateTime(Birthday), @Key),
		Password = dbo.ComputeDataHash(dbo.fn_ClearText(Password))
END

IF @SelectTestUsers = 1
BEGIN
	PRINT 'Selecting test users'
	SELECT
		'tblUser' AS [Source Table],
		dbo.AesDecrypt(curr.UserFName, @Key) AS [Decrypted First Name],
		dbo.AesDecrypt(curr.UserLName, @Key) AS [Decrypted Last Name],
		'N/A' AS [Decrypted Birthday],
		CASE dbo.ValidateHashData(dbo.fn_ClearText(orig.UserPassword), curr.UserPassword)
			WHEN 1 THEN 'Password hash validated.'
			ELSE 'Password hash validation failure.'
		END AS [Password Has Validation Results]
	FROM
		dbo.tblUser curr JOIN [GMPLAY-Original].dbo.tblUser orig ON curr.UserEmailAddress = orig.UserEmailAddress
	WHERE curr.UserEmailAddress = 'maneesh.chaku@austintexas.gov'
END

IF @SelectTestClients = 1
BEGIN
	PRINT 'Selecting test clients'
	SELECT
		'tblClient' AS [Source Table],
		dbo.AesDecrypt(curr.FirstName, @Key) AS [Decrypted First Name],
		dbo.AesDecrypt(curr.LastName, @Key) AS [Decrypted Last Name],
		dbo.AesDecrypt(curr.Birthday, @Key) AS [Decrypted Birthday],
		CASE dbo.ValidateHashData(dbo.fn_ClearText(orig.Password), curr.Password) 
			WHEN 1 THEN 'Password hash validated.'
			ELSE 'Password hash validation failure.'
		END AS [Password Has Validation Results]
	FROM
		dbo.tblClient curr JOIN [GMPLAY-Original].dbo.tblClient orig ON curr.ClientID = orig.ClientID
	WHERE curr.ClientID = 14
END