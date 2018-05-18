using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using securityfilter;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Controllers {
    [Route ("api/tool/[controller]")]
    public class StateTransitionHistoryController : Controller {
        private readonly IStateTransitionHistoryService _stateTransitionHistoryService;
        public StateTransitionHistoryController (IStateTransitionHistoryService stateTransitionHistoryService) {
            _stateTransitionHistoryService = stateTransitionHistoryService;
        }

        [HttpGet]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetId ([FromQuery] int toolid, [FromQuery] long? from = null, [FromQuery] long? to = null) {
            if (from == null) {
                from = DateTime.Now.Date.Ticks;
            }
            if (to == null) {
                to = DateTime.Now.AddDays (1).Date.Ticks;
            }
            var history = await _stateTransitionHistoryService.getToolHistory (toolid, from.Value, to.Value);
            return Ok (new { values = history, total = history.Count });

        }
    }
}