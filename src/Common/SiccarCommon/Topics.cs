/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

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


