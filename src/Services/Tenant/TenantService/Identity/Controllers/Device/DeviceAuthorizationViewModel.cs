// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors


namespace IdentityServerHost.Quickstart.UI
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}