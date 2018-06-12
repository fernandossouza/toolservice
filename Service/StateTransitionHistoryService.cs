using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using toolservice.Data;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Service
{

    public class StateTransitionHistoryService : IStateTransitionHistoryService
    {

        private readonly ApplicationDbContext _context;
        private readonly IToolService _toolService;
        public StateTransitionHistoryService(ApplicationDbContext context, IToolService toolService)
        {
            _context = context;
            _toolService = toolService;
        }

        public async Task addToolHistory(int toolid, bool justificationNeeded, double previoustLife,
         Tool tool, Justification justification, string previousState, string nextState)
        {
            StateTransitionHistory stateTransitionHistory = new StateTransitionHistory();
            stateTransitionHistory.justification = justification;
            stateTransitionHistory.justificationNeeded = justificationNeeded;
            stateTransitionHistory.toolId = tool.toolId;
            stateTransitionHistory.justification = justification;
            stateTransitionHistory.previousState = previousState;
            stateTransitionHistory.nextState = nextState;
            stateTransitionHistory.timeStampTicks = DateTime.Now.Ticks;
            stateTransitionHistory.previoustLife = previoustLife;
            stateTransitionHistory.username = tool.username;
            _context.StateTransitionHistories.Add(stateTransitionHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<StateTransitionHistory>> getToolHistory(int toolid, long from, long to)
        {
            var histories = await _context.StateTransitionHistories
                .Where(x => x.toolId == toolid && x.timeStampTicks >= from && x.timeStampTicks <= to)
                .Include(x => x.justification)
                .ToListAsync();
            foreach (var item in histories)
            {
                item.tool = await _toolService.getTool(item.toolId);
            }
            return histories;
        }
    }
}
