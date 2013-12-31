CREATE TABLE [dbo].[CompanyThemeMeeting] (
    [companyThemeMeetingId] INT              NOT NULL,
    [logoURL]               VARCHAR (50)     NULL,
    [backgroundColor]       VARCHAR (50)     NULL,
    [meetinglogoImageId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CompanyThemeMeeting] PRIMARY KEY CLUSTERED ([companyThemeMeetingId] ASC),
    CONSTRAINT [FK_CompanyThemeMeeting_Image] FOREIGN KEY ([meetinglogoImageId]) REFERENCES [dbo].[File] ([fileId])
);





