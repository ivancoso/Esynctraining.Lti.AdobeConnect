﻿CREATE TABLE [dbo].[CompanyAcDomains](
	[AcDomainId] [int] NOT NULL,
	[AcServer] [nvarchar](100) NOT NULL,
	[Username] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[IsDefault] [bit] NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_CompanyAcDomains] PRIMARY KEY CLUSTERED 
(
	[AcDomainId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CompanyAcDomains]  WITH CHECK ADD  CONSTRAINT [FK_CompanyAcDomains_CompanyLms] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([companyId])
GO

ALTER TABLE [dbo].[CompanyAcDomains] CHECK CONSTRAINT [FK_CompanyAcDomains_CompanyLms]
GO
