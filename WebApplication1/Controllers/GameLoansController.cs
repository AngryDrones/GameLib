using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameLib.Models;

namespace GameLib.Controllers
{
    public class GameLoansController : Controller
    {
        private readonly GameLibContext _context;

        public GameLoansController(GameLibContext context)
        {
            _context = context;
        }

        // GET: GameLoans
        public async Task<IActionResult> Index()
        {
            var gameLibContext = _context.GameLoans.Include(g => g.Game).Include(g => g.User);
            return View(await gameLibContext.ToListAsync());
        }

        // GET: GameLoans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLoan = await _context.GameLoans
                .Include(g => g.Game)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (gameLoan == null)
            {
                return NotFound();
            }

            return View(gameLoan);
        }

        // GET: GameLoans/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: GameLoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanId,UserId,GameId,Date")] GameLoan gameLoan)
        {
            User user = _context.Users.FirstOrDefault(c => c.UserId == gameLoan.UserId);
            Game game = _context.Games.Include(g => g.User).Include(g => g.Genre).FirstOrDefault(c => c.GameId == gameLoan.GameId);

            if (user != null && game != null)
            {
                gameLoan.User = user;
                gameLoan.Game = game;

                ModelState.Clear();
                TryValidateModel(gameLoan);

                if (ModelState.IsValid)
                {
                    _context.Add(gameLoan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                Console.WriteLine("User or Game not found.");
            }

            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameLoan.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", gameLoan.UserId);
            return View(gameLoan);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("LoanId,UserId,GameId,Date")] GameLoan gameLoan)
        //{
        //    User user = _context.Users.FirstOrDefault(c => c.UserId == gameLoan.UserId);
        //    gameLoan.User = user;
        //    Game game = _context.Games.FirstOrDefault(c => c.GameId == gameLoan.GameId);
        //    gameLoan.Game = game;

        //    ModelState.Clear();
        //    TryValidateModel(gameLoan);

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(gameLoan);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var key in ModelState.Keys)
        //        {
        //            var state = ModelState[key];
        //            foreach (var error in state.Errors)
        //            {
        //                Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
        //            }
        //        }
        //    }


        //    ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameLoan.GameId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", gameLoan.UserId);
        //    return View(gameLoan);
        //}

        // GET: GameLoans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLoan = await _context.GameLoans.FindAsync(id);
            if (gameLoan == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameLoan.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", gameLoan.UserId);
            return View(gameLoan);
        }

        // POST: GameLoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,UserId,GameId,Date")] GameLoan gameLoan)
        {
            if (id != gameLoan.LoanId)
            {
                return NotFound();
            }

            User user = _context.Users.FirstOrDefault(c => c.UserId == gameLoan.UserId);
            Game game = _context.Games.Include(g => g.User).Include(g => g.Genre).FirstOrDefault(c => c.GameId == gameLoan.GameId);
            gameLoan.User = user;
            gameLoan.Game = game;

            ModelState.Clear();
            TryValidateModel(gameLoan);

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                TryValidateModel(gameLoan);

                try
                {
                    _context.Update(gameLoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameLoanExists(gameLoan.LoanId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "GameId", "GameId", gameLoan.GameId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", gameLoan.UserId);
            return View(gameLoan);
        }

        // GET: GameLoans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameLoan = await _context.GameLoans
                .Include(g => g.Game)
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (gameLoan == null)
            {
                return NotFound();
            }

            return View(gameLoan);
        }

        // POST: GameLoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameLoan = await _context.GameLoans.FindAsync(id);
            if (gameLoan != null)
            {
                _context.GameLoans.Remove(gameLoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameLoanExists(int id)
        {
            return _context.GameLoans.Any(e => e.LoanId == id);
        }
    }
}
