WITH SearchGames AS (
    SELECT g.GameID
    FROM Users u
    JOIN GameLoans gl ON u.UserID = gl.UserID
    JOIN Games g ON gl.GameID = g.GameID
    WHERE u.Fullname = @Fullname
),
UserGames AS (
    SELECT gl.UserID, gl.GameID
    FROM GameLoans gl
)

SELECT DISTINCT u.UserID, u.Fullname
FROM Users u
WHERE NOT EXISTS (
    SELECT sg.GameID
    FROM SearchGames sg
    WHERE NOT EXISTS (
        SELECT ug.GameID
        FROM UserGames ug
        WHERE ug.UserID = u.UserID
        AND ug.GameID = sg.GameID
        )
)
AND u.Fullname <> @Fullname;