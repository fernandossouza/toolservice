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
    public class ToolInformationController : Controller {
        private readonly IToolInformationService _toolInformationService;

        public ToolInformationController (IToolInformationService toolInformationService) {
            _toolInformationService = toolInformationService;
        }

        [HttpPost ("{toolId}")]
        [SecurityFilter ("tools__allow_update")]
        public async Task<IActionResult> Post (int toolId, [FromBody] ToolInformation toolInformation) {
            try {
                if (ModelState.IsValid) {
                    toolInformation.toolInformationId = 0;
                    toolInformation = await _toolInformationService.addToolInformation (toolId, toolInformation);
                    return Created ($"api/ToolInformation/{ toolInformation.toolInformationId }", toolInformation);
                }
                return BadRequest (ModelState);
            } catch (Exception ex) {
                return StatusCode (500, ex.ToString ());
            }
        }

    }
}