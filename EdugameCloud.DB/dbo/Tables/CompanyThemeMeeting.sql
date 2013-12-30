CREATE TABLE [dbo].[CompanyThemeMeeting] (
    [companyThemeMeetingId] INT          NOT NULL,
    [meetingLogoImageId]    INT          NULL,
    [logoURL]               VARCHAR (50) NULL,
    [backgroundColor]       VARCHAR (50) NULL,
    CONSTRAINT [PK_CompanyThemeMeeting] PRIMARY KEY CLUSTERED ([companyThemeMeetingId] ASC),
    CONSTRAINT [FK_CompanyThemeMeeting_Image] FOREIGN KEY ([companyThemeMeetingId]) REFERENCES [dbo].[File] ([fileId])
);



