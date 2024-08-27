using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Siccar.Platform;
using Siccar.Common.ServiceClients;
using System.ComponentModel.DataAnnotations;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class CreateWallet
    {
        [CascadingParameter]
        public Error Error { get; set; }
        [Inject]
        public IWalletServiceClient WalletServiceClient { get; set; }

        public CreateWalletRequest Wallet { get; set; }

        private CreateWalletResponse _createdWalletResponse;
        private EditForm _form;
        private bool _showMnemonic;
        protected override void OnInitialized()
        {
            Wallet = new CreateWalletRequest();
        }

        private async Task CreateWalletCall()
        {
            var valid = _form.EditContext!.Validate();
            if (valid)
            {
                try
                {
                    _createdWalletResponse = await WalletServiceClient.CreateWallet(Wallet.Name!, Wallet.Description);
                    _showMnemonic = true;
                }
                catch (Exception e)
                {
                    Error?.ProcessError(e);
                }
            }
        }

        public class CreateWalletRequest
        {
            [Required]
            [MaxLength(120), MinLength(1)]
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}