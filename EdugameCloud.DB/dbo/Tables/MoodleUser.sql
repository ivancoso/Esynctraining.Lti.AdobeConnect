CREATE TABLE [dbo].[MoodleUser] (
    [moodleUserId] INT            IDENTITY (1, 1) NOT NULL,
    [userName]     NVARCHAR (100) NULL,
    [password]     NVARCHAR (100) NULL,
    [domain]       NVARCHAR (100) NOT NULL,
    [token]        NVARCHAR (50)  NOT NULL,
    [userId]       INT            NOT NULL,
    [dateModified] DATETIME       NOT NULL,
    CONSTRAINT [PK_MoodleUser2] PRIMARY KEY CLUSTERED ([moodleUserId] ASC)
);

