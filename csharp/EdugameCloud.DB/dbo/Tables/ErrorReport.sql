CREATE TABLE [dbo].[ErrorReport] (
    [userId]             INT           IDENTITY (1, 1) NOT NULL,
    [os]                 VARCHAR (50)  NULL,
    [flashVersion]       VARCHAR (50)  NULL,
    [message]            VARCHAR (MAX) NOT NULL,
    [applicationVersion] VARCHAR (50)  NOT NULL,
    [dateCreated]        SMALLDATETIME CONSTRAINT [DF_ErrorReport_date] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ErrorReport] PRIMARY KEY CLUSTERED ([userId] ASC)
);

