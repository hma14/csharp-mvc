using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service.Interfaces
{
    public interface IQboTokensService
    {
        void Dispose();
        int AddQboTokens(QboTokens entity);
        QboTokens FindQboTokens(int Id);
        QboTokens FindQboTokens();
        List<QboTokens> FindQboTokensList();
        void UpdateQboTokens(QboTokens entity);
        void RemoveQboTokens(QboTokens entity);
    }
}
