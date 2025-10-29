// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors


namespace IdentityServerHost.Quickstart.UI
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}