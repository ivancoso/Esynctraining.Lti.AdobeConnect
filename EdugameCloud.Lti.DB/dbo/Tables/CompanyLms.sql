CREATE TABLE [dbo].[CompanyLms] (
    [companyLmsId]         INT            IDENTITY (1, 1) NOT NULL,
    [companyId]            INT            NOT NULL,
    [lmsProviderId]        INT            NOT NULL,
    [acServer]             NVARCHAR (100) NOT NULL,
    [acUsername]           NVARCHAR (100) NOT NULL,
    [acPassword]           NVARCHAR (100) NOT NULL,
    [consumerKey]          NVARCHAR (100) NOT NULL,
    [sharedSecret]         NVARCHAR (100) NOT NULL,
    [createdBy]            INT            NOT NULL,
    [modifiedBy]           INT            NULL,
    [dateCreated]          DATE           NOT NULL,
    [dateModified]         DATE           NULL,
    [primaryColor]         NVARCHAR (50)  NULL,
    [adminUserId]          INT            NULL,
    [acScoId]              NVARCHAR (50)  NULL,
    [acTemplateScoId]      NVARCHAR (50)  NULL,
    [lmsDomain]            NVARCHAR (100) NULL,
    [showAnnouncements]    BIT            NULL,
    [title]                NVARCHAR (100) NULL,
    [useSSL]               BIT            NULL,
    [lastSignalId]         BIGINT         CONSTRAINT [DF_CompanyLms_lastSignalId] DEFAULT ((0)) NOT NULL,
    [useUserFolder]        BIT            NULL,
    [userFolderName]       NVARCHAR (50)  NULL,
    [canRemoveMeeting]     BIT            NULL,
    [canEditMeeting]       BIT            NULL,
    [isSettingsVisible]    BIT            NULL,
    [enableOfficeHours]    BIT            NULL,
    [enableStudyGroups]    BIT            NULL,
    [enableCourseMeetings] BIT            NULL,
    [showLmsHelp]          BIT            NULL,
    [showEGCHelp]          BIT            NULL,
    CONSTRAINT [PK_CompanyLms] PRIMARY KEY CLUSTERED ([companyLmsId] ASC),
    CONSTRAINT [FK_CompanyLms_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]),
    CONSTRAINT [FK_CompanyLms_LmsUser] FOREIGN KEY ([adminUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId])
);









