using Siccar.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
#nullable enable

namespace Siccar.Registers.Core
{
    public interface IRegisterRepository
    {
        public Task<bool> IsLocalRegisterAsync(string RegisterId);
        public Task<IEnumerable<Register>> GetRegistersAsync();
        public Task<IEnumerable<Register>> QueryRegisters(Func<Register, bool> predicate);
        public Task<Register> GetRegisterAsync(string registerId);
        public Task<Register> InsertRegisterAsync(Register newRegister);
        public Task<Register> UpdateRegisterAsync(Register register);
        public Task DeleteRegisterAsync(string registerId);
        public Task<IEnumerable<Docket>> GetDocketsAsync(string RegisterId);
        public Task<Docket> GetDocketAsync(string RegisterId, UInt64 DocketId);
        public Task<Docket> InsertDocketAsync(Docket docket);

       public Task<IQueryable<TransactionModel>> GetTransactionsAsync(string RegisterId);

        public Task<TransactionModel> GetTransactionAsync(string RegisterId, string TransactionId);
        public Task<TransactionModel> InsertTransactionAsync(TransactionModel transaction);
        public Task<IEnumerable<TransactionModel>> QueryTransactions(string RegisterId, Expression<Func<TransactionModel, bool>> predicate);
        public Task<IEnumerable<TransactionModel>> QueryTransactionPayload(string RegisterId, Expression<Func<TransactionModel, bool>> predicate);
        public Task<int> CountRegisters();
    }
}
