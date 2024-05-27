using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public int Rating { get; set; }

    public string Caption { get; set; } = null!;

    public DateTime Date { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
