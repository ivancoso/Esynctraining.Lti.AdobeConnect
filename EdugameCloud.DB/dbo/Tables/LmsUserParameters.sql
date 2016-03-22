CREATE TABLE [dbo].[LmsUserParameters] (
	[lmsUserParametersId]	INT				NOT NULL	IDENTITY (1, 1),
	[wstoken]				NVARCHAR(128)		NULL,
	[course]				INT				NOT NULL,
	[acId]					NVARCHAR(10)	NOT NULL,
	[lmsUserId]				INT					NULL,
	[companyLmsId]			INT					NULL,
	[courseName]			NVARCHAR(4000)		NULL,
	[userEmail]				NVARCHAR(254)		NULL,
	[lastLoggedIn]			DATETIME			NULL,
	CONSTRAINT [PK_LmsUserParameters] PRIMARY KEY CLUSTERED ([lmsUserParametersId] ASC),
	CONSTRAINT [FK_LmsUserParameters_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE,
	CONSTRAINT [FK_LmsUserParameters_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);







