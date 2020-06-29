SET IDENTITY_INSERT [dbo].[Language] ON
GO

INSERT [dbo].[Language] ([languageId], [language], [twoLetterCode]) VALUES(5, 'English', 'en')
INSERT [dbo].[Language] ([languageId], [language], [twoLetterCode]) VALUES(10, 'Spanish', 'es')

SET IDENTITY_INSERT [dbo].[Language] OFF
GO

SET IDENTITY_INSERT [dbo].[TimeZone] ON
GO

INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(5, 'IST', 5.500000000000000e+000)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(6, 'HAST', -1.000000000000000e+001)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(7, 'AKST', -9.000000000000000e+000)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(8, 'PST', -8.000000000000000e+000)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(9, 'MST', -7.000000000000000e+000)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(10, 'CST', -6.000000000000000e+000)
INSERT [dbo].[TimeZone] ([timeZoneId], [timeZone], [timeZoneGMTDiff]) VALUES(11, 'EST', -5.000000000000000e+000)

SET IDENTITY_INSERT [dbo].[TimeZone] OFF
GO

SET IDENTITY_INSERT [dbo].[UserRole] ON
GO

INSERT [dbo].[UserRole] ([userRoleId], [userRole]) VALUES(9, 'Admin')
INSERT [dbo].[UserRole] ([userRoleId], [userRole]) VALUES(10, 'User')
INSERT [dbo].[UserRole] ([userRoleId], [userRole]) VALUES(11, 'Super Admin')

SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO

SET IDENTITY_INSERT [dbo].[ACUserMode] ON
GO

INSERT [dbo].[ACUserMode] ([acUserModeId], [userMode], [imageId]) VALUES(1, 'Single User', NULL)
INSERT [dbo].[ACUserMode] ([acUserModeId], [userMode], [imageId]) VALUES(2, 'Multiple Users', NULL)

SET IDENTITY_INSERT [dbo].[ACUserMode] OFF
GO

SET IDENTITY_INSERT [dbo].[Country] ON
GO

INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(2, 'AL', 'ALB', 'Albania', 41.1474080, 20.0708120, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(3, 'AS', 'ASM', 'American Samoa', -19.0510016, -169.9022980, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(4, 'AD', 'AND', 'Andorra', 42.5452500, 1.5762970, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(5, 'AO', 'AGO', 'Angola', -12.2971000, 17.2801000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(6, 'AI', 'AIA', 'Anguilla', 18.2216430, -63.0589710, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(7, 'AG', 'ATG', 'Antigua/Barbuda', 17.3512740, -61.7934790, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(8, 'AR', 'ARG', 'Argentina', -39.9922120, -65.1439810, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(9, 'AM', 'ARM', 'Armenia', 40.2928470, 44.9386830, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(10, 'AW', 'ABW', 'Aruba', 12.5000000, -69.9775010, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(11, 'AU', 'AUS', 'Australia', -25.0000000, 134.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(12, 'AT', 'AUT', 'Austria', 47.4607320, 13.6690250, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(13, 'AZ', 'AZE', 'Azerbaijan', 40.2875300, 48.0066520, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(14, 'BS', 'BHS', 'Bahamas', 24.3379780, -77.9521100, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(15, 'BH', 'BHR', 'Bahrain', 25.9186740, 50.5500000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(16, 'BD', 'BGD', 'Bangladesh', 24.0000000, 50.5745690, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(17, 'BB', 'BRB', 'Barbados', 13.1674460, -59.5552020, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(18, 'BY', 'BLR', 'Belarus', 53.5428920, 28.0539800, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(19, 'BE', 'BEL', 'Belgium', 50.5303430, 4.7309500, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(20, 'BZ', 'BLZ', 'Belize', 17.1972370, -88.6997680, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(21, 'BJ', 'BEN', 'Benin', 9.2611130, 2.4520000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(22, 'BM', 'BMU', 'Bermuda', 32.3167640, -64.7568660, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(23, 'BT', 'BTN', 'Bhutan', 27.3391990, 90.4375000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(24, 'BO', 'BOL', 'Bolivia', -16.7100710, -64.6653130, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(25, 'BA', 'BIH', 'Bosnia-Herzegovina', 43.9750640, 17.7406920, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(26, 'BW', 'BWA', 'Botswana', -22.4016780, 23.8130450, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(27, 'BR', 'BRA', 'Brazil', -15.5661680, -55.2134130, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(28, 'VG', 'VGB', 'British Virgin Islands', 18.4877560, -64.5849150, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(29, 'BN', 'BRN', 'Brunei', 4.5409000, 114.7191010, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(30, 'BG', 'BGR', 'Bulgaria', 42.2905430, 25.3846360, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(31, 'BF', 'BFA', 'Burkina Faso', 12.3616000, -1.5377000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(32, 'BI', 'BDI', 'Burundi', -3.3949000, 29.6717000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(33, 'KH', 'KHM', 'Cambodia', 12.4425000, 104.7337040, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(34, 'CM', 'CMR', 'Cameroon', 7.0132560, 12.2442110, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(35, 'CA', 'CAN', 'Canada', 57.7025670, -96.8259660, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(36, 'CV', 'CPV', 'Cape Verde', 16.0000000, -24.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(37, 'KY', 'CYM', 'Cayman Islands', 19.3193610, -81.2536770, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(38, 'TD', 'TCD', 'Chad', 14.0228000, 18.5205990, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(39, 'CL', 'CHL', 'Chile', -39.0004160, -70.5287860, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(40, 'CN', 'CHN', 'China', 35.0000000, 105.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(41, 'CO', 'COL', 'Colombia', 4.0000000, -73.0000000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(42, 'CG', 'COG', 'Congo Brazzaville', -1.5680540, 14.7743840, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(43, 'CD', 'COD', 'Congo Republic', -3.0719040, 24.5522160, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(44, 'CK', 'COK', 'Cook Islands', -21.2364410, -159.7800940, 13)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(45, 'CR', 'CRI', 'Costa Rica', 10.0000000, -84.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(46, 'HR', 'HRV', 'Croatia', 44.6348630, 16.8043520, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(47, 'CY', 'CYP', 'Cyprus', 35.0000000, 33.0000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(48, 'CZ', 'CZE', 'Czech Republic', 49.7500000, 15.5000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(49, 'DK', 'DNK', 'Denmark', 56.0000000, 9.3172460, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(50, 'DJ', 'DJI', 'Djibouti', 11.5000000, 43.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(51, 'DM', 'DMA', 'Dominica', 15.4167000, -61.3333000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(52, 'DO', 'DOM', 'Dominican Republic', 18.7894040, -70.2931900, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(53, 'EC', 'ECU', 'Ecuador', -1.7615380, -77.9833990, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(54, 'EG', 'EGY', 'Egypt', 27.0000000, 30.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(55, 'SV', 'SLV', 'El Salvador', 13.7247520, -88.8702770, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(56, 'GQ', 'GNQ', 'Equatorial Guinea', 1.5640330, 10.4155590, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(57, 'ER', 'ERI', 'Eritrea', 15.0000000, 39.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(58, 'EE', 'EST', 'Estonia', 59.0000000, 26.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(59, 'ET', 'ETH', 'Ethiopia', 8.8735560, 39.5202260, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(60, 'FO', 'FRO', 'Faeroe Islands', 61.8626490, -6.8164980, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(61, 'FJ', 'FJI', 'Fiji', -17.6779680, 178.6503140, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(62, 'FI', 'FIN', 'Finland', 65.2743160, 25.6941970, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(63, 'FR', 'FRA', 'France', 46.8385860, 2.9895910, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(64, 'GF', 'GUF', 'French Guiana', 3.8231990, -52.8272410, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(65, 'PF', 'PYF', 'French Polynesia', -17.6655930, -149.4762580, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(66, 'GA', 'GAB', 'Gabon', -0.8073440, 11.9380380, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(67, 'GM', 'GMB', 'Gambia', 13.1919110, -15.3584380, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(68, 'GE', 'GEO', 'Georgia', 42.0000000, 43.5000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(69, 'DE', 'DEU', 'Germany', 51.2146550, 10.1624800, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(70, 'GH', 'GHA', 'Ghana', 7.9530860, -1.1980090, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(71, 'GI', 'GIB', 'Gibraltar', 36.1833000, -5.3667000, 13)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(72, 'GR', 'GRC', 'Greece', 38.0740120, 22.8222050, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(73, 'GL', 'GRL', 'Greenland', 75.2493760, -34.7928810, 3)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(74, 'GD', 'GRD', 'Grenada', 12.1796620, -61.6588780, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(75, 'GP', 'GLP', 'Guadeloupe', 16.1599010, -61.5047150, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(76, 'GU', 'GUM', 'Guam', 13.4473810, 144.7486420, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(77, 'GT', 'GTM', 'Guatemala', 15.6940900, -90.3561400, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(78, 'GN', 'GIN', 'Guinea', 10.0018090, -11.1983860, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(79, 'GY', 'GUY', 'Guyana', 4.7992340, -59.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(80, 'HT', 'HTI', 'Haiti', 19.0000000, -72.4167000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(81, 'HN', 'HND', 'Honduras', 15.0000000, -86.5000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(82, 'HK', 'HKG', 'Hong Kong', 22.3366090, 114.1667000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(83, 'HU', 'HUN', 'Hungary', 47.0000000, 20.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(84, 'IS', 'ISL', 'Iceland', 65.0000000, -18.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(85, 'IN', 'IND', 'India', 20.0000000, 77.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(86, 'ID', 'IDN', 'Indonesia', -5.2884210, 118.1871500, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(87, 'IE', 'IRL', 'Ireland', 53.0000000, -8.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(88, 'IL', 'ISR', 'Israel', 31.5000000, 34.7500000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(89, 'IT', 'ITA', 'Italy/Vatican City', 42.7132480, 12.0204770, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(90, 'CI', 'CIV', 'Ivory Coast', 7.5889000, -5.8056630, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(91, 'JM', 'JAM', 'Jamaica', 18.2500000, -77.5000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(92, 'JP', 'JPN', 'Japan', 36.0000000, 138.0000000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(93, 'JO', 'JOR', 'Jordan', 31.1871120, 36.8054230, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(94, 'KZ', 'KAZ', 'Kazakhstan', 48.0443720, 67.5951610, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(95, 'KE', 'KEN', 'Kenya', -0.0012370, 38.3839720, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(96, 'KW', 'KWT', 'Kuwait', 29.3375000, 47.6581000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(97, 'KG', 'KGZ', 'Kyrgyzstan', 40.8852330, 74.7970880, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(98, 'LA', 'LAO', 'Laos', 17.8037070, 104.3968350, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(99, 'LV', 'LVA', 'Latvia', 56.8543280, 24.9310150, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(100, 'LB', 'LBN', 'Lebanon', 33.9417000, 35.9239010, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(101, 'LS', 'LSO', 'Lesotho', -29.6075280, 28.2085340, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(102, 'LR', 'LBR', 'Liberia', 6.3962180, -9.3157210, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(103, 'LI', 'LIE', 'Liechtenstein', 47.1448650, 9.5352350, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(104, 'LT', 'LTU', 'Lithuania', 55.3356290, 23.9082110, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(105, 'LU', 'LUX', 'Luxembourg', 49.7500000, 6.1667000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(106, 'MO', 'MAC', 'Macau', 22.1667000, 113.5500000, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(107, 'MK', 'MKD', 'Macedonia', 41.8333000, 21.7077900, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(108, 'MW', 'MWI', 'Malawi', -13.2728150, 35.3642230, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(109, 'MY', 'MYS', 'Malaysia', 3.8300940, 108.6981130, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(110, 'MV', 'MDV', 'Maldives', -0.6413360, 73.1569830, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(111, 'ML', 'MLI', 'Mali', 16.8192010, -3.6522000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(112, 'MT', 'MLT', 'Malta', 35.9346860, 14.4188750, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(113, 'MH', 'MHL', 'Marshall Islands', 7.1216280, 171.2296590, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(114, 'MQ', 'MTQ', 'Martinique', 14.6667000, -61.0000000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(115, 'MR', 'MRT', 'Mauritania', 20.8935780, -11.6523340, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(116, 'MU', 'MUS', 'Mauritius', -20.2833000, 57.5500000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(117, 'MX', 'MEX', 'Mexico', 23.7293450, -103.2575910, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(118, 'FM', 'FSM', 'Micronesia', 6.8665340, 158.2384050, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(119, 'MD', 'MDA', 'Moldova', 47.0000000, 29.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(120, 'MC', 'MCO', 'Monaco', 43.7369430, 7.4314700, 14)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(121, 'MN', 'MNG', 'Mongolia', 46.5866910, 104.0414880, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(122, 'MS', 'MSR', 'Montserrat', 16.7500000, -62.2000000, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(123, 'MA', 'MAR', 'Morocco', 28.0418640, -8.2143780, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(124, 'MZ', 'MOZ', 'Mozambique', -18.8796020, 36.8715930, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(125, 'NA', 'NAM', 'Namibia', -23.1307630, 17.0364550, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(126, 'NP', 'NPL', 'Nepal', 28.0000000, 84.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(127, 'NL', 'NLD', 'Netherlands', 52.1221790, 5.7189770, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(128, 'AN', 'ANT', 'Netherlands Antilles', 12.2084880, -68.9905620, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(129, 'NC', 'NCL', 'New Caledonia', -21.2364820, 165.5473480, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(130, 'NZ', 'NZL', 'New Zealand', -43.0751900, 170.2696680, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(131, 'NI', 'NIC', 'Nicaragua', 12.8399020, -85.0334550, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(132, 'NE', 'NER', 'Niger', 17.7390810, 7.9032270, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(133, 'NG', 'NGA', 'Nigeria', 8.7006000, 8.5172000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(134, 'NO', 'NOR', 'Norway', 62.0000000, 10.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(135, 'OM', 'OMN', 'Oman', 20.4950010, 55.5928990, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(136, 'PK', 'PAK', 'Pakistan', 31.4932390, 69.3858260, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(137, 'PW', 'PLW', 'Palau', 7.4439890, 134.5198210, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(138, 'PS', 'PSE', 'Palestine', 31.8674840, 35.2006680, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(139, 'PA', 'PAN', 'Panama', 8.5204830, -80.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(140, 'PG', 'PNG', 'Papua New Guinea', -6.4886810, 146.2801820, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(141, 'PY', 'PRY', 'Paraguay', -23.7665950, -58.4161600, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(142, 'PE', 'PER', 'Peru', -9.9986460, -75.1967390, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(143, 'PH', 'PHL', 'Philippines', 11.3730790, 122.4274910, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(144, 'PL', 'POL', 'Poland', 52.1212160, 19.4083180, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(145, 'PT', 'PRT', 'Portugal', 39.6958320, -8.0399280, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(146, 'PR', 'PRI', 'Puerto Rico', 18.1570730, -66.4374850, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(147, 'QA', 'QAT', 'Qatar', 25.3497010, 51.0119020, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(148, 'RE', 'REU', 'Reunion', -21.1514720, 55.4867660, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(149, 'RO', 'ROU', 'Romania', 45.9731480, 25.3054590, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(150, 'RU', 'RUS', 'Russian Federation', 64.9342200, 130.4464860, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(151, 'RW', 'RWA', 'Rwanda', -1.9519000, 30.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(152, 'MP', 'MNP', 'Saipan', 15.2000000, 145.7500000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(153, 'SA', 'SAU', 'Saudi Arabia', 24.6392990, 46.6864010, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(154, 'SN', 'SEN', 'Senegal', 14.3930320, -14.3692400, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(155, 'SC', 'SYC', 'Seychelles', -4.5915130, 55.5307420, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(156, 'SG', 'SGP', 'Singapore', 1.3234810, 103.8000000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(157, 'SK', 'SVK', 'Slovak Republic', 48.7081490, 19.6140220, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(158, 'SI', 'SVN', 'Slovenia', 45.9331190, 14.7524590, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(159, 'ZA', 'ZAF', 'South Africa', -31.2156640, 24.8034460, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(160, 'KR', 'KOR', 'South Korea', 36.4855840, 127.5445480, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(161, 'ES', 'ESP', 'Spain', 40.0779550, -3.3340820, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(162, 'LK', 'LKA', 'Sri Lanka', 7.0000000, 81.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(163, 'KN', 'KNA', 'St. Kitts/Nevis', 17.2524310, -62.6952670, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(164, 'LC', 'LCA', 'St. Lucia', 13.8752110, -60.9525610, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(165, 'VC', 'VCT', 'St. Vincent', 13.0311150, -61.1613010, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(166, 'SR', 'SUR', 'Suriname', 4.0000000, -56.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(167, 'SZ', 'SWZ', 'Swaziland', -26.5000000, 31.5000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(168, 'SE', 'SWE', 'Sweden', 62.0000000, 15.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(169, 'CH', 'CHE', 'Switzerland', 46.8206050, 8.3562720, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(170, 'SY', 'SYR', 'Syria', 34.9218210, 38.2103920, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(171, 'TW', 'TWN', 'Taiwan', 23.5000000, 121.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(172, 'TZ', 'TZA', 'Tanzania', -6.8101830, 35.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(173, 'TH', 'THA', 'Thailand', 15.5596800, 100.7799230, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(174, 'TG', 'TGO', 'Togo', 8.0000000, 1.0349990, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(175, 'TT', 'TTO', 'Trinidad/Tobago', 10.4261830, -61.1833260, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(176, 'TN', 'TUN', 'Tunisia', 34.0000000, 9.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(177, 'TR', 'TUR', 'Turkey', 38.0783360, 34.7972530, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(178, 'TM', 'TKM', 'Turkmenistan', 38.9919980, 59.5275760, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(179, 'TC', 'TCA', 'Turks & Caicos Islands', 21.6064500, -71.7681060, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(180, 'VI', 'VIR', 'U.S. Virgin Islands', 18.3333000, -64.8333000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(181, 'US', 'USA', 'United States', 38.0000000, -97.0000000, 4)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(182, 'UG', 'UGA', 'Uganda', 1.3224030, 32.7446390, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(183, 'UA', 'UKR', 'Ukraine', 49.0241010, 31.4000380, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(184, 'AE', 'ARE', 'United Arab Emirates', 24.0000000, 54.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(185, 'GB', 'GBR', 'United Kingdom', 54.0000000, -2.0000000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(186, 'UY', 'URY', 'Uruguay', -33.0000000, -56.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(187, 'UZ', 'UZB', 'Uzbekistan', 41.2319180, 62.6418920, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(188, 'VU', 'VUT', 'Vanuatu', -15.5198030, 167.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(189, 'VE', 'VEN', 'Venezuela', 6.1773990, -66.0479350, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(190, 'VN', 'VNM', 'Vietnam', 16.0000000, 106.0000000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(191, 'WF', 'WLF', 'Wallis & Futuna', -14.3086640, -178.1076510, 12)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(192, 'YE', 'YEM', 'Yemen', 15.3745060, 47.7171480, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(193, 'YU', 'YUG', 'Yugoslavia', 44.6348630, 16.8043520, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(194, 'ZM', 'ZMB', 'Zambia', -13.7143440, 28.2159420, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(195, 'AF', 'AFG', 'Afghanistan', 33.6904330, 66.1654200, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(196, 'DZ', 'DZA', 'Algeria', 28.6066120, 3.4554610, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(197, 'BV', 'BVT', 'Bouvet Island', -54.4333000, 3.4000000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(199, 'IO', 'IOT', 'British Indian Ocean Territory', -6.0000000, 71.5000000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(200, 'CF', 'CAF', 'Central African Republic', 7.0000000, 21.0000000, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(201, 'CX', 'CXR', 'Christmas Island', -10.5000000, 105.6667000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(202, 'CC', 'CCK', 'Cocos (Keeling) Islands', -12.5000000, 96.8333000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(203, 'KM', 'COM', 'Comoros', -12.1667000, 44.2500000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(204, 'CU', 'CUB', 'Cuba', 21.2285610, -79.5337530, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(205, 'FK', 'FLK', 'Falkland Islands (Malvinas)', -51.7500000, -59.0000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(206, 'TF', 'ATF', 'French Southern Territories', -43.0000000, 67.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(207, 'GG', 'GGY', 'Guernsey', 49.5000000, -2.5600000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(208, 'GW', 'GNB', 'Guinea-Bissau', 12.0000000, -15.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(209, 'HM', 'HMD', 'Heard Island and McDonald Islands', -53.1000000, 72.5167000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(210, 'VA', 'VAT', 'Holy See (Vatican City State)', 41.9000000, 12.4500000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(211, 'IR', 'IRN', 'Iran, Islamic Republic of', 32.8847560, 52.1044310, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(212, 'IQ', 'IRQ', 'Iraq', 32.8755290, 43.9306030, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(213, 'IM', 'IMN', 'Isle of Man', 54.2300000, -4.5500000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(214, 'JE', 'JEY', 'Jersey', 49.2100000, -2.1300000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(215, 'KI', 'KIR', 'Kiribati', 1.4167000, 173.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(216, 'KP', 'PRK', 'Korea, Democratic People''s Republic of', 40.0000000, 127.0000000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(217, 'LY', 'LBY', 'Libyan Arab Jamahiriya', 25.0000000, 17.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(218, 'MG', 'MDG', 'Madagascar', -20.0000000, 47.0000000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(219, 'YT', 'MYT', 'Mayotte', -12.8333000, 45.1667000, 8)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(220, 'ME', 'MNE', 'Montenegro', 42.0000000, 19.0000000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(221, 'MM', 'MMR', 'Myanmar', 22.0000000, 98.0000000, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(222, 'NR', 'NRU', 'Nauru', -0.5333000, 166.9167000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(223, 'NU', 'NIU', 'Niue', -19.0333000, -169.8667000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(224, 'NF', 'NFK', 'Norfolk Island', -29.0333000, 167.9500000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(225, 'PN', 'PCN', 'Pitcairn', -24.7000000, -127.4000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(226, 'SH', 'SHN', 'Saint Helena, Ascension and Tristan da Cunha', -15.9333000, -5.7000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(227, 'PM', 'SPM', 'Saint Pierre and Miquelon', 46.8333000, -56.3333000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(228, 'WS', 'WSM', 'Samoa', -13.5833000, -172.3333000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(229, 'SM', 'SMR', 'San Marino', 43.7667000, 12.4167000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(230, 'ST', 'STP', 'Sao Tome and Principe', 1.0000000, 7.0000000, 9)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(231, 'RS', 'SRB', 'Serbia', 44.0000000, 21.0000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(232, 'SL', 'SLE', 'Sierra Leone', 8.5000000, -11.5000000, 7)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(233, 'SB', 'SLB', 'Solomon Islands', -8.0000000, 159.0000000, 10)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(234, 'SO', 'SOM', 'Somalia', 4.1334220, 47.1166380, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(235, 'GS', 'SGS', 'South Georgia and the South Sandwich Islands', -54.5000000, -37.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(236, 'SD', 'SDN', 'Sudan', 14.6108010, 31.5599960, 5)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(237, 'SJ', 'SJM', 'Svalbard and Jan Mayen', 78.0000000, 20.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(238, 'TJ', 'TJK', 'Tajikistan', 39.0000000, 71.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(239, 'TL', 'TLS', 'Timor-Leste', -8.5500000, 125.5167000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(240, 'TK', 'TKL', 'Tokelau', -9.0000000, -172.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(241, 'TO', 'TON', 'Tonga', -21.2387030, -175.1580120, 11)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(242, 'TV', 'TUV', 'Tuvalu', -8.0000000, 178.0000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(243, 'UM', 'UMI', 'United States Minor Outlying Islands', 19.2833000, 166.6000000, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(244, 'EH', 'ESH', 'Western Sahara', 25.0414130, -11.8359990, 6)
INSERT [dbo].[Country] ([countryId], [countryCode], [countryCode3], [country], [latitude], [longitude], [zoomLevel]) VALUES(245, 'ZW', 'ZWE', 'Zimbabwe', -19.2534840, 30.2196650, 6)

SET IDENTITY_INSERT [dbo].[Country] OFF
GO

SET IDENTITY_INSERT [dbo].[QuestionType] ON
GO

INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(1, 'Single / Multiple Choice (Text)', 1, 'This is single or multiple question that uses text for answers (distractors)', 'select one', 'correct text', 'incorrect message', 1, 'ICON_MULTI_CHOICE')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(2, 'True / False', 2, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TRUE_FALSE')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(3, 'Matching', 4, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_MATCHING')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(4, 'Fill in the Blank', 5, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_FILL_IN')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(5, 'Speedometer', 8, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_SPPEDO_METER')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(6, 'Hotspot', 6, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_HOTSPOT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(7, 'Single / Multiple Choice (Image)', 7, 'This is single or multiple question that uses images for answers (distractors)', 'select one', 'correct text', 'incorrect message', 1, 'ICON_IMAGE_CHOICE')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(8, 'Sequence', 3, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_ORDER')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(10, 'Open Answer (Single Line)', 3, NULL, NULL, NULL, NULL, 1, 'ICON_TEXT_ANSWER_SHORT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(11, 'Open Answer (Multi Line)', 4, NULL, NULL, NULL, NULL, 1, 'ICON_TEXT_ANSWER_LONG')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(12, 'Rate', 5, NULL, NULL, NULL, NULL, 1, 'ICON_TEXT_RATE')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(13, 'Rate Scale (Likert)', 6, NULL, NULL, NULL, NULL, 1, 'ICON_TEXT_LIKERT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(14, 'Weighted Bucket (Ratio)', 10, NULL, NULL, NULL, NULL, 1, 'ICON_WEIGHTED_BUCKET')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(15, 'Short Answer', 11, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TEXT_ANSWER_SHORT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(16, 'Essay', 12, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TEXT_ANSWER_LONG')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(17, 'Numerical', 13, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TEXT_ANSWER_SHORT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(18, 'Calculated', 14, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TEXT_ANSWER_SHORT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(19, 'Calculated Multichoice', 15, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_TEXT_ANSWER_SHORT')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(20, 'Multiple Dropdowns', 16, NULL, 'select one', 'correct text', 'incorrect message', 1, 'ICON_MULTI_DROP')
INSERT [dbo].[QuestionType] ([questionTypeId], [type], [questionTypeOrder], [questionTypeDescription], [instruction], [correctText], [incorrectMessage], [isActive], [iconSource]) VALUES(21, 'Text', 17, NULL, NULL, NULL, NULL, 1, 'ICON_TEXT_ANSWER_SHORT')

SET IDENTITY_INSERT [dbo].[QuestionType] OFF
GO

SET IDENTITY_INSERT [dbo].[Module] ON
GO

INSERT [dbo].[Module] ([moduleId], [moduleName], [dateCreated], [isActive]) VALUES(1, 'Educational Gaming', 'Dec 13 2012  9:18:00:000AM', 1)
INSERT [dbo].[Module] ([moduleId], [moduleName], [dateCreated], [isActive]) VALUES(2, 'Quiz_Test', 'Dec 25 2012 10:25:00:000AM', 1)

SET IDENTITY_INSERT [dbo].[Module] OFF
GO

SET IDENTITY_INSERT [dbo].[QuizFormat] ON
GO

INSERT [dbo].[QuizFormat] ([quizFormatId], [quizFormatName], [dateCreated], [isActive]) VALUES(1, 'Quiz', 'Dec 24 2012 10:24:00:000AM', 1)
INSERT [dbo].[QuizFormat] ([quizFormatId], [quizFormatName], [dateCreated], [isActive]) VALUES(2, 'Trivia (Knowledge Champion)', 'Aug  7 2013  7:55:00:000AM', 1)

SET IDENTITY_INSERT [dbo].[QuizFormat] OFF
GO

SET IDENTITY_INSERT [dbo].[ScoreType] ON
GO

INSERT [dbo].[ScoreType] ([scoreTypeId], [scoreType], [dateCreated], [isActive], [prefix], [defaultValue]) VALUES(1, 'Default', 'Dec 24 2012 10:23:00:000AM', 1, 'points', 10)
INSERT [dbo].[ScoreType] ([scoreTypeId], [scoreType], [dateCreated], [isActive], [prefix], [defaultValue]) VALUES(2, 'Custom', 'May  1 2013  9:20:00:000AM', 1, '$', 10)
INSERT [dbo].[ScoreType] ([scoreTypeId], [scoreType], [dateCreated], [isActive], [prefix], [defaultValue]) VALUES(3, 'Percentage', 'Oct 10 2013 12:00:00:000AM', 1, '%', 100)

SET IDENTITY_INSERT [dbo].[ScoreType] OFF
GO

SET IDENTITY_INSERT [dbo].[SNMapProvider] ON
GO

INSERT [dbo].[SNMapProvider] ([snMapProviderId], [mapProvider]) VALUES(1, 'MapQuest')
INSERT [dbo].[SNMapProvider] ([snMapProviderId], [mapProvider]) VALUES(2, 'IBM')

SET IDENTITY_INSERT [dbo].[SNMapProvider] OFF
GO

SET IDENTITY_INSERT [dbo].[SNService] ON
GO

INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(1, 'Facebook')
INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(2, 'Linkedin')
INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(3, 'Twitter')
INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(4, 'SlideShare')
INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(5, 'Blog')
INSERT [dbo].[SNService] ([snServiceId], [socialService]) VALUES(6, 'Other Web Links')

SET IDENTITY_INSERT [dbo].[SNService] OFF
GO

SET IDENTITY_INSERT [dbo].[State] ON
GO

INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(4, 'AK', 'Alaska', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(5, 'AL', 'Alabama', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(6, 'AS', 'American Samoa', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(7, 'AR', 'Arkansas', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(8, 'AZ', 'Arizona', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(10, 'CA', 'California', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(11, 'CO', 'Colorado', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(12, 'CT', 'Connecticut', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(13, 'DC', 'District of Columbia', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(14, 'DE', 'Delaware', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(15, 'FL', 'Florida', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(16, 'GA', 'Georgia', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(17, 'GU', 'Guam', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(18, 'HI', 'Hawaii', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(19, 'IA', 'Iowa', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(20, 'ID', 'Idaho', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(21, 'IL', 'Illinois', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(22, 'IN', 'Indiana', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(23, 'KS', 'Kansas', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(24, 'KY', 'Kentucky', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(25, 'LA', 'Louisiana', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(26, 'MA', 'Massachusetts', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(28, 'MD', 'Maryland', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(29, 'ME', 'Maine', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(30, 'MI', 'Michigan', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(31, 'MN', 'Minnesota', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(32, 'MO', 'Missouri', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(33, 'MS', 'Mississippi', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(34, 'MT', 'Montana', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(35, 'MP', 'Northern Mariana Islands', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(36, 'NC', 'North Carolina', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(37, 'ND', 'North Dakota', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(38, 'NE', 'Nebraska', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(39, 'NH', 'New Hampshire', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(40, 'NJ', 'New Jersey', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(42, 'NM', 'New Mexico', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(47, 'NV', 'Nevada', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(48, 'NY', 'New York', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(49, 'OH', 'Ohio', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(50, 'OK', 'Oklahoma', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(52, 'OR', 'Oregon', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(53, 'PA', 'Pennsylvania', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(55, 'PR', 'Puerto Rico', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(58, 'RI', 'Rhode Island', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(60, 'SC', 'South Carolina', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(61, 'SD', 'South Dakota', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(64, 'TN', 'Tennessee', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(65, 'TX', 'Texas', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(66, 'UT', 'Utah', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(67, 'VA', 'Virginia', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(68, 'VI', 'Virgin Islands', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(69, 'VT', 'Vermont', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(70, 'WA', 'Washington', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(72, 'WI', 'Wisconsin', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(73, 'WV', 'West Virginia', 1, 0.0000000, 0.0000000, 0)
INSERT [dbo].[State] ([stateId], [stateCode], [stateName], [isActive], [latitude], [longitude], [zoomLevel]) VALUES(74, 'WY', 'Wyoming', 1, 0.0000000, 0.0000000, 0)

SET IDENTITY_INSERT [dbo].[State] OFF
GO

SET IDENTITY_INSERT [dbo].[SubModule] ON
GO

INSERT [dbo].[SubModule] ([subModuleId], [moduleID], [subModuleName], [dateCreated], [isActive]) VALUES(1, 1, 'Crossword', 'Dec 13 2012  9:20:00:000AM', 1)
INSERT [dbo].[SubModule] ([subModuleId], [moduleID], [subModuleName], [dateCreated], [isActive]) VALUES(2, 2, 'Quiz', 'Dec 25 2012 10:20:00:000AM', 1)
INSERT [dbo].[SubModule] ([subModuleId], [moduleID], [subModuleName], [dateCreated], [isActive]) VALUES(3, 2, 'Test', 'Apr 22 2013  3:46:00:000AM', 1)
INSERT [dbo].[SubModule] ([subModuleId], [moduleID], [subModuleName], [dateCreated], [isActive]) VALUES(4, 2, 'Survey', 'Apr 22 2013  3:47:00:000AM', 1)
INSERT [dbo].[SubModule] ([subModuleId], [moduleID], [subModuleName], [dateCreated], [isActive]) VALUES(5, 2, 'SNProfile', 'Apr 22 2013  3:47:00:000AM', 1)

SET IDENTITY_INSERT [dbo].[SubModule] OFF
GO

SET IDENTITY_INSERT [dbo].[SurveyGroupingType] ON
GO

INSERT [dbo].[SurveyGroupingType] ([surveyGroupingTypeId], [surveyGroupingType]) VALUES(1, 'Single Page')
INSERT [dbo].[SurveyGroupingType] ([surveyGroupingTypeId], [surveyGroupingType]) VALUES(2, 'Auto Page Break')
INSERT [dbo].[SurveyGroupingType] ([surveyGroupingTypeId], [surveyGroupingType]) VALUES(3, 'Manual Page')

SET IDENTITY_INSERT [dbo].[SurveyGroupingType] OFF
GO

SET IDENTITY_INSERT [dbo].[LmsMeetingType] ON
GO

INSERT [dbo].[LmsMeetingType] ([lmsMeetingTypeId], [lmsMeetingTypeName]) VALUES(1, 'Meeting')
INSERT [dbo].[LmsMeetingType] ([lmsMeetingTypeId], [lmsMeetingTypeName]) VALUES(2, 'Office Hours')
INSERT [dbo].[LmsMeetingType] ([lmsMeetingTypeId], [lmsMeetingTypeName]) VALUES(4, 'Seminar')
INSERT [dbo].[LmsMeetingType] ([lmsMeetingTypeId], [lmsMeetingTypeName]) VALUES(3, 'Study Group')
INSERT [dbo].[LmsMeetingType] ([lmsMeetingTypeId], [lmsMeetingTypeName]) VALUES(5, 'Virtual Classroom')

SET IDENTITY_INSERT [dbo].[LmsMeetingType] OFF
GO

SET IDENTITY_INSERT [dbo].[LmsQuestionType] ON
GO


SET IDENTITY_INSERT [dbo].[LmsQuestionType] OFF
GO