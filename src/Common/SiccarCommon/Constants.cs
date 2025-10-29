// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

namespace Siccar.Common
{
    public static class Constants
    {
        // URI Endpoints
        public const string TenantAPIURL = "/api/Tenants";
        public const string BlueprintAPIURL = "/api/Blueprints";
        public const string ActionAPIURL = "/api/Actions";
        public const string FilesAPIURL = "/api/Files";
        public const string WalletAPIURL = "/api/Wallets";
        public const string PendingTransactionsAPIURL = "/api/PendingTransactions";
        public const string RegisterAPIURL = "/api/Registers";
        public const string ValidatorAPIURL = "/api/Validators";
        public const string PeerAPIURL = "/api/Peer";
        public const string PeerHub = "/PeerHub";
        public const string ProbesAPIURL = "/api/Probes";

        // Networking Defaults
        public const string NetworkVersion = "0.9.0";
        public const int NetworkTimeToLive = 15;

        // Licenses and Terms
        public const string ContactEmail = "support@siccar.net";
        public const string ContactName = "Siccar Support";
        public const string TermsOfServiceURI = "https://www.siccar.net/terms";
        public const string LicenseName = "To Be Determined";
        public const string LicenseURI = "https://www.siccar.net/license";

        // Role Groups
        public enum RoleGroups 
        {
            Blueprint,
            Installation,
            Register,
            Tenant,
            Wallet
        }

        // User roles
        public const string BlueprintAdminRole = "blueprint.admin";
        public const string BlueprintAuthoriserRole = "blueprint.authoriser";
        public const string InstallationAdminRole = "installation.admin";
        public const string InstallationReaderRole = "installation.reader";
        public const string InstallationBillingRole = "installation.billing";
        public const string RegisterCreatorRole = "register.creator";
        public const string RegisterMaintainerRole = "register.maintainer";
        public const string RegisterReaderRole = "register.reader";
        public const string TenantBillingRole = "tenant.billing";
        public const string TenantAdminRole = "tenant.admin";
        public const string TenantAppAdminRole = "tenant.app.admin";
        public const string WalletUserRole = "wallet.user";
        public const string WalletOwnerRole = "wallet.owner";
        public const string WalletDelegateRole = "wallet.delegate";


    }
}
