using System;
using System.Collections.Generic;
using 
    .Models;

namespace GameLib;

public partial class Game
{
    public int GameId { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public int GenreId { get; set; }

    public bool Available { get; set; }

    public virtual ICollection<GameLoan> GameLoans { get; set; } = new List<GameLoan>();

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User User { get; set; } = null!;
}
