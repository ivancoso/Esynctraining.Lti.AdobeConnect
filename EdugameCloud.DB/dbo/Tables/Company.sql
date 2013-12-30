CREATE TABLE [dbo].[Company] (
    [companyId]        INT           IDENTITY (1, 1) NOT NULL,
    [companyName]      VARCHAR (50)  NOT NULL,
    [addressId]        INT           NULL,
    [status]           INT           CONSTRAINT [DF_Company_isActive] DEFAULT ((0)) NOT NULL,
    [dateCreated]      SMALLDATETIME CONSTRAINT [DF_Company_dateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified]     SMALLDATETIME CONSTRAINT [DF_Company_dateModified] DEFAULT (getdate()) NOT NULL,
    [primaryContactId] INT           NULL,
    CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ([companyId] ASC),
    CONSTRAINT [FK_Company_Address] FOREIGN KEY ([addressId]) REFERENCES [dbo].[Address] ([addressId]),
    CONSTRAINT [FK_Company_PrimaryContact] FOREIGN KEY ([primaryContactId]) REFERENCES [dbo].[User] ([userId])
);


GO
ALTER TABLE [dbo].[Company] NOCHECK CONSTRAINT [FK_Company_Address];




GO
ALTER TABLE [dbo].[Company] NOCHECK CONSTRAINT [FK_Company_Address];




GO
ALTER TABLE [dbo].[Company] NOCHECK CONSTRAINT [FK_Company_Address];

