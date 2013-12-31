CREATE TABLE [dbo].[BuildVersion] (
    [buildVersionId]     INT              IDENTITY (1, 1) NOT NULL,
    [buildNumber]        NVARCHAR (20)    NOT NULL,
    [buildVersionTypeId] INT              NOT NULL,
    [isActive]           BIT              NOT NULL,
    [descriptionSmall]   NVARCHAR (255)   NULL,
    [descriptionHTML]    NVARCHAR (MAX)   NULL,
    [dateCreated]        DATETIME         NOT NULL,
    [dateModified]       DATETIME         NOT NULL,
    [fileId]             UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ProductVersion] PRIMARY KEY CLUSTERED ([buildVersionId] ASC),
    CONSTRAINT [FK_BuildVersion_BuildVersionType] FOREIGN KEY ([buildVersionTypeId]) REFERENCES [dbo].[BuildVersionType] ([buildVersionTypeId]),
    CONSTRAINT [FK_BuildVersion_File] FOREIGN KEY ([fileId]) REFERENCES [dbo].[File] ([fileId])
);









