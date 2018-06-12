using System.Threading.Tasks;
using toolservice.Model;

namespace toolservice.Service.Interface
{
    public interface IStateManagementService
    {
        Task<Tool> setToolToStatusById(int toolId, stateEnum newState, Justification justification,string username);
        Task<Tool> setTootlToStatusByNumber(string toolSerialNumber, stateEnum newState, Justification justification,string username);
        StateConfiguration getPossibleStatusTransition();
    }

    public enum stateEnum
    {
        available,
        in_use,
        in_maintenance,
        not_available,
        inactive
    }
}
