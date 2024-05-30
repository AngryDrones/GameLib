SELECT DISTINCT U1.Fullname
FROM Users U1
WHERE U1.Fullname <> @Fullname
AND NOT EXISTS (
    SELECT GameID
    FROM GameLoans GL1
    WHERE GL1.UserID = U1.UserID
    AND NOT EXISTS (
        SELECT *
        FROM GameLoans GL2
        WHERE GL2.UserID = (
            SELECT UserID
            FROM Users
            WHERE Fullname = @Fullname
        )
        AND GL2.GameID = GL1.GameID
    )
);
