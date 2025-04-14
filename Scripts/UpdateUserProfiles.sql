-- Update users with null or empty FirstName
UPDATE AspNetUsers
SET FirstName = COALESCE(UserName, LEFT(Email, CHARINDEX('@', Email) - 1), 'User')
WHERE FirstName IS NULL OR LTRIM(RTRIM(FirstName)) = '';

-- Update users with null or empty LastName
UPDATE AspNetUsers
SET LastName = 'Account'
WHERE LastName IS NULL OR LTRIM(RTRIM(LastName)) = '';

-- Update users with null or empty DisplayName
UPDATE AspNetUsers
SET DisplayName = CONCAT(FirstName, ' ', LastName)
WHERE DisplayName IS NULL OR LTRIM(RTRIM(DisplayName)) = '';

-- Create addresses for users who don't have one
-- First, get all users without an address
DECLARE @UsersWithoutAddress TABLE (UserId NVARCHAR(450));
INSERT INTO @UsersWithoutAddress
SELECT u.Id
FROM AspNetUsers u
LEFT JOIN Addresses a ON u.Id = a.UserId
WHERE a.UserId IS NULL;

-- Then create addresses for these users
INSERT INTO Addresses (Name, Street, City, Country, UserId)
SELECT 
    CONCAT(u.FirstName, ' ', u.LastName),
    'Not specified',
    'Not specified',
    'Not specified',
    u.Id
FROM AspNetUsers u
INNER JOIN @UsersWithoutAddress w ON u.Id = w.UserId;

-- Print completion message
SELECT 
    (SELECT COUNT(*) FROM AspNetUsers) AS TotalUsers,
    (SELECT COUNT(*) FROM @UsersWithoutAddress) AS UsersWithAddressesCreated; 