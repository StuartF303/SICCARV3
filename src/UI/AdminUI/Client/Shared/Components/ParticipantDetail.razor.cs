using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using IdentityServer4.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Participant = Siccar.Application.Participant;
using ChangeEventArgs =  Microsoft.AspNetCore.Components.ChangeEventArgs ;
using Syncfusion.Blazor.Notifications;
using Siccar.Common;
using Syncfusion.Blazor.DropDowns;
using Microsoft.IdentityModel.Tokens;
using Siccar.Platform;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ParticipantDetail
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }

        [Parameter]
        [CanBeNull]
        public Participant Participant { get; set; }

        [Parameter]
        public string RegisterId { get; set; }

        private SfToast _toastNotification;
        private bool _isNew;
        private EditForm _form;
        private SfDropDownList<string, Wallet> WalletChoiceList { get; set; }
        public List<Wallet> WalletListDataSource { get; set; }
        public Tenant tenant { get; set; }
        public bool canPublish = false;


        protected override async Task OnParametersSetAsync()
        {
            if (Participant == null)
            {
                _isNew = true;
            }
            else
            {
                _isNew = false;
            }

            Participant ??= new Participant();
            
            try
            {
                if (_isNew) Participant.Organisation = (await GetAccessTokenTenant()).Name;
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }

            canPublish = true;
            try
            {
                WalletListDataSource = await WalletServiceClient.ListWallets();
                if (WalletListDataSource.Count == 0)
                {
                    await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = "You do not have access to any wallets", CssClass = "e-toast-danger" });
                }
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }
          
        }

        private async Task PublishParticipant()
        {
            if (_form.EditContext!.Validate())
            {
                if (_isNew)
                {
                    try
                    {
                        if (Participant.WalletAddress.IsNullOrEmpty())
                        {
                            await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = "Please select participant's Wallet", CssClass = "e-toast-danger" });
                            return;
                        }
                        
                        await TenantServiceClient.PublishParticipant(RegisterId, WalletListDataSource[0].Address, Participant);
                        await JsRuntime.InvokeVoidAsync("ChangeUrl", $"/participant/{RegisterId}/{Participant.Id}");
                        _isNew = false;
                        StateHasChanged();
                        await _toastNotification.ShowAsync(new ToastModel{Title = "Success", Content = "Successfully created Participant", CssClass = "e-toast-success"});

                        NavigationManager.NavigateTo(GoBackPath());
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Failed to create participant");
                        Error?.ProcessError(e);
                    }
                }
                else
                {
                    Logger.LogError("You cannot change participant");
                    await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = "You cannot change participant", CssClass = "e-toast-danger" });
                }
            }
        }

        private void CancelAdding()
        {
            NavigationManager.NavigateTo(GoBackPath());
        }

        public async Task<Tenant> GetAccessTokenTenant()
        {
            var accessTokenResult = await TokenProvider.RequestAccessToken();
            var AccessToken = string.Empty;
            if (accessTokenResult != null && accessTokenResult.Status == AccessTokenResultStatus.Success)
            {
                if (accessTokenResult.TryGetToken(out var tkn))
                {
                    AccessToken = tkn.Value;
                }
            }

            var tenants = (new JwtSecurityToken(jwtEncodedString: AccessToken)).Claims.Where(el => el.Type == "tenant").ToList();
            if (tenants.Count == 0) { return new Tenant(); }

            Tenant tenantById = null;
            try
            {
                tenantById = await TenantServiceClient.GetTenantById(tenants[0].Value);
            }
            catch (Exception e)
            {
                Error?.ProcessError(e);
            }

            return tenantById;
        }

        private string GoBackPath () 
        {
            var path = pageHistoryState.GetGoBackPage()!.Replace("participantslist/noregister", $"participantslist/{RegisterId}");
            return path;
        }
    }
}