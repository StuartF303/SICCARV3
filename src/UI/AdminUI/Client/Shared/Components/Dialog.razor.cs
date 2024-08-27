using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class Dialog
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public string Content { get; set; }
        [Parameter]
        public EventCallback OnOkClick { get; set; }
        [Parameter]
        public EventCallback OnCancelClick { get; set; }
        [Parameter]
        public bool IsModal { get; set; } = true;
        [Parameter]
        public bool ShowCloseIcon { get; set; } = true;
        [Parameter]
        public string Width { get; set; } = "250px";
        [Parameter]
        public bool IsVisible { get; set; }

        private async Task CancelClick()
        {
            await OnCancelClick.InvokeAsync();
            IsVisible = false;
        }

        private async Task OkClick()
        {
            await OnOkClick.InvokeAsync();
            IsVisible = false;
        }
    }
}