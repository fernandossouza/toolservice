using System.Threading.Tasks;
using System.Collections.Generic;
using toolservice.Model;

namespace toolservice.Service.Interface
{
    public interface IToolService
    {
        Task<List<Tool>> getTools(int startat,int quantity);
        Task<Tool> getTool(int toolId);
        Task<Tool> updateTool(int toolId,Tool tool);
        Task<Tool> deleteTool(int toolId);
        Task<Tool> addTool(Tool tool);    
    }
}