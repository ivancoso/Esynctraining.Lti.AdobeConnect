CREATE TABLE [dbo].[SubscriptionHistoryLog] (
    [subscriptionHistoryLogId] INT            IDENTITY (1, 1) NOT NULL,
    [subscriptionTag]          NVARCHAR (500) NOT NULL,
    [lastQueryTime]            DATETIME       NULL,
    [subscriptionId]           INT            NULL,
    CONSTRAINT [PK_SubscriptionHistoryLog] PRIMARY KEY CLUSTERED ([subscriptionHistoryLogId] ASC)
);

