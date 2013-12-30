CREATE TABLE [dbo].[SNMapSettings] (
    [snMapSettingsId] INT IDENTITY (1, 1) NOT NULL,
    [snMapProviderId] INT NULL,
    [zoomLevel]       INT NULL,
    [countryId]       INT NULL,
    CONSTRAINT [PK_SNProfileMapSettings] PRIMARY KEY CLUSTERED ([snMapSettingsId] ASC),
    CONSTRAINT [fk_SNMapSettings_Country] FOREIGN KEY ([countryId]) REFERENCES [dbo].[Country] ([countryId]),
    CONSTRAINT [FK_SNProfileMapSettings_SNMapProvider] FOREIGN KEY ([snMapProviderId]) REFERENCES [dbo].[SNMapProvider] ([snMapProviderId])
);



