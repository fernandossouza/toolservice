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
    public class AssociateToolController : Controller {
        private readonly IAssociateToolService _associateToolService;
        public AssociateToolController (IAssociateToolService associateToolService) {
            _associateToolService = associateToolService;
        }

        [HttpPut ("associate/")]
        [Produces ("application/json")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Associate ([FromQuery] int thingId, [FromQuery] int toolId, [FromQuery] int? position) {
            Tool tool;
            string result;
            if (position == null)
                (tool, result) = await _associateToolService.AssociateWithoutPosition (thingId, toolId);
            else
                (tool, result) = await _associateToolService.AssociateWithPosition (thingId, toolId, position);

            if (tool == null)
                return BadRequest (result);
            return Ok (tool);
        }

        [HttpPut ("disassociate/")]
        [Produces ("application/json")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Disassociate ([FromBody] Tool tool) {
            if (ModelState.IsValid) {
                var (returnTool, result) = await _associateToolService.DisassociateTool (tool);
                if (returnTool == null)
                    return BadRequest (result);
                return Ok (returnTool);
            }
            return BadRequest (ModelState);
        }
    }
}