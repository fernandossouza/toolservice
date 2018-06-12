using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using toolservice.Data;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Service {
    public class AssociateToolService : IAssociateToolService {
        private readonly IToolService _toolService;
        private readonly IToolTypeService _toolTypeService;
        private readonly IStateManagementService _stateManagementService;
        private readonly IThingGroupService _thingGroupService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient client = new HttpClient ();

        public AssociateToolService (IStateManagementService stateManagementService,
            IToolTypeService toolTypeService,
            IToolService toolService,
            IThingGroupService thingGroupService,
            IConfiguration configuration) {
            _toolTypeService = toolTypeService;
            _toolService = toolService;
            _stateManagementService = stateManagementService;
            _thingGroupService = thingGroupService;
            _configuration = configuration;
        }
        public async Task<(Tool, string)> AssociateTool (int thingId, int toolId) {
            var tool = await _toolService.getTool (toolId);
            if (tool == null)
                return (null, "Tool Not Found");
            var availableTools = await _toolService.getToolsAvailable ();
            if (availableTools.Where (x => x.toolId == toolId).Count () < 1)
                return (null, "Tool Not Available");
            var toolType = await _toolTypeService.getToolType (tool.typeId.Value);
            if (toolType == null)
                return (null, "Tool Type Not Found");
            var thingGroups = toolType.thingGroups;
            bool contains = false;
            foreach (var group in thingGroups) {
                var (completeGroup, status) = await _thingGroupService.getGroup (group.thingGroupId);
                if (status == HttpStatusCode.OK)
                    contains = completeGroup.things.Select (x => x.thingId).Contains (thingId);
            }
            if (!contains) {
                return (null, "This Tool can't  be associated with this thing.");
            }

            return (tool, "Tool Set to Use");
        }
        public async Task<(Tool, string)> AssociateTool (int thingId, int toolId, int? position) {
            var tool = await _toolService.getTool (toolId);
            var toolsInUse = await _toolService.getToolsInUSe ();
            foreach (var otherTool in toolsInUse) {
                if (position == otherTool.position &&
                    otherTool.currentThingId == thingId) {
                    return (null, "There is already a tool of the same type and position");
                }
            }
            tool = await _toolService.getTool (toolId);
            return (tool, "Tool Set to Use");
        }

        public async Task<(Tool, string)> AssociateWithoutPosition (int thingId, int toolId, string username) {
            var tool = await _toolService.getTool (toolId);
            tool.username = username;
            string result;
            (tool, result) = await AssociateTool (thingId, toolId);
            if (tool == null)
                return (null, result);
            tool = await _stateManagementService.setToolToStatusById (toolId, stateEnum.in_use, null, username);
            tool = await _toolService.setToolToThing (tool, thingId);
            tool = await _toolService.getTool (tool.toolId);

            Trigger (tool);
            return (tool, result);
        }

        public async Task<(Tool, string)> AssociateWithPosition (int thingId, int toolId, int? position, string username) {
            Tool tool;
            string result;

            (tool, result) = await AssociateTool (thingId, toolId);
            if (tool == null) {
                return (null, result);
            }

            (tool, result) = await AssociateTool (thingId, toolId, position);
            if (tool == null) {
                return (null, result);
            }

            tool.username = username;
            tool = await _stateManagementService.setToolToStatusById (toolId, stateEnum.in_use, null, "");
            tool = await _toolService.setToolToThing (tool, thingId);
            tool = await _toolService.setToolToPosition (tool, position);
            tool = await _toolService.getTool (tool.toolId);

            Trigger (tool);
            return (tool, result);
        }
        public async Task<(Tool, string)> DisassociateTool (Tool tool, string username) {
            var toolDb = await _toolService.getTool (tool.toolId);
            if (tool == null)
                return (null, "Tool Not Found");
            if (toolDb.status != stateEnum.in_use.ToString ())
                return (null, "Tool Not In Use");
            if (toolDb.currentLife > tool.currentLife)
                return (null, "Current Life can be greater than the previous Life");
            tool.username = username;
            toolDb = await _stateManagementService.setToolToStatusById (tool.toolId, stateEnum.available, null, username);
            toolDb = await _toolService.setToolToThing (toolDb, null);
            toolDb = await _toolService.setToolToPosition (toolDb, null);
            toolDb.currentThing = null;
            Trigger (toolDb);
            return (toolDb, "Tool Set to Available");
        }

        private async void Trigger (Tool tool) {
            try {
                if (_configuration["AssociationPostEndpoint"] != null) {
                    client.DefaultRequestHeaders.Accept.Clear ();
                    client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                    var content = new StringContent (JsonConvert.SerializeObject (tool), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync (_configuration["AssociationPostEndpoint"], content);
                    Console.WriteLine (JsonConvert.SerializeObject (tool));
                    if (response.IsSuccessStatusCode) {
                        Console.WriteLine ("Data posted on AssociationPostEndpoint");
                    } else {
                        Console.WriteLine (response.StatusCode);
                        Console.WriteLine (await response.Content.ReadAsStringAsync ());
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine ("Error on AssociationPostEndpoint");
                Console.WriteLine (ex.ToString ());
            }
        }
    }
}