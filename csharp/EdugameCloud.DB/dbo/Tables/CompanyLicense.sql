CREATE TABLE [dbo].[CompanyLicense] (
    [companyLicenseId]       INT           IDENTITY (1, 1) NOT NULL,
    [companyId]              INT           NOT NULL,
    [licenseNumber]          VARCHAR (50)  NOT NULL,
    [domain]                 VARCHAR (100) NULL,
    [expiryDate]             SMALLDATETIME NOT NULL,
    [createdBy]              INT           NOT NULL,
    [modifiedBy]             INT           NOT NULL,
    [dateCreated]            SMALLDATETIME CONSTRAINT [DF_CompanyLicense_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]           SMALLDATETIME CONSTRAINT [DF_CompanyLicense_dateModified] DEFAULT (getdate()) NOT NULL,
    [totalLicensesCount]     INT           CONSTRAINT [DF_CompanyLicense_totalLicensesCount] DEFAULT ((1)) NOT NULL,
    [licenseStatus]          INT           NOT NULL,
    [dateStart]              DATETIME      NOT NULL,
    [totalParticipantsCount] INT           CONSTRAINT [DF_CompanyLicense_totalParticipantsCount] DEFAULT ((100)) NOT NULL,
    [hasApi]                 BIT           NOT NULL,
    CONSTRAINT [PK_CompanyLicense] PRIMARY KEY CLUSTERED ([companyLicenseId] ASC),
    CONSTRAINT [FK_CompanyLicense_Company] FOREIGN KEY ([companyId]) REFERENCES [dbo].[Company] ([companyId]),
    CONSTRAINT [FK_CompanyLicense_CreatedBy] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId]),
    CONSTRAINT [FK_CompanyLicense_ModifiedBy] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);





















