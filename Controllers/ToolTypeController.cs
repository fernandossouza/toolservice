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
    public class ToolTypeController : Controller {
        private readonly IToolTypeService _toolTypeService;

        public ToolTypeController (IToolTypeService toolTypeService) {
            _toolTypeService = toolTypeService;
        }

        [HttpGet]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetList ([FromQuery] int startat, [FromQuery] int quantity) {
            try {
                if (quantity == 0)
                    quantity = 50;
                var toolTypes = await _toolTypeService.getToolTypes (startat, quantity);
                return Ok (toolTypes);
            } catch (Exception ex) {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpGet ("{id}")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetId (int id) {
            try {
                var toolType = await _toolTypeService.getToolType (id);
                return Ok (toolType);
            } catch (Exception ex) {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpPost]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Post ([FromBody] ToolType toolType) {
            try {
                if (ModelState.IsValid) {
                    toolType.toolTypeId = 0;

                    toolType = await _toolTypeService.addToolType (toolType);
                    return Created ($"api/Extract/{toolType.toolTypeId}", toolType);
                }
                return BadRequest (ModelState);
            } catch (Exception ex) {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpPut ("{id}")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Put (int id, [FromBody] ToolType toolType) {
            try {
                if (ModelState.IsValid) {

                    var toolTypeDb = await _toolTypeService.updateToolType (id, toolType);
                    if (toolTypeDb == null) {
                        return NotFound ();
                    }
                    return Ok (toolTypeDb);
                }
                return BadRequest (ModelState);
            } catch (Exception ex) {
                return StatusCode (500, ex.Message);
            }
        }

        [HttpDelete ("{id}")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Delete (int id) {
            try {

                if (id > 0) {

                    var toolTypeDb = await _toolTypeService.deleteToolType (id);
                    if (toolTypeDb == null) {
                        return NotFound ();
                    }
                    return Ok (toolTypeDb);
                }
                return BadRequest (ModelState);
            } catch (Exception ex) {
                return StatusCode (500, ex.Message);
            }
        }

    }
}