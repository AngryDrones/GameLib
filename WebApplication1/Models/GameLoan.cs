using System;
using System.Collections.Generic;

namespace GameLib.Models;

public partial class GameLoan
{
    public int LoanId { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public DateTime Date { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
