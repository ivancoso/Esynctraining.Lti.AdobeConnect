CREATE TABLE [dbo].[Image] (
    [imageId]     INT              IDENTITY (1, 1) NOT NULL,
    [imageName]   NVARCHAR (255)   NOT NULL,
    [path]        VARCHAR (MAX)    NULL,
    [height]      INT              NULL,
    [width]       INT              NULL,
    [createdBy]   INT              NULL,
    [dateCreated] DATETIME         CONSTRAINT [DF__Image__DateCreat__6E01572D] DEFAULT (getdate()) NOT NULL,
    [isActive]    BIT              CONSTRAINT [DF__Image__IsActive__6EF57B66] DEFAULT ((0)) NULL,
    [status]      INT              CONSTRAINT [DF__Image__Status__6EF57B66] DEFAULT ((0)) NULL,
    [x]           INT              NULL,
    [y]           INT              NULL,
    [webOrbId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED ([imageId] ASC),
    CONSTRAINT [FK_Image_User] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId])
);







