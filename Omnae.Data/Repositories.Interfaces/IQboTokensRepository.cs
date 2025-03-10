using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface IQboTokensRepository : IRepository<QboTokens>
    {
        void Dispose();
        int AddQboTokens(QboTokens entity);
        QboTokens GetQboTokens(int Id);
        QboTokens GetQboTokens();
        List<QboTokens> GetQboTokensList();
        void UpdateQboTokens(QboTokens entity);
        void RemoveQboTokens(QboTokens entity);
    }
}
