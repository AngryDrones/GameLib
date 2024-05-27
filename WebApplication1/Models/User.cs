using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<GameLoan> GameLoans { get; set; } = new List<GameLoan>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
