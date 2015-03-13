CREATE TABLE [dbo].[EmailHistory] (
    [emailHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [sentTo]         NVARCHAR (50)  NOT NULL,
    [sentFrom]       NVARCHAR (50)  NOT NULL,
    [sentCC]         NVARCHAR (200) NULL,
    [sentBCC]        NVARCHAR (200) NULL,
    [subject]        NVARCHAR (500) NULL,
    [message]        NVARCHAR (MAX) NULL,
    [body]           NVARCHAR (MAX) NULL,
    [date]           DATETIME       NOT NULL,
    [userId]         INT            NULL,
    [sentToName]     NVARCHAR (100) NULL,
    [sentFromName]   NVARCHAR (100) NULL
);



