CREATE PROCEDURE [dbo].[getSurveyResultAnswers]
(
	@surveyResultIds AS XML
)
AS
BEGIN
	WITH UserIdsCTE AS
	(
		SELECT x.i.value('.[1]', 'int') as Id
		FROM @surveyResultIds.nodes('//Ids/Id') as x(i)
	)
	select sqra.surveyQuestionResultAnswerId,
		sqra.surveyQuestionResultId,
		sqra.value,
		sqra.surveyDistractorId,
		sqra.surveyDistractorAnswerId,
		ISNULL(sqr.questionId,0) as questionId, 
		ISNULL(sqr.questionTypeId,0) as questionTypeId
from SurveyQuestionResultAnswer sqra
	inner join UserIdsCTE ON UserIdsCTE.Id = sqra.surveyQuestionResultId
	left join SurveyQuestionResult sqr on sqra.surveyQuestionResultId = sqr.surveyQuestionResultId
END