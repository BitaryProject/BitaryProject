-- Script to diagnose and fix user address issues
-- Run this on your database to see what's wrong

-- 1. Check if there are any users without addresses
SELECT u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber
FROM AspNetUsers u
LEFT JOIN Address a ON u.Id = a.UserId
WHERE u.Discriminator = 'User' -- Only interested in application users, not IdentityUser
AND a.Id IS NULL
ORDER BY u.Email;

-- 2. Check for orphaned addresses (addresses without valid user references)
SELECT a.Id, a.UserId, a.Name, a.Street, a.City, a.Country
FROM Address a
LEFT JOIN AspNetUsers u ON a.UserId = u.Id
WHERE u.Id IS NULL;

-- 3. Show all users with their addresses
SELECT u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber, 
       a.Id as AddressId, a.Name, a.Street, a.City, a.Country
FROM AspNetUsers u
LEFT JOIN Address a ON u.Id = a.UserId
WHERE u.Discriminator = 'User'
ORDER BY u.Email;

-- 4. Check if there are duplicate addresses for a single user
SELECT UserId, COUNT(*) as AddressCount
FROM Address
GROUP BY UserId
HAVING COUNT(*) > 1;

-- 5. Fix script: Update addresses with the user ID if they are missing
-- Uncomment and modify this if you need to update addresses
/*
DECLARE @UserEmail NVARCHAR(256) = 'your.email@example.com'; -- Replace with your email
DECLARE @UserId NVARCHAR(450);

-- Get the user ID
SELECT @UserId = Id FROM AspNetUsers WHERE Email = @UserEmail;

IF @UserId IS NOT NULL
BEGIN
    -- Check if user already has an address
    DECLARE @AddressExists INT;
    SELECT @AddressExists = COUNT(*) FROM Address WHERE UserId = @UserId;
    
    IF @AddressExists = 0
    BEGIN
        -- Create a new address for this user
        INSERT INTO Address (Name, Street, City, Country, UserId)
        VALUES ('Default Name', 'Default Street', 'Default City', 'Default Country', @UserId);
        
        SELECT 'Address created for user ' + @UserEmail AS Status;
    END
    ELSE
    BEGIN
        SELECT 'User already has an address' AS Status;
    END
END
ELSE
BEGIN
    SELECT 'User not found' AS Status;
END
*/ 