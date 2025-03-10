using System.Collections.Generic;
using System.Linq;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;

namespace Omnae.BusinessLayer
{
    public class TaskDataCustomerBL
    {
        public TaskDataCustomerBL(ITaskDataService taskDataService, IProductService productService, ILogedUserContext userContext)
        {
            TaskDataService = taskDataService;
            ProductService = productService;
            UserContext = userContext;
        }
        private ILogedUserContext UserContext { get; }
        private ITaskDataService TaskDataService { get; }
        private IProductService ProductService { get; }


        public List<Product> GetCustomerProductsOnTaskId(int taskId)
        {
            //TODO Refactor
            var taskData = TaskDataService.FindById(taskId);
            return taskData == null ? null : GetCustomerProductsOnTask(taskData);
        }

        public List<Product> GetCustomerProductsOnTask(TaskData taskData)
        {
            //TODO Refactor
            List<Product> prodList = GetCustomerProducts();
            var prods = prodList.Where(x => x.Id == taskData.ProductId).ToList();
            return prods.Any() ? prods : null;
        }

        protected List<Product> GetCustomerProducts()
        {
            //TODO Refactor
            return ProductService.FindProductListByCustomerId(UserContext.Company.Id);
        }


    }
}