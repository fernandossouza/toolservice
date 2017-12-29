using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using toolservice.Service.Interface;
using toolservice.Data;
using toolservice.Model;

namespace toolservice.Service
{
    public class ToolService : IToolService
    {
        private readonly ApplicationDbContext _context;
        private readonly IToolTypeService _toolTypeService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Context DB</param>
        public ToolService(ApplicationDbContext context,IToolTypeService toolTypeService)
        {
            _context = context;
            _toolTypeService = toolTypeService;
        }

        public async Task<(List<Tool>,int)> getTools(int startat,int quantity, ToolFieldEnum fieldFilter, 
        string fieldValue, ToolFieldEnum orderField, OrderEnum order)
        {
            var queryTool = _context.Tools.Where(x=>x.id>0);
            queryTool = ApplyFilter(queryTool, fieldFilter, fieldValue);
            queryTool = ApplyOrder(queryTool, orderField, order);
            var toolId = await queryTool
            .Skip(startat).Take(quantity)
            .Select(x => x.id)
            .ToListAsync();

            var queryToolCount = _context.Tools.Where(x=>x.id>0);
            queryToolCount = ApplyFilter(queryToolCount, fieldFilter, fieldValue);
            queryToolCount = ApplyOrder(queryToolCount, orderField, order);
            var totalCount = await queryToolCount.CountAsync();
           
            List<Tool> tools = new List<Tool>();
            foreach (var item in toolId)
            {
                var tool = await getTool(item);
                if (tool != null)
                    tools.Add(tool);
            }

            return (tools,totalCount);

        }

        public async Task<Tool> getTool(int toolId)
        {
            var tool = await _context.Tools                   
                     .Where(x => x.id == toolId)
                     .FirstOrDefaultAsync();

            var toolType = await _toolTypeService.getToolType(tool.typeId.Value);
            tool.typeName = toolType.name;

            
            return tool;
        }

        public async Task<Tool> updateTool(int toolId,Tool tool)
        {
            var toolDB = await _context.Tools                    
                     .Where(x => x.id == toolId )
                     .AsNoTracking()
                     .FirstOrDefaultAsync();


            if (toolId != toolDB.id && toolDB == null)
            {
                return null;
            }           

            _context.Tools.Update(tool);
            await _context.SaveChangesAsync();
            return tool;
        }

        public async Task<Tool> deleteTool(int toolId)
        {
            var toolDb = await getTool(toolId);

            if(toolDb == null)
            {
                return null;
            }

            toolDb.status = "inactive";

            toolDb = await updateTool(toolId,toolDb);

            return toolDb;
        }

        public async Task<Tool> addTool(Tool tool)
        {
             _context.Tools.Add(tool);
            await _context.SaveChangesAsync();
            return tool;
        }


        private IQueryable<Tool> ApplyFilter(IQueryable<Tool> queryTool,
       ToolFieldEnum fieldFilter, string fieldValue)
        {
            switch (fieldFilter)
            {
                case ToolFieldEnum.Code:
                    queryTool = queryTool.Where(x => x.code.Contains(fieldValue));
                    break;
                case ToolFieldEnum.Description:
                    queryTool = queryTool.Where(x => x.description.Contains(fieldValue));
                    break;
                case ToolFieldEnum.Name:
                    queryTool = queryTool.Where(x => x.name.Contains(fieldValue));
                    break;
                case ToolFieldEnum.SerialNumber:
                    queryTool = queryTool.Where(x => x.serialNumber.Contains(fieldValue));
                    break;
                case ToolFieldEnum.Status:
                    queryTool = queryTool.Where(x => x.status.Contains(fieldValue));
                    break;
                case ToolFieldEnum.TypeName:
                    queryTool = queryTool.Where(x => x.typeName.Contains(fieldValue));
                    break;
                case ToolFieldEnum.UnitOfMeasurement:
                    queryTool = queryTool.Where(x => x.unitOfMeasurement.Contains(fieldValue));
                    break;
                default:
                    break;
            }
            return queryTool;
        }

        private IQueryable<Tool> ApplyOrder(IQueryable<Tool> queryTags,
        ToolFieldEnum orderField, OrderEnum order)
        {
            switch (orderField)
            {
                case ToolFieldEnum.Code:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.code);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.code);
                    break;
                case ToolFieldEnum.Description:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.description);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.description);
                    break;
                case ToolFieldEnum.Name:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.name);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.name);
                    break;
                case ToolFieldEnum.SerialNumber:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.serialNumber);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.serialNumber);
                    break;
                case ToolFieldEnum.Status:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.status);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.status);
                    break;
                case ToolFieldEnum.TypeName:
                    if (order == OrderEnum.Ascending)
                        queryTags = queryTags.OrderBy(x => x.typeName);
                    else
                        queryTags = queryTags.OrderByDescending(x => x.typeName);
                    break;
                default:
                    queryTags = queryTags.OrderBy(x => x.id);
                    break;
            }
            return queryTags;
        }



        
    }
}