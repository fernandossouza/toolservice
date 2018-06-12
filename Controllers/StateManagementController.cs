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
    public class StateManagementController : Controller {
        private readonly IStateManagementService _stateManagementService;
        public StateManagementController (IStateManagementService stateManagementService) {
            _stateManagementService = stateManagementService;
        }

        [HttpPut ("id/")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> UpdateById ([FromBody] Justification justification, [FromQuery] int toolid, [FromQuery] string state,[FromQuery] string username) {

            stateEnum newState = stateEnum.available;
            if (!Enum.TryParse (state, out newState))
                return BadRequest ("State Not Found");

            var tools = await _stateManagementService.setToolToStatusById (toolid, newState, justification,username);
            if (tools == null)
                return BadRequest ("State Change not Allowed By Configuration");
            return Ok (tools);
        }

        [HttpPut ("number/")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> UpdateByNumber ([FromBody] Justification justification, [FromQuery] string serial, [FromQuery] string state, [FromQuery] string username) {

            stateEnum newState = stateEnum.available;
            if (!Enum.TryParse (state, out newState))
                return BadRequest ("State Not Found");
            var tools = await _stateManagementService.setTootlToStatusByNumber (serial, newState, justification, username);
            if (tools == null)
                return BadRequest ("State Change not Allowed By Configuration");
            return Ok (tools);
        }
    }
}