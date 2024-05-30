using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameLib.Models
{
    public class SqlQueryViewModel
    {
        public List<SqlQueryOption> QueryOptions { get; set; }

        public string SelectedQuery { get; set; }

        public List<string> ColumnNames { get; set; }

        public List<List<string>> Rows { get; set; }

        // Параметри
        public int IsAvailable { get; set; } // Q1
        public int Rating { get; set; } // Q2
        public string Email { get; set; } // Q3
        public string Fullname { get; set; } // Q4
        public List<string> Fullnames { get; set; } // Q4

    }

    public class SqlQueryOption
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
    }
}
