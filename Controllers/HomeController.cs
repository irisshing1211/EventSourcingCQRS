using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EventSourcingCQRS.Commands;
using EventSourcingCQRS.Entities;
using EventSourcingCQRS.Entities.NoSql;
using EventSourcingCQRS.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventSourcingCQRS.Models;
using EventSourcingCQRS.Query;
using MongoDB.Driver;

namespace EventSourcingCQRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQueryService<CountItem> _queries;
        private readonly CommandHandler _cmdHandler;

        public HomeController(ILogger<HomeController> logger,
                              IQueryService<CountItem> queries,
                              QueryContext queryCtx,
                              EventSourceContext eventCtx)
        {
            _logger = logger;
            _queries = queries;
            _cmdHandler = new CommandHandler(eventCtx, queryCtx);
        }

        public async Task<IActionResult> Index()
        {
            var list = await _queries.GetAll();

            return View(list);
        }

        public IActionResult UpdateCount(UpdateCountModel req)
        {
            var item = _queries.GetById(req.Id);

            if (req.Value > 0)
            {
                var cmd = new AddCountCommand {Id = req.Id, NewValue = item.Count + 1};
                _cmdHandler.AddCount(cmd);
            }
            else
            {
                var cmd = new ReduceCountCommand {Id = req.Id, NewValue = item.Count - 1};
                _cmdHandler.ReduceCount(cmd);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy() { return View(); }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
