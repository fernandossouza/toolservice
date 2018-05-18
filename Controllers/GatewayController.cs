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
    [Route ("")]
    public class GatewayController : Controller {
        private IConfiguration _configuration;
        private IThingGroupService _thingGroupService;
        public GatewayController (IConfiguration configuration,
            IThingGroupService thingGroupService) {
            _configuration = configuration;
            _thingGroupService = thingGroupService;
        }

        [HttpGet ("gateway/thinggroups/")]
        [Produces ("application/json")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetGroups ([FromQuery] int startat, [FromQuery] int quantity, [FromQuery] string fieldFilter, [FromQuery] string fieldValue, [FromQuery] string orderField, [FromQuery] string order) {

            var (thingGroups, resultCode) = await _thingGroupService.getGroups (startat, quantity, fieldFilter,
                fieldValue, orderField, order);
            switch (resultCode) {
                case HttpStatusCode.OK:
                    return Ok (thingGroups);
                case HttpStatusCode.NotFound:
                    return NotFound ();
            }
            return StatusCode (StatusCodes.Status500InternalServerError);
        }

        [HttpGet ("gateway/thinggroups/{id}")]
        [Produces ("application/json")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetGroup (int id) {
            var (thingGroup, resultCode) = await _thingGroupService.getGroup (id);
            switch (resultCode) {
                case HttpStatusCode.OK:
                    return Ok (thingGroup);
                case HttpStatusCode.NotFound:
                    return NotFound ();
            }
            return StatusCode (StatusCodes.Status500InternalServerError);
        }

        [HttpGet ("gateway/thinggroups/attachedthings/{groupid}")]
        [Produces ("application/json")]
        [SecurityFilter ("tools__allow_read")]
        public async Task<IActionResult> GetAttachedThings (int groupid) {
            var (things, resultCode) = await _thingGroupService.GetAttachedThings (groupid);
            switch (resultCode) {
                case HttpStatusCode.OK:
                    return Ok (things);
                case HttpStatusCode.NotFound:
                    return NotFound ();
            }
            return StatusCode (StatusCodes.Status500InternalServerError);
        }
    }
}