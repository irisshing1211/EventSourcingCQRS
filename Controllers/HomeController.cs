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
        private readonly EventSourceContext _eventContext;

        public HomeController(ILogger<HomeController> logger,
                              IQueryService<CountItem> queries,
                              QueryContext queryCtx,
                              EventSourceContext eventCtx)
        {
            _logger = logger;
            _queries = queries;
            _cmdHandler = new CommandHandler(eventCtx, queryCtx);
            _eventContext = eventCtx;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _queries.GetAll();
            var logs = _eventContext.EventLogs.OrderBy(a => a.Time).Select(a => a.Time).ToList();
            var model = new IndexModel {Items = list, Logs = logs};
            return View(model);
        }

        public IActionResult UpdateCount(UpdateCountModel req)
        {
            var item = _queries.GetById(req.Id);
            var cmd = new UpdateCountCommand(item, req.Value > 0 ? 1 : -1);
            _cmdHandler.UpdateCount(cmd);

            return RedirectToAction("Index");
        }

        public IActionResult Rollback(RollbackModel req)
        {
            _cmdHandler.Rollback(new RollbackCommand {Time = req.Time});

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
