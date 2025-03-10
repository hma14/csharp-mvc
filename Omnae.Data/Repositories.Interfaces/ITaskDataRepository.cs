using Omnae.Common;
using Omnae.Data.Abstracts;
using Omnae.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnae.Data.Repositories.Interfaces
{
    public interface ITaskDataRepository : IRepository<TaskData>
    {
        List<TaskData> GetTaskDataByCustomerId(int customerId, States? state = null);
        List<TaskData> GetTaskDataByVendorId(int vendorId, States? state = null);
        TaskData GetTaskDataByAdminId(string adminId);
        TaskData GetTaskDataById(int Id);
        TaskData GetOriginalTaskDataByProductId(int productId);
        TaskData GetTaskDataByIdWithExtraData(int id);

        TaskData GetTaskDataByProductId(int productId);
        List<TaskData> GetTaskDataListByStateId(int stateId);
        List<TaskData> GetTaskDataList();
        int AddTaskData(TaskData entity);
        void UpdateTaskData(TaskData entity);

        void RemoveTaskData(TaskData entity);

        TaskData GetTaskDataByRFQBidId(int bidId);

        List<TaskData> GetTaskDataListByProductId(int productId);

        List<TaskData> GetTasksThatNeedAdminAtention();
        IQueryable<TaskData> GetTaskDatasByCustomerId(int customerId);
        IQueryable<TaskData> GetTaskDatasByVendorId(int vendorId);
        IQueryable<TaskData> GetTaskDatas();
        IQueryable<TaskData> GetTaskDatasByCompanyId(int companyId);
        IQueryable<TaskData> GetTaskDatasByProductId(int productId);
    }
}
