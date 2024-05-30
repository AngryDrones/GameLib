SELECT Games.Name, Genres.Name AS Genre
FROM Games
JOIN Genres ON Games.GenreID = Genres.GenreID
WHERE Games.Available = @IsAvailable;
