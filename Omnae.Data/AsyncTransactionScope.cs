using System.Transactions;

namespace Omnae.Data
{
    public static class AsyncTransactionScope
    {
        public static TransactionScope StartNew()
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }
        public static TransactionScope StartNew(TransactionOptions option)
        {
            return new TransactionScope(TransactionScopeOption.Required, option, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
