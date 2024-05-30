SELECT DISTINCT Fullname
FROM Users
WHERE UserID IN (
    SELECT DISTINCT UserID
    FROM GameLoans
    WHERE GameID IN (
        SELECT DISTINCT GameID
        FROM GameLoans
        WHERE UserID = (SELECT UserID FROM Users WHERE Fullname = @Fullname)
    )
);
