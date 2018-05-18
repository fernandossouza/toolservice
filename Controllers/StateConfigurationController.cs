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
using toolservice.Service.Interface;

namespace toolservice.Controllers {
    [Route ("api/tool/[controller]")]
    public class StateConfigurationController : Controller {
        private readonly IStateManagementService _stateManagementService;
        public StateConfigurationController (IStateManagementService stateManagementService) {
            _stateManagementService = stateManagementService;
        }

        [HttpGet]
        [SecurityFilter ("tools__allow_read")]
        public IActionResult Get () {
            var stateConfiguration = _stateManagementService.getPossibleStatusTransition ();
            if (stateConfiguration == null)
                return NotFound ();
            return Ok (stateConfiguration);
        }
    }
}