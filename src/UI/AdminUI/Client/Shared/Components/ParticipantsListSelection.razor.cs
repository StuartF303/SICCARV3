using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siccar.Application;
using Siccar.Common.ServiceClients;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ParticipantsListSelection
    {
        private IEnumerable<Participant> _availableParticipants = new List<Participant>();
        private string _registerId;
        private bool _registerIdChanged = false;

        [CascadingParameter]
        public Error Error { get; set; }

        [Inject]
        public ITenantServiceClient TenantServiceClient { get; set; }

#pragma warning disable BL0007
		[Parameter]
        public string RegisterId
        {
            get => _registerId;
            set
            {
                _registerIdChanged = _registerId != value;
                _registerId = value;
            }
        }
#pragma warning restore BL0007

		[Parameter]
        public EventCallback<List<Participant>> SelectedParticipantsChanged { get; set; }

        private List<Participant> _selectedParticipants;

#pragma warning disable BL0007
		[Parameter]
		public List<Participant> SelectedParticipants
		{
            get => _selectedParticipants;
            set
            {
                if (_selectedParticipants == value)
                {
                    return;
                }

                _selectedParticipants = value;
                SelectedParticipantsChanged.InvokeAsync(value);
            }
        }
#pragma warning restore BL0007

		protected override async Task OnParametersSetAsync()
        {
            await LoadParticipants();
        }

        private async Task LoadParticipants()
        {
            if (_registerIdChanged)
            {
                try
                {
                    _availableParticipants = await TenantServiceClient.GetPublishedParticipants(RegisterId);
                    _registerIdChanged = false;
                }
                catch (Exception e)
                {
                    Error?.ProcessError(e);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadParticipants();
        }
    }
}