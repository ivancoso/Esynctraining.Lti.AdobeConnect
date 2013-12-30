CREATE TABLE [dbo].[SNService] (
    [snServiceId]   INT            IDENTITY (1, 1) NOT NULL,
    [socialService] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_SNService] PRIMARY KEY CLUSTERED ([snServiceId] ASC)
);



