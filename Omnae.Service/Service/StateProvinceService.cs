using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Model.Models;
using Omnae.Data.Repositories;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;

namespace Omnae.Service.Service
{
    public class StateProvinceService : IStateProvinceService
    {
        private readonly IStateProvinceRepository stateProvinceRepository;
        private readonly IUnitOfWork unitOfWork;

        public StateProvinceService(IStateProvinceRepository stateProvinceRepo, IUnitOfWork unitOfW)
        {
            stateProvinceRepository = stateProvinceRepo;
            unitOfWork = unitOfW;
        }

        public void Dispose()
        {
            stateProvinceRepository.Dispose();
        }

        public int AddStateProvince(StateProvince entity)
        {
            return stateProvinceRepository.AddStateProvince(entity);
        }
        public List<StateProvince> FindStateProvince()
        {
            return stateProvinceRepository.GetStateProvince();
        }
        public IQueryable<StateProvince> FindStateProvinceList()
        {
            return stateProvinceRepository.GetStateProvinceList();
        }

        public List<StateProvince> FindStateProvinceByCode(int code)
        {
            return stateProvinceRepository.GetStateProviceByCode(code);
        }

        public StateProvince FindStateProvinceById(int Id)
        {
            return stateProvinceRepository.FindStateProvinceById(Id);
        }

        public StateProvince FindStateProvinceByName(string name)
        {
            return stateProvinceRepository.FindStateProvinceByName(name);
        }
    }
}
