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

using Siccar.Application.HelperFunctions;
using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SiccarApplicationTests.HelperFunctions
{
    public class BlueprintHelperFunctionsTest
    {
        private readonly TransactionModel _blueprintTrans1;
        private readonly TransactionModel _blueprintTrans2;
        private readonly TransactionModel _blueprintTrans3;
        private readonly TransactionModel _blueprintTrans4;
        private readonly List<TransactionModel> bps = new();

        public BlueprintHelperFunctionsTest()
        {
            _blueprintTrans1 = new TransactionModel
            {
                TxId = Guid.NewGuid().ToString(),
                PrevTxId = new string($"").PadLeft(64, '0'),
                MetaData = new TransactionMetaData
                {
                    BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5",
                }
            };
            bps.Add(_blueprintTrans1);

            _blueprintTrans2 = new TransactionModel
            {
                TxId = Guid.NewGuid().ToString(),
                PrevTxId = _blueprintTrans1.Id,
                MetaData = new TransactionMetaData
                {
                    BlueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5",
                }
            };
            bps.Add(_blueprintTrans2);

            _blueprintTrans3 = new TransactionModel
            {
                TxId = Guid.NewGuid().ToString(),
                PrevTxId = new string("").PadLeft(64, '0'),
                MetaData = new TransactionMetaData
                {
                    BlueprintId = "A1234567-3DFE-4154-908B-59D31FB760E5",
                }
            };
            bps.Add(_blueprintTrans3);

            _blueprintTrans4 = new TransactionModel
            {
                TxId = Guid.NewGuid().ToString(),
                PrevTxId = _blueprintTrans3.Id,
                MetaData = new TransactionMetaData
                {
                    BlueprintId = "A1234567-3DFE-4154-908B-59D31FB760E5",
                }
            };
        }
        [Fact]
        public void ShouldRetrieveVersionAndPrevTransIdOfLatestBlueprint()
        {
            var blueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5";
            var result = BlueprintHelperFunctions.GetLatestBlueprintVersionAndPrevTransId(blueprintId, bps);
            Assert.Equal(3, result.version);
            Assert.Equal(_blueprintTrans2.TxId, result.prevTransId);
        }

        [Fact]
        public void ShouldRetrieveAllBlueprintsWithSpecifiedBlueprintIdFromListOfTransactionsInDescendingOrder()
        {
            var blueprintId = "C65F6542-3DFE-4154-908B-59D31FB760E5";

            var result = BlueprintHelperFunctions.GetListBlueprintTransactionsLatestFirst(blueprintId, bps);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(_blueprintTrans1.TxId, result[1].TxId);
            Assert.Equal(_blueprintTrans2.TxId, result[0].TxId);
        }
    }
}
