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

using System.Collections.Generic;
using System.Linq;
using Siccar.Platform;
#nullable enable

namespace Siccar.Application.HelperFunctions
{
    public static class BlueprintHelperFunctions
    {
        public static List<TransactionModel> GetLatestBlueprintTransactions(List<TransactionModel> transactions)
        {
            var latestBlueprintTransactions = new List<TransactionModel>();
            var uniqueBlueprintIds = transactions.GroupBy(bpid => bpid.MetaData?.BlueprintId)
                .Select(grp => new { grp.Key })
                .ToList();

            foreach (var bp in uniqueBlueprintIds)
            {
                var trans = GetListBlueprintTransactionsLatestFirst(bp.Key!, transactions);
                latestBlueprintTransactions.Add(trans.First());
            }

            return latestBlueprintTransactions;
        }

        public static (int version, string prevTransId) GetLatestBlueprintVersionAndPrevTransId(string blueprintId, List<TransactionModel> bps)
        {
            var prevTransId = "0";
            var version = 1;

            if (bps.Count > 0)
            {
                List<TransactionModel> blueprintsWithBlueprintId = GetListBlueprintTransactionsLatestFirst(blueprintId, bps);

                if (blueprintsWithBlueprintId.Count > 0)
                {
                    version = blueprintsWithBlueprintId.Count + 1;
                    prevTransId = blueprintsWithBlueprintId.First().TxId;
                }
            }
            return (version, prevTransId);
        }

        public static List<TransactionModel> GetListBlueprintTransactionsLatestFirst(string blueprintId, List<TransactionModel> bps)
        {
            var listOfBlueprintTransactions = bps.FindAll(bp => bp.MetaData?.BlueprintId == blueprintId).ToList();
            return SortBlueprintTransactionsLatestFirst(listOfBlueprintTransactions);
        }

        private static List<TransactionModel> SortBlueprintTransactionsLatestFirst(List<TransactionModel> listOfBlueprintTransactions)
        {
            string? nextBlueprintPrevTransId = new string($"").PadLeft(64, '0');

            var sortedListOfBlueprintTransactions = new List<TransactionModel>();

            foreach (var trans in listOfBlueprintTransactions)
            {
                var blueprintTrans = GetBlueprintWithPrevTxIdEqualToValue(listOfBlueprintTransactions, nextBlueprintPrevTransId);
                nextBlueprintPrevTransId = blueprintTrans?.Id;
                sortedListOfBlueprintTransactions.Insert(0, blueprintTrans!);
            }

            return sortedListOfBlueprintTransactions;
        }
        private static TransactionModel? GetBlueprintWithPrevTxIdEqualToValue(List<TransactionModel> listOfBlueprintTransactions, string? key)
        {
            return listOfBlueprintTransactions.SingleOrDefault(s => s.PrevTxId == key);
        }
    }
}
