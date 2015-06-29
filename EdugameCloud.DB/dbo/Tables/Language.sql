CREATE TABLE [dbo].[Language] (
    [languageId] INT            IDENTITY (1, 1) NOT NULL,
    [language]   NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([languageId] ASC)
);



