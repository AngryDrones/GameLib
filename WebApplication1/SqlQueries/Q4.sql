SELECT Games.Name, GameLoans.Date
FROM Games
JOIN GameLoans ON Games.GameID = GameLoans.GameID
JOIN Users ON GameLoans.UserID = Users.UserID
WHERE Users.Fullname = @Fullname;
