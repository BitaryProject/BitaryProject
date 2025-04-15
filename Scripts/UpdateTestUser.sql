-- Replace 'your-email@example.com' with the actual email of the user you want to update
DECLARE @UserEmail NVARCHAR(256) = 'your-email@example.com';
DECLARE @UserId NVARCHAR(450);

-- Get the user ID
SELECT @UserId = Id FROM AspNetUsers WHERE Email = @UserEmail;

IF @UserId IS NOT NULL
BEGIN
    -- Update user information
    UPDATE AspNetUsers
    SET 
        FirstName = 'TestFirstName',
        LastName = 'TestLastName',
        DisplayName = 'TestFirstName TestLastName',
        Gender = 1,
        PhoneNumber = '98765432100'
    WHERE Id = @UserId;
    
    -- Check if address exists
    DECLARE @AddressExists INT;
    SELECT @AddressExists = COUNT(*) FROM Addresses WHERE UserId = @UserId;
    
    IF @AddressExists > 0
    BEGIN
        -- Update existing address
        UPDATE Addresses
        SET 
            Name = 'TestUser',
            Street = 'TestStreet',
            City = 'TestCity',
            Country = 'TestCountry'
        WHERE UserId = @UserId;
    END
    ELSE
    BEGIN
        -- Insert new address
        INSERT INTO Addresses (Name, Street, City, Country, UserId)
        VALUES ('TestUser', 'TestStreet', 'TestCity', 'TestCountry', @UserId);
    END
    
    SELECT 'User and address updated successfully' AS Status;
END
ELSE
BEGIN
    SELECT 'User not found' AS Status;
END 