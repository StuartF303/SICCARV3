using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Data;
using Siccar.Common.ServiceClients;
using Siccar.UI.Admin.Models;
using static Siccar.Common.Constants;
using Syncfusion.Blazor.DropDowns;
using Siccar.Platform;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Siccar.UI.Admin.Shared.Components;

namespace Siccar.UI.Admin.Pages.Tenant
{
    public partial class RoleManagement
    {
        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        [CascadingParameter]
        public Error Error { get; set; }
        public SfDataManager dm { get; set; }
        public bool checkboxDisabled = true;
        public bool installationAdminDisabled = true;

        public string highestRole = string.Empty;

        public bool SpinnerVisible { get; set; }
        public string[] ColumnItems = new string[] { "INSTALLATION BILLING" };
        public List<RoleModel> RoleData { get; set; } = new List<RoleModel>();
        private SfGrid<RoleModel> Grid;
        private List<User> Users;
        public string[] RoleGroupEnums { get; set; } = Enum.GetNames(typeof(RoleGroups));
        public string CurrentRoleGroupSelected { get; set; } = "Installation";
        string Role1 = string.Empty;
        string Role2 = string.Empty;
        string Role3 = string.Empty;
        string Header1 = "INSTALLATION ADMIN";
        string Header2 = "INSTALLATION READER";
        string Header3 = "INSTALLATION BILLING";

        [Inject]
        IUserServiceClient UserServiceClient { get; set; }
        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }
        [Inject]
        Siccar.UI.Admin.Services.PageHistoryState pageHistoryState { get; set; }
        [Inject]
        NavigationManager navManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            DetermineHighestRole(user);

            await GetUserDataFromDB();
            SetRoles();
            RoleData = CreateListOfRoleModels(Users);
            pageHistoryState.AddPageToHistory(navManager.Uri);
        }

        private void SetRoles()
        {
            ShowColumns();
            if (highestRole == TenantAdminRole)
            {
                checkboxDisabled = false;
            }

            switch (CurrentRoleGroupSelected)
            {
                case "Installation":
                    Role1 = InstallationAdminRole;
                    Role2 = InstallationReaderRole;
                    Role3 = InstallationBillingRole;
                    Header1 = "INSTALLATION ADMIN";
                    Header2 = "INSTALLATION READER";
                    Header3 = "INSTALLATION BILLING";
                    if (highestRole == TenantAdminRole)
                    {
                        checkboxDisabled = true;
                    }
                    break;
                case "Register":
                    Role1 = RegisterCreatorRole;
                    Role2 = RegisterReaderRole;
                    Role3 = RegisterMaintainerRole;
                    Header1 = "REGISTER CREATOR";
                    Header2 = "REGISTER READER";
                    Header3 = "REGISTER MAINTAINER";
                    break;
                case "Blueprint":
                    Role1 = BlueprintAdminRole;
                    Role2 = BlueprintAuthoriserRole;
                    Role3 = "";
                    Header1 = "BLUEPRINT ADMIN";
                    Header2 = "BLUEPRINT AUTHORISER";
                    HideLastColumn();
                    break;
                case "Tenant":
                    Role1 = TenantAdminRole;
                    Role2 = TenantAppAdminRole;
                    Role3 = TenantBillingRole;
                    Header1 = "TENANT ADMIN";
                    Header2 = "TENANT APP ADMIN";
                    Header3 = "TENANT BILLING";
                    break;
                case "Wallet":
                    Role1 = WalletUserRole;
                    Role2 = WalletOwnerRole;
                    Role3 = WalletDelegateRole;
                    Header1 = "WALLET USER";
                    Header2 = "WALLET OWNER";
                    Header3 = "WALLET DELEGATE";
                    break;
                default:
                    break;
            }
        }
        private void DetermineHighestRole(ClaimsPrincipal user)
        {
            if (user.IsInRole(InstallationAdminRole))
            {
                highestRole = InstallationAdminRole;
                checkboxDisabled = false;
            }
            else if (user.IsInRole(TenantAdminRole))
            {
                highestRole = TenantAdminRole;
                checkboxDisabled = false;
            }

        }

        private void HideLastColumn()
        {
            Array.Clear(ColumnItems);
            ColumnItems[0] = Grid.Columns[3].HeaderText;
            HideColumns();
        }

        public static void ActionFailureHandler(FailureEventArgs args)
        {
            Console.WriteLine($"Action Failure Args = {args}");
        }

        public void OnRoleGroupDropDownChange(ChangeEventArgs<string, string> args)
        {
            SetRoles();
            RoleData = CreateListOfRoleModels(Users);
        }

        public async Task OnCheckBoxChange(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args, RoleModel roleModel, string role)
        {
            try
            {
                if (args.Checked)
                {
                    await UserServiceClient.AddToRole(roleModel.Id, role);
                }
                else
                {
                    await UserServiceClient.RemoveFromRole(roleModel.Id, role);
                }
                await GetUserDataFromDB();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Error?.ProcessError(e);
            }
        }

        private List<RoleModel> CreateListOfRoleModels(List<Platform.User> users)
        {
            var roles = users.Select(user => new RoleModel()
            {
                Id = user.Id,
                User = user.UserName,
                Admin = user.Roles.Any(str => str.Contains(Role1)),
                Reader = user.Roles.Any(str => str.Contains(Role2)),
                Billing = user.Roles.Any(str => str.Contains(Role3))
            });
            return roles.ToList();
        }
        public void ShowColumns()
        {
            Grid.ShowColumnsAsync(ColumnItems);
        }
        public void HideColumns()
        {
            Grid.HideColumnsAsync(ColumnItems);
        }

        private async Task GetUserDataFromDB()
        {
            try
            {
                Users = await UserServiceClient.All();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Error?.ProcessError(e);
            }
        }
    }
}