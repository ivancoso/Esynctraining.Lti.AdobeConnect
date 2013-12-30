CREATE TABLE [dbo].[SNMapProvider] (
    [snMapProviderId] INT            IDENTITY (1, 1) NOT NULL,
    [mapProvider]     NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SNMapProvider] PRIMARY KEY CLUSTERED ([snMapProviderId] ASC)
);

