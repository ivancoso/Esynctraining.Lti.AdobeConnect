CREATE TABLE [dbo].[ApplicationVersion] (
    [majorVersion] INT NOT NULL,
    [minorVersion] INT NOT NULL,
    CONSTRAINT [PK_ApplicationVersion] PRIMARY KEY CLUSTERED ([majorVersion] ASC, [minorVersion] ASC)
);

