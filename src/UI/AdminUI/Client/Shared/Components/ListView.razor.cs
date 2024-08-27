using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ListView
    {
        [Parameter]
        public List<string> DataSource { get; set; } = new();

        [Parameter]
        public EventCallback OnItemAdded { get; set; }

        [Parameter]
        public EventCallback OnItemRemoved { get; set; }

        public string ItemToAdd { get; set; }

        private async Task AddItem()
        {
            DataSource.Add(ItemToAdd);
            await OnItemAdded.InvokeAsync();
            ItemToAdd = "";
        }

        private async Task RemoveItem(string itemToRemove)
        {
            DataSource.Remove(itemToRemove);
            await OnItemRemoved.InvokeAsync();
        }
    }
}