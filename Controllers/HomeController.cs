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
            var cmd = new UpdateCountCommand(item, req.Value > 0 ? 1 : -1);
            _cmdHandler.UpdateCount(cmd);

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
