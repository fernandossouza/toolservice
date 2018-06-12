using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using securityfilter;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Controllers {
    [Route ("api/[controller]")]
    public class ToolController : Controller {
        private readonly IToolService _toolService;

        public ToolController (IToolService toolService) {
            _toolService = toolService;
        }

        [HttpGet]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetList ([FromQuery] int startat, [FromQuery] int quantity, [FromQuery] string fieldFilter, [FromQuery] string fieldValue, [FromQuery] string orderField, [FromQuery] string order) {

            var fieldFilterEnum = ToolFieldEnum.Default;
            Enum.TryParse (fieldFilter, true, out fieldFilterEnum);
            var orderFieldEnum = ToolFieldEnum.Default;
            Enum.TryParse (orderField, true, out orderFieldEnum);
            var orderEnumValue = OrderEnum.Ascending;
            Enum.TryParse (order, true, out orderEnumValue);

            if (quantity == 0)
                quantity = 50;
            var (tools, count) = await _toolService.getTools (startat, quantity, fieldFilterEnum, fieldValue, orderFieldEnum, orderEnumValue);
            return Ok (new { values = tools, total = count });

        }

        [HttpGet ("{id}")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetId (int id) {

            var tool = await _toolService.getTool (id);
            return Ok (tool);

        }

        [HttpGet ("thing/{thingid}")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetThindId (int thingid) {

            var tool = await _toolService.getToolsOnThing (thingid);
            return Ok (tool);

        }

        [HttpGet ("inuse/")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> getInUse () {
            var tool = await _toolService.getToolsInUSe ();
            return Ok (tool);

        }

        [HttpPost]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Post ([FromBody] Tool tool) {

            if (ModelState.IsValid) {
                tool.toolId = 0;

                tool = await _toolService.addTool (tool);
                return Created ($"api/Extract/{tool.toolId}", tool);
            }
            return BadRequest (ModelState);

        }

        [HttpPut ("{id}")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Put (int id, [FromBody] Tool tool, [FromQuery] string username) {

            if (ModelState.IsValid) {

                var toolDb = await _toolService.updateTool (id, tool,username);
                if (toolDb == null) {
                    return NotFound ();
                }
                return Ok (toolDb);
            }
            return BadRequest (ModelState);

        }

        [HttpDelete ("{id}")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Delete (int id, [FromQuery] string username) {
            {

                if (id > 0) {

                    var toolDb = await _toolService.deleteTool (id, username);
                    if (toolDb == null) {
                        return NotFound ();
                    }
                    return Ok (toolDb);
                }
                return BadRequest (ModelState);

            }

        }
    }
}