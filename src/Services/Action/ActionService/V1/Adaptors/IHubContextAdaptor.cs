using System.Threading.Tasks;

namespace ActionService.V1.Adaptors
{
    public interface IHubContextAdaptor
    {
        public Task SendToGroupAsync(string group, string method, object arg1);

        public Task SendToAllAsync(string method, object arg1);

    }
}
