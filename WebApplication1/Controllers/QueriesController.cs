using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using GameLib.Models;
using Microsoft.EntityFrameworkCore;

public class QueriesController : Controller
{
    private readonly string _connectionString;
    private readonly GameLibContext _context;
    private readonly string _sqlQueriesPath = Path.Combine(Directory.GetCurrentDirectory(), "SqlQueries");

    public QueriesController(IConfiguration configuration, GameLibContext context)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new SqlQueryViewModel
        {
            QueryOptions = GetQueryOptions(),
            Fullnames = GetUserFullnames()
        };

        return View(model);
    }

    private List<string> GetUserFullnames()
    {
        var userFullnames = _context.Users.Select(u => u.Fullname).ToList();
        return userFullnames;
    }


    [HttpPost]
    public async Task<IActionResult> ExecuteQuery(SqlQueryViewModel model)
    {
        if (string.IsNullOrEmpty(model.SelectedQuery))
        {
            ModelState.AddModelError("", "Please select a query.");
            model.QueryOptions = GetQueryOptions();
            return View("Index", model);
        }

        var queryFilePath = Path.Combine(_sqlQueriesPath, model.SelectedQuery);
        var query = await System.IO.File.ReadAllTextAsync(queryFilePath);

        var parameters = new List<SqlParameter>();
        if (model.SelectedQuery == "Q1.sql")
        {
            var isAvailableParam = new SqlParameter("@IsAvailable", SqlDbType.Bit);
            isAvailableParam.Value = model.IsAvailable;
            parameters.Add(isAvailableParam);
        }
        else if (model.SelectedQuery == "Q2.sql")
        {
            var ratingParam = new SqlParameter("@Rating", SqlDbType.Int);
            ratingParam.Value = model.Rating;
            parameters.Add(ratingParam);
        }
        else if (model.SelectedQuery == "Q3.sql")
        {
            var emailParam = new SqlParameter("@Email", SqlDbType.NVarChar);
            emailParam.Value = model.Email;
            parameters.Add(emailParam);
        }

        var queryResult = await ExecuteSqlQuery(query, parameters.ToArray());
        model.ColumnNames = queryResult.Item1;
        model.Rows = queryResult.Item2;
        model.QueryOptions = GetQueryOptions();

        return View("Index", model);
    }

    private List<SqlQueryOption> GetQueryOptions()
    {
        var queryOptions = new List<SqlQueryOption>();
        var queryFiles = Directory.GetFiles(_sqlQueriesPath, "*.sql");

        foreach (var file in queryFiles)
        {
            queryOptions.Add(new SqlQueryOption
            {
                Name = Path.GetFileNameWithoutExtension(file),
                FilePath = Path.GetFileName(file)
            });
        }

        return queryOptions;
    }

    private async Task<(List<string>, List<List<string>>)> ExecuteSqlQuery(string query, params SqlParameter[] parameters)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var columnNames = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columnNames.Add(reader.GetName(i));
                    }

                    var rows = new List<List<string>>();
                    while (await reader.ReadAsync())
                    {
                        var row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i).ToString());
                        }
                        rows.Add(row);
                    }

                    // Логування результатів
                    Console.WriteLine("Columns: " + string.Join(", ", columnNames));
                    Console.WriteLine("Rows: " + rows.Count);

                    return (columnNames, rows);
                }
            }
        }
    }

    // Some queries are just broken and 0 rows are returned. Everything works with direct queries in SQL Management Studio though.
    [HttpPost]
    public async Task<IActionResult> Query_4(string fullname)
    {
        string query =
            @"
            SELECT Games.Name, GameLoans.Date
            FROM Games
            JOIN GameLoans ON Games.GameID = GameLoans.GameID
            JOIN Users ON GameLoans.UserID = Users.UserID
            WHERE Users.Fullname = @Fullname;
            ";

        var fullnameParam = new SqlParameter("@Fullname", SqlDbType.NVarChar);
        fullnameParam.Value = fullname;

        var queryResult = await ExecuteSqlQuery(query, fullnameParam);

        var model = new SqlQueryViewModel
        {
            ColumnNames = queryResult.Item1,
            Rows = queryResult.Item2,
            QueryOptions = GetQueryOptions()
        };

        return View("Index", model);
    }


    [HttpPost]
    public async Task<IActionResult> Query_6(string fullname)
    {
        string query =
            @"
            SELECT DISTINCT U1.Fullname
            FROM Users U1
            WHERE U1.Fullname <> @Fullname
            AND NOT EXISTS (
                SELECT GameID
                FROM GameLoans GL1
                WHERE GL1.UserID = U1.UserID
                AND NOT EXISTS (
                    SELECT *
                    FROM GameLoans GL2
                    WHERE GL2.UserID = (
                        SELECT UserID
                        FROM Users
                        WHERE Fullname = @Fullname
                    )
                    AND GL2.GameID = GL1.GameID
                )
            );
            ";

        var fullnameParam = new SqlParameter("@Fullname", SqlDbType.NVarChar);
        fullnameParam.Value = fullname;

        var queryResult = await ExecuteSqlQuery(query, fullnameParam);

        var model = new SqlQueryViewModel
        {
            ColumnNames = queryResult.Item1,
            Rows = queryResult.Item2,
            QueryOptions = GetQueryOptions()
        };

        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> Query_7(string fullname)
    {
        string query =
            @"
            WITH SearchGames AS (
                SELECT g.GameID
                FROM Users u
                JOIN GameLoans gl ON u.UserID = gl.UserID
                JOIN Games g ON gl.GameID = g.GameID
                WHERE u.Fullname = @Fullname
            ),
            UserGames AS (
                SELECT gl.UserID, gl.GameID
                FROM GameLoans gl
            )

            SELECT DISTINCT u.UserID, u.Fullname
            FROM Users u
            WHERE NOT EXISTS (
                SELECT sg.GameID
                FROM SearchGames sg
                WHERE NOT EXISTS (
                    SELECT ug.GameID
                    FROM UserGames ug
                    WHERE ug.UserID = u.UserID
                    AND ug.GameID = sg.GameID
                )
            )
            AND u.Fullname <> @Fullname;
            ";

        var fullnameParam = new SqlParameter("@Fullname", SqlDbType.NVarChar);
        fullnameParam.Value = fullname;

        var queryResult = await ExecuteSqlQuery(query, fullnameParam);

        var model = new SqlQueryViewModel
        {
            ColumnNames = queryResult.Item1,
            Rows = queryResult.Item2,
            QueryOptions = GetQueryOptions()
        };

        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> Query_8(string fullname)
    {
        string query =
            @"
            SELECT DISTINCT Fullname
            FROM Users
            WHERE UserID IN (
                SELECT DISTINCT UserID
                FROM GameLoans
                WHERE GameID IN (
                    SELECT DISTINCT GameID
                    FROM GameLoans
                    WHERE UserID = (SELECT UserID FROM Users WHERE Fullname = @Fullname)
                )
            );
            ";
        var fullnameParam = new SqlParameter("@Fullname", SqlDbType.NVarChar);
        fullnameParam.Value = fullname;

        var queryResult = await ExecuteSqlQuery(query, fullnameParam);

        var model = new SqlQueryViewModel
        {
            ColumnNames = queryResult.Item1,
            Rows = queryResult.Item2,
            QueryOptions = GetQueryOptions()
        };

        return View("Index", model);
    }
}
