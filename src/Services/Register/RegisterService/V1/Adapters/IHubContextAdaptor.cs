using System.Threading.Tasks;
#nullable enable

namespace Siccar.Registers.RegisterService.V1.Adapters
{
    public interface IHubContextAdapter
    {
        public Task SendToGroupAsync(string group, string method, object arg1);
        public Task SendToAllAsync(string method, object arg1);

    }
}
