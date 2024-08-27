using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using IdentityServer4.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Client =  Siccar.Common.ServiceClients.Models.Tenant.Client;
using ChangeEventArgs =  Microsoft.AspNetCore.Components.ChangeEventArgs ;
using Syncfusion.Blazor.Notifications;
using Siccar.Common;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ClientDetail
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Parameter]
        [CanBeNull]
        public Client Client { get; set; }

        [Parameter]
        public string TenantId { get; set; }

        public List<string> rolesChoice { get; set; }

        private SfToast _toastNotification;
        private bool _isNew;
        private bool _isNewClientSecret;
        private EditForm _form;

        protected override void OnParametersSet()
        {
            if (Client == null)
            {
                _isNew = true;
            }
            else
            {
                _isNew = false;
            }

            Client ??= new Client{Id = Guid.NewGuid(), AllowedGrantTypes = new List<string>{"authorization_code", "client_credentials"}, ClientId = Guid.NewGuid().ToString(), TenantId = TenantId, RequireClientSecret = false, AllowedScopes = new List<string>{"openid", "profile"}};
            _isNewClientSecret = !Client.ClientSecrets.Any();

            rolesChoice = new List<string>(10)
            {
                Constants.BlueprintAdminRole,
                Constants.BlueprintAuthoriserRole,
                Constants.InstallationAdminRole,
                Constants.InstallationReaderRole,
                Constants.InstallationBillingRole,
                Constants.RegisterCreatorRole,
                Constants.RegisterMaintainerRole,
                Constants.RegisterReaderRole,
                Constants.TenantBillingRole,
                Constants.TenantAdminRole,
                Constants.TenantAppAdminRole,
                Constants.WalletUserRole,
                Constants.WalletOwnerRole,
                Constants.WalletDelegateRole
            };

            rolesChoice.Sort();

            foreach (var el in Client.Claims.Where(m => m.Type == "role"))
              rolesChoice.Remove(el.Value);

        }

        private async Task SaveChanges()
        {
            if (_form.EditContext!.Validate())
            {
                if (_isNew)
                {
                    try
                    {
                        Client = await TenantServiceClient.Create(Client!);
                        await JsRuntime.InvokeVoidAsync("ChangeUrl", $"/tenants/{TenantId}/clients/{Client.ClientId}");
                        _isNew = false;
                        _isNewClientSecret = !Client.ClientSecrets.Any() && Client.RequireClientSecret;
                        StateHasChanged();
                        await _toastNotification.ShowAsync(new ToastModel{Title = "Success", Content = "Successfully created client", CssClass = "e-toast-success"});
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Failed to create client");
                        Error?.ProcessError(e);
                    }
                }
                else
                {
                    try
                    {
                        Client = await TenantServiceClient.ClientUpdate(Client!);
                        _isNewClientSecret = !Client.ClientSecrets.Any() && Client.RequireClientSecret;
                        StateHasChanged();
                        await _toastNotification.ShowAsync(new ToastModel{Title = "Success", Content = "Successfully updated client", CssClass = "e-toast-success"});
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, "Failed to update client");
                        Error?.ProcessError(e);
                    }
                }
            }
        }

        private void RequireClientSecretChanged(ChangeEventArgs obj)
        {
            if (Client!.RequireClientSecret && !Client.ClientSecrets.Any())
            {
                _isNewClientSecret = true;
                Client.ClientSecrets = new List<Secret>{new(Guid.NewGuid().ToString(), "")
                {Value = Guid.NewGuid().ToString()}};
            }
        }

        private async Task CopySharedSecret()
        {
            await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", Client!.ClientSecrets.First().Value);
        }

        private void CancelChanges()
        {
            NavigationManager.NavigateTo($"tenants/{TenantId}/");
        }
    }
}