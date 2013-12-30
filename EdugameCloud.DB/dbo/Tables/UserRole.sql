CREATE TABLE [dbo].[UserRole] (
    [userRoleId] INT          IDENTITY (1, 1) NOT NULL,
    [userRole]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([userRoleId] ASC)
);

