using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using toolservice.Data;
using toolservice.Model;
using toolservice.Service.Interface;

namespace toolservice.Service {
    public class ToolService : IToolService {
        private readonly ApplicationDbContext _context;
        private readonly IToolTypeService _toolTypeService;
        private readonly IThingService _thingService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context DB</param>
        public ToolService (ApplicationDbContext context, IToolTypeService toolTypeService,
            IThingService thingService) {
            _context = context;
            _toolTypeService = toolTypeService;
            _thingService = thingService;
        }

        public async Task<(List<Tool>, int)> getTools (int startat, int quantity, ToolFieldEnum fieldFilter,
            string fieldValue, ToolFieldEnum orderField, OrderEnum order) {
            var queryTool = _context.Tools.Where (x => x.toolId > 0);
            queryTool = ApplyFilter (queryTool, fieldFilter, fieldValue);
            queryTool = ApplyOrder (queryTool, orderField, order);
            var toolId = await queryTool
                .Skip (startat).Take (quantity)
                .Select (x => x.toolId)
                .ToListAsync ();

            var queryToolCount = _context.Tools.Where (x => x.toolId > 0);
            queryToolCount = ApplyFilter (queryToolCount, fieldFilter, fieldValue);
            queryToolCount = ApplyOrder (queryToolCount, orderField, order);
            var totalCount = await queryToolCount.CountAsync ();

            List<Tool> tools = new List<Tool> ();
            foreach (var item in toolId) {
                var tool = await getTool (item);
                if (tool != null && fieldFilter != ToolFieldEnum.TypeName)
                    tools.Add (tool);
                else if (tool != null && fieldFilter == ToolFieldEnum.TypeName)
                    if (tool.typeName == fieldValue)
                        tools.Add (tool);
                    else if (tool != null && fieldFilter == ToolFieldEnum.typeId)
                    if (tool.typeId.ToString () == fieldValue)
                        tools.Add (tool);

            }

            return (tools, totalCount);

        }
        public async Task<List<Tool>> getToolsAvailable () {
            var tools = await _context.Tools
                .Where (x => x.currentThingId == null && x.status == stateEnum.available.ToString ())
                .ToListAsync ();
            foreach (var tool in tools) {
                var toolType = await _toolTypeService.getToolType (tool.typeId.Value);
                tool.typeName = toolType.name;
                tool.lifeCycle = toolType.lifeCycle;
                tool.unitOfMeasurement = toolType.unitOfMeasurement;

                if (tool.currentThingId != null) {
                    var (thing, status) = await _thingService.getThing (tool.currentThingId.Value);
                    if (status == HttpStatusCode.OK)
                        tool.currentThing = thing;
                }
            }
            return tools;
        }

        public async Task<List<Tool>> getToolsInUSe () {
            var tools = await _context.Tools
                .Where (x => x.currentThingId != null)
                .ToListAsync ();
            foreach (var tool in tools) {
                var toolType = await _toolTypeService.getToolType (tool.typeId.Value);
                tool.typeName = toolType.name;
                tool.lifeCycle = toolType.lifeCycle;
                tool.unitOfMeasurement = toolType.unitOfMeasurement;
                if (tool.currentThingId != null) {
                    var (thing, status) = await _thingService.getThing (tool.currentThingId.Value);
                    if (status == HttpStatusCode.OK)
                        tool.currentThing = thing;
                }
            }
            return tools;
        }
        public async Task<List<Tool>> getToolsOnThing (int thingId) {
            var tools = await _context.Tools
                .Where (x => x.currentThingId == thingId)
                .ToListAsync ();
            if (tools == null)
                return null;
            var returnList = new List<Tool> ();
            foreach (var tool in tools) {
                returnList.Add (await getTool (tool.toolId));
            }
            return tools;
        }

        public async Task<Tool> getTool (int toolId) {
            var tool = await _context.Tools
                .Where (x => x.toolId == toolId)
                .Include ("informations")
                .Include ("informations.informationAdditional")
                .FirstOrDefaultAsync ();
            if (tool == null)
                return null;
            var toolType = await _toolTypeService.getToolType (tool.typeId.Value);
            tool.typeName = toolType.name;
            tool.lifeCycle = toolType.lifeCycle;
            tool.unitOfMeasurement = toolType.unitOfMeasurement;

            if (tool.currentThingId != null) {
                var (thing, status) = await _thingService.getThing (tool.currentThingId.Value);
                if (status == HttpStatusCode.OK)
                    tool.currentThing = thing;
            }
            return tool;
        }
        public async Task<Tool> setToolToThing (Tool tool, int? thingId) {
            var toolDB = tool;
            if (toolDB == null) {
                return null;
            }
            toolDB.currentThingId = thingId;
            toolDB.username = tool.username;

            _context.Tools.Update (toolDB);
            await _context.SaveChangesAsync ();
            return toolDB;
        }
        public async Task<Tool> setToolToPosition (Tool tool, int? position) {
            var toolDB = tool;
            if (toolDB == null) {
                return null;
            }
            toolDB.position = position;
            _context.Tools.Update (toolDB);
            await _context.SaveChangesAsync ();
            return toolDB;
        }
        public async Task<Tool> updateTool (int toolId, Tool tool, string username) {
            var toolDB = await _context.Tools
                .Where (x => x.toolId == toolId)
                .AsNoTracking ()
                .FirstOrDefaultAsync ();

            tool.currentThingId = toolDB.currentThingId;

            if (toolId != toolDB.toolId && toolDB == null) {
                return null;
            }

            tool.position = toolDB.position;
            tool.status = toolDB.status;
            _context.Tools.Update (tool);
            await _context.SaveChangesAsync ();
            return await getTool (tool.toolId);
        }

        public async Task<Tool> deleteTool (int toolId, string username) {
            var toolDb = await getTool (toolId);
            if (toolDb == null) {
                return null;
            }
            toolDb.status = "inactive";
            toolDb = await updateTool (toolId, toolDb, username);
            return toolDb;
        }

        public async Task<Tool> addTool (Tool tool) {
            tool.currentThingId = null;
            tool.position = null;
            _context.Tools.Add (tool);
            await _context.SaveChangesAsync ();
            return tool;
        }

        private IQueryable<Tool> ApplyFilter (IQueryable<Tool> queryTool,
            ToolFieldEnum fieldFilter, string fieldValue) {
            switch (fieldFilter) {
                case ToolFieldEnum.Code:
                    queryTool = queryTool.Where (x => x.code.Contains (fieldValue));
                    break;
                case ToolFieldEnum.Description:
                    queryTool = queryTool.Where (x => x.description.Contains (fieldValue));
                    break;
                case ToolFieldEnum.Name:
                    queryTool = queryTool.Where (x => x.name.Contains (fieldValue));
                    break;
                case ToolFieldEnum.SerialNumber:
                    queryTool = queryTool.Where (x => x.serialNumber.Contains (fieldValue));
                    break;
                case ToolFieldEnum.Status:
                    queryTool = queryTool.Where (x => x.status.Contains (fieldValue));
                    break;

                default:
                    break;
            }
            return queryTool;
        }

        private IQueryable<Tool> ApplyOrder (IQueryable<Tool> queryTags,
            ToolFieldEnum orderField, OrderEnum order) {
            switch (orderField) {
                case ToolFieldEnum.Code:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy (x => x.code);
                    else
                        queryTags = queryTags.OrderByDescending (x => x.code);
                    break;
                case ToolFieldEnum.Description:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy (x => x.description);
                    else
                        queryTags = queryTags.OrderByDescending (x => x.description);
                    break;
                case ToolFieldEnum.Name:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy (x => x.name);
                    else
                        queryTags = queryTags.OrderByDescending (x => x.name);
                    break;
                case ToolFieldEnum.SerialNumber:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy (x => x.serialNumber);
                    else
                        queryTags = queryTags.OrderByDescending (x => x.serialNumber);
                    break;
                case ToolFieldEnum.Status:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy (x => x.status);
                    else
                        queryTags = queryTags.OrderByDescending (x => x.status);
                    break;
                default:
                    queryTags = queryTags.OrderBy (x => x.toolId);
                    break;
            }
            return queryTags;
        }

    }
}