PRINT N'Creating [dbo].[AcCachePrincipal]...';


GO
CREATE TABLE [dbo].[AcCachePrincipal] (
    [acCachePrincipalId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCompanyId]       INT            NOT NULL,
    [accountId]          NVARCHAR (512) NULL,
    [displayId]          NVARCHAR (512) NULL,
    [email]              NVARCHAR (512) NULL,
    [firstName]          NVARCHAR (512) NULL,
    [hasChildren]        BIT            NULL,
    [isHidden]           BIT            NULL,
    [isPrimary]          BIT            NULL,
    [lastName]           NVARCHAR (512) NULL,
    [login]              NVARCHAR (512) NULL,
    [name]               NVARCHAR (512) NULL,
    [principalId]        NVARCHAR (512) NULL,
    [type]               NVARCHAR (512) NULL,
    CONSTRAINT [PK_AcCachePrincipal] PRIMARY KEY CLUSTERED ([acCachePrincipalId] ASC)
);


GO
PRINT N'Creating [dbo].[ACSession]...';


GO
CREATE TABLE [dbo].[ACSession] (
    [acSessionId]     INT            IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT            NOT NULL,
    [userId]          INT            NOT NULL,
    [acUserModeId]    INT            NOT NULL,
    [accountId]       INT            NOT NULL,
    [meetingURL]      NVARCHAR (500) NOT NULL,
    [scoId]           INT            NOT NULL,
    [dateCreated]     SMALLDATETIME  NOT NULL,
    [languageId]      INT            NOT NULL,
    [status]          INT            NOT NULL,
    [includeAcEmails] BIT            NULL,
    CONSTRAINT [PK_ACSession] PRIMARY KEY CLUSTERED ([acSessionId] ASC)
);


GO
PRINT N'Creating [dbo].[ACUserMode]...';


GO
CREATE TABLE [dbo].[ACUserMode] (
    [acUserModeId] INT              IDENTITY (1, 1) NOT NULL,
    [userMode]     VARCHAR (50)     NOT NULL,
    [imageId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ACUserMode] PRIMARY KEY CLUSTERED ([acUserModeId] ASC)
);


GO
PRINT N'Creating [dbo].[ACUserMode].[UI_ACUserMode_userMode]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_ACUserMode_userMode]
    ON [dbo].[ACUserMode]([userMode] ASC);


GO
PRINT N'Creating [dbo].[Address]...';


GO
CREATE TABLE [dbo].[Address] (
    [addressId]    INT            IDENTITY (1, 1) NOT NULL,
    [countryId]    INT            NULL,
    [stateId]      INT            NULL,
    [address1]     NVARCHAR (255) NULL,
    [address2]     NVARCHAR (255) NULL,
    [city]         NVARCHAR (255) NULL,
    [dateCreated]  SMALLDATETIME  NOT NULL,
    [dateModified] SMALLDATETIME  NOT NULL,
    [latitude]     FLOAT (53)     NULL,
    [longitude]    FLOAT (53)     NULL,
    [zip]          VARCHAR (30)   NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([addressId] ASC)
);


GO
PRINT N'Creating [dbo].[AppletItem]...';


GO
CREATE TABLE [dbo].[AppletItem] (
    [appletItemId]    INT           IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT           NULL,
    [appletName]      VARCHAR (50)  NOT NULL,
    [documentXML]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_AppletItem] PRIMARY KEY CLUSTERED ([appletItemId] ASC)
);


GO
PRINT N'Creating [dbo].[AppletResult]...';


GO
CREATE TABLE [dbo].[AppletResult] (
    [appletResultId]  INT            IDENTITY (1, 1) NOT NULL,
    [appletItemId]    INT            NOT NULL,
    [acSessionId]     INT            NOT NULL,
    [participantName] NVARCHAR (200) NOT NULL,
    [score]           INT            NOT NULL,
    [startTime]       DATETIME       NOT NULL,
    [endTime]         DATETIME       NOT NULL,
    [dateCreated]     SMALLDATETIME  NOT NULL,
    [isArchive]       BIT            NULL,
    [email]           NVARCHAR (500) NULL,
    CONSTRAINT [PK_AppletResult] PRIMARY KEY CLUSTERED ([appletResultId] ASC)
);


GO
PRINT N'Creating [dbo].[ApplicationVersion]...';


GO
CREATE TABLE [dbo].[ApplicationVersion] (
    [majorVersion] INT NOT NULL,
    [minorVersion] INT NOT NULL,
    CONSTRAINT [PK_ApplicationVersion] PRIMARY KEY CLUSTERED ([majorVersion] ASC, [minorVersion] ASC)
);


GO
PRINT N'Creating [dbo].[BuildVersion]...';


GO
CREATE TABLE [dbo].[BuildVersion] (
    [buildVersionId]     INT              IDENTITY (1, 1) NOT NULL,
    [buildNumber]        NVARCHAR (20)    NOT NULL,
    [buildVersionTypeId] INT              NOT NULL,
    [isActive]           BIT              NOT NULL,
    [descriptionSmall]   NVARCHAR (255)   NULL,
    [descriptionHTML]    NVARCHAR (MAX)   NULL,
    [dateCreated]        DATETIME         NOT NULL,
    [dateModified]       DATETIME         NOT NULL,
    [fileId]             UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ProductVersion] PRIMARY KEY CLUSTERED ([buildVersionId] ASC)
);


GO
PRINT N'Creating [dbo].[BuildVersionType]...';


GO
CREATE TABLE [dbo].[BuildVersionType] (
    [buildVersionTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [buildVersionType]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_ProductVersionType] PRIMARY KEY CLUSTERED ([buildVersionTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[Company]...';


GO
CREATE TABLE [dbo].[Company] (
    [companyId]        INT              IDENTITY (1, 1) NOT NULL,
    [companyName]      VARCHAR (50)     NOT NULL,
    [addressId]        INT              NULL,
    [status]           INT              NOT NULL,
    [dateCreated]      SMALLDATETIME    NOT NULL,
    [dateModified]     SMALLDATETIME    NOT NULL,
    [primaryContactId] INT              NULL,
    [companyThemeId]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([companyId] ASC)
);


GO
PRINT N'Creating [dbo].[CompanyLicense]...';


GO
CREATE TABLE [dbo].[CompanyLicense] (
    [companyLicenseId]       INT           IDENTITY (1, 1) NOT NULL,
    [companyId]              INT           NOT NULL,
    [licenseNumber]          VARCHAR (50)  NOT NULL,
    [domain]                 VARCHAR (100) NULL,
    [expiryDate]             SMALLDATETIME NOT NULL,
    [createdBy]              INT           NOT NULL,
    [modifiedBy]             INT           NOT NULL,
    [dateCreated]            SMALLDATETIME NOT NULL,
    [dateModified]           SMALLDATETIME NOT NULL,
    [totalLicensesCount]     INT           NOT NULL,
    [licenseStatus]          INT           NOT NULL,
    [dateStart]              DATETIME      NOT NULL,
    [totalParticipantsCount] INT           NOT NULL,
    [hasApi]                 BIT           NOT NULL,
    CONSTRAINT [PK_CompanyLicense] PRIMARY KEY CLUSTERED ([companyLicenseId] ASC)
);


GO
PRINT N'Creating [dbo].[CompanyLicenseHistory]...';


GO
CREATE TABLE [dbo].[CompanyLicenseHistory] (
    [companyLicenseHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [companyLicenseId]        INT           NOT NULL,
    [createdBy]               INT           NULL,
    [modifiedBy]              INT           NULL,
    [dateCreated]             SMALLDATETIME NOT NULL,
    [dateModified]            SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_CompanyLicenseHistory] PRIMARY KEY CLUSTERED ([companyLicenseHistoryId] ASC)
);


GO
PRINT N'Creating [dbo].[CompanyLms]...';


GO
CREATE TABLE [dbo].[CompanyLms] (
    [companyLmsId]            INT            IDENTITY (1, 1) NOT NULL,
    [companyId]               INT            NOT NULL,
    [lmsProviderId]           INT            NOT NULL,
    [acServer]                NVARCHAR (100) NOT NULL,
    [acUsername]              NVARCHAR (100) NOT NULL,
    [acPassword]              NVARCHAR (100) NOT NULL,
    [consumerKey]             NVARCHAR (100) NOT NULL,
    [sharedSecret]            NVARCHAR (100) NOT NULL,
    [createdBy]               INT            NOT NULL,
    [modifiedBy]              INT            NULL,
    [dateCreated]             DATE           NOT NULL,
    [dateModified]            DATE           NULL,
    [primaryColor]            NVARCHAR (50)  NULL,
    [adminUserId]             INT            NULL,
    [acScoId]                 NVARCHAR (50)  NULL,
    [acTemplateScoId]         NVARCHAR (50)  NULL,
    [lmsDomain]               NVARCHAR (100) NULL,
    [showAnnouncements]       BIT            NULL,
    [title]                   NVARCHAR (100) NULL,
    [useSSL]                  BIT            NULL,
    [lastSignalId]            BIGINT         NOT NULL,
    [useUserFolder]           BIT            NULL,
    [userFolderName]          NVARCHAR (50)  NULL,
    [canRemoveMeeting]        BIT            NULL,
    [canEditMeeting]          BIT            NULL,
    [isSettingsVisible]       BIT            NULL,
    [enableOfficeHours]       BIT            NULL,
    [enableStudyGroups]       BIT            NULL,
    [enableCourseMeetings]    BIT            NULL,
    [showLmsHelp]             BIT            NULL,
    [showEGCHelp]             BIT            NULL,
    [enableProxyToolMode]     BIT            NULL,
    [proxyToolSharedPassword] NVARCHAR (255) NULL,
    [acUsesEmailAsLogin]      BIT            NULL,
    [loginUsingCookie]        BIT            NULL,
    [addPrefixToMeetingName]  BIT            NULL,
    [isActive]                BIT            NOT NULL,
    CONSTRAINT [PK_CompanyLms] PRIMARY KEY CLUSTERED ([companyLmsId] ASC)
);


GO
PRINT N'Creating [dbo].[CompanyTheme]...';


GO
CREATE TABLE [dbo].[CompanyTheme] (
    [companyThemeId]             UNIQUEIDENTIFIER NOT NULL,
    [headerBackgroundColor]      VARCHAR (10)     NULL,
    [buttonColor]                VARCHAR (10)     NULL,
    [buttonTextColor]            VARCHAR (10)     NULL,
    [gridHeaderTextColor]        VARCHAR (10)     NULL,
    [gridHeaderBackgroundColor]  VARCHAR (10)     NULL,
    [gridRolloverColor]          VARCHAR (10)     NULL,
    [logoId]                     UNIQUEIDENTIFIER NULL,
    [loginHeaderTextColor]       VARCHAR (10)     NULL,
    [popupHeaderBackgroundColor] VARCHAR (10)     NULL,
    [popupHeaderTextColor]       VARCHAR (10)     NULL,
    [questionColor]              VARCHAR (10)     NULL,
    [questionHeaderColor]        VARCHAR (10)     NULL,
    [welcomeTextColor]           VARCHAR (10)     NULL,
    CONSTRAINT [PK_CompanyTheme] PRIMARY KEY CLUSTERED ([companyThemeId] ASC)
);


GO
PRINT N'Creating [dbo].[Country]...';


GO
CREATE TABLE [dbo].[Country] (
    [countryId]    INT             IDENTITY (1, 1) NOT NULL,
    [countryCode]  VARCHAR (3)     NOT NULL,
    [countryCode3] VARCHAR (4)     NOT NULL,
    [country]      NVARCHAR (255)  NOT NULL,
    [latitude]     DECIMAL (18, 7) NOT NULL,
    [longitude]    DECIMAL (18, 7) NOT NULL,
    [zoomLevel]    INT             NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([countryId] ASC)
);


GO
PRINT N'Creating [dbo].[Distractor]...';


GO
CREATE TABLE [dbo].[Distractor] (
    [distractorID]    INT              IDENTITY (1, 1) NOT NULL,
    [questionID]      INT              NULL,
    [distractor]      NVARCHAR (MAX)   NOT NULL,
    [distractorOrder] INT              NOT NULL,
    [score]           VARCHAR (50)     NULL,
    [isCorrect]       BIT              NULL,
    [createdBy]       INT              NULL,
    [modifiedBy]      INT              NULL,
    [dateCreated]     SMALLDATETIME    NOT NULL,
    [dateModified]    SMALLDATETIME    NOT NULL,
    [isActive]        BIT              NULL,
    [distractorType]  INT              NULL,
    [imageId]         UNIQUEIDENTIFIER NULL,
    [lmsAnswer]       NVARCHAR (100)   NULL,
    [lmsProviderId]   INT              NULL,
    [lmsAnswerId]     INT              NULL,
    CONSTRAINT [PK_Distractor] PRIMARY KEY CLUSTERED ([distractorID] ASC)
);


GO
PRINT N'Creating [dbo].[DistractorHistory]...';


GO
CREATE TABLE [dbo].[DistractorHistory] (
    [distractoryHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [distractorId]         INT           NOT NULL,
    [questionId]           INT           NULL,
    [imageId]              INT           NULL,
    [distractor]           VARCHAR (MAX) NOT NULL,
    [distractorOrder]      INT           NOT NULL,
    [score]                VARCHAR (50)  NULL,
    [isCorrect]            BIT           NULL,
    [createdBy]            INT           NULL,
    [modifiedBy]           INT           NULL,
    [dateCreated]          SMALLDATETIME NOT NULL,
    [dateModified]         SMALLDATETIME NOT NULL,
    [isActive]             BIT           NULL,
    CONSTRAINT [PK_DistractorHistory] PRIMARY KEY CLUSTERED ([distractoryHistoryId] ASC)
);


GO
PRINT N'Creating [dbo].[EmailHistory]...';


GO
CREATE TABLE [dbo].[EmailHistory] (
    [emailHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [sentTo]         NVARCHAR (50)  NOT NULL,
    [sentFrom]       NVARCHAR (50)  NOT NULL,
    [sentCC]         NVARCHAR (200) NULL,
    [sentBCC]        NVARCHAR (200) NULL,
    [subject]        NVARCHAR (500) NULL,
    [message]        NVARCHAR (MAX) NULL,
    [body]           NVARCHAR (MAX) NULL,
    [date]           DATETIME       NOT NULL,
    [userId]         INT            NULL,
    [sentToName]     NVARCHAR (100) NULL,
    [sentFromName]   NVARCHAR (100) NULL,
    [status]         INT            NOT NULL
);


GO
PRINT N'Creating [dbo].[ErrorReport]...';


GO
CREATE TABLE [dbo].[ErrorReport] (
    [userId]             INT           IDENTITY (1, 1) NOT NULL,
    [os]                 VARCHAR (50)  NULL,
    [flashVersion]       VARCHAR (50)  NULL,
    [message]            VARCHAR (MAX) NOT NULL,
    [applicationVersion] VARCHAR (50)  NOT NULL,
    [dateCreated]        SMALLDATETIME NULL,
    CONSTRAINT [PK_ErrorReport] PRIMARY KEY CLUSTERED ([userId] ASC)
);


GO
PRINT N'Creating [dbo].[File]...';


GO
CREATE TABLE [dbo].[File] (
    [fileId]      UNIQUEIDENTIFIER NOT NULL,
    [fileName]    NVARCHAR (255)   NOT NULL,
    [path]        VARCHAR (MAX)    NULL,
    [height]      INT              NULL,
    [width]       INT              NULL,
    [createdBy]   INT              NULL,
    [dateCreated] DATETIME         NOT NULL,
    [isActive]    BIT              NULL,
    [status]      INT              NULL,
    [x]           INT              NULL,
    [y]           INT              NULL,
    CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([fileId] ASC)
);


GO
PRINT N'Creating [dbo].[Language]...';


GO
CREATE TABLE [dbo].[Language] (
    [languageId]    INT            IDENTITY (1, 1) NOT NULL,
    [language]      NVARCHAR (100) NOT NULL,
    [twoLetterCode] CHAR (2)       NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([languageId] ASC)
);


GO
PRINT N'Creating [dbo].[Language].[UI_Language_language]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_Language_language]
    ON [dbo].[Language]([language] ASC);


GO
PRINT N'Creating [dbo].[LmsCalendarEvent]...';


GO
CREATE TABLE [dbo].[LmsCalendarEvent] (
    [lmsCalendarEventId] INT            IDENTITY (1, 1) NOT NULL,
    [eventId]            NVARCHAR (50)  NOT NULL,
    [name]               NVARCHAR (200) NOT NULL,
    [startDate]          DATETIME2 (7)  NOT NULL,
    [endDate]            DATETIME2 (7)  NOT NULL,
    [lmsCourseMeetingId] INT            NOT NULL,
    CONSTRAINT [PK_LmsCalendarEvent] PRIMARY KEY CLUSTERED ([lmsCalendarEventId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCompanyRoleMapping]...';


GO
CREATE TABLE [dbo].[LmsCompanyRoleMapping] (
    [lmsCompanyRoleMappingId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCompanyId]            INT            NOT NULL,
    [lmsRoleName]             NVARCHAR (100) NOT NULL,
    [isDefaultLmsRole]        BIT            NOT NULL,
    [acRole]                  INT            NOT NULL,
    CONSTRAINT [PK_LmsCompanyRoleMapping] PRIMARY KEY CLUSTERED ([lmsCompanyRoleMappingId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCompanyRoleMapping].[UI_LmsCompanyRoleMapping_lmsCompanyId_lmsRoleName]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCompanyRoleMapping_lmsCompanyId_lmsRoleName]
    ON [dbo].[LmsCompanyRoleMapping]([lmsCompanyId] ASC, [lmsRoleName] ASC);


GO
PRINT N'Creating [dbo].[LmsCompanySetting]...';


GO
CREATE TABLE [dbo].[LmsCompanySetting] (
    [lmsCompanySettingId] INT            IDENTITY (1, 1) NOT NULL,
    [lmsCompanyId]        INT            NOT NULL,
    [name]                NVARCHAR (100) NOT NULL,
    [value]               NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LmsCompanySetting] PRIMARY KEY CLUSTERED ([lmsCompanySettingId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCompanySetting].[UI_LmsCompanySetting_lmsCompanyId_name]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCompanySetting_lmsCompanyId_name]
    ON [dbo].[LmsCompanySetting]([lmsCompanyId] ASC, [name] ASC);


GO
PRINT N'Creating [dbo].[LmsCourseMeeting]...';


GO
CREATE TABLE [dbo].[LmsCourseMeeting] (
    [lmsCourseMeetingId]    INT             IDENTITY (1, 1) NOT NULL,
    [courseId]              INT             NOT NULL,
    [scoId]                 NVARCHAR (50)   NULL,
    [companyLmsId]          INT             NOT NULL,
    [lmsMeetingTypeId]      INT             NOT NULL,
    [officeHoursId]         INT             NULL,
    [ownerId]               INT             NULL,
    [meetingNameJson]       NVARCHAR (4000) NULL,
    [reused]                BIT             NULL,
    [sourceCourseMeetingId] INT             NULL,
    [audioProfileId]        NVARCHAR (50)   NULL,
    [enableDynamicProvisioning]	BIT NOT NULL,
    CONSTRAINT [PK_LmsCourseMeeting] PRIMARY KEY CLUSTERED ([lmsCourseMeetingId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCourseMeetingGuest]...';


GO
CREATE TABLE [dbo].[LmsCourseMeetingGuest] (
    [lmsCourseMeetingGuestId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsCourseMeetingId]      INT           NOT NULL,
    [principalId]             NVARCHAR (30) NOT NULL,
    CONSTRAINT [PK_LmsCourseMeetingGuest] PRIMARY KEY CLUSTERED ([lmsCourseMeetingGuestId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCourseMeetingGuest].[UI_LmsCourseMeetingGuest_lmsCourseMeetingId_principalId]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCourseMeetingGuest_lmsCourseMeetingId_principalId]
    ON [dbo].[LmsCourseMeetingGuest]([lmsCourseMeetingId] ASC, [principalId] ASC);


GO
PRINT N'Creating [dbo].[LmsCourseMeetingRecording]...';


GO
CREATE TABLE [dbo].[LmsCourseMeetingRecording] (
    [lmsCourseMeetingRecordingId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsCourseMeetingId]          INT           NOT NULL,
    [scoId]                       NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LmsCourseMeetingRecording] PRIMARY KEY CLUSTERED ([lmsCourseMeetingRecordingId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsCourseMeetingRecording].[UI_LmsCourseMeetingRecording_lmsCourseMeetingId_[scoId]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsCourseMeetingRecording_lmsCourseMeetingId_[scoId]
    ON [dbo].[LmsCourseMeetingRecording]([lmsCourseMeetingId] ASC, [scoId] ASC);


GO
PRINT N'Creating [dbo].[LmsMeetingType]...';


GO
CREATE TABLE [dbo].[LmsMeetingType] (
    [lmsMeetingTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [lmsMeetingTypeName] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_LmsMeetingType] PRIMARY KEY CLUSTERED ([lmsMeetingTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsMeetingType].[UI_LmsMeetingType_lmsMeetingTypeName]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsMeetingType_lmsMeetingTypeName]
    ON [dbo].[LmsMeetingType]([lmsMeetingTypeName] ASC);


GO
PRINT N'Creating [dbo].[LmsProvider]...';


GO
CREATE TABLE [dbo].[LmsProvider] (
    [lmsProviderId]    INT            NOT NULL,
    [lmsProvider]      NVARCHAR (50)  NOT NULL,
    [shortName]        NVARCHAR (50)  NOT NULL,
    [configurationUrl] NVARCHAR (100) NULL,
    [userGuideFileUrl] NVARCHAR (100) NULL,
    CONSTRAINT [PK_LmsProvider] PRIMARY KEY CLUSTERED ([lmsProviderId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsProvider].[UI_LmsProvider_lmsProvider]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsProvider_lmsProvider]
    ON [dbo].[LmsProvider]([lmsProvider] ASC);


GO
PRINT N'Creating [dbo].[LmsProvider].[UI_LmsProvider_shortName]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_LmsProvider_shortName]
    ON [dbo].[LmsProvider]([shortName] ASC);


GO
PRINT N'Creating [dbo].[LmsQuestionType]...';


GO
CREATE TABLE [dbo].[LmsQuestionType] (
    [lmsQuestionTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsProviderId]     INT           NOT NULL,
    [questionTypeId]    INT           NOT NULL,
    [lmsQuestionType]   NVARCHAR (50) NOT NULL,
    [subModuleId]       INT           NULL,
    CONSTRAINT [PK_LmsQuestionType] PRIMARY KEY CLUSTERED ([lmsQuestionTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsUser]...';


GO
CREATE TABLE [dbo].[LmsUser] (
    [lmsUserId]        INT            IDENTITY (1, 1) NOT NULL,
    [companyLmsId]     INT            NOT NULL,
    [userId]           NVARCHAR (50)  NOT NULL,
    [username]         NVARCHAR (50)  NULL,
    [password]         NVARCHAR (50)  NULL,
    [token]            NVARCHAR (100) NULL,
    [acConnectionMode] INT            NULL,
    [primaryColor]     NVARCHAR (50)  NULL,
    [principalId]      NVARCHAR (30)  NULL,
    [name]             NVARCHAR (100) NULL,
    [email]            NVARCHAR (100) NULL,
    [userIdExtended]   NVARCHAR (50)  NULL,
    [sharedKey]        NVARCHAR (MAX) NULL,
    [acPasswordData]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_LmsUser] PRIMARY KEY CLUSTERED ([lmsUserId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsUser].[IX_LmsUser_companyLmsId_userId]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_LmsUser_companyLmsId_userId]
    ON [dbo].[LmsUser]([companyLmsId] ASC, [userId] ASC);


GO
PRINT N'Creating [dbo].[LmsUserMeetingRole]...';


GO
CREATE TABLE [dbo].[LmsUserMeetingRole] (
    [lmsUserMeetingRoleId] INT           IDENTITY (1, 1) NOT NULL,
    [lmsUserId]            INT           NOT NULL,
    [lmsCourseMeetingId]   INT           NOT NULL,
    [lmsRole]              NVARCHAR (50) NULL,
    CONSTRAINT [PK_LmsUserMeetingRole_1] PRIMARY KEY CLUSTERED ([lmsUserMeetingRoleId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsUserParameters]...';


GO
CREATE TABLE [dbo].[LmsUserParameters] (
    [lmsUserParametersId] INT             IDENTITY (1, 1) NOT NULL,
    [wstoken]             NVARCHAR (128)  NULL,
    [course]              INT             NOT NULL,
    [acId]                NVARCHAR (10)   NOT NULL,
    [lmsUserId]           INT             NULL,
    [companyLmsId]        INT             NULL,
    [courseName]          NVARCHAR (4000) NULL,
    [userEmail]           NVARCHAR (254)  NULL,
    [lastLoggedIn]        DATETIME        NULL,
    CONSTRAINT [PK_LmsUserParameters] PRIMARY KEY CLUSTERED ([lmsUserParametersId] ASC)
);


GO
PRINT N'Creating [dbo].[LmsUserSession]...';


GO
CREATE TABLE [dbo].[LmsUserSession] (
    [lmsUserSessionId] UNIQUEIDENTIFIER NOT NULL,
    [sessionData]      NTEXT            NULL,
    [dateCreated]      DATETIME         NOT NULL,
    [dateModified]     DATETIME         NULL,
    [companyLmsId]     INT              NOT NULL,
    [lmsUserId]        INT              NULL,
    [lmsCourseId]      INT              NOT NULL,
    CONSTRAINT [PK_LmsUserSession] PRIMARY KEY CLUSTERED ([lmsUserSessionId] ASC)
);


GO
PRINT N'Creating [dbo].[Module]...';


GO
CREATE TABLE [dbo].[Module] (
    [moduleId]    INT           IDENTITY (1, 1) NOT NULL,
    [moduleName]  VARCHAR (50)  NOT NULL,
    [dateCreated] SMALLDATETIME NOT NULL,
    [isActive]    BIT           NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([moduleId] ASC)
);


GO
PRINT N'Creating [dbo].[NewsletterSubscription]...';


GO
CREATE TABLE [dbo].[NewsletterSubscription] (
    [newsLetterSubscriptionId] INT           IDENTITY (1, 1) NOT NULL,
    [email]                    NVARCHAR (50) NOT NULL,
    [isActive]                 BIT           NOT NULL,
    [dateSubscribed]           DATETIME      NOT NULL,
    [dateUnsubscribed]         DATETIME      NULL
);


GO
PRINT N'Creating [dbo].[OfficeHours]...';


GO
CREATE TABLE [dbo].[OfficeHours] (
    [officeHoursId] INT            IDENTITY (1, 1) NOT NULL,
    [hours]         NVARCHAR (100) NULL,
    [scoId]         NVARCHAR (50)  NOT NULL,
    [lmsUserId]     INT            NOT NULL,
    CONSTRAINT [PK_OfficeHours] PRIMARY KEY CLUSTERED ([officeHoursId] ASC)
);


GO
PRINT N'Creating [dbo].[OfficeHours].[UI_OfficeHours_lmsUserId]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_OfficeHours_lmsUserId]
    ON [dbo].[OfficeHours]([lmsUserId] ASC);


GO
PRINT N'Creating [dbo].[Question]...';


GO
CREATE TABLE [dbo].[Question] (
    [questionId]       INT              IDENTITY (1, 1) NOT NULL,
    [questionTypeId]   INT              NOT NULL,
    [subModuleItemId]  INT              NULL,
    [question]         NVARCHAR (2000)  NOT NULL,
    [questionOrder]    INT              NOT NULL,
    [instruction]      NVARCHAR (MAX)   NULL,
    [correctMessage]   NVARCHAR (MAX)   NULL,
    [correctReference] NVARCHAR (2000)  NULL,
    [incorrectMessage] NVARCHAR (MAX)   NULL,
    [hint]             NVARCHAR (MAX)   NULL,
    [createdBy]        INT              NULL,
    [modifiedBy]       INT              NULL,
    [dateCreated]      SMALLDATETIME    NOT NULL,
    [dateModified]     SMALLDATETIME    NOT NULL,
    [isActive]         BIT              NULL,
    [scoreValue]       INT              NOT NULL,
    [imageId]          UNIQUEIDENTIFIER NULL,
    [lmsQuestionId]    INT              NULL,
    [isMoodleSingle]   BIT              NULL,
    [lmsProviderId]    INT              NULL,
    [randomizeAnswers] BIT              NULL,
    [rows]             INT              NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([questionId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForLikert]...';


GO
CREATE TABLE [dbo].[QuestionForLikert] (
    [questionForLikertId] INT IDENTITY (1, 1) NOT NULL,
    [questionId]          INT NOT NULL,
    [allowOther]          BIT NULL,
    [pageNumber]          INT NULL,
    [isMandatory]         BIT NOT NULL,
    CONSTRAINT [PK_QuestionForLikert] PRIMARY KEY CLUSTERED ([questionForLikertId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForOpenAnswer]...';


GO
CREATE TABLE [dbo].[QuestionForOpenAnswer] (
    [questionForOpenAnswerId] INT            IDENTITY (1, 1) NOT NULL,
    [questionId]              INT            NOT NULL,
    [restrictions]            NVARCHAR (255) NULL,
    [pageNumber]              INT            NULL,
    [isMandatory]             BIT            NOT NULL,
    CONSTRAINT [PK_QuestionForOpenAnswer] PRIMARY KEY CLUSTERED ([questionForOpenAnswerId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForRate]...';


GO
CREATE TABLE [dbo].[QuestionForRate] (
    [questionForRateId]    INT            IDENTITY (1, 1) NOT NULL,
    [questionId]           INT            NOT NULL,
    [restrictions]         NVARCHAR (255) NULL,
    [allowOther]           BIT            NULL,
    [pageNumber]           INT            NULL,
    [isMandatory]          BIT            NOT NULL,
    [isAlwaysRateDropdown] BIT            NULL,
    CONSTRAINT [PK_QuestionForRate] PRIMARY KEY CLUSTERED ([questionForRateId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForSingleMultipleChoice]...';


GO
CREATE TABLE [dbo].[QuestionForSingleMultipleChoice] (
    [questionForSingleMultipleChoiceId] INT            IDENTITY (1, 1) NOT NULL,
    [questionId]                        INT            NOT NULL,
    [allowOther]                        BIT            NULL,
    [pageNumber]                        INT            NULL,
    [isMandatory]                       BIT            NOT NULL,
    [restrictions]                      NVARCHAR (255) NULL,
    CONSTRAINT [PK_QuestionForSingleMultipleChoice] PRIMARY KEY CLUSTERED ([questionForSingleMultipleChoiceId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForTrueFalse]...';


GO
CREATE TABLE [dbo].[QuestionForTrueFalse] (
    [questionForTrueFalseId] INT IDENTITY (1, 1) NOT NULL,
    [questionId]             INT NOT NULL,
    [pageNumber]             INT NULL,
    [isMandatory]            BIT NOT NULL,
    CONSTRAINT [PK_QuestionForTrueFalse] PRIMARY KEY CLUSTERED ([questionForTrueFalseId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionForWeightBucket]...';


GO
CREATE TABLE [dbo].[QuestionForWeightBucket] (
    [questionForWeightBucketId] INT             IDENTITY (1, 1) NOT NULL,
    [questionId]                INT             NOT NULL,
    [totalWeightBucket]         DECIMAL (18, 9) NULL,
    [weightBucketType]          INT             NULL,
    [allowOther]                BIT             NULL,
    [pageNumber]                INT             NULL,
    [isMandatory]               BIT             NOT NULL,
    CONSTRAINT [PK_QuestionForWeightBucket] PRIMARY KEY CLUSTERED ([questionForWeightBucketId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionHistory]...';


GO
CREATE TABLE [dbo].[QuestionHistory] (
    [questionHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [questionId]        INT           NOT NULL,
    [questionTypeId]    INT           NOT NULL,
    [subModuleItemId]   INT           NULL,
    [imageId]           INT           NULL,
    [question]          VARCHAR (50)  NOT NULL,
    [questionOrder]     INT           NOT NULL,
    [instruction]       VARCHAR (MAX) NULL,
    [incorrectMessage]  VARCHAR (MAX) NULL,
    [hint]              VARCHAR (MAX) NULL,
    [createdBy]         INT           NULL,
    [modifiedBy]        INT           NULL,
    [dateCreated]       SMALLDATETIME NOT NULL,
    [dateModified]      SMALLDATETIME NOT NULL,
    [isActive]          BIT           NULL,
    CONSTRAINT [PK_QuestionHistory] PRIMARY KEY CLUSTERED ([questionHistoryId] ASC)
);


GO
PRINT N'Creating [dbo].[QuestionType]...';


GO
CREATE TABLE [dbo].[QuestionType] (
    [questionTypeId]          INT            IDENTITY (1, 1) NOT NULL,
    [type]                    VARCHAR (50)   NOT NULL,
    [questionTypeOrder]       INT            NULL,
    [questionTypeDescription] VARCHAR (200)  NULL,
    [instruction]             VARCHAR (500)  NULL,
    [correctText]             VARCHAR (500)  NULL,
    [incorrectMessage]        VARCHAR (500)  NULL,
    [isActive]                BIT            NULL,
    [iconSource]              NVARCHAR (500) NULL,
    CONSTRAINT [PK_QuestionType] PRIMARY KEY CLUSTERED ([questionTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[Quiz]...';


GO
CREATE TABLE [dbo].[Quiz] (
    [quizId]          INT            IDENTITY (1, 1) NOT NULL,
    [subModuleItemId] INT            NULL,
    [quizFormatId]    INT            NULL,
    [scoreTypeId]     INT            NULL,
    [quizName]        NVARCHAR (100) NOT NULL,
    [description]     NVARCHAR (MAX) NULL,
    [lmsQuizId]       INT            NULL,
    [lmsProviderId]   INT            NULL,
    CONSTRAINT [PK_Quiz] PRIMARY KEY CLUSTERED ([quizId] ASC)
);


GO
PRINT N'Creating [dbo].[QuizFormat]...';


GO
CREATE TABLE [dbo].[QuizFormat] (
    [quizFormatId]   INT           IDENTITY (1, 1) NOT NULL,
    [quizFormatName] VARCHAR (50)  NULL,
    [dateCreated]    SMALLDATETIME NOT NULL,
    [isActive]       BIT           NULL,
    CONSTRAINT [PK_QuizFormat] PRIMARY KEY CLUSTERED ([quizFormatId] ASC)
);


GO
PRINT N'Creating [dbo].[QuizQuestionResult]...';


GO
CREATE TABLE [dbo].[QuizQuestionResult] (
    [quizQuestionResultId] INT             IDENTITY (1, 1) NOT NULL,
    [quizResultId]         INT             NOT NULL,
    [questionId]           INT             NOT NULL,
    [question]             NVARCHAR (1500) NOT NULL,
    [questionTypeId]       INT             NOT NULL,
    [isCorrect]            BIT             NOT NULL,
    CONSTRAINT [PK_QuizQuestionResult] PRIMARY KEY CLUSTERED ([quizQuestionResultId] ASC)
);


GO
PRINT N'Creating [dbo].[QuizResult]...';


GO
CREATE TABLE [dbo].[QuizResult] (
    [quizResultId]        INT            IDENTITY (1, 1) NOT NULL,
    [quizId]              INT            NOT NULL,
    [acSessionId]         INT            NOT NULL,
    [participantName]     NVARCHAR (200) NOT NULL,
    [score]               INT            NOT NULL,
    [startTime]           DATETIME       NOT NULL,
    [endTime]             DATETIME       NOT NULL,
    [dateCreated]         DATETIME       NOT NULL,
    [isArchive]           BIT            NULL,
    [email]               NVARCHAR (500) NULL,
    [lmsId]               INT            NULL,
    [acEmail]             NVARCHAR (500) NULL,
    [isCompleted]         BIT            NULL,
    [lmsUserParametersId] INT            NULL,
    CONSTRAINT [PK_QuizResult] PRIMARY KEY CLUSTERED ([quizResultId] ASC)
);


GO
PRINT N'Creating [dbo].[Schedule]...';


GO
CREATE TABLE [dbo].[Schedule] (
    [scheduleId]         INT      IDENTITY (1, 1) NOT NULL,
    [interval]           INT      NOT NULL,
    [nextRun]            DATETIME NOT NULL,
    [scheduleDescriptor] INT      NOT NULL,
    [scheduleType]       INT      NOT NULL,
    [isEnabled]          BIT      NOT NULL,
    CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED ([scheduleId] ASC)
);


GO
PRINT N'Creating [dbo].[ScoreType]...';


GO
CREATE TABLE [dbo].[ScoreType] (
    [scoreTypeId]  INT           IDENTITY (1, 1) NOT NULL,
    [scoreType]    VARCHAR (50)  NULL,
    [dateCreated]  SMALLDATETIME NOT NULL,
    [isActive]     BIT           NULL,
    [prefix]       VARCHAR (50)  NULL,
    [defaultValue] INT           NOT NULL,
    CONSTRAINT [PK_ScoreType] PRIMARY KEY CLUSTERED ([scoreTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[ServerStatus]...';


GO
CREATE TABLE [dbo].[ServerStatus] (
    [online]  BIT           NOT NULL,
    [message] VARCHAR (MAX) NULL
);


GO
PRINT N'Creating [dbo].[SNGroupDiscussion]...';


GO
CREATE TABLE [dbo].[SNGroupDiscussion] (
    [snGroupDiscussionId]  INT            IDENTITY (1, 1) NOT NULL,
    [acSessionId]          INT            NOT NULL,
    [groupDiscussionData]  NTEXT          NOT NULL,
    [groupDiscussionTitle] NVARCHAR (255) NULL,
    [dateCreated]          DATETIME       NOT NULL,
    [dateModified]         DATETIME       NULL,
    [isActive]             BIT            NOT NULL,
    CONSTRAINT [PK_SNGroupDiscussion] PRIMARY KEY CLUSTERED ([snGroupDiscussionId] ASC)
);


GO
PRINT N'Creating [dbo].[SNLink]...';


GO
CREATE TABLE [dbo].[SNLink] (
    [snLinkId]    INT             IDENTITY (1, 1) NOT NULL,
    [snProfileId] INT             NOT NULL,
    [linkName]    NVARCHAR (255)  NOT NULL,
    [linkValue]   NVARCHAR (2000) NULL,
    CONSTRAINT [PK_SNLink] PRIMARY KEY CLUSTERED ([snLinkId] ASC)
);


GO
PRINT N'Creating [dbo].[SNMapProvider]...';


GO
CREATE TABLE [dbo].[SNMapProvider] (
    [snMapProviderId] INT            IDENTITY (1, 1) NOT NULL,
    [mapProvider]     NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SNMapProvider] PRIMARY KEY CLUSTERED ([snMapProviderId] ASC)
);


GO
PRINT N'Creating [dbo].[SNMapSettings]...';


GO
CREATE TABLE [dbo].[SNMapSettings] (
    [snMapSettingsId] INT IDENTITY (1, 1) NOT NULL,
    [snMapProviderId] INT NULL,
    [zoomLevel]       INT NULL,
    [countryId]       INT NULL,
    CONSTRAINT [PK_SNProfileMapSettings] PRIMARY KEY CLUSTERED ([snMapSettingsId] ASC)
);


GO
PRINT N'Creating [dbo].[SNMember]...';


GO
CREATE TABLE [dbo].[SNMember] (
    [snMemberId]         INT            IDENTITY (1, 1) NOT NULL,
    [acSessionId]        INT            NOT NULL,
    [participant]        NVARCHAR (255) NOT NULL,
    [participantProfile] NTEXT          NULL,
    [dateCreated]        DATETIME       NULL,
    [isBlocked]          BIT            NOT NULL,
    CONSTRAINT [PK_SNSessionMember] PRIMARY KEY CLUSTERED ([snMemberId] ASC)
);


GO
PRINT N'Creating [dbo].[SNProfile]...';


GO
CREATE TABLE [dbo].[SNProfile] (
    [snProfileId]     INT            IDENTITY (1, 1) NOT NULL,
    [profileName]     NVARCHAR (255) NOT NULL,
    [userName]        NVARCHAR (255) NOT NULL,
    [jobTitle]        NVARCHAR (500) NULL,
    [addressId]       INT            NULL,
    [email]           NVARCHAR (255) NULL,
    [phone]           NVARCHAR (255) NULL,
    [about]           NTEXT          NULL,
    [snMapSettingsId] INT            NULL,
    [subModuleItemId] INT            NOT NULL,
    CONSTRAINT [PK_SNProfile] PRIMARY KEY CLUSTERED ([snProfileId] ASC)
);


GO
PRINT N'Creating [dbo].[SNProfileSNService]...';


GO
CREATE TABLE [dbo].[SNProfileSNService] (
    [snProfileSNServiceId] INT             IDENTITY (1, 1) NOT NULL,
    [snProfileId]          INT             NOT NULL,
    [snServiceId]          INT             NOT NULL,
    [isEnabled]            BIT             NOT NULL,
    [serviceUrl]           NVARCHAR (2000) NULL,
    CONSTRAINT [PK_SNProfileSNService] PRIMARY KEY CLUSTERED ([snProfileSNServiceId] ASC)
);


GO
PRINT N'Creating [dbo].[SNService]...';


GO
CREATE TABLE [dbo].[SNService] (
    [snServiceId]   INT            IDENTITY (1, 1) NOT NULL,
    [socialService] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SNService] PRIMARY KEY CLUSTERED ([snServiceId] ASC)
);


GO
PRINT N'Creating [dbo].[SocialUserTokens]...';


GO
CREATE TABLE [dbo].[SocialUserTokens] (
    [socialUserTokensId] INT             IDENTITY (1, 1) NOT NULL,
    [key]                NVARCHAR (255)  NULL,
    [userId]             INT             NULL,
    [token]              NVARCHAR (1000) NOT NULL,
    [secret]             NVARCHAR (1000) NULL,
    [provider]           NVARCHAR (500)  NULL,
    CONSTRAINT [PK_SocialUserTokens] PRIMARY KEY CLUSTERED ([socialUserTokensId] ASC)
);


GO
PRINT N'Creating [dbo].[State]...';


GO
CREATE TABLE [dbo].[State] (
    [stateId]   INT             IDENTITY (1, 1) NOT NULL,
    [stateCode] NVARCHAR (10)   NOT NULL,
    [stateName] NVARCHAR (50)   NOT NULL,
    [isActive]  BIT             NOT NULL,
    [latitude]  DECIMAL (18, 7) NOT NULL,
    [longitude] DECIMAL (18, 7) NOT NULL,
    [zoomLevel] INT             NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([stateId] ASC)
);


GO
PRINT N'Creating [dbo].[SubModule]...';


GO
CREATE TABLE [dbo].[SubModule] (
    [subModuleId]   INT           IDENTITY (1, 1) NOT NULL,
    [moduleID]      INT           NOT NULL,
    [subModuleName] VARCHAR (50)  NOT NULL,
    [dateCreated]   SMALLDATETIME NOT NULL,
    [isActive]      BIT           NULL,
    CONSTRAINT [PK_SubModule] PRIMARY KEY CLUSTERED ([subModuleId] ASC)
);


GO
PRINT N'Creating [dbo].[SubModuleCategory]...';


GO
CREATE TABLE [dbo].[SubModuleCategory] (
    [subModuleCategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [userId]              INT            NOT NULL,
    [subModuleId]         INT            NOT NULL,
    [categoryName]        NVARCHAR (255) NULL,
    [modifiedBy]          INT            NULL,
    [dateModified]        SMALLDATETIME  NOT NULL,
    [isActive]            BIT            NULL,
    [lmsCourseId]         INT            NULL,
    [lmsProviderId]       INT            NULL,
    [companyLmsId]        INT            NULL,
    CONSTRAINT [PK_SubModuleCategory] PRIMARY KEY CLUSTERED ([subModuleCategoryId] ASC)
);


GO
PRINT N'Creating [dbo].[SubModuleItem]...';


GO
CREATE TABLE [dbo].[SubModuleItem] (
    [subModuleItemId]     INT           IDENTITY (1, 1) NOT NULL,
    [subModuleCategoryId] INT           NOT NULL,
    [createdBy]           INT           NULL,
    [isShared]            BIT           NULL,
    [modifiedBy]          INT           NULL,
    [dateCreated]         SMALLDATETIME NOT NULL,
    [dateModified]        SMALLDATETIME NOT NULL,
    [isActive]            BIT           NULL,
    CONSTRAINT [PK_SubModuleItem] PRIMARY KEY CLUSTERED ([subModuleItemId] ASC)
);


GO
PRINT N'Creating [dbo].[SubModuleItemTheme]...';


GO
CREATE TABLE [dbo].[SubModuleItemTheme] (
    [subModuleItemThemeId] INT              IDENTITY (1, 1) NOT NULL,
    [bgColor]              VARCHAR (10)     NULL,
    [bgImageId]            UNIQUEIDENTIFIER NULL,
    [titleColor]           VARCHAR (10)     NULL,
    [questionColor]        VARCHAR (10)     NULL,
    [instructionColor]     VARCHAR (10)     NULL,
    [correctColor]         VARCHAR (10)     NULL,
    [incorrectColor]       VARCHAR (10)     NULL,
    [selectionColor]       VARCHAR (10)     NULL,
    [hintColor]            VARCHAR (10)     NULL,
    [subModuleItemId]      INT              NOT NULL,
    CONSTRAINT [PK_SubModuleItemTheme] PRIMARY KEY CLUSTERED ([subModuleItemThemeId] ASC)
);


GO
PRINT N'Creating [dbo].[SubscriptionHistoryLog]...';


GO
CREATE TABLE [dbo].[SubscriptionHistoryLog] (
    [subscriptionHistoryLogId] INT            IDENTITY (1, 1) NOT NULL,
    [subscriptionTag]          NVARCHAR (500) NOT NULL,
    [lastQueryTime]            DATETIME       NULL,
    [subscriptionId]           INT            NULL,
    CONSTRAINT [PK_SubscriptionHistoryLog] PRIMARY KEY CLUSTERED ([subscriptionHistoryLogId] ASC)
);


GO
PRINT N'Creating [dbo].[SubscriptionUpdate]...';


GO
CREATE TABLE [dbo].[SubscriptionUpdate] (
    [subscriptionUpdateId] INT             IDENTITY (1, 1) NOT NULL,
    [subscription_id]      INT             NOT NULL,
    [object]               NVARCHAR (20)   NOT NULL,
    [object_id]            NVARCHAR (1000) NOT NULL,
    [changed_aspect]       NVARCHAR (50)   NOT NULL,
    [time]                 NVARCHAR (255)  NOT NULL,
    [createdDate]          DATETIME        NOT NULL,
    CONSTRAINT [PK_SubscriptionUpdate] PRIMARY KEY CLUSTERED ([subscriptionUpdateId] ASC)
);


GO
PRINT N'Creating [dbo].[Survey]...';


GO
CREATE TABLE [dbo].[Survey] (
    [surveyId]             INT            IDENTITY (1, 1) NOT NULL,
    [subModuleItemId]      INT            NULL,
    [surveyName]           NVARCHAR (255) NOT NULL,
    [description]          NVARCHAR (MAX) NULL,
    [surveyGroupingTypeId] INT            NOT NULL,
    [lmsSurveyId]          INT            NULL,
    [lmsProviderId]        INT            NULL,
    CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED ([surveyId] ASC)
);


GO
PRINT N'Creating [dbo].[SurveyGroupingType]...';


GO
CREATE TABLE [dbo].[SurveyGroupingType] (
    [surveyGroupingTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [surveyGroupingType]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SurveyGroupingType] PRIMARY KEY CLUSTERED ([surveyGroupingTypeId] ASC)
);


GO
PRINT N'Creating [dbo].[SurveyQuestionResult]...';


GO
CREATE TABLE [dbo].[SurveyQuestionResult] (
    [surveyQuestionResultId] INT            IDENTITY (1, 1) NOT NULL,
    [surveyResultId]         INT            NOT NULL,
    [questionId]             INT            NOT NULL,
    [question]               NVARCHAR (500) NOT NULL,
    [questionTypeId]         INT            NOT NULL,
    [isCorrect]              BIT            NOT NULL,
    CONSTRAINT [PK_SurveyQuestionResult] PRIMARY KEY CLUSTERED ([surveyQuestionResultId] ASC)
);


GO
PRINT N'Creating [dbo].[SurveyQuestionResultAnswer]...';


GO
CREATE TABLE [dbo].[SurveyQuestionResultAnswer] (
    [surveyQuestionResultAnswerId] INT             IDENTITY (1, 1) NOT NULL,
    [surveyQuestionResultId]       INT             NOT NULL,
    [surveyDistractorId]           INT             NULL,
    [value]                        NVARCHAR (4000) NOT NULL,
    [surveyDistractorAnswerId]     INT             NULL,
    CONSTRAINT [PK_SurveyQuestionResultAnswer] PRIMARY KEY CLUSTERED ([surveyQuestionResultAnswerId] ASC)
);


GO
PRINT N'Creating [dbo].[SurveyResult]...';


GO
CREATE TABLE [dbo].[SurveyResult] (
    [surveyResultId]      INT            IDENTITY (1, 1) NOT NULL,
    [surveyId]            INT            NOT NULL,
    [acSessionId]         INT            NOT NULL,
    [participantName]     NVARCHAR (50)  NOT NULL,
    [score]               INT            NOT NULL,
    [startTime]           DATETIME       NOT NULL,
    [endTime]             DATETIME       NOT NULL,
    [dateCreated]         DATETIME       NOT NULL,
    [isArchive]           BIT            NULL,
    [email]               NVARCHAR (500) NULL,
    [lmsUserParametersId] INT            NULL,
    [acEmail]             NVARCHAR (500) NULL,
    CONSTRAINT [PK_SurveyResult] PRIMARY KEY CLUSTERED ([surveyResultId] ASC)
);


GO
PRINT N'Creating [dbo].[Test]...';


GO
CREATE TABLE [dbo].[Test] (
    [testId]                 INT             IDENTITY (1, 1) NOT NULL,
    [subModuleItemId]        INT             NULL,
    [scoreTypeId]            INT             NULL,
    [testName]               NVARCHAR (50)   NOT NULL,
    [description]            NVARCHAR (MAX)  NULL,
    [passingScore]           DECIMAL (18, 9) NULL,
    [timeLimit]              INT             NULL,
    [instructionTitle]       NVARCHAR (MAX)  NULL,
    [instructionDescription] NVARCHAR (MAX)  NULL,
    [scoreFormat]            VARCHAR (50)    NULL,
    CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED ([testId] ASC)
);


GO
PRINT N'Creating [dbo].[TestQuestionResult]...';


GO
CREATE TABLE [dbo].[TestQuestionResult] (
    [testQuestionResultId] INT            IDENTITY (1, 1) NOT NULL,
    [testResultId]         INT            NOT NULL,
    [questionId]           INT            NOT NULL,
    [question]             NVARCHAR (500) NOT NULL,
    [questionTypeId]       INT            NOT NULL,
    [isCorrect]            BIT            NOT NULL,
    CONSTRAINT [PK_TestQuestionResult] PRIMARY KEY CLUSTERED ([testQuestionResultId] ASC)
);


GO
PRINT N'Creating [dbo].[TestResult]...';


GO
CREATE TABLE [dbo].[TestResult] (
    [testResultId]    INT            IDENTITY (1, 1) NOT NULL,
    [testId]          INT            NOT NULL,
    [acSessionId]     INT            NOT NULL,
    [participantName] NVARCHAR (200) NOT NULL,
    [score]           INT            NOT NULL,
    [startTime]       DATETIME       NOT NULL,
    [endTime]         DATETIME       NOT NULL,
    [dateCreated]     DATETIME       NOT NULL,
    [isArchive]       BIT            NULL,
    [email]           NVARCHAR (500) NULL,
    [acEmail]         NVARCHAR (500) NULL,
    [isCompleted]     BIT            NULL,
    CONSTRAINT [PK_TestResult] PRIMARY KEY CLUSTERED ([testResultId] ASC)
);


GO
PRINT N'Creating [dbo].[Theme]...';


GO
CREATE TABLE [dbo].[Theme] (
    [themeId]      INT           IDENTITY (1, 1) NOT NULL,
    [themeName]    VARCHAR (50)  NOT NULL,
    [createdBy]    INT           NULL,
    [modifiedBy]   INT           NULL,
    [dateCreated]  SMALLDATETIME NOT NULL,
    [dateModified] SMALLDATETIME NOT NULL,
    [isActive]     BIT           NULL,
    CONSTRAINT [PK_Theme] PRIMARY KEY CLUSTERED ([themeId] ASC)
);


GO
PRINT N'Creating [dbo].[ThemeAttribute]...';


GO
CREATE TABLE [dbo].[ThemeAttribute] (
    [themeAttributeId]         INT           NOT NULL,
    [themeId]                  INT           NOT NULL,
    [themeOrder]               INT           NULL,
    [bgColor]                  CHAR (6)      NOT NULL,
    [titleColor]               CHAR (6)      NULL,
    [categoryColor]            CHAR (6)      NULL,
    [selectionColor]           CHAR (6)      NULL,
    [questionHintColor]        CHAR (6)      NULL,
    [questionTextColor]        CHAR (6)      NULL,
    [questionInstructionColor] CHAR (6)      NULL,
    [responseCorrectColor]     CHAR (6)      NULL,
    [responseIncorrectColor]   CHAR (6)      NULL,
    [distractorTextColor]      CHAR (6)      NULL,
    [createdBy]                INT           NULL,
    [modifiedBy]               INT           NULL,
    [dateCreated]              SMALLDATETIME NOT NULL,
    [dateModified]             SMALLDATETIME NOT NULL,
    [isActive]                 BIT           NULL
);


GO
PRINT N'Creating [dbo].[TimeZone]...';


GO
CREATE TABLE [dbo].[TimeZone] (
    [timeZoneId]      INT          IDENTITY (1, 1) NOT NULL,
    [timeZone]        VARCHAR (50) NOT NULL,
    [timeZoneGMTDiff] FLOAT (53)   NOT NULL,
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([timeZoneId] ASC)
);


GO
PRINT N'Creating [dbo].[TimeZone].[UI_TimeZone_timeZone]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UI_TimeZone_timeZone]
    ON [dbo].[TimeZone]([timeZone] ASC);


GO
PRINT N'Creating [dbo].[User]...';


GO
CREATE TABLE [dbo].[User] (
    [userId]                     INT              IDENTITY (1, 1) NOT NULL,
    [companyId]                  INT              NOT NULL,
    [languageId]                 INT              NOT NULL,
    [timeZoneId]                 INT              NOT NULL,
    [userRoleId]                 INT              NOT NULL,
    [firstName]                  VARCHAR (100)    NOT NULL,
    [lastName]                   VARCHAR (100)    NULL,
    [password]                   VARCHAR (100)    NULL,
    [email]                      VARCHAR (450)    NOT NULL,
    [createdBy]                  INT              NULL,
    [modifiedBy]                 INT              NULL,
    [dateCreated]                SMALLDATETIME    NOT NULL,
    [dateModified]               SMALLDATETIME    NOT NULL,
    [status]                     SMALLINT         NOT NULL,
    [sessionToken]               NVARCHAR (64)    NULL,
    [sessionTokenExpirationDate] DATETIME         NULL,
    [logoId]                     UNIQUEIDENTIFIER NULL,
    [isUnsubscribed]             BIT              NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([userId] ASC)
);


GO
PRINT N'Creating [dbo].[UserActivation]...';


GO
CREATE TABLE [dbo].[UserActivation] (
    [userActivationId] INT           IDENTITY (1, 1) NOT NULL,
    [userId]           INT           NOT NULL,
    [activationCode]   VARCHAR (50)  NOT NULL,
    [dateExpires]      SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_UserActivation] PRIMARY KEY CLUSTERED ([userActivationId] ASC)
);


GO
PRINT N'Creating [dbo].[UserLoginHistory]...';


GO
CREATE TABLE [dbo].[UserLoginHistory] (
    [userLoginHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [fromIP]             VARCHAR (50)  NULL,
    [userId]             INT           NOT NULL,
    [application]        VARCHAR (255) NULL,
    [dateCreated]        DATETIME      NOT NULL,
    CONSTRAINT [PK_ContactLoginHistory] PRIMARY KEY CLUSTERED ([userLoginHistoryId] ASC)
);


GO
PRINT N'Creating [dbo].[UserRole]...';


GO
CREATE TABLE [dbo].[UserRole] (
    [userRoleId] INT          IDENTITY (1, 1) NOT NULL,
    [userRole]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([userRoleId] ASC)
);


GO
PRINT N'Creating [dbo].[Webinar]...';


GO
CREATE TABLE [dbo].[Webinar] (
    [webinar_id]          INT           IDENTITY (1, 1) NOT NULL,
    [webinar_date]        SMALLDATETIME NULL,
    [webinar_host]        NCHAR (50)    NULL,
    [webinar_description] NCHAR (255)   NULL,
    CONSTRAINT [PK_Webinar] PRIMARY KEY CLUSTERED ([webinar_id] ASC)
);


GO
PRINT N'Creating [dbo].[DF_ACSession_dateCreated]...';
GO

ALTER TABLE [dbo].[LmsCourseMeeting]
    ADD CONSTRAINT [DF_LmsCourseMeeting_enableDynamicProvisioning] DEFAULT (0) FOR [enableDynamicProvisioning];


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [DF_ACSession_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_ACSession_languageId]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [DF_ACSession_languageId] DEFAULT ((5)) FOR [languageId];


GO
PRINT N'Creating [dbo].[DF_ACSession_status]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [DF_ACSession_status] DEFAULT ((2)) FOR [status];


GO
PRINT N'Creating [dbo].[DF_Address_DateCreated]...';


GO
ALTER TABLE [dbo].[Address]
    ADD CONSTRAINT [DF_Address_DateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Address_DateModified]...';


GO
ALTER TABLE [dbo].[Address]
    ADD CONSTRAINT [DF_Address_DateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_AppletResult_dateCreated]...';


GO
ALTER TABLE [dbo].[AppletResult]
    ADD CONSTRAINT [DF_AppletResult_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Company_isActive]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [DF_Company_isActive] DEFAULT ((0)) FOR [status];


GO
PRINT N'Creating [dbo].[DF_Company_dateCreated]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [DF_Company_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Company_dateModified]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [DF_Company_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_CompanyLicense_dateCreated]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [DF_CompanyLicense_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_CompanyLicense_dateModified]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [DF_CompanyLicense_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_CompanyLicense_totalLicensesCount]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [DF_CompanyLicense_totalLicensesCount] DEFAULT ((1)) FOR [totalLicensesCount];


GO
PRINT N'Creating [dbo].[DF_CompanyLicense_totalParticipantsCount]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [DF_CompanyLicense_totalParticipantsCount] DEFAULT ((100)) FOR [totalParticipantsCount];


GO
PRINT N'Creating [dbo].[DF_CompanyLicenseHistory_dateCreated]...';


GO
ALTER TABLE [dbo].[CompanyLicenseHistory]
    ADD CONSTRAINT [DF_CompanyLicenseHistory_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_CompanyLicenseHistory_dateModified]...';


GO
ALTER TABLE [dbo].[CompanyLicenseHistory]
    ADD CONSTRAINT [DF_CompanyLicenseHistory_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_CompanyLms_lastSignalId]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [DF_CompanyLms_lastSignalId] DEFAULT ((0)) FOR [lastSignalId];


GO
PRINT N'Creating [dbo].[DF_Country_latitude]...';


GO
ALTER TABLE [dbo].[Country]
    ADD CONSTRAINT [DF_Country_latitude] DEFAULT ((0)) FOR [latitude];


GO
PRINT N'Creating [dbo].[DF_Country_longitude]...';


GO
ALTER TABLE [dbo].[Country]
    ADD CONSTRAINT [DF_Country_longitude] DEFAULT ((0)) FOR [longitude];


GO
PRINT N'Creating [dbo].[DF_Country_zoomLevel]...';


GO
ALTER TABLE [dbo].[Country]
    ADD CONSTRAINT [DF_Country_zoomLevel] DEFAULT ((0)) FOR [zoomLevel];


GO
PRINT N'Creating [dbo].[DF__Distracto__IsCor__10566F31]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [DF__Distracto__IsCor__10566F31] DEFAULT ((0)) FOR [isCorrect];


GO
PRINT N'Creating [dbo].[DF_Distractor_dateCreated]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [DF_Distractor_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Distractor_dateModified]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [DF_Distractor_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_Distractor_isActive]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [DF_Distractor_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_DistractorHistory_isCorrect]...';


GO
ALTER TABLE [dbo].[DistractorHistory]
    ADD CONSTRAINT [DF_DistractorHistory_isCorrect] DEFAULT ((0)) FOR [isCorrect];


GO
PRINT N'Creating [dbo].[DF_DistractorHistory_dateCreated]...';


GO
ALTER TABLE [dbo].[DistractorHistory]
    ADD CONSTRAINT [DF_DistractorHistory_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_DistractorHistory_dateModified]...';


GO
ALTER TABLE [dbo].[DistractorHistory]
    ADD CONSTRAINT [DF_DistractorHistory_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_DistractorHistory_isActive]...';


GO
ALTER TABLE [dbo].[DistractorHistory]
    ADD CONSTRAINT [DF_DistractorHistory_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_ErrorReport_date]...';


GO
ALTER TABLE [dbo].[ErrorReport]
    ADD CONSTRAINT [DF_ErrorReport_date] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__Image__DateCreat__6E01572D]...';


GO
ALTER TABLE [dbo].[File]
    ADD CONSTRAINT [DF__Image__DateCreat__6E01572D] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__Image__IsActive__6EF57B66]...';


GO
ALTER TABLE [dbo].[File]
    ADD CONSTRAINT [DF__Image__IsActive__6EF57B66] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__Image__Status__6EF57B66]...';


GO
ALTER TABLE [dbo].[File]
    ADD CONSTRAINT [DF__Image__Status__6EF57B66] DEFAULT ((0)) FOR [status];


GO
PRINT N'Creating [dbo].[DF_LmsUser_acConnectionMode]...';


GO
ALTER TABLE [dbo].[LmsUser]
    ADD CONSTRAINT [DF_LmsUser_acConnectionMode] DEFAULT ((0)) FOR [acConnectionMode];


GO
PRINT N'Creating [dbo].[DF_Module_dateCreated]...';


GO
ALTER TABLE [dbo].[Module]
    ADD CONSTRAINT [DF_Module_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Module_isActive]...';


GO
ALTER TABLE [dbo].[Module]
    ADD CONSTRAINT [DF_Module_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__Question__DateCr__0B91BA14]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [DF__Question__DateCr__0B91BA14] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__Question__DateMo__0C85DE4D]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [DF__Question__DateMo__0C85DE4D] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF__Question__IsActi__0D7A0286]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [DF__Question__IsActi__0D7A0286] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_Question_scoreValue]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [DF_Question_scoreValue] DEFAULT ((0)) FOR [scoreValue];


GO
PRINT N'Creating [dbo].[DF_QuestionHistory_dateCreated]...';


GO
ALTER TABLE [dbo].[QuestionHistory]
    ADD CONSTRAINT [DF_QuestionHistory_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_QuestionHistory_dateModified]...';


GO
ALTER TABLE [dbo].[QuestionHistory]
    ADD CONSTRAINT [DF_QuestionHistory_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_QuestionHistory_isActive]...';


GO
ALTER TABLE [dbo].[QuestionHistory]
    ADD CONSTRAINT [DF_QuestionHistory_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__QuestionT__IsAct__1CBC4616]...';


GO
ALTER TABLE [dbo].[QuestionType]
    ADD CONSTRAINT [DF__QuestionT__IsAct__1CBC4616] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__QuizForma__DateC__75A278F5]...';


GO
ALTER TABLE [dbo].[QuizFormat]
    ADD CONSTRAINT [DF__QuizForma__DateC__75A278F5] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__QuizForma__IsAct__76969D2E]...';


GO
ALTER TABLE [dbo].[QuizFormat]
    ADD CONSTRAINT [DF__QuizForma__IsAct__76969D2E] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_QuizResult_dateCreated]...';


GO
ALTER TABLE [dbo].[QuizResult]
    ADD CONSTRAINT [DF_QuizResult_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__ScoreType__DateC__71D1E811]...';


GO
ALTER TABLE [dbo].[ScoreType]
    ADD CONSTRAINT [DF__ScoreType__DateC__71D1E811] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__ScoreType__IsAct__72C60C4A]...';


GO
ALTER TABLE [dbo].[ScoreType]
    ADD CONSTRAINT [DF__ScoreType__IsAct__72C60C4A] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_ScoreType_defaultValue]...';


GO
ALTER TABLE [dbo].[ScoreType]
    ADD CONSTRAINT [DF_ScoreType_defaultValue] DEFAULT ((10)) FOR [defaultValue];


GO
PRINT N'Creating [dbo].[DF_ServerStatus_online]...';


GO
ALTER TABLE [dbo].[ServerStatus]
    ADD CONSTRAINT [DF_ServerStatus_online] DEFAULT ((1)) FOR [online];


GO
PRINT N'Creating [dbo].[DF_SNSessionMember_isBlocked]...';


GO
ALTER TABLE [dbo].[SNMember]
    ADD CONSTRAINT [DF_SNSessionMember_isBlocked] DEFAULT ((0)) FOR [isBlocked];


GO
PRINT N'Creating [dbo].[DF_State_latitude]...';


GO
ALTER TABLE [dbo].[State]
    ADD CONSTRAINT [DF_State_latitude] DEFAULT ((0)) FOR [latitude];


GO
PRINT N'Creating [dbo].[DF_State_longitude]...';


GO
ALTER TABLE [dbo].[State]
    ADD CONSTRAINT [DF_State_longitude] DEFAULT ((0)) FOR [longitude];


GO
PRINT N'Creating [dbo].[DF_State_zoomLevel]...';


GO
ALTER TABLE [dbo].[State]
    ADD CONSTRAINT [DF_State_zoomLevel] DEFAULT ((0)) FOR [zoomLevel];


GO
PRINT N'Creating [dbo].[DF__SubModule__DateC__5AEE82B9]...';


GO
ALTER TABLE [dbo].[SubModule]
    ADD CONSTRAINT [DF__SubModule__DateC__5AEE82B9] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__SubModule__IsAct__5CD6CB2B]...';


GO
ALTER TABLE [dbo].[SubModule]
    ADD CONSTRAINT [DF__SubModule__IsAct__5CD6CB2B] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__UserSubMo__DateM__3A81B327]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [DF__UserSubMo__DateM__3A81B327] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF__UserSubMo__IsAct__3B75D760]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [DF__UserSubMo__IsAct__3B75D760] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_SubModuleItem_subModuleCategoryId]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [DF_SubModuleItem_subModuleCategoryId] DEFAULT ((0)) FOR [subModuleCategoryId];


GO
PRINT N'Creating [dbo].[DF__SubModule__IsSha__3E52440B]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [DF__SubModule__IsSha__3E52440B] DEFAULT ((0)) FOR [isShared];


GO
PRINT N'Creating [dbo].[DF__SubModule__DateC__3F466844]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [DF__SubModule__DateC__3F466844] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__SubModule__DateM__403A8C7D]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [DF__SubModule__DateM__403A8C7D] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF__SubModule__IsAct__412EB0B6]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [DF__SubModule__IsAct__412EB0B6] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_SurveyResult_dateCreated]...';


GO
ALTER TABLE [dbo].[SurveyResult]
    ADD CONSTRAINT [DF_SurveyResult_dateCreated] DEFAULT (GETDATE()) FOR [startTime];


GO
PRINT N'Creating [dbo].[DF_SurveyResult_dateModified]...';


GO
ALTER TABLE [dbo].[SurveyResult]
    ADD CONSTRAINT [DF_SurveyResult_dateModified] DEFAULT (GETDATE()) FOR [endTime];


GO
PRINT N'Creating [dbo].[DF_TestResult_dateCreated]...';


GO
ALTER TABLE [dbo].[TestResult]
    ADD CONSTRAINT [DF_TestResult_dateCreated] DEFAULT (GETDATE()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Theme_dateCreated]...';


GO
ALTER TABLE [dbo].[Theme]
    ADD CONSTRAINT [DF_Theme_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_Theme_dateModified]...';


GO
ALTER TABLE [dbo].[Theme]
    ADD CONSTRAINT [DF_Theme_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_Theme_isActive]...';


GO
ALTER TABLE [dbo].[Theme]
    ADD CONSTRAINT [DF_Theme_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF_ThemeAttribute_dateCreated]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [DF_ThemeAttribute_dateCreated] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF_ThemeAttribute_dateModified]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [DF_ThemeAttribute_dateModified] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_ThemeAttribute_isActive]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [DF_ThemeAttribute_isActive] DEFAULT ((0)) FOR [isActive];


GO
PRINT N'Creating [dbo].[DF__User__DateCreate__6477ECF3]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [DF__User__DateCreate__6477ECF3] DEFAULT (getdate()) FOR [dateCreated];


GO
PRINT N'Creating [dbo].[DF__User__DateModifi__656C112C]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [DF__User__DateModifi__656C112C] DEFAULT (getdate()) FOR [dateModified];


GO
PRINT N'Creating [dbo].[DF_User_status]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [DF_User_status] DEFAULT ((1)) FOR [status];


GO
PRINT N'Creating [dbo].[FK_AcCachePrincipal_CompanyLms]...';


GO
ALTER TABLE [dbo].[AcCachePrincipal]
    ADD CONSTRAINT [FK_AcCachePrincipal_CompanyLms] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_ACSession_ACUserMode]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [FK_ACSession_ACUserMode] FOREIGN KEY ([acUserModeId]) REFERENCES [dbo].[ACUserMode] ([acUserModeId]);


GO
PRINT N'Creating [dbo].[FK_ACSession_Language]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [FK_ACSession_Language] FOREIGN KEY ([languageId]) REFERENCES [dbo].[Language] ([languageId]);


GO
PRINT N'Creating [dbo].[FK_ACSession_SubModuleItem]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [FK_ACSession_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_ACSession_User]...';


GO
ALTER TABLE [dbo].[ACSession]
    ADD CONSTRAINT [FK_ACSession_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_ACUserMode_Image]...';


GO
ALTER TABLE [dbo].[ACUserMode]
    ADD CONSTRAINT [FK_ACUserMode_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_Address_Country]...';


GO
ALTER TABLE [dbo].[Address]
    ADD CONSTRAINT [FK_Address_Country] FOREIGN KEY ([countryId]) REFERENCES [dbo].[Country] ([countryId]);


GO
PRINT N'Creating [dbo].[FK_Address_State]...';


GO
ALTER TABLE [dbo].[Address]
    ADD CONSTRAINT [FK_Address_State] FOREIGN KEY ([stateId]) REFERENCES [dbo].[State] ([stateId]);


GO
PRINT N'Creating [dbo].[FK_AppletItem_SubModuleItem]...';


GO
ALTER TABLE [dbo].[AppletItem]
    ADD CONSTRAINT [FK_AppletItem_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_AppletResult_ACSession]...';


GO
ALTER TABLE [dbo].[AppletResult]
    ADD CONSTRAINT [FK_AppletResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_AppletResult_AppletItem]...';


GO
ALTER TABLE [dbo].[AppletResult]
    ADD CONSTRAINT [FK_AppletResult_AppletItem] FOREIGN KEY ([appletItemId]) REFERENCES [dbo].[AppletItem] ([appletItemId]);


GO
PRINT N'Creating [dbo].[FK_BuildVersion_BuildVersionType]...';


GO
ALTER TABLE [dbo].[BuildVersion]
    ADD CONSTRAINT [FK_BuildVersion_BuildVersionType] FOREIGN KEY ([buildVersionTypeId]) REFERENCES [dbo].[BuildVersionType] ([buildVersionTypeId]);


GO
PRINT N'Creating [dbo].[FK_BuildVersion_File]...';


GO
ALTER TABLE [dbo].[BuildVersion]
    ADD CONSTRAINT [FK_BuildVersion_File] FOREIGN KEY ([fileId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_Company_Address]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [FK_Company_Address] FOREIGN KEY ([addressId]) REFERENCES [dbo].[Address] ([addressId]);


GO
ALTER TABLE [dbo].[Company] NOCHECK CONSTRAINT [FK_Company_Address];


GO
PRINT N'Creating [dbo].[FK_Company_CompanyTheme]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [FK_Company_CompanyTheme] FOREIGN KEY ([companyThemeId]) REFERENCES [dbo].[CompanyTheme] ([companyThemeId]);


GO
PRINT N'Creating [dbo].[FK_Company_PrimaryContact]...';


GO
ALTER TABLE [dbo].[Company]
    ADD CONSTRAINT [FK_Company_PrimaryContact] FOREIGN KEY ([primaryContactId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLicense_Company]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [FK_CompanyLicense_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLicense_CreatedBy]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [FK_CompanyLicense_CreatedBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLicense_ModifiedBy]...';


GO
ALTER TABLE [dbo].[CompanyLicense]
    ADD CONSTRAINT [FK_CompanyLicense_ModifiedBy] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLicenseHistory_CompanyLicense]...';


GO
ALTER TABLE [dbo].[CompanyLicenseHistory]
    ADD CONSTRAINT [FK_CompanyLicenseHistory_CompanyLicense] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLms_Company]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [FK_CompanyLms_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_CompanyLms_LmsProvider]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [FK_CompanyLms_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLms_LmsUser]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [FK_CompanyLms_LmsUser] FOREIGN KEY ([adminUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLms_User]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [FK_CompanyLms_User] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_CompanyLms_User2]...';


GO
ALTER TABLE [dbo].[CompanyLms]
    ADD CONSTRAINT [FK_CompanyLms_User2] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]) ON DELETE SET NULL;


GO
PRINT N'Creating [dbo].[FK_CompanyTheme_File]...';


GO
ALTER TABLE [dbo].[CompanyTheme]
    ADD CONSTRAINT [FK_CompanyTheme_File] FOREIGN KEY ([logoId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_Distractor_Distractor]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_Distractor] FOREIGN KEY ([distractorID]) REFERENCES [dbo].[Distractor] ([distractorID]);


GO
PRINT N'Creating [dbo].[FK_Distractor_Image]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_Distractor_lmsProvider]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_lmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_Distractor_Question]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_Question] FOREIGN KEY ([questionID]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_Distractor_UserCreated]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_Distractor_UserModified]...';


GO
ALTER TABLE [dbo].[Distractor]
    ADD CONSTRAINT [FK_Distractor_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_DistractorHistory_Distractor]...';


GO
ALTER TABLE [dbo].[DistractorHistory]
    ADD CONSTRAINT [FK_DistractorHistory_Distractor] FOREIGN KEY ([distractorId]) REFERENCES [dbo].[Distractor] ([distractorID]);


GO
PRINT N'Creating [dbo].[FK_Image_User]...';


GO
ALTER TABLE [dbo].[File]
    ADD CONSTRAINT [FK_Image_User] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_LmsCalendarEvent_LmsCourseMeeting]...';


GO
ALTER TABLE [dbo].[LmsCalendarEvent]
    ADD CONSTRAINT [FK_LmsCalendarEvent_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]);


GO
PRINT N'Creating [dbo].[FK_LmsCompanyRoleMapping_LmsCompany]...';


GO
ALTER TABLE [dbo].[LmsCompanyRoleMapping]
    ADD CONSTRAINT [FK_LmsCompanyRoleMapping_LmsCompany] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsCompanySetting_LmsCompany]...';


GO
ALTER TABLE [dbo].[LmsCompanySetting]
    ADD CONSTRAINT [FK_LmsCompanySetting_LmsCompany] FOREIGN KEY ([lmsCompanyId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeeting_LmsMeetingType]...';


GO
ALTER TABLE [dbo].[LmsCourseMeeting]
    ADD CONSTRAINT [FK_LmsCourseMeeting_LmsMeetingType] FOREIGN KEY ([lmsMeetingTypeId]) REFERENCES [dbo].[LmsMeetingType] ([lmsMeetingTypeId]);


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeeting_CompanyLms]...';


GO
ALTER TABLE [dbo].[LmsCourseMeeting]
    ADD CONSTRAINT [FK_LmsCourseMeeting_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]);


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeeting_OfficeHours]...';


GO
ALTER TABLE [dbo].[LmsCourseMeeting]
    ADD CONSTRAINT [FK_LmsCourseMeeting_OfficeHours] FOREIGN KEY ([officeHoursId]) REFERENCES [dbo].[OfficeHours] ([officeHoursId]);


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeeting_LmsUser]...';


GO
ALTER TABLE [dbo].[LmsCourseMeeting]
    ADD CONSTRAINT [FK_LmsCourseMeeting_LmsUser] FOREIGN KEY ([ownerId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]);


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeetingGuest_LmsCourseMeeting]...';


GO
ALTER TABLE [dbo].[LmsCourseMeetingGuest]
    ADD CONSTRAINT [FK_LmsCourseMeetingGuest_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsCourseMeetingRecording_LmsCourseMeeting]...';


GO
ALTER TABLE [dbo].[LmsCourseMeetingRecording]
    ADD CONSTRAINT [FK_LmsCourseMeetingRecording_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]);


GO
PRINT N'Creating [dbo].[FK_LmsQuestionType_LmsProvider]...';


GO
ALTER TABLE [dbo].[LmsQuestionType]
    ADD CONSTRAINT [FK_LmsQuestionType_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_LmsQuestionType_QuestionType]...';


GO
ALTER TABLE [dbo].[LmsQuestionType]
    ADD CONSTRAINT [FK_LmsQuestionType_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]);


GO
PRINT N'Creating [dbo].[FK_LmsUser_CompanyLms]...';


GO
ALTER TABLE [dbo].[LmsUser]
    ADD CONSTRAINT [FK_LmsUser_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsUserMeetingRole_LmsCourseMeeting]...';


GO
ALTER TABLE [dbo].[LmsUserMeetingRole]
    ADD CONSTRAINT [FK_LmsUserMeetingRole_LmsCourseMeeting] FOREIGN KEY ([lmsCourseMeetingId]) REFERENCES [dbo].[LmsCourseMeeting] ([lmsCourseMeetingId]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsUserMeetingRole_LmsUser]...';


GO
ALTER TABLE [dbo].[LmsUserMeetingRole]
    ADD CONSTRAINT [FK_LmsUserMeetingRole_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]) ON DELETE CASCADE ON UPDATE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsUserParameters_CompanyLms]...';


GO
ALTER TABLE [dbo].[LmsUserParameters]
    ADD CONSTRAINT [FK_LmsUserParameters_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsUserParameters_LmsUser]...';


GO
ALTER TABLE [dbo].[LmsUserParameters]
    ADD CONSTRAINT [FK_LmsUserParameters_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]);


GO
PRINT N'Creating [dbo].[FK_LmsUserSession_CompanyLms]...';


GO
ALTER TABLE [dbo].[LmsUserSession]
    ADD CONSTRAINT [FK_LmsUserSession_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_LmsUserSession_LmsUser]...';


GO
ALTER TABLE [dbo].[LmsUserSession]
    ADD CONSTRAINT [FK_LmsUserSession_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]);


GO
PRINT N'Creating [dbo].[FK_OfficeHours_LmsUser]...';


GO
ALTER TABLE [dbo].[OfficeHours]
    ADD CONSTRAINT [FK_OfficeHours_LmsUser] FOREIGN KEY ([lmsUserId]) REFERENCES [dbo].[LmsUser] ([lmsUserId]);


GO
PRINT N'Creating [dbo].[FK_Question_Image]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [FK_Question_Image] FOREIGN KEY ([imageId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_Question_LmsProvider]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [FK_Question_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_Question_QuestionType]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [FK_Question_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]);


GO
PRINT N'Creating [dbo].[FK_Question_SubModuleItem]...';


GO
ALTER TABLE [dbo].[Question]
    ADD CONSTRAINT [FK_Question_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForLikert_Question]...';


GO
ALTER TABLE [dbo].[QuestionForLikert]
    ADD CONSTRAINT [FK_QuestionForLikert_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForOpenAnswer_Question]...';


GO
ALTER TABLE [dbo].[QuestionForOpenAnswer]
    ADD CONSTRAINT [FK_QuestionForOpenAnswer_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForRate_Question]...';


GO
ALTER TABLE [dbo].[QuestionForRate]
    ADD CONSTRAINT [FK_QuestionForRate_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForSingleMultipleChoice_Question]...';


GO
ALTER TABLE [dbo].[QuestionForSingleMultipleChoice]
    ADD CONSTRAINT [FK_QuestionForSingleMultipleChoice_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForTrueFalse_Question]...';


GO
ALTER TABLE [dbo].[QuestionForTrueFalse]
    ADD CONSTRAINT [FK_QuestionForTrueFalse_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionForWeightBucket_Question]...';


GO
ALTER TABLE [dbo].[QuestionForWeightBucket]
    ADD CONSTRAINT [FK_QuestionForWeightBucket_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuestionHistory_Question]...';


GO
ALTER TABLE [dbo].[QuestionHistory]
    ADD CONSTRAINT [FK_QuestionHistory_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_Quiz_LmsProvider]...';


GO
ALTER TABLE [dbo].[Quiz]
    ADD CONSTRAINT [FK_Quiz_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_Quiz_QuizFormat]...';


GO
ALTER TABLE [dbo].[Quiz]
    ADD CONSTRAINT [FK_Quiz_QuizFormat] FOREIGN KEY ([quizFormatId]) REFERENCES [dbo].[QuizFormat] ([quizFormatId]);


GO
PRINT N'Creating [dbo].[FK_Quiz_ScoreType]...';


GO
ALTER TABLE [dbo].[Quiz]
    ADD CONSTRAINT [FK_Quiz_ScoreType] FOREIGN KEY ([scoreTypeId]) REFERENCES [dbo].[ScoreType] ([scoreTypeId]);


GO
PRINT N'Creating [dbo].[FK_Quiz_SubModuleItem]...';


GO
ALTER TABLE [dbo].[Quiz]
    ADD CONSTRAINT [FK_Quiz_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_QuizQuestionResult_Question]...';


GO
ALTER TABLE [dbo].[QuizQuestionResult]
    ADD CONSTRAINT [FK_QuizQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_QuizQuestionResult_QuestionType]...';


GO
ALTER TABLE [dbo].[QuizQuestionResult]
    ADD CONSTRAINT [FK_QuizQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]);


GO
PRINT N'Creating [dbo].[FK_QuizQuestionResult_QuizResult]...';


GO
ALTER TABLE [dbo].[QuizQuestionResult]
    ADD CONSTRAINT [FK_QuizQuestionResult_QuizResult] FOREIGN KEY ([quizResultId]) REFERENCES [dbo].[QuizResult] ([quizResultId]);


GO
PRINT N'Creating [dbo].[FK_QuizResult_ACSession]...';


GO
ALTER TABLE [dbo].[QuizResult]
    ADD CONSTRAINT [FK_QuizResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_QuizResult_Quiz]...';


GO
ALTER TABLE [dbo].[QuizResult]
    ADD CONSTRAINT [FK_QuizResult_Quiz] FOREIGN KEY ([quizId]) REFERENCES [dbo].[Quiz] ([quizId]);


GO
PRINT N'Creating [dbo].[FK_SNGroupDiscussion_ACSession]...';


GO
ALTER TABLE [dbo].[SNGroupDiscussion]
    ADD CONSTRAINT [FK_SNGroupDiscussion_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_SNLink_SNProfile]...';


GO
ALTER TABLE [dbo].[SNLink]
    ADD CONSTRAINT [FK_SNLink_SNProfile] FOREIGN KEY ([snProfileId]) REFERENCES [dbo].[SNProfile] ([snProfileId]);


GO
PRINT N'Creating [dbo].[fk_SNMapSettings_Country]...';


GO
ALTER TABLE [dbo].[SNMapSettings]
    ADD CONSTRAINT [fk_SNMapSettings_Country] FOREIGN KEY ([countryId]) REFERENCES [dbo].[Country] ([countryId]);


GO
PRINT N'Creating [dbo].[FK_SNProfileMapSettings_SNMapProvider]...';


GO
ALTER TABLE [dbo].[SNMapSettings]
    ADD CONSTRAINT [FK_SNProfileMapSettings_SNMapProvider] FOREIGN KEY ([snMapProviderId]) REFERENCES [dbo].[SNMapProvider] ([snMapProviderId]);


GO
PRINT N'Creating [dbo].[FK_SNMember_ACSession]...';


GO
ALTER TABLE [dbo].[SNMember]
    ADD CONSTRAINT [FK_SNMember_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_SNProfile_Address]...';


GO
ALTER TABLE [dbo].[SNProfile]
    ADD CONSTRAINT [FK_SNProfile_Address] FOREIGN KEY ([addressId]) REFERENCES [dbo].[Address] ([addressId]);


GO
PRINT N'Creating [dbo].[FK_SNProfile_SNMapSettings]...';


GO
ALTER TABLE [dbo].[SNProfile]
    ADD CONSTRAINT [FK_SNProfile_SNMapSettings] FOREIGN KEY ([snMapSettingsId]) REFERENCES [dbo].[SNMapSettings] ([snMapSettingsId]);


GO
PRINT N'Creating [dbo].[FK_SNProfile_SubModuleItem]...';


GO
ALTER TABLE [dbo].[SNProfile]
    ADD CONSTRAINT [FK_SNProfile_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_SNProfileSNService_SNProfile]...';


GO
ALTER TABLE [dbo].[SNProfileSNService]
    ADD CONSTRAINT [FK_SNProfileSNService_SNProfile] FOREIGN KEY ([snProfileId]) REFERENCES [dbo].[SNProfile] ([snProfileId]);


GO
PRINT N'Creating [dbo].[FK_SNProfileSNService_SNService]...';


GO
ALTER TABLE [dbo].[SNProfileSNService]
    ADD CONSTRAINT [FK_SNProfileSNService_SNService] FOREIGN KEY ([snServiceId]) REFERENCES [dbo].[SNService] ([snServiceId]);


GO
PRINT N'Creating [dbo].[FK_SocialUserTokens_User]...';


GO
ALTER TABLE [dbo].[SocialUserTokens]
    ADD CONSTRAINT [FK_SocialUserTokens_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_SubModule_Module]...';


GO
ALTER TABLE [dbo].[SubModule]
    ADD CONSTRAINT [FK_SubModule_Module] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[Module] ([moduleId]);


GO
PRINT N'Creating [dbo].[FK_SubModuleCategory_CompanyLms]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [FK_SubModuleCategory_CompanyLms] FOREIGN KEY ([companyLmsId]) REFERENCES [dbo].[CompanyLms] ([companyLmsId]) ON DELETE SET NULL;


GO
PRINT N'Creating [dbo].[FK_SubModuleCategory_LmsProvider]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [FK_SubModuleCategory_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_SubModuleCategory_User]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [FK_SubModuleCategory_User] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_UserSubModuleCategory_SubModule]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [FK_UserSubModuleCategory_SubModule] FOREIGN KEY ([subModuleId]) REFERENCES [dbo].[SubModule] ([subModuleId]);


GO
PRINT N'Creating [dbo].[FK_UserSubModuleCategory_User]...';


GO
ALTER TABLE [dbo].[SubModuleCategory]
    ADD CONSTRAINT [FK_UserSubModuleCategory_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_SubModuleItem_SubModuleCategory]...';


GO
ALTER TABLE [dbo].[SubModuleItem]
    ADD CONSTRAINT [FK_SubModuleItem_SubModuleCategory] FOREIGN KEY ([subModuleCategoryId]) REFERENCES [dbo].[SubModuleCategory] ([subModuleCategoryId]);


GO
PRINT N'Creating [dbo].[FK_SubModuleItemTheme_File]...';


GO
ALTER TABLE [dbo].[SubModuleItemTheme]
    ADD CONSTRAINT [FK_SubModuleItemTheme_File] FOREIGN KEY ([bgImageId]) REFERENCES [dbo].[File] ([fileId]);


GO
PRINT N'Creating [dbo].[FK_SubModuleItemTheme_SubModuleItem]...';


GO
ALTER TABLE [dbo].[SubModuleItemTheme]
    ADD CONSTRAINT [FK_SubModuleItemTheme_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_Survey_LmsProvider]...';


GO
ALTER TABLE [dbo].[Survey]
    ADD CONSTRAINT [FK_Survey_LmsProvider] FOREIGN KEY ([lmsProviderId]) REFERENCES [dbo].[LmsProvider] ([lmsProviderId]);


GO
PRINT N'Creating [dbo].[FK_Survey_SubModuleItem]...';


GO
ALTER TABLE [dbo].[Survey]
    ADD CONSTRAINT [FK_Survey_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_Survey_SurveyGroupingType]...';


GO
ALTER TABLE [dbo].[Survey]
    ADD CONSTRAINT [FK_Survey_SurveyGroupingType] FOREIGN KEY ([surveyGroupingTypeId]) REFERENCES [dbo].[SurveyGroupingType] ([surveyGroupingTypeId]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResult_Question]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResult]
    ADD CONSTRAINT [FK_SurveyQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResult_QuestionType]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResult]
    ADD CONSTRAINT [FK_SurveyQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResult_SurveyResult]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResult]
    ADD CONSTRAINT [FK_SurveyQuestionResult_SurveyResult] FOREIGN KEY ([surveyResultId]) REFERENCES [dbo].[SurveyResult] ([surveyResultId]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResultAnswer_Distractor]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResultAnswer]
    ADD CONSTRAINT [FK_SurveyQuestionResultAnswer_Distractor] FOREIGN KEY ([surveyDistractorId]) REFERENCES [dbo].[Distractor] ([distractorID]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResultAnswer_SurveyQuestionResult]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResultAnswer]
    ADD CONSTRAINT [FK_SurveyQuestionResultAnswer_SurveyQuestionResult] FOREIGN KEY ([surveyQuestionResultId]) REFERENCES [dbo].[SurveyQuestionResult] ([surveyQuestionResultId]);


GO
PRINT N'Creating [dbo].[FK_SurveyQuestionResultAnswer_SurveyQuestionResultAnswer]...';


GO
ALTER TABLE [dbo].[SurveyQuestionResultAnswer]
    ADD CONSTRAINT [FK_SurveyQuestionResultAnswer_SurveyQuestionResultAnswer] FOREIGN KEY ([surveyDistractorAnswerId]) REFERENCES [dbo].[Distractor] ([distractorID]);


GO
PRINT N'Creating [dbo].[FK_SurveyResult_LmsUserParameters]...';


GO
ALTER TABLE [dbo].[SurveyResult]
    ADD CONSTRAINT [FK_SurveyResult_LmsUserParameters] FOREIGN KEY ([lmsUserParametersId]) REFERENCES [dbo].[LmsUserParameters] ([lmsUserParametersId]);


GO
PRINT N'Creating [dbo].[FK_SurveyResult_Survey]...';


GO
ALTER TABLE [dbo].[SurveyResult]
    ADD CONSTRAINT [FK_SurveyResult_Survey] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_Test_ScoreType]...';


GO
ALTER TABLE [dbo].[Test]
    ADD CONSTRAINT [FK_Test_ScoreType] FOREIGN KEY ([scoreTypeId]) REFERENCES [dbo].[ScoreType] ([scoreTypeId]);


GO
PRINT N'Creating [dbo].[FK_Test_SubModuleItem]...';


GO
ALTER TABLE [dbo].[Test]
    ADD CONSTRAINT [FK_Test_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId]);


GO
PRINT N'Creating [dbo].[FK_TestQuestionResult_Question]...';


GO
ALTER TABLE [dbo].[TestQuestionResult]
    ADD CONSTRAINT [FK_TestQuestionResult_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId]);


GO
PRINT N'Creating [dbo].[FK_TestQuestionResult_QuestionType]...';


GO
ALTER TABLE [dbo].[TestQuestionResult]
    ADD CONSTRAINT [FK_TestQuestionResult_QuestionType] FOREIGN KEY ([questionTypeId]) REFERENCES [dbo].[QuestionType] ([questionTypeId]);


GO
PRINT N'Creating [dbo].[FK_TestQuestionResult_TestResult]...';


GO
ALTER TABLE [dbo].[TestQuestionResult]
    ADD CONSTRAINT [FK_TestQuestionResult_TestResult] FOREIGN KEY ([testResultId]) REFERENCES [dbo].[TestResult] ([testResultId]);


GO
PRINT N'Creating [dbo].[FK_TestResult_ACSession]...';


GO
ALTER TABLE [dbo].[TestResult]
    ADD CONSTRAINT [FK_TestResult_ACSession] FOREIGN KEY ([acSessionId]) REFERENCES [dbo].[ACSession] ([acSessionId]);


GO
PRINT N'Creating [dbo].[FK_TestResult_Test]...';


GO
ALTER TABLE [dbo].[TestResult]
    ADD CONSTRAINT [FK_TestResult_Test] FOREIGN KEY ([testId]) REFERENCES [dbo].[Test] ([testId]);


GO
PRINT N'Creating [dbo].[FK_Theme_UserCreated]...';


GO
ALTER TABLE [dbo].[Theme]
    ADD CONSTRAINT [FK_Theme_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_Theme_UserModified]...';


GO
ALTER TABLE [dbo].[Theme]
    ADD CONSTRAINT [FK_Theme_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_ThemeAttribute_Theme]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [FK_ThemeAttribute_Theme] FOREIGN KEY ([themeId]) REFERENCES [dbo].[Theme] ([themeId]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_ThemeAttribute_UserCreated]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [FK_ThemeAttribute_UserCreated] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_ThemeAttribute_UserModified]...';


GO
ALTER TABLE [dbo].[ThemeAttribute]
    ADD CONSTRAINT [FK_ThemeAttribute_UserModified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_User_Company]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]);


GO
PRINT N'Creating [dbo].[FK_User_CreatedBy]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_CreatedBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_User_Language]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_Language] FOREIGN KEY ([languageId]) REFERENCES [dbo].[Language] ([languageId]);


GO
PRINT N'Creating [dbo].[FK_User_Modified]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_Modified] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_User_TimeZone]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_TimeZone] FOREIGN KEY ([timeZoneId]) REFERENCES [dbo].[TimeZone] ([timeZoneId]);


GO
PRINT N'Creating [dbo].[FK_User_UserRole]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [FK_User_UserRole] FOREIGN KEY ([userRoleId]) REFERENCES [dbo].[UserRole] ([userRoleId]);


GO
PRINT N'Creating [dbo].[FK_UserActivation_User]...';


GO
ALTER TABLE [dbo].[UserActivation]
    ADD CONSTRAINT [FK_UserActivation_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[FK_UserLoginHistory_User]...';


GO
ALTER TABLE [dbo].[UserLoginHistory]
    ADD CONSTRAINT [FK_UserLoginHistory_User] FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([userId]);


GO
PRINT N'Creating [dbo].[deleteLmsCompanyWithDependencies]...';


GO
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
				WHERE lmsUserId IN
				(
					SELECT lmsUserId
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
			WHERE lmsUserId IN
			(
				SELECT lmsUserId
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
		WHERE lmsUserId IN
		(
			SELECT lmsUserId
			FROM LmsUser
			WHERE companyLmsId = @lmsCompanyId
		)
	)

	DELETE
	FROM LmsUserParameters
	WHERE lmsUserId IN
	(
		SELECT lmsUserId
		FROM LmsUser
		WHERE companyLmsId = @lmsCompanyId
	)

	DELETE
	FROM OfficeHours
	WHERE lmsUserId IN
	(
		SELECT lmsUserId
		FROM LmsUser
		WHERE companyLmsId = @lmsCompanyId
	)

	DELETE
	FROM LmsUserSession
	WHERE lmsUserId IN
	(
		SELECT lmsUserId
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
PRINT N'Creating [dbo].[getAppletCategoriesByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the appletItem (crossword) and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getAppletCategoriesByUserID]
	@userId int = null
AS
BEGIN

SELECT  DISTINCT SMC.subModuleCategoryId,
		SMC.userId, 
		SMC.subModuleId, 
		SMC.categoryName, 
		SMC.modifiedBy,
		SMC.dateModified,
		SMC.isActive
		
FROM    SubModuleItem SMI INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT JOIN
        AppletItem AI ON AI.subModuleItemId = SMI.subModuleItemId 
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND AI.appletItemId <> ''

END
GO
PRINT N'Creating [dbo].[getAppletSubModuleItemsByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the appletItem (crossword) and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getAppletSubModuleItemsByUserID]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT OUTER JOIN
        AppletItem AS AI ON AI.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId) AND (AI.appletItemId <> '')

END
GO
PRINT N'Creating [dbo].[getCrosswordReportDataByUserIdAndSessionID]...';


GO

-- =============================================
-- Author:		Anju 
-- Create date: 02.18.2013
-- Usage:		Admin
-- Description:	is used to get a list of crosswords sessions 
--              by userId //Please delete this once the stupid bug 
--				to get reporting data from Flex to cf.
-- =============================================
CREATE PROCEDURE [dbo].[getCrosswordReportDataByUserIdAndSessionID]  
	@userId int = null , @sessionid int=null
AS
BEGIN

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS Total, 
	   (SELECT COUNT(AR.appletResultId)
       FROM AppletResult AR
       WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS Active,
       AI.appletName, 
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG ON ACS.languageId = LNG.languageId INNER JOIN
       AppletResult AR ON ACS.acSessionId = AR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       AppletItem AI ON AR.appletItemId = AI.appletItemId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId) AND (ACS.acSessionId = @sessionid)

END
GO
PRINT N'Creating [dbo].[getCrosswordResultByACSessionId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Admin
-- Description:	is used to get a list of crosswords results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getCrosswordResultByACSessionId]  
	@acSessionId int = null
AS
BEGIN

SELECT   AR.appletResultId,
		 AR.participantName,
		 AI.documentXML,
		 AR.score,
	 	 AR.startTime,
		 AR.endTime,
		 ROW_NUMBER() OVER (ORDER BY AR.score DESC) AS position
		    
FROM     AppletItem AI INNER JOIN
         AppletResult AR ON AI.appletItemId = AR.appletItemId

WHERE    AR.acSessionId = @acSessionId

END
GO
PRINT N'Creating [dbo].[getCrosswordSessionsByUserId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Admin
-- Description:	is used to get a list of crosswords sessions 
--              by userId
-- =============================================
CREATE PROCEDURE [dbo].[getCrosswordSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS totalParticipants, 
	   (SELECT COUNT(AR.appletResultId)
       FROM AppletResult AR
       WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS activeParticipants,
       AI.appletName, 
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       AppletResult AR ON ACS.acSessionId = AR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       AppletItem AI ON AR.appletItemId = AI.appletItemId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END
GO
PRINT N'Creating [dbo].[getCrosswordSessionsByUserIdMeetingUrl]...';


GO
-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getCrosswordSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 
	   AR.appletItemId, 
	   AR.acSessionId, 
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.appletName,
	   COUNT(AR.appletResultId) AS totalParticipants, 
	   (SELECT COUNT(AR.appletResultId)
		FROM AppletResult AR
		WHERE AR.score > 0 AND ACS.acSessionId = AR.acSessionId) AS activeParticipants,
       AI.appletName, 
       usr.userId

FROM ACSession ACS 
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN       AppletResult AR ON ACS.acSessionId = AR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN         AppletItem AI ON AR.appletItemId = AI.appletItemId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI.subModuleItemId 
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] usr ON ACS.userId = usr.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language], AR.appletItemId, AR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.appletName, SMC.categoryName, ACS.acUserModeId, usr.userId, ACS.acSessionId

END
GO
PRINT N'Creating [dbo].[getQuizCategoriesByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the Quiz and for the current user only 
--				(not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getQuizCategoriesByUserID]
	@userId int = null
AS
BEGIN

SELECT  DISTINCT SMC.subModuleCategoryId,
		SMC.userId, 
		SMC.subModuleId, 
		SMC.categoryName, 
		SMC.modifiedBy,
		SMC.dateModified,
		SMC.isActive
		
FROM    SubModuleItem SMI INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT JOIN
        Quiz Q ON Q.subModuleItemId = SMI.subModuleItemId
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND Q.quizId <> ''

END
GO
PRINT N'Creating [dbo].[getQuizQuestionsForAdminBySMIId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current quiz by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getQuizQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 QT.type as questionTypeName,
		 (SELECT  
		 SUM(case QT.isCorrect
		  when 1
		  then 1
		  end)	    
FROM     QuizResult QR 
         LEFT join  QuizQuestionResult QT on QT.quizResultId = qr.quizResultId
         LefT join Question que on que.questionId = qt.questionId

WHERE    QR.acSessionId = @acSessionID and que.questionId = Q.questionId group by que.questionId) as correctAnswerCount
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END
GO
PRINT N'Creating [dbo].[getQuizResultByACSessionId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of quiz results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getQuizResultByACSessionId]  
	@acSessionId int = null,@subModuleItemID int = null
AS
BEGIN
select sub.quizResultId, sub.participantName, sub.acEmail, sub.score, sub.TotalQuestion, sub.startTime, sub.endTime, 
		 ROW_NUMBER() OVER (ORDER BY sub.score desc, sub.dateDifference asc) AS position, sub.isCompleted from (
SELECT   QR.quizResultId,
		 QR.participantName,	
		 QR.acEmail,	 
		 QR.score,
		 (select Count(Q.questionId) from Question Q where Q.subModuleItemId=@subModuleItemID) as TotalQuestion,
	 	 QR.startTime,
		 QR.endTime,
		 DATEDIFF(second, QR.startTime, QR.endTime) as dateDifference,
		 QR.isCompleted
		 
		    
FROM     Quiz Q INNER JOIN
         QuizResult QR ON Q.quizId = QR.quizId

WHERE    QR.acSessionId = @acSessionId
) as sub


END
GO
PRINT N'Creating [dbo].[getQuizResultByACSessionIdAcEmail]...';


GO
CREATE PROCEDURE getQuizResultByACSessionIdAcEmail
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
) 
AS
BEGIN

