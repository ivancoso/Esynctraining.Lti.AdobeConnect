CREATE TABLE [dbo].[Address] (
    [addressId]    INT            IDENTITY (1, 1) NOT NULL,
    [countryId]    INT            NULL,
    [stateId]      INT            NULL,
    [address1]     NVARCHAR (255) NULL,
    [address2]     NVARCHAR (255) NULL,
    [city]         NVARCHAR (255) NULL,
    [dateCreated]  SMALLDATETIME  CONSTRAINT [DF_Address_DateCreated] DEFAULT (getdate()) NOT NULL,
    [dateModified] SMALLDATETIME  CONSTRAINT [DF_Address_DateModified] DEFAULT (getdate()) NOT NULL,
    [latitude]     FLOAT (53)     NULL,
    [longitude]    FLOAT (53)     NULL,
    [zip]          VARCHAR (30)   NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([addressId] ASC),
    CONSTRAINT [FK_Address_Country] FOREIGN KEY ([countryId]) REFERENCES [dbo].[Country] ([countryId]),
    CONSTRAINT [FK_Address_State] FOREIGN KEY ([stateId]) REFERENCES [dbo].[State] ([stateId])
);







