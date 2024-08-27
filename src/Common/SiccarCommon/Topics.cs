using System;

namespace Siccar.Common
{
    public class Topics
    {
        // Wallet Related Events
        public const string WalletAddressCreationTopicName = "OnWallet_AddressCreated";

        // Register related events
        public const string DocketConfirmedTopicName = "OnDocket_Confirmed";
        public const string TransactionSubmittedTopicName = "OnTransaction_Submitted";
        public const string TransactionBuiltAndSignedTopicName = "OnTransaction_BuiltAndSigned";
        public const string TransactionPendingTopicName = "OnTransaction_Pending";
        public const string TransactionValidationCompletedTopicName = "OnTransaction_ValidationCompleted";
        public const string TransactionConfirmedTopicName = "OnTransaction_Confirmed";
        public const string MemberCreatedTopicName = "OnMember_Created";
        public const string RegisterCreatedTopicName = "OnRegister_Created";
        public const string RegisterDeletedTopicName = "OnRegister_Deleted";


        //can put in here failed topics for Saga pattern...
        //public const string TransactionValidationFailedTopicName = "OnTransaction_ValidationFailed";
    }
}


