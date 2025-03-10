using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Data.Repositories.Interfaces;
using Omnae.Data.Abstracts;
using Omnae.Data;

namespace Omnae.Service.Service
{
    public class TaskDataService : ITaskDataService
    {
        private readonly ITaskDataRepository taskDataRepository;
        private readonly IUnitOfWork unitOfWork;

        public TaskDataService( ITaskDataRepository taskDataRepository, IUnitOfWork unitOfWork)
        {
            this.taskDataRepository = taskDataRepository;
            this.unitOfWork = unitOfWork;
        }

        public int AddTaskData(TaskData taskData)
        {
            return taskDataRepository.AddTaskData(taskData);
        }

        public void DeleteTaskData(TaskData entity)
        {
            taskDataRepository.RemoveTaskData(entity);
        }

        public TaskData FindById(int id)
        {
            return taskDataRepository.GetTaskDataById(id);
        }

        public TaskData FindByIdWithExtraData(int id)
        {
            return taskDataRepository.GetTaskDataByIdWithExtraData(id);
        }

 
        public TaskData FindTaskDataByAdminId(string adminId)
        {
            return taskDataRepository.GetTaskDataByAdminId(adminId);
        }

        public List<TaskData> FindTaskDataByCustomerId(int customerId, States? state = null)
        {
            return taskDataRepository.GetTaskDataByCustomerId(customerId, state);
        }

        public IQueryable<TaskData> FindTaskDatasByCompanyId(int companyId)
        {
            return taskDataRepository.GetTaskDatasByCompanyId(companyId);
        }
        public IQueryable<TaskData> FindTaskDatasByCustomerId(int customerId)
        {
            return taskDataRepository.GetTaskDatasByCustomerId(customerId);
        }

        public IQueryable<TaskData> FindTaskDatasByVendorId(int vendorId)
        {
            return taskDataRepository.GetTaskDatasByVendorId(vendorId);
        }
        public IQueryable<TaskData> FindTaskDatas()
        {
            return taskDataRepository.GetTaskDatas();
        }
        public TaskData FindTaskDataByProductId(int productId)
        {
            return taskDataRepository.GetTaskDataByProductId(productId);
        }

        public TaskData FindOriginalTaskDataByProductId(int productId)
        {
            return taskDataRepository.GetOriginalTaskDataByProductId(productId);
        }
        public TaskData FindTaskDataByRFQBidId(int bidId)
        {
            return taskDataRepository.GetTaskDataByRFQBidId(bidId);
        }

        public List<TaskData> FindTaskDataByVendorId(int vendorId, States? state = null)
        {
            return taskDataRepository.GetTaskDataByVendorId(vendorId, state);
        }

        public List<TaskData> FindTaskDataListByProductId(int productId)
        {
            return taskDataRepository.GetTaskDataListByProductId(productId);
        }

        public IQueryable<TaskData> FindTaskDatasByProductId(int productId)
        {
            return taskDataRepository.GetTaskDatasByProductId(productId);
        }
        public List<TaskData> GetTaskDataAll()
        {
            return taskDataRepository.GetTaskDataList().Where(x => x.isEnterprise == false).ToList();
        }

        public List<TaskData> GetTasksThatNeedAdminAtention()
        {
            return taskDataRepository.GetTasksThatNeedAdminAtention();
        } 


        public List<TaskData> GetTaskDataListByStateId(int stateId)
        {
            return taskDataRepository.GetTaskDataListByStateId(stateId);
        }

        public void SaveTaskData()
        {
            unitOfWork.Commit();
        }

        public void Update(TaskData entity)
        {
            taskDataRepository.UpdateTaskData(entity);
        }
    }
}
