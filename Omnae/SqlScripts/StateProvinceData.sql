
SET IDENTITY_INSERT [dbo].[StateProvinces] ON

DECLARE @US_CountryId int
SET @US_CountryId = 226
DECLARE @CAD_CountryId int
SET @CAD_CountryId = 38

DECLARE @Others int
SET @Others = 100


INSERT INTO [dbo].[StateProvinces] (Id, Name, Abbreviation, Code) VALUES
(1, 'Alaska', 'AK', @US_CountryId),
(2, 'Alabama', 'AL', @US_CountryId),
(3, 'American Samoa', 'AS', @US_CountryId),
(4, 'Arizona', 'AZ', @US_CountryId),
(5, 'Arkansas', 'AR', @US_CountryId),
(6, 'California', 'CA', @US_CountryId),
(7, 'Colorado', 'CO', @US_CountryId),
(8, 'Connecticut', 'CT', @US_CountryId),
(9, 'Delaware', 'DE', @US_CountryId),
(10, 'District of Columbia', 'DC', @US_CountryId),
(11, 'Florida', 'FL', @US_CountryId),
(12, 'Georgia', 'GA', @US_CountryId), 
(13, 'Guam', 'GU', @US_CountryId),
(14, 'Hawaii', 'HI', @US_CountryId),
(15, 'Idaho', 'ID', @US_CountryId),
(16, 'Illinois', 'IL', @US_CountryId),
(17, 'Indiana', 'IN', @US_CountryId),
(18, 'Iowa', 'IA', @US_CountryId),
(19, 'Kansas', 'KS', @US_CountryId),
(20, 'Kentucky', 'KY', @US_CountryId),
(21, 'Louisiana', 'LA', @US_CountryId),
(22, 'Maine', 'ME', @US_CountryId),
(23, 'Maryland', 'MD', @US_CountryId),
(24, 'Massachusetts', 'MA', @US_CountryId),
(25, 'Michigan', 'MI', @US_CountryId),
(26, 'Minnesota', 'MN', @US_CountryId),
(27, 'Mississippi', 'MS', @US_CountryId),
(28, 'Missouri', 'MO', @US_CountryId),
(29, 'Montana', 'MT', @US_CountryId),
(30, 'Nebraska', 'NE', @US_CountryId),
(31, 'Nevada', 'NV', @US_CountryId),
(32, 'New Hampshire', 'NH', @US_CountryId),
(33, 'New Jersey', 'NJ', @US_CountryId),
(34, 'New Mexico', 'NM', @US_CountryId), 
(35, 'New York', 'NY', @US_CountryId),
(36, 'North Carolina', 'NC', @US_CountryId),
(37, 'North Dakota', 'ND', @US_CountryId),
(38, 'Northern Mariana Islands', 'MP', @US_CountryId),
(39, 'Ohio', 'OH', @US_CountryId),
(40, 'Oklahoma', 'OK', @US_CountryId),
(41, 'Oregon', 'OR', @US_CountryId),
(42, 'Palau', 'PW', @US_CountryId), 
(43, 'Pennsylvania', 'PA', @US_CountryId),
(44, 'Puerto Rico', 'PR', @US_CountryId),
(45, 'Rhode Island', 'RI', @US_CountryId),
(46, 'South Carolina', 'SC', @US_CountryId),
(47, 'South Dakota', 'SD', @US_CountryId),
(48, 'Tennessee', 'TN', @US_CountryId),
(49, 'Texas', 'TX', @US_CountryId), 
(50, 'Utah', 'UT', @US_CountryId),
(51, 'Vermont', 'VT', @US_CountryId),
(52, 'Virgin Islands', 'VI', @US_CountryId),
(53, 'Virginia', 'VA', @US_CountryId),
(54, 'Washington', 'WA', @US_CountryId),
(55, 'West Virginia', 'WV', @US_CountryId),
(56, 'Wisconsin', 'WI', @US_CountryId), 
(57, 'Wyoming', 'WY', @US_CountryId),
(58, 'Alberta', 'AB', @CAD_CountryId),
(59, 'British Columbia', 'BC', @CAD_CountryId),
(60, 'Manitoba', 'MB', @CAD_CountryId),
(61, 'New Brunswick', 'NB', @CAD_CountryId),
(62, 'Newfoundland and Labrador', 'NL', @CAD_CountryId),
(63, 'Northwest Territories', 'NT', @CAD_CountryId),
(64, 'Nova Scotia', 'NS', @CAD_CountryId),
(65, 'Nunavut', 'NU', @CAD_CountryId),
(66, 'Ontario', 'ON', @CAD_CountryId),
(67, 'Prince Edward Island', 'PE', @CAD_CountryId),
(68, 'Qu�bec', 'QC', @CAD_CountryId),
(69, 'Saskatchewan', 'SK', @CAD_CountryId),
(70, 'Yukon Territory', 'YT', @CAD_CountryId),
(100, 'Others', '', @Others); 

SET IDENTITY_INSERT [dbo].[StateProvinces] OFF
