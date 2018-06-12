using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using toolservice.Data;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Service {
    public class StateManagementService : IStateManagementService {
        private readonly StateConfiguration _stateConfiguration;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;

        private readonly IStateTransitionHistoryService _stateTransitionHistoryService;

        public StateManagementService (ApplicationDbContext context, IConfiguration configuration,
            IStateTransitionHistoryService stateTransitionHistoryService) {
            _context = context;
            _configuration = configuration;
            _stateTransitionHistoryService = stateTransitionHistoryService;
            if (File.Exists ("stateconfiguration.json")) {
                using (StreamReader r = new StreamReader ("stateconfiguration.json")) {
                    string json = r.ReadToEnd ();
                    _stateConfiguration = JsonConvert.DeserializeObject<StateConfiguration> (json);
                }
            } else
                _stateConfiguration = new StateConfiguration ();
            if (_configuration["ChangeStatePostEndpoint"] != null) {
                client = new HttpClient ();

            }
        }

        public async Task<Tool> setToolToStatusById (int toolId, stateEnum newState, Justification justification,string username) {
            var tool = await _context.Tools
                .Where (x => x.toolId == toolId)
                .FirstOrDefaultAsync ();

            tool.username = username;
            Console.WriteLine("setToolToStatusById -  username: ");
            Console.WriteLine(username);
            
            if (tool == null)
                return null;
            return await updateTool (tool, newState, justification, username);
        }

        public async Task<Tool> setTootlToStatusByNumber (string toolSerialNumber, stateEnum newState,
            Justification justification, string username) {
            var tool = await _context.Tools
                .Where (x => x.serialNumber == toolSerialNumber)
                .FirstOrDefaultAsync ();
            if (tool == null)
                return null;
            return await updateTool (tool, newState, justification,username);
        }

        public StateConfiguration getPossibleStatusTransition () {
            return _stateConfiguration;
        }
        private async Task<Tool> updateTool (Tool tool, stateEnum newState, Justification justification, string username) {

            var curState = _stateConfiguration.states
                .Where (x => x.state == tool.status.ToString ()).FirstOrDefault ();
            if (curState == null)
                return null;
            var newStateObject = _stateConfiguration.states
                .Where (x => x.state == newState.ToString ()).FirstOrDefault ();
            if (newStateObject.needsJustification) {
                if (justification == null)
                    return null;
                else if (String.IsNullOrEmpty (justification.text))
                    return null;
            } else
                justification = null;
            if (curState.possibleNextStates.Contains (newState.ToString ())) {
                tool.status = newState.ToString ();
                _context.Entry (tool).State = EntityState.Modified;
                await _context.SaveChangesAsync ();
                // passa o nome do usuario que fez a modificação na ferramenta
                tool.username = username;
                await _stateTransitionHistoryService.addToolHistory (tool.toolId, newStateObject.needsJustification, tool.currentLife,
                    tool, justification, curState.state.ToString (), newState.ToString ());
                Trigger (tool);
                return tool;
            }
            return null;
        }
        private async void Trigger (Tool tool) {
            try {
                if (_configuration["ChangeStatePostEndpoint"] != null) {
                    client.DefaultRequestHeaders.Accept.Clear ();
                    client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                    var content = new StringContent (JsonConvert.SerializeObject (tool), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync (_configuration["ChangeStatePostEndpoint"], content);
                    if (response.IsSuccessStatusCode) {
                        Console.WriteLine ("Data posted on ChangeStatePostEndpoint");
                        Console.WriteLine (await response.Content.ReadAsStringAsync ());
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine ("Error on ChangeStatePostEndpoint");
                Console.WriteLine (ex.ToString ());
            }
        }

    }
}