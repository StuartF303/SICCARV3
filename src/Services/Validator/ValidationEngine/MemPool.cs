// MemoryPool Implementation File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using System.Text.Json;
using System.Collections.Generic;
using Siccar.Platform;
using Siccar.Registers.ValidatorCore;
using System.Linq;
#nullable enable

namespace Siccar.Registers.ValidationEngine
{
    public class MemPool : IMemPool
    {
        private readonly object _locker = new();
        private readonly Dictionary<string, List<TransactionModel>> pool = new();

        public int RegisterCount()
        {
            int _rc = 0;

            lock (_locker)
            {
                _rc = pool.Count;
            }

            return _rc;
        }

        public void AddTxToPool(TransactionModel tx)
        {
            lock (_locker)
            {
                if (pool.ContainsKey(tx.MetaData!.RegisterId))
                    pool[tx.MetaData.RegisterId].Add(tx);
                else
                    pool.Add(tx.MetaData.RegisterId, new List<TransactionModel> { tx });
            }
        }
        public List<TransactionModel> GetPool(string RegisterId = "")
        {
            if (pool.Any())
                if (pool.Where( r => r.Key == RegisterId).Any())
                {
                    lock (_locker)
                    {
                        // btw I know this is horrible but trying to change as little as poss at mo
                        var regView =  pool[RegisterId];
                        pool.Remove(RegisterId);
                        return regView;
                       // return JsonSerializer.Deserialize<Dictionary<string, List<TransactionModel>>>(serialize)!;
                    }
                }
            return new List<TransactionModel>();
        }

        // On Docket created we should remove any transactions at that point....
    }
}
