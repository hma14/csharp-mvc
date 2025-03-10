using Omnae.Data.Abstracts;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Service.Service
{
    public class QboTokensService : IQboTokensService
    {
        private readonly IQboTokensRepository qboTokensRepository;
        private readonly IUnitOfWork unitOfWork;

        public QboTokensService(IQboTokensRepository qboTokensRepository, IUnitOfWork unitOfWork)
        {
            this.qboTokensRepository = qboTokensRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddQboTokens(QboTokens entity)
        {
            return qboTokensRepository.AddQboTokens(entity);
        }

        public void Dispose()
        {
            qboTokensRepository.Dispose();
        }

        public QboTokens FindQboTokens(int Id)
        {
            return qboTokensRepository.GetQboTokens(Id);
        }

        public QboTokens FindQboTokens()
        {
            return qboTokensRepository.GetQboTokens();
        }

        public List<QboTokens> FindQboTokensList()
        {
            return qboTokensRepository.GetQboTokensList();
        }

        public void RemoveQboTokens(QboTokens entity)
        {
            qboTokensRepository.RemoveQboTokens(entity);
        }

        public void UpdateQboTokens(QboTokens entity)
        {
            qboTokensRepository.UpdateQboTokens(entity);
        }
    }
}
