CREATE TABLE [dbo].[SNProfileSNService] (
    [snProfileSNServiceId] INT             IDENTITY (1, 1) NOT NULL,
    [snProfileId]          INT             NOT NULL,
    [snServiceId]          INT             NOT NULL,
    [isEnabled]            BIT             NOT NULL,
    [serviceUrl]           NVARCHAR (2000) NULL,
    CONSTRAINT [PK_SNProfileSNService] PRIMARY KEY CLUSTERED ([snProfileSNServiceId] ASC),
    CONSTRAINT [FK_SNProfileSNService_SNProfile] FOREIGN KEY ([snProfileId]) REFERENCES [dbo].[SNProfile] ([snProfileId]),
    CONSTRAINT [FK_SNProfileSNService_SNService] FOREIGN KEY ([snServiceId]) REFERENCES [dbo].[SNService] ([snServiceId])
);



