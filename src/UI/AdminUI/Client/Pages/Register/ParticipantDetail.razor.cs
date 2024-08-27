using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using JetBrains.Annotations;
using System;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Register
{
    public partial class ParticipantDetail
    {
        [CascadingParameter] 
        public Error Error { get; set; }

        [Parameter]
        public string RegisterId { get; set; }

        [Parameter]
        [CanBeNull]
        public string ParticipantId { get; set; }

        [Inject]
        NavigationManager navManager { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }

        private Siccar.Application.Participant _participant;
       
        protected override Task OnInitializedAsync()
        {
            pageHistoryState.AddPageToHistory(navManager.Uri);
            return base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(ParticipantId) && ParticipantId.ToLower() != "new")
            {
                try
                {
                    _participant = await TenantServiceClient.GetPublishedParticipantById(RegisterId, ParticipantId);
                }
                catch (Exception e)
                {
                    Error?.ProcessError(e);
                }
            }
        }
    }
}
