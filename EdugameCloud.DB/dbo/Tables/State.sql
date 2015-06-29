CREATE TABLE [dbo].[State] (
    [stateId]   INT             IDENTITY (1, 1) NOT NULL,
    [stateCode] NVARCHAR (10)   NOT NULL,
    [stateName] NVARCHAR (50)   NOT NULL,
    [isActive]  BIT             NOT NULL,
    [latitude]  DECIMAL (18, 7) CONSTRAINT [DF_State_latitude] DEFAULT ((0)) NOT NULL,
    [longitude] DECIMAL (18, 7) CONSTRAINT [DF_State_longitude] DEFAULT ((0)) NOT NULL,
    [zoomLevel] INT             CONSTRAINT [DF_State_zoomLevel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([stateId] ASC)
);







