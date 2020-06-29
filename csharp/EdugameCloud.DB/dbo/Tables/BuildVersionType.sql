CREATE TABLE [dbo].[BuildVersionType] (
    [buildVersionTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [buildVersionType]   NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_ProductVersionType] PRIMARY KEY CLUSTERED ([buildVersionTypeId] ASC)
);

