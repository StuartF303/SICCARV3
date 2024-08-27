using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using JetBrains.Annotations;

namespace Siccar.UI.Admin.Pages.Register
{
    public partial class ParticipantsList
    {
        [Inject]
        NavigationManager navManager { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }

        [Parameter]
        public string RegisterId { get; set; } = "noregister";

        protected override Task OnInitializedAsync()
        {
            pageHistoryState.AddPageToHistory(navManager.Uri);
            return base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
        }
    }
}
