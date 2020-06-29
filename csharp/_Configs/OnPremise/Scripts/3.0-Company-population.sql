SET IDENTITY_INSERT [dbo].[Address] ON
GO

INSERT [dbo].[Address] ([addressId], [dateCreated], [dateModified]) 
VALUES(1000, GETDATE(), GETDATE())

SET IDENTITY_INSERT [dbo].[Address] OFF
GO

SET IDENTITY_INSERT [dbo].[Company] ON
GO

INSERT [dbo].[Company] ([companyId], [companyName], [addressId], [status], [dateCreated]) 
VALUES(1000, 'PLACE YOUR COMPANY NAME', 1000, 1, GETDATE())

SET IDENTITY_INSERT [dbo].[Company] OFF
GO

SET IDENTITY_INSERT [dbo].[User] ON
GO

INSERT [dbo].[User] ([userId], [companyId], [languageId], [timeZoneId], [userRoleId], [firstName], [lastName], [password], [email], [dateCreated], [dateModified], [status], [isUnsubscribed]) 
VALUES(1001, 1000, 5, 11, 11, 
'FirstAdminName', 
'LastAdminName', 
'1125FDC497615BFCA72844CF726765334B3B5908FBC5EAD70CA1B6AE7F7D083F', -- == changepass
'PLACE_ADMIN_EMAIL_ADDRESS', GETDATE(), GETDATE(), 1, 0)

SET IDENTITY_INSERT [dbo].[User] OFF
GO

INSERT [dbo].[CompanyLicense] ([companyId], [licenseNumber], [expiryDate], [createdBy], [modifiedBy], [dateCreated], [dateModified], [totalLicensesCount], [licenseStatus], [dateStart], [totalParticipantsCount], [hasApi]) 
VALUES (1000, '05A42811-EDB7-40A8-AAA5-A8255ECFA9B2', DATEADD(yy, 1, GETDATE()), 1001, 1001, GETDATE(), GETDATE(), 50, 3, GETDATE(), 100, 0)
GO

UPDATE Company SET primaryContactId = 1001 WHERE companyId = 1000
GO
