SELECT Genres.Name, COUNT(Games.GameID) AS GameCount
FROM Genres
LEFT JOIN Games ON Genres.GenreID = Games.GenreID
GROUP BY Genres.Name;