namespace Omnae.Data.Abstracts
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly OmnaeContext DbContext;

        public UnitOfWork(OmnaeContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Commit()
        {
            DbContext.Commit();
        }
    }
}
