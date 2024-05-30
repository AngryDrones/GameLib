SELECT Users.Fullname, Users.Email
FROM Users
JOIN Reviews ON Users.UserID = Reviews.UserID
WHERE Reviews.Rating = @Rating;