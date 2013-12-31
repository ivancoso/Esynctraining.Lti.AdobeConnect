CREATE TABLE [dbo].[File] (
    [fileId]      UNIQUEIDENTIFIER NOT NULL,
    [fileName]    NVARCHAR (255)   NOT NULL,
    [path]        VARCHAR (MAX)    NULL,
    [height]      INT              NULL,
    [width]       INT              NULL,
    [createdBy]   INT              NULL,
    [dateCreated] DATETIME         CONSTRAINT [DF__Image__DateCreat__6E01572D] DEFAULT (getdate()) NOT NULL,
    [isActive]    BIT              CONSTRAINT [DF__Image__IsActive__6EF57B66] DEFAULT ((0)) NULL,
    [status]      INT              CONSTRAINT [DF__Image__Status__6EF57B66] DEFAULT ((0)) NULL,
    [x]           INT              NULL,
    [y]           INT              NULL,
    CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED ([fileId] ASC),
    CONSTRAINT [FK_Image_User] FOREIGN KEY ([createdBy]) REFERENCES [dbo].[User] ([userId])
);



