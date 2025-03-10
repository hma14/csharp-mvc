using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omnae.Common;

namespace Omnae.Service.Service.Interfaces
{
    public interface ITaskDataService
    {
        TaskData FindById(int id);
        TaskData FindTaskDataByProductId(int productId);
        int AddTaskData(TaskData taskData);
        List<TaskData> GetTaskDataListByStateId(int stateId);
        List<TaskData> GetTaskDataAll();
        void Update(TaskData entity);
        void SaveTaskData();
        List<TaskData> FindTaskDataByCustomerId(int customerId, States? state = null);
        List<TaskData> FindTaskDataByVendorId(int vendorId, States? state = null);
        TaskData FindTaskDataByAdminId(string adminId);
        TaskData FindTaskDataByRFQBidId(int bidId);
        TaskData FindOriginalTaskDataByProductId(int productId);

        List<TaskData> FindTaskDataListByProductId(int productId);

        void DeleteTaskData(TaskData entity);
        TaskData FindByIdWithExtraData(int id);
        List<TaskData> GetTasksThatNeedAdminAtention();
        IQueryable<TaskData> FindTaskDatasByCustomerId(int customerId);
        IQueryable<TaskData> FindTaskDatasByVendorId(int vendorId);
        IQueryable<TaskData> FindTaskDatas();
        IQueryable<TaskData> FindTaskDatasByCompanyId(int companyId);
        IQueryable<TaskData> FindTaskDatasByProductId(int productId);
    }
}
