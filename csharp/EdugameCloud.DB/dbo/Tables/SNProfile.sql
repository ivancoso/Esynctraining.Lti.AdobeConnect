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
    CONSTRAINT [PK_SNProfile] PRIMARY KEY CLUSTERED ([snProfileId] ASC),
    CONSTRAINT [FK_SNProfile_Address] FOREIGN KEY ([addressId]) REFERENCES [dbo].[Address] ([addressId]),
    CONSTRAINT [FK_SNProfile_SNMapSettings] FOREIGN KEY ([snMapSettingsId]) REFERENCES [dbo].[SNMapSettings] ([snMapSettingsId]),
    CONSTRAINT [FK_SNProfile_SubModuleItem] FOREIGN KEY ([subModuleItemId]) REFERENCES [dbo].[SubModuleItem] ([subModuleItemId])
);













