CREATE PROCEDURE [dbo].[getUsersByLmsCompanyId]
(
	@lmsCompanyId	INT,
	@userFilter		XML
)
AS
BEGIN

	-- NOTE: for test purpose only
	--SET @userFilter = '<users>
	--<user id="1" email="em1" login="l1" />
	--<user id="2" email="em2" login="l2" />
	--</users>'

	--SELECT T.x.value('@id', 'NVARCHAR(256)') AS id,
	--	   T.x.value('@email', 'NVARCHAR(256)') AS email,
	--	   T.x.value('@login', 'NVARCHAR(256)') AS login
	--FROM @userFilter.nodes('/users/user') T(x)​

	DECLARE @tmp AS TABLE
	(
		filter_user_id	NVARCHAR(256),
		lms_user_id		NVARCHAR(50),
		lmsUserId		INT
	)

	INSERT INTO @tmp(filter_user_id, lms_user_id, lmsUserId)
	SELECT flt.id, lms_usr.userId, lms_usr.lmsUserId
	FROM 
	(
		SELECT T.x.value('@id', 'NVARCHAR(256)') AS id,
			   T.x.value('@email', 'NVARCHAR(256)') AS email,
			   T.x.value('@login', 'NVARCHAR(256)') AS login
		FROM @userFilter.nodes('/users/user') AS T(x)​
	) flt
	LEFT OUTER JOIN LmsUser lms_usr
		ON lms_usr.userId = flt.id OR lms_usr.username = flt.login OR lms_usr.username = flt.email
	WHERE lms_usr.companyLmsId = @lmsCompanyId

	--UPDATE lms_usr
	--	SET lms_usr.userId = tmp.filter_user_id
	--FROM LmsUser lms_usr
	--	INNER JOIN @tmp tmp ON lms_usr.lmsUserId = tmp.lmsUserId
	--WHERE lms_usr.userId <> tmp.filter_user_id AND LEN(tmp.filter_user_id) > 1

	SELECT lms_usr.*
	FROM LmsUser lms_usr
		INNER JOIN @tmp tmp ON lms_usr.lmsUserId = tmp.lmsUserId

END