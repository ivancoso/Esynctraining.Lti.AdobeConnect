CREATE TABLE [dbo].[CompanyLicenseHistory] (
    [companyLicenseHistoryId] INT           IDENTITY (1, 1) NOT NULL,
    [companyLicenseId]        INT           NOT NULL,
    [createdBy]               INT           NULL,
    [modifiedBy]              INT           NULL,
    [dateCreated]             SMALLDATETIME CONSTRAINT [DF_CompanyLicenseHistory_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]            SMALLDATETIME CONSTRAINT [DF_CompanyLicenseHistory_dateModified] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_CompanyLicenseHistory] PRIMARY KEY CLUSTERED ([companyLicenseHistoryId] ASC),
    CONSTRAINT [FK_CompanyLicenseHistory_CompanyLicense] FOREIGN KEY ([modifiedBy]) REFERENCES [dbo].[User] ([userId])
);



