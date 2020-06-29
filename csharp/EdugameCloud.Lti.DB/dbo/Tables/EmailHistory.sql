CREATE TABLE [dbo].[EmailHistory] (
    [emailHistoryId] INT            IDENTITY (1, 1) NOT NULL,
    [sentTo]         NVARCHAR (50)  NOT NULL,
    [sentFrom]       NVARCHAR (50)  NOT NULL,
    [sentCC]         NCHAR (200)    NULL,
    [sentBCC]        NCHAR (200)    NULL,
    [subject]        NCHAR (500)    NULL,
    [message]        NVARCHAR (MAX) NULL,
    [body]           NVARCHAR (MAX) NULL,
    [date]           DATETIME       NOT NULL,
    [userId]         INT            NULL,
    [sentToName]     NCHAR (100)    NULL,
    [sentFromName]   NCHAR (100)    NULL
);

