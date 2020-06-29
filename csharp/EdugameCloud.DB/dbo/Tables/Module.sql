CREATE TABLE [dbo].[Module] (
    [moduleId]    INT           IDENTITY (1, 1) NOT NULL,
    [moduleName]  VARCHAR (50)  NOT NULL,
    [dateCreated] SMALLDATETIME CONSTRAINT [DF_Module_dateCreated] DEFAULT (getdate()) NOT NULL,
    [isActive]    BIT           CONSTRAINT [DF_Module_isActive] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([moduleId] ASC)
);

