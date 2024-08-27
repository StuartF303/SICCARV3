using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Siccar.Platform;
using Syncfusion.Blazor.Notifications;
using Microsoft.Extensions.Logging;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class AddRegister
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; }

        private Register _register;
        private EditForm _form;
        private SfToast _toastNotification;

        protected override void OnInitialized()
        {
            _register = new Register{Advertise = true, IsFullReplica = true};
        }

        private async Task CreateRegister(MouseEventArgs obj)
        {
            if (_form.EditContext!.Validate())
            {
                try
                {
                    await RegisterServiceClient.CreateRegister(_register);
                    await _toastNotification.ShowAsync(new ToastModel{Title = "Success", Content = "Successfully created register", CssClass = "e-toast-success"});
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "Failed to create register");
                    Error?.ProcessError(e);
                }

                await OnSave.InvokeAsync();
            }
        }
    }
}