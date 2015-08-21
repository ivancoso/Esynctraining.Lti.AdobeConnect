CREATE PROCEDURE deleteLmsCompanyWithDependencies
(
	@lmsCompanyId	INT
)
AS
BEGIN
  BEGIN TRY
    BEGIN TRANSACTION

	UPDATE CompanyLms
	SET adminUserId		= NULL,
		acTemplateScoId	= NULL,
		acScoId			= NULL
	WHERE companyLmsId = @lmsCompanyId

	DELETE
	FROM LmsCourseMeeting
	WHERE companyLmsId = @lmsCompanyId

	DELETE
	FROM SurveyQuestionResultAnswer
	WHERE surveyQuestionResultId IN
	(
		SELECT surveyQuestionResultId
		FROM SurveyQuestionResult 
		WHERE surveyResultId IN
		(
			SELECT surveyResultId
			FROM SurveyResult 
			WHERE lmsUserParametersId IN
			(
				SELECT lmsUserParametersId
				FROM LmsUserParameters
				WHERE lmsuserid IN
				(
					SELECT lmsuserid
					FROM LmsUser
					WHERE companyLmsId = @lmsCompanyId
				)
			)
		)
	)

	DELETE
	FROM SurveyQuestionResult
	WHERE surveyResultId IN
	(
		SELECT surveyResultId
		FROM SurveyResult 
		WHERE lmsUserParametersId IN
		(
			SELECT lmsUserParametersId
			FROM LmsUserParameters
			WHERE lmsuserid IN
			(
				SELECT lmsuserid
				FROM LmsUser
				WHERE companyLmsId = @lmsCompanyId
			)
		)
	)

	DELETE
	FROM SurveyResult 
	WHERE lmsUserParametersId IN
	(
		SELECT lmsUserParametersId
		FROM LmsUserParameters
		WHERE lmsuserid IN
		(
			SELECT lmsuserid
			FROM LmsUser
			WHERE companyLmsId = @lmsCompanyId
		)
	)

	DELETE
	FROM LmsUserParameters
	WHERE lmsuserid IN
	(
		SELECT lmsuserid
		FROM LmsUser
		WHERE companyLmsId = @lmsCompanyId
	)

	DELETE
	FROM OfficeHours
	WHERE lmsuserid IN
	(
		SELECT lmsuserid
		FROM LmsUser
		WHERE companyLmsId = @lmsCompanyId
	)

	DELETE
	FROM LmsUserSession
	WHERE lmsuserid IN
	(
		SELECT lmsuserid
		FROM LmsUser
		WHERE companyLmsId = @lmsCompanyId
	)

	DELETE
	FROM LmsUser
	WHERE companyLmsId = @lmsCompanyId

	DELETE
	FROM CompanyLms
	WHERE companyLmsId = @lmsCompanyId

    COMMIT TRANSACTION
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
    ROLLBACK TRANSACTION;
    THROW
  END CATCH
END
GO
