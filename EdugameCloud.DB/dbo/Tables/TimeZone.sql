CREATE TABLE [dbo].[TimeZone] (
    [timeZoneId]      INT          IDENTITY (1, 1) NOT NULL,
    [timeZone]        VARCHAR (50) NULL,
    [timeZoneGMTDiff] FLOAT (53)   NULL,
    CONSTRAINT [PK_TimeZone] PRIMARY KEY CLUSTERED ([timeZoneId] ASC)
);