SELECT
	sub.quizResultId, 
	sub.participantName,
	sub.acEmail,
	sub.score,
	sub.TotalQuestion, -- TRICK: TotalQuestion
	sub.startTime,
	sub.endTime, 
	ROW_NUMBER() OVER (ORDER BY sub.score desc, sub.dateDifference asc) AS position,
	sub.isCompleted 
FROM
(
	SELECT  QR.quizResultId,
			QR.participantName,	
			QR.acEmail,	 
			QR.score,
			(SELECT Count(Q.questionId) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion, -- TRICK: TotalQuestion
			QR.startTime,
			QR.endTime,
			DATEDIFF(second, QR.startTime, QR.endTime) AS dateDifference,
			QR.isCompleted
	FROM Quiz Qz
		INNER JOIN         QuizResult QR ON Qz.quizId = QR.quizId
	WHERE QR.acSessionId = @acSessionId AND QR.acEmail = @acEmail
) AS sub


END
GO
PRINT N'Creating [dbo].[getQuizSessionsByUserId]...';


GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of quiz sessions 
--              by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getQuizSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   QR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.quizName,	 
	   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from QuizResult where acSessionId = QR.acSessionId ) AS TotalScore,
       AI.quizName, 
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       QuizResult QR ON ACS.acSessionId = QR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Quiz AI ON QR.quizId = AI.quizId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  QR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.quizName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END
GO
PRINT N'Creating [dbo].[getQuizSessionsByUserIdMeetingUrl]...';


GO
-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getQuizSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   QR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.quizName,
	   COUNT(QR.quizResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.quizResultId)
       FROM QuizResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from QuizResult where acSessionId = QR.acSessionId ) AS TotalScore,
       AI.quizName, 
       USR.userId

FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId 
	INNER JOIN         QuizResult QR ON ACS.acSessionId = QR.acSessionId 
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId 
	INNER JOIN               Quiz AI ON QR.quizId = AI.quizId 
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId 
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  QR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.quizName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO
PRINT N'Creating [dbo].[getQuizSubModuleItemsByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the Quiz and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getQuizSubModuleItemsByUserID]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT OUTER JOIN
        Quiz AS Q ON Q.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId) AND (Q.quizId <> '')

END
GO
PRINT N'Creating [dbo].[getSharedForUserCrosswordsByUserId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of shared crosswords (not own) 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getSharedForUserCrosswordsByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		AI.appletItemId,
		AI.appletName
		
FROM    AppletItem AI INNER JOIN
        SubModuleItem SMI ON AI.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' AND SMI.isShared = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
		INNER JOIN [User] U2 ON (SMI.createdBy = U2.userId AND U2.[status] = 1)
        
WHERE   U2.userId != @userId AND U2.companyId IN (SELECT TOP 1 companyId FROM [User] WHERE userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSharedForUserQuizzesByUserId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of shared quizzes (not own) 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getSharedForUserQuizzesByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		Q.quizId,
		Q.quizName,
		Q.[description]
		
FROM    Quiz Q INNER JOIN
        SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' AND SMI.isShared = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
		INNER JOIN [User] U2 ON (SMI.createdBy = U2.userId AND U2.[status] = 1)
        
WHERE   U2.userId != @userId AND U2.companyId IN (SELECT TOP 1 companyId FROM [User] WHERE userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSharedForUserSurveysByUserId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of shared quizzes (not own) 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getSharedForUserSurveysByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		S.surveyId,
		S.surveyName,
		S.[description],
		S.surveyGroupingTypeId
		
FROM    Survey S INNER JOIN
        SubModuleItem SMI ON S.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' AND SMI.isShared = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
		INNER JOIN [User] U2 ON (SMI.createdBy = U2.userId AND U2.[status] = 1)
        
WHERE   U2.userId != @userId AND U2.companyId IN (SELECT TOP 1 companyId FROM [User] WHERE userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSharedForUserTestsByUserId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Public
-- Description:	is used to get a list of shared tests (not own) 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getSharedForUserTestsByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		T.testId,
		T.testName,
		T.[description]
		
FROM    Test T INNER JOIN
        SubModuleItem SMI ON T.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' AND SMI.isShared = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
		INNER JOIN [User] U2 ON (SMI.createdBy = U2.userId AND U2.[status] = 1)
        
WHERE   U2.userId != @userId AND U2.companyId IN (SELECT TOP 1 companyId FROM [User] WHERE userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSMIDistractorsBySMIId]...';


GO
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of distractors 
--				for each question of current quiz 
--				by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getSMIDistractorsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Distinct
		 D.distractorID,
		 D.questionId,
		 D.distractorType,
		 D.distractor,
		 D.distractorOrder,
		 D.isCorrect,
		 D.imageId,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Distractor D INNER JOIN
         Question Q ON D.questionID = Q.questionId INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [File] I ON I.fileId = D.imageId
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1 AND D.isActive = 1

END
GO
PRINT N'Creating [dbo].[getSMIQuestionsBySMIId]...';


GO
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of question for 
--				current quiz by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getSMIQuestionsBySMIId]
	@subModuleItemId int = null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,
		 Q.questionOrder,
		 Q.questionTypeId,
		 Q.instruction,
		 
		 Q.incorrectMessage,
		 Q.correctReference,
		 Q.correctMessage,
		 Q.hint,
		 Q.imageId,
		 Q.scoreValue,
		 Q.randomizeAnswers,
		 Q.[rows],
		 CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.restrictions 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.restrictions 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.restrictions
			ELSE null END AS restrictions,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.allowOther 
			WHEN Q.questionTypeId = 13 THEN ql.allowOther 
			WHEN Q.questionTypeId = 14 THEN qw.allowOther 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.allowOther 
			ELSE null END AS allowOther,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.isMandatory 
			WHEN Q.questionTypeId = 13 THEN ql.isMandatory 
			WHEN Q.questionTypeId = 14 THEN qw.isMandatory 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.isMandatory 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.isMandatory 
			WHEN Q.questionTypeId = 2 THEN qtf.isMandatory 
			ELSE null END AS isMandatory,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.pageNumber 
			WHEN Q.questionTypeId = 13 THEN ql.pageNumber 
			WHEN Q.questionTypeId = 14 THEN qw.pageNumber 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.pageNumber 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.pageNumber 
			WHEN Q.questionTypeId = 2 THEN qtf.pageNumber 
			ELSE null END AS pageNumber,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.totalWeightBucket 
			ELSE null END AS totalWeightBucket,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.weightBucketType 
			ELSE null END AS weightBucketType,
		CASE 
			WHEN Q.questionTypeId = 12 THEN qr.isAlwaysRateDropdown
			ELSE null END AS isAlwaysRateDropdown,
		 I.x,
		 I.y,
		 I.height,
		 I.width
		   
FROM     Question Q 
	     left outer join QuestionForLikert ql on ql.questionId = q.questionId
		 left outer join QuestionForOpenAnswer qo on qo.questionId = q.questionId
		 left outer join QuestionForRate qr on qr.questionId = q.questionId
		 left outer join QuestionForTrueFalse qtf on qtf.questionId = q.questionId
		 left outer join QuestionForWeightBucket qw on qw.questionId = q.questionId
		 left outer join QuestionForSingleMultipleChoice qc on qc.questionId = q.questionId 
		 INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId LEFT OUTER JOIN
         [File] I ON I.fileId = Q.imageId
         
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END
GO
PRINT N'Creating [dbo].[getSNProfileSubModuleItemsByUserID]...';


GO
-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 05.23.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the SNProfile 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getSNProfileSubModuleItemsByUserID]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
        SNProfile AS Q ON Q.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSNSessionsByUserId]...';


GO



-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 05.15.2013
-- Usage:		Admin
-- Description:	is used to get a list of sn sessions  by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getSNSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   ACS.acSessionId, 	
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   COUNT(SNM.snMemberId) AS totalParticipants, 
	   (SELECT COUNT(SNM.snMemberId) FROM SNMember SNM WHERE ACS.acSessionId = SNM.acSessionId and SNM.isBlocked = 0) AS activeParticipants,
	   SNP.snProfileId, 
       SNP.profileName, 
	   SNG.snGroupDiscussionId,
	   SNG.groupDiscussionTitle,
       u.userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId LEFT OUTER JOIN
	   SNGroupDiscussion SNG ON SNG.acSessionId = ACS.acSessionId INNER JOIN
       SNMember SNM ON ACS.acSessionId = SNM.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId LEFT OUTER JOIN
	   SNProfile SNP ON SNP.subModuleItemId = ACS.subModuleItemId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] u ON ACS.userId = u.userId
       
GROUP BY LNG.[language],  ACS.acSessionId, ACS.subModuleItemId, ACS.dateCreated, SNP.snProfileId, SNP.profileName, SMC.categoryName, ACUM.acUserModeId, 
SNG.snGroupDiscussionId, SNG.groupDiscussionTitle, u.userId

HAVING      (u.userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSurveyCategoriesByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the Survey and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getSurveyCategoriesByUserID]
	@userId int = null
AS
BEGIN

SELECT  DISTINCT SMC.subModuleCategoryId,
		SMC.userId, 
		SMC.subModuleId, 
		SMC.categoryName, 
		SMC.modifiedBy,
		SMC.dateModified,
		SMC.isActive
		
FROM    SubModuleItem SMI INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT JOIN
        Survey S ON S.subModuleItemId = SMI.subModuleItemId 
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND S.surveyId <> ''

END
GO
PRINT N'Creating [dbo].[getSurveyQuestionsForAdminBySMIId]...';


GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current survey by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getSurveyQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 Q.questionOrder,
		 CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.restrictions 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.restrictions 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.restrictions 
			ELSE null END AS restrictions,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.allowOther 
			WHEN Q.questionTypeId = 13 THEN ql.allowOther 
			WHEN Q.questionTypeId = 14 THEN qw.allowOther 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.allowOther 
			ELSE null END AS allowOther,
		CASE 
		    WHEN Q.questionTypeId = 12 THEN qr.isMandatory 
			WHEN Q.questionTypeId = 13 THEN ql.isMandatory 
			WHEN Q.questionTypeId = 14 THEN qw.isMandatory 
			WHEN Q.questionTypeId = 1 or Q.questionTypeId = 7 THEN qc.isMandatory 
			WHEN Q.questionTypeId = 10 or Q.questionTypeId = 11 THEN qo.isMandatory 
			WHEN Q.questionTypeId = 2 THEN qtf.isMandatory 
			ELSE null END AS isMandatory,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.totalWeightBucket 
			ELSE null END AS totalWeightBucket,
		 CASE 
			WHEN Q.questionTypeId = 14 THEN qw.weightBucketType 
			ELSE null END AS weightBucketType,
		 QT.[type] as questionTypeName,
		 (SELECT  
		 SUM(case QT.isCorrect
		  when 1
		  then 1
		  end)	    
FROM     SurveyResult SR 
         LEFT join  SurveyQuestionResult QT on QT.surveyResultId = SR.surveyResultId
         LefT join Question que on que.questionId = qt.questionId

WHERE    SR.acSessionId = @acSessionID and que.questionId = Q.questionId group by que.questionId) as correctAnswerCount
		   
FROM     Question Q 
		 left outer join QuestionForLikert ql on ql.questionId = q.questionId
		 left outer join QuestionForOpenAnswer qo on qo.questionId = q.questionId
		 left outer join QuestionForRate qr on qr.questionId = q.questionId
		 left outer join QuestionForWeightBucket qw on qw.questionId = q.questionId
		 left outer join QuestionForTrueFalse qtf on qtf.questionId = q.questionId
		 left outer join QuestionForSingleMultipleChoice qc on qc.questionId = q.questionId
		 INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END
GO
PRINT N'Creating [dbo].[getSurveyResultAnswers]...';


GO
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
GO
PRINT N'Creating [dbo].[getSurveyResultByACSessionId]...';


GO
CREATE PROCEDURE getSurveyResultByACSessionId
(
	@acSessionId		INT,
	@subModuleItemId	INT
)
AS
BEGIN

SELECT   SR.surveyResultId,
		 SR.participantName,		 
		 SR.score,
		 (select Count(Q.questionId) from Question Q where Q.subModuleItemId=@subModuleItemId) as TotalQuestion,
	 	 SR.startTime,
		 SR.endTime,
		 SR.acEmail,
		 ROW_NUMBER() OVER (ORDER BY SR.score DESC) AS position
		    
FROM     Survey S INNER JOIN
         SurveyResult SR ON S.surveyId = SR.surveyId

WHERE    SR.acSessionId = @acSessionId

END
GO
PRINT N'Creating [dbo].[getSurveyResultByACSessionIdAcEmail]...';


GO
CREATE PROCEDURE [dbo].[getSurveyResultByACSessionIdAcEmail] 
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
)
AS
BEGIN

SELECT   SR.surveyResultId,
		 SR.participantName,		 
		 SR.score,
		 (SELECT COUNT(Q.questionId) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion,
	 	 SR.startTime,
		 SR.endTime,
		 SR.acEmail,
		 ROW_NUMBER() OVER (ORDER BY SR.score DESC) AS position
		    
FROM     Survey S 
	INNER JOIN SurveyResult SR ON S.surveyId = SR.surveyId
WHERE    SR.acSessionId = @acSessionId AND SR.acEmail = @acEmail

END
GO
PRINT N'Creating [dbo].[getSurveySessionsByUserId]...';


GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Admin
-- Description:	is used to get a list of quiz sessions 
--              by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getSurveySessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   SR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.surveyName,	 
	   COUNT(SR.surveyResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.surveyResultId)
       FROM SurveyResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from SurveyResult where acSessionId = SR.acSessionId ) AS TotalScore,
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       SurveyResult SR ON ACS.acSessionId = SR.acSessionId INNER JOIN
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Survey AI ON SR.surveyId = AI.surveyId INNER JOIN
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  SR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.surveyName, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END
GO
PRINT N'Creating [dbo].[getSurveySessionsByUserIdMeetingUrl]...';


GO
-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getSurveySessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   SR.acSessionId, 	
	   (select Count(Q.questionId) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.surveyName,	 
	   COUNT(SR.surveyResultId) AS totalParticipants, 
	   (SELECT COUNT(QR.surveyResultId)
       FROM SurveyResult QR
       WHERE QR.score > 0 AND ACS.acSessionId = QR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from SurveyResult where acSessionId = SR.acSessionId ) AS TotalScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN       SurveyResult SR ON ACS.acSessionId = SR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN             Survey AI ON SR.surveyId = AI.surveyId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  SR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, AI.surveyName, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO
PRINT N'Creating [dbo].[getSurveySubModuleItemsByUserID]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.25.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the Survey and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getSurveySubModuleItemsByUserID]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT OUTER JOIN
        Survey AS S ON S.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId) AND (S.surveyId <> '')

END
GO
PRINT N'Creating [dbo].[getTestCategoriesByUserID]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	returns all categories spicific for 
--				the Test and for the current user only 
--				(not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getTestCategoriesByUserID]
	@userId int = null
AS
BEGIN

SELECT  DISTINCT SMC.subModuleCategoryId,
		SMC.userId, 
		SMC.subModuleId, 
		SMC.categoryName, 
		SMC.modifiedBy,
		SMC.dateModified,
		SMC.isActive
		
FROM    SubModuleItem SMI INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT JOIN
        Test T ON T.subModuleItemId = SMI.subModuleItemId
          
WHERE   SMC.userId = @userId AND SMI.createdBy = @userId AND T.testId <> ''

END
GO
PRINT N'Creating [dbo].[getTestQuestionsForAdminBySMIId]...';


GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	is used to get a list of questions for 
--				current test by subModuleItemId.
-- =============================================
CREATE PROCEDURE [dbo].[getTestQuestionsForAdminBySMIId]
	@subModuleItemId int = null, @acSessionID int =null
AS
BEGIN

SELECT   Q.questionId,
		 Q.question,	
		 Q.questionTypeId,
		 QT.type as questionTypeName,
		 (SELECT  
		 SUM(CAST(tqr.isCorrect AS INT))	    
FROM     TestResult tr 
         LEFT join  TestQuestionResult tqr on tqr.testResultId = tr.testResultId
         LefT join Question que on que.questionId = tqr.questionId

WHERE    tr.acSessionId = @acSessionID and que.questionId = Q.questionId group by que.questionId) as correctAnswerCount
		   
FROM     Question Q INNER JOIN
         SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId INNER JOIN
         QuestionType QT ON Q.questionTypeId = QT.questionTypeId
     
WHERE	 Q.subModuleItemId = @subModuleItemId AND Q.isActive = 1

END
GO
PRINT N'Creating [dbo].[getTestResultByACSessionId]...';


GO


-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of test results 
--				by acSessionId
-- =============================================
CREATE PROCEDURE [dbo].[getTestResultByACSessionId]  
	@acSessionId int = null,@subModuleItemId int = null
AS
BEGIN

SELECT   TR.testResultId,
		 TR.participantName,		 
		 TR.acEmail,
		 TR.score,
		 (select Count(Q.questionId) from Question Q where Q.subModuleItemId=@subModuleItemId) as TotalQuestion,
	 	 TR.startTime,
		 TR.endTime,
		 ROW_NUMBER() OVER (ORDER BY TR.score DESC) AS position,
		 TR.isCompleted
		    
FROM     Test T INNER JOIN
         TestResult TR ON T.testId = TR.testId

WHERE    TR.acSessionId = @acSessionId

END
GO
PRINT N'Creating [dbo].[getTestResultByACSessionIdAcEmail]...';


GO

CREATE PROCEDURE getTestResultByACSessionIdAcEmail
(
	@acSessionId		INT,
	@subModuleItemID	INT,
	@acEmail			NVARCHAR(500)
)
AS
BEGIN

SELECT   TR.testResultId,
		 TR.participantName,		 
		 TR.acEmail,
		 TR.score,
		 (SELECT COUNT(Q.questionId) FROM Question Q WHERE Q.subModuleItemId = @subModuleItemID) AS TotalQuestion,
	 	 TR.startTime,
		 TR.endTime,
		 ROW_NUMBER() OVER (ORDER BY TR.score DESC) AS position,
		 TR.isCompleted
		    
FROM Test T
	INNER JOIN TestResult TR ON T.testId = TR.testId
WHERE TR.acSessionId = @acSessionId AND TR.acEmail = @acEmail

END
GO
PRINT N'Creating [dbo].[getTestSessionsByUserId]...';


GO




-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.10.2014
-- Usage:		Admin
-- Description:	is used to get a list of test sessions 
--              by userId for Admin Reporting
-- =============================================
CREATE PROCEDURE [dbo].[getTestSessionsByUserId]  
	@userId int = null
AS
BEGIN

SELECT LNG.[language], 	   
	   TR.acSessionId, 	
	   --(select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACUM.acUserModeId,
	   AI.testName,	 
	   AI.passingScore,
	   COUNT(TR.testResultId) AS totalParticipants, 
	   (SELECT COUNT(TR.testResultId)
       FROM TestResult TR
       WHERE TR.score > 0 AND ACS.acSessionId = TR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from TestResult where acSessionId = TR.acSessionId) AS TotalScore,
	   (SELECT AVG(score) from TestResult where acSessionId = TR.acSessionId) AS avgScore,
       [User].userId
       
FROM   ACSession ACS INNER JOIN
	   [Language] LNG  ON ACS.languageId = LNG.languageId INNER JOIN
       TestResult TR ON ACS.acSessionId = TR.acSessionId INNER JOIN
	   
       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId INNER JOIN
       Test AI ON TR.testId = AI.testId INNER JOIN
	   
       SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId INNER JOIN
       SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId INNER JOIN
       [User] ON ACS.userId = [User].userId
       
GROUP BY LNG.[language],  TR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.testName, AI.passingScore, SMC.categoryName, ACUM.acUserModeId, [User].userId, ACS.acSessionId

HAVING      ([User].userId = @userId)

END
GO
PRINT N'Creating [dbo].[getTestSessionsByUserIdMeetingUrl]...';


GO
-- =============================================
-- Author:		Sergey Isakovich
-- Create date: 11/10/2015
-- Usage:		Public API
-- =============================================
CREATE PROCEDURE getTestSessionsByUserIdMeetingUrl
(
	@userId		INT,
	@meetingURL	NVARCHAR(500)
)
AS
BEGIN

DECLARE @companyId INT
SELECT @companyId = companyId
FROM [User]
WHERE userId = @userId

SELECT LNG.[language], 	   
	   TR.acSessionId, 	
	   --(select Count(Q.questionid) from Question Q where Q.subModuleItemId=ACS.subModuleItemId and q.isActive = 1) as TotalQuestion,
	   ACS.subModuleItemId, 
	   ACS.dateCreated,
	   ACS.includeAcEmails,
	   SMC.categoryName,
	   ACS.acUserModeId,
	   AI.testName,	 
	   AI.passingScore,
	   COUNT(TR.testResultId) AS totalParticipants, 
	   (SELECT COUNT(TR.testResultId)
       FROM TestResult TR
       WHERE TR.score > 0 AND ACS.acSessionId = TR.acSessionId) AS activeParticipants,
       (SELECT SUM(score)from TestResult where acSessionId = TR.acSessionId) AS TotalScore,
	   (SELECT AVG(score) from TestResult where acSessionId = TR.acSessionId) AS avgScore,
       USR.userId
       
FROM ACSession ACS
	INNER JOIN        [Language] LNG ON ACS.languageId = LNG.languageId
	INNER JOIN         TestResult TR ON ACS.acSessionId = TR.acSessionId
	--INNER JOIN       ACUserMode ACUM ON ACUM.acUserModeId = ACS.acUserModeId
	INNER JOIN               Test AI ON TR.testId = AI.testId
	INNER JOIN     SubModuleItem SMI ON ACS.subModuleItemId = SMI.subModuleItemId AND AI.subModuleItemId = SMI .subModuleItemId
	INNER JOIN SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId
	INNER JOIN            [User] USR ON ACS.userId = USR.userId
WHERE usr.companyId = @companyId AND ACS.meetingURL = @meetingURL
GROUP BY LNG.[language],  TR.acSessionId, ACS.subModuleItemId, ACS.dateCreated, ACS.includeAcEmails, AI.testName, AI.passingScore, SMC.categoryName, ACS.acUserModeId, USR.userId, ACS.acSessionId

END
GO
PRINT N'Creating [dbo].[getTestSubModuleItemsByUserId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Admin
-- Description:	returns all SMIs spicific for 
--				the Test and for the 
--				current user only (not shared by others)
-- =============================================
CREATE PROCEDURE [dbo].[getTestSubModuleItemsByUserId]
	@userId int = null
AS
BEGIN

SELECT  SMI.subModuleItemId, 
		SMC.subModuleId,
		SMI.subModuleCategoryId,
		SMI.createdBy,
		SMI.isShared,
		SMI.modifiedBy,
		SMI.dateCreated,
		SMI.dateModified,
		SMI.isActive
		
FROM    SubModuleItem AS SMI INNER JOIN
        SubModuleCategory AS SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId LEFT OUTER JOIN
        Test AS T ON T.subModuleItemId = SMI.subModuleItemId
        
WHERE   (SMI.createdBy = @userId) AND (SMC.userId = @userId) AND (T.testId <> '')

END
GO
PRINT N'Creating [dbo].[getUsersByLmsCompanyId]...';


GO
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
		FROM @userFilter.nodes('/users/user') AS T(x)
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
GO
PRINT N'Creating [dbo].[getUsersCrosswordsByUserId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of crosswords 
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersCrosswordsByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		AI.appletItemId,
		AI.appletName
		
FROM    AppletItem AI INNER JOIN
        SubModuleItem SMI ON AI.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END
GO
PRINT N'Creating [dbo].[getUsersQuizzesByUserId]...';


GO
-- =============================================
-- Author:		Yury Paulouski
-- Create date: 02.18.2013
-- Usage:		Public
-- Description:	is used to get a list of shared quizzes
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersQuizzesByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		Q.quizId,
		Q.quizName,
		Q.[description]
		
FROM    Quiz Q INNER JOIN
        SubModuleItem SMI ON Q.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END
GO
PRINT N'Creating [dbo].[getUsersSurveysByUserId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 08.30.2013
-- Usage:		Public
-- Description:	is used to get a list of shared quizzes
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersSurveysByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		S.surveyId,
		S.surveyName,
		S.[description],
		S.surveyGroupingTypeId
		
FROM    Survey S INNER JOIN
        SubModuleItem SMI ON S.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END
GO
PRINT N'Creating [dbo].[getUsersTestsByUserId]...';


GO

-- =============================================
-- Author:		Eugene Baranovsky
-- Create date: 10.24.2013
-- Usage:		Public
-- Description:	is used to get a list of shared tests
--              for current user by userId
-- =============================================
CREATE PROCEDURE [dbo].[getUsersTestsByUserId] 
	@userId int = null
AS
BEGIN

SELECT  SMC.subModuleCategoryId, 
		SMC.categoryName, 
		SMI.subModuleItemId, 
		SMI.dateModified, 
		SMI.createdBy,
		U2.firstName AS createdByName,
		U2.lastName AS createdByLastName, 
		U.userId, 
		U.firstName, 
		U.lastName, 
		T.testId,
		T.testName,
		T.[description],
		T.[passingScore],
		T.[timeLimit],
		T.[instructionTitle],
		T.[instructionDescription],
		T.[scoreFormat]
		
FROM    Test T INNER JOIN
        SubModuleItem SMI ON T.subModuleItemId = SMI.subModuleItemId AND SMI.isActive = 'True' INNER JOIN
        SubModuleCategory SMC ON SMI.subModuleCategoryId = SMC.subModuleCategoryId AND SMC.isActive = 'True' INNER JOIN
        [User] U ON SMC.userId = U.userId
        LEFT OUTER JOIN [User] U2 ON SMI.createdBy = U2.userId
        
WHERE U2.userId = @userId
      
END
GO

PRINT N'Update complete.';


GO
