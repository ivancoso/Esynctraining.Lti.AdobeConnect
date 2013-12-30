CREATE TABLE [dbo].[BuildVersion] (
    [buildVersionId]     INT            IDENTITY (1, 1) NOT NULL,
    [fileId]             INT            NULL,
    [buildNumber]        NVARCHAR (20)  NOT NULL,
    [buildVersionTypeId] INT            NOT NULL,
    [isActive]           BIT            NOT NULL,
    [descriptionSmall]   NVARCHAR (255) NULL,
    [descriptionHTML]    NVARCHAR (MAX) NULL,
    [dateCreated]        DATETIME       NOT NULL,
    [dateModified]       DATETIME       NOT NULL,
    CONSTRAINT [PK_ProductVersion] PRIMARY KEY CLUSTERED ([buildVersionId] ASC),
    CONSTRAINT [FK_BuildVersion_BuildVersion] FOREIGN KEY ([buildVersionId]) REFERENCES [dbo].[BuildVersion] ([buildVersionId]),
    CONSTRAINT [FK_BuildVersion_BuildVersionType] FOREIGN KEY ([fileId]) REFERENCES [dbo].[File] ([fileId])
);





