CREATE TABLE [dbo].[ServerStatus] (
    [online]  BIT           CONSTRAINT [DF_ServerStatus_online] DEFAULT ((1)) NOT NULL,
    [message] VARCHAR (MAX) NULL
);

