ALTER TABLE CompanyLicense
    ADD hasApi BIT NULL
GO

UPDATE CompanyLicense
SET hasApi = 0
GO

ALTER TABLE CompanyLicense
    ALTER COLUMN hasApi BIT NOT NULL
GO

						
ALTER TABLE QuizResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE QuizResult
SET participantName = RTRIM(LTRIM(participantName))
GO

ALTER TABLE AppletResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE AppletResult
SET participantName = RTRIM(LTRIM(participantName))
GO

ALTER TABLE TestResult
ALTER COLUMN participantName NVARCHAR(200) NOT NULL
GO
UPDATE TestResult
SET participantName = RTRIM(LTRIM(participantName))
GO


ALTER TABLE ACSession
ALTER COLUMN meetingURL NVARCHAR(500) NOT NULL
GO
UPDATE ACSession
SET meetingURL = RTRIM(LTRIM(meetingURL))
GO

ALTER TABLE TestQuestionResult
ALTER COLUMN question NVARCHAR(500) NOT NULL
GO
UPDATE TestQuestionResult
SET question = RTRIM(LTRIM(question))
GO