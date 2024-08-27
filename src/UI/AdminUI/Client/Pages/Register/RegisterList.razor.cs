using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Data;
using System.Linq;
using Siccar.Common.ServiceClients;
using static Siccar.Common.Constants;
using Siccar.UI.Admin.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using JetBrains.Annotations;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Register
{
    public partial class RegisterList
    {
        [CascadingParameter] 
        public Error Error { get; set; }
        [Inject]
        IRegisterServiceClient RegisterServiceClient { get; set; }
        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject] 
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }
        [Parameter]
        public List<string> RegisterIds { get; set; }

        public bool SpinnerVisible { get; set; }
        public string[] pageDropdown { get; set; } = new[] { "All", "5", "10", "15", "20" };
        public List<RegisterModel> Registers { get; set; }
        public SfGrid<RegisterModel> Grid;
        private bool ShowAddRegisterDialog { get; set; }
        private ClaimsPrincipal ActingUser { get; set; }
        private bool DisableAddRegister { get;set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                DisableAddRegister = true; 

                ActingUser = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
                if (ActingUser == null)
                    throw new UnauthorizedAccessException();
                if (ActingUser.IsInRole(InstallationAdminRole) ||
                    ActingUser.IsInRole(RegisterCreatorRole))
                    DisableAddRegister = false;

                await LoadRegisters();

                pageHistoryState.AddPageToHistory(NavigationManager.Uri);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task LoadRegisters()
        {
            try
            {
                var registers = (await RegisterServiceClient.GetRegisters()).Select(register =>
                {
                    var registerModel = new RegisterModel
                    {
                        Id = register.Id,
                        Name = register.Name,
                        Height = register.Height,
                        Votes = register.Votes,
                        Advertise = register.Advertise,
                        IsFullReplica = register.IsFullReplica,
                        Status = register.Status
                    };
                    return registerModel;
                });

                if (RegisterIds != null && RegisterIds.Any())
                {
                    Registers = registers.ToList().Where(r => RegisterIds.Contains(r.Id)).ToList();
                }
                else
                {
                    Registers = registers.ToList();
                }
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }

        }

        private void AddRegister()
        {
            ShowAddRegisterDialog = true;
        }

        private async Task RegistryCreated()
        {
            ShowAddRegisterDialog = false;
            await LoadRegisters();
        }

        private void RowSelectHandler(RowSelectEventArgs<RegisterModel> args)
        {
            NavigationManager.NavigateTo($"registers/{args.Data.Id}");
        }
    }
}