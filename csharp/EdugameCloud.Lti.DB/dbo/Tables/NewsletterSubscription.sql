CREATE TABLE [dbo].[NewsletterSubscription] (
    [newsLetterSubscriptionId] INT           IDENTITY (1, 1) NOT NULL,
    [email]                    NVARCHAR (50) NOT NULL,
    [isActive]                 BIT           NOT NULL,
    [dateSubscribed]           DATETIME      NOT NULL,
    [dateUnsubscribed]         DATETIME      NULL
);

