CREATE TABLE [dbo].[SubscriptionUpdate] (
    [subscriptionUpdateId] INT             IDENTITY (1, 1) NOT NULL,
    [subscription_id]      INT             NOT NULL,
    [object]               NVARCHAR (20)   NOT NULL,
    [object_id]            NVARCHAR (1000) NOT NULL,
    [changed_aspect]       NVARCHAR (50)   NOT NULL,
    [time]                 NVARCHAR (255)  NOT NULL,
    [createdDate]          DATETIME        NOT NULL,
    CONSTRAINT [PK_SubscriptionUpdate] PRIMARY KEY CLUSTERED ([subscriptionUpdateId] ASC)
);

