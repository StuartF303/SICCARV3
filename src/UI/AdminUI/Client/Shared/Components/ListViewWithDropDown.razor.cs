using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Notifications;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class ListViewWithDropDown
    {
        [Parameter]
        public ICollection<ClientClaim> DataSource { get; set; }

        [Parameter]
        public List<string> DropdownListDataSource { get; set; } = new();

        [Parameter]
        public EventCallback OnItemAdded { get; set; }

        [Parameter]
        public EventCallback OnItemRemoved { get; set; }

        private SfToast _toastNotification;

        //public string ItemToAdd { get; set; }

        public SfDropDownList<string,string> RolesChoiceList { get; set; }

        private async Task AddItem()
        {
            try
            {
                string ItemToAdd = RolesChoiceList.Value;
                if (ItemToAdd.IsNullOrEmpty())
                {
                    Logger.LogError("You should specify a role");
                    await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = "You should specify a role", CssClass = "e-toast-danger" });
                    return;
                }
                DataSource.Add(new ClientClaim { Type = "role", Value = ItemToAdd });
                await OnItemAdded.InvokeAsync();

                DropdownListDataSource.Remove(ItemToAdd);
                await RolesChoiceList.RefreshDataAsync();

            } catch (Exception e)
            {
                Logger.LogError(e, "Failed to add claim");
                await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = e.Message, CssClass = "e-toast-danger" });
            }
        }

        private async Task RemoveItem(string itemToRemove)
        {
            try
            {
                DataSource.Remove(new ClientClaim { Type = "role", Value = itemToRemove });
                await OnItemRemoved.InvokeAsync();

                DropdownListDataSource.Add(itemToRemove);
                DropdownListDataSource.Sort();
                await RolesChoiceList.RefreshDataAsync();
            } catch (Exception e)
            {
                Logger.LogError(e, "Failed to remove claim");
                await _toastNotification.ShowAsync(new ToastModel { Title = "Error", Content = e.Message, CssClass = "e-toast-danger" });
            }
        }
    }
}