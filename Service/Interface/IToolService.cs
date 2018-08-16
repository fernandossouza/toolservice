using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using toolservice.Model;

namespace toolservice.Service.Interface {
    public interface IToolService {
        Task<(List<Tool>, int)> getTools (int startat, int quantity, ToolFieldEnum fieldFilter, string fieldValue, ToolFieldEnum orderField, OrderEnum order);
        Task<List<Tool>> getToolsInUSe ();
        Task<List<Tool>> getToolsOnThing (int thingId);
        Task<List<Tool>> getToolsAvailable ();
        Task<Tool> setToolToThing (Tool tool, int? thingId);
        Task<Tool> setToolToPosition (Tool tool, int? position);
        Task<Tool> getTool (int toolId);
        Task<Tool> updateTool (int toolId, Tool tool, string username);
        Task<Tool> deleteTool (int toolId, string username);
        Task<Tool> addTool (Tool tool);
        Task<(List<Tool>,HttpStatusCode)> getToolsOnIds(List<int> nums);
    }

    public enum ToolFieldEnum {
        Default,
        Name,
        Description,
        SerialNumber,
        Code,
        TypeName,
        Status,
        typeId

    }
}