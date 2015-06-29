CREATE TABLE [dbo].[Country] (
    [countryId]    INT             IDENTITY (1, 1) NOT NULL,
    [countryCode]  VARCHAR (3)     NOT NULL,
    [countryCode3] VARCHAR (4)     NOT NULL,
    [country]      NVARCHAR (255)  NOT NULL,
    [latitude]     DECIMAL (18, 7) CONSTRAINT [DF_Country_latitude] DEFAULT ((0)) NOT NULL,
    [longitude]    DECIMAL (18, 7) CONSTRAINT [DF_Country_longitude] DEFAULT ((0)) NOT NULL,
    [zoomLevel]    INT             CONSTRAINT [DF_Country_zoomLevel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([countryId] ASC)
);







