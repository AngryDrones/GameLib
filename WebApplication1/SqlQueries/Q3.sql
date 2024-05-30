SELECT DISTINCT g.Name
FROM Games g
JOIN GameLoans gl ON g.GameId = gl.GameId
JOIN Users u ON gl.UserId = u.UserId
WHERE u.Email LIKE '%' + @Email
