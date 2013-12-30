CREATE TABLE [dbo].[QuestionForWeightBucket] (
    [questionForWeightBucketId] INT             IDENTITY (1, 1) NOT NULL,
    [questionId]                INT             NOT NULL,
    [totalWeightBucket]         DECIMAL (18, 9) NULL,
    [weightBucketType]          INT             NULL,
    [allowOther]                BIT             NULL,
    [pageNumber]                INT             NULL,
    [isMandatory]               BIT             NOT NULL,
    CONSTRAINT [PK_QuestionForWeightBucket] PRIMARY KEY CLUSTERED ([questionForWeightBucketId] ASC),
    CONSTRAINT [FK_QuestionForWeightBucket_Question] FOREIGN KEY ([questionId]) REFERENCES [dbo].[Question] ([questionId])
);

