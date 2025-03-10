using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Context;
using Omnae.Model.Models;
using Omnae.Service.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using static Omnae.Data.Query.NcrQuery;
using static Omnae.Data.Query.OrderQuery;

namespace Omnae.BusinessLayer
{
    public class ChartBL
    {
        private IProductService ProductService { get; }
        private ITaskDataService TaskDataService { get; }
        private IOrderService OrderService { get; }
        private INCReportService NcreportService { get; }
        private ICompanyService CompanyService { get; }
        private ILogedUserContext UserContext { get; }
        private INCReportService NCReportService { get; }

        public ChartBL(IProductService productService, ITaskDataService taskDataService, IOrderService orderService, INCReportService ncreportService,
                       ICompanyService companyService, ILogedUserContext userContext, INCReportService nCReportService)
        {
            ProductService = productService;
            TaskDataService = taskDataService;
            OrderService = orderService;
            NcreportService = ncreportService;
            CompanyService = companyService;
            UserContext = userContext;
            NCReportService = nCReportService;
        }

        public void SetChartData(ref ChartInfoViewModel model, ref List<NCReport> ncrs, int companyId, IEnumerable<Order> orders, NCR_FILTERS? filter, int? val, USER_TYPE userType, ref List<Order> completedOrders)
        {
            
            if (filter == null)
            {
                var totalQuantity = orders
                    .Where(x => x.Product.CustomerCompany.isEnterprise == false)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var local_ncrs = userType == USER_TYPE.Admin ? NcreportService.FindNCReports() 
                                                             : (userType == USER_TYPE.Customer ? NcreportService.FindNCReportByCustomerId(companyId)
                                                                                               : NcreportService.FindNCReportByVendorId(companyId));
                
                ncrs = local_ncrs.Where(x => x.CustomerCompany.isEnterprise == false).ToList();

                var totalNcrs = ncrs
                    .Where(x => x.RootCause != null)
                    .GroupBy(x => new { x.Order.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) }).ToList();


                var lst2 = (from ord in totalQuantity
                            join ncr in totalNcrs
                                on ord.Month equals ncr.Month into ord_ncr
                            from nc in ord_ncr.DefaultIfEmpty()
                            select new
                            {
                                Month = ord.Month,
                                //Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                                Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                            }).ToList();


                model.TotalQuantity = totalQuantity
                    .GroupBy(x => new { x.Month })
                    .Select(g => g.Sum(q => (int)q.Qty)).ToList();


                model.TotalQuantityWithoutNcrs = lst2
                           .GroupBy(x => new { x.Month })
                           .Select(g => g.Sum(q => (int)q.Qty)).ToList();

                model.DateRange = orders
                    .OrderBy(x => x.OrderDate.Month)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => g.First().OrderDate.ToString("MMMM"))                 
                    .ToList();

                completedOrders = orders.Where(x => x.TaskData != null &&
                                                    x.TaskData.StateId == (int)States.ProductionComplete).ToList();
            }
            else if (filter == NCR_FILTERS.Product && val != null && val > 0)
            {

                var totalQuantity = orders
                    .Where(x => x.ProductId == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var local_ncrs = userType == USER_TYPE.Admin ? NcreportService.FindNCReports().Where(x => x.ProductId == val.Value) 
                                                             : (userType == USER_TYPE.Customer ? NcreportService.FindNCReportByCustomerId(companyId)
                                                                                               : NcreportService.FindNCReportByVendorId(companyId))
                                                             .Where(x => x.ProductId == val.Value);
                
                ncrs = local_ncrs.Where(x => x.CustomerCompany.isEnterprise == false).ToList();

                var totalNcrs = ncrs
                    .Where(x => x.RootCause != null)
                    .GroupBy(x => new { x.Order.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) }).ToList();

                var lst2 = (from ord in totalQuantity
                            join ncr in totalNcrs
                                on ord.Month equals ncr.Month into ord_ncr
                            from nc in ord_ncr.DefaultIfEmpty()
                            select new
                            {
                                Month = ord.Month,
                                Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                            }).ToList();

                model.TotalQuantity = totalQuantity
                    .GroupBy(x => new { x.Month })
                    .Select(g => g.Sum(q => (int)q.Qty)).ToList();


                model.TotalQuantityWithoutNcrs = lst2
                           .GroupBy(x => new { x.Month })
                           .Select(g => g.Sum(q => (int)q.Qty)).ToList();

                var ords = orders.OrderBy(x => x.OrderDate.Month).Where(x => x.ProductId == val);
                model.DateRange = (from ord in ords
                                   join ncr in ncrs
                                       on ord.Id equals ncr.OrderId into ord_ncr
                                   from nc in ord_ncr.DefaultIfEmpty()
                                   select (nc != null && nc.Quantity > 0 ? nc.NCDetectedDate.Value : ord.OrderDate)
                                       .ToString("MMMM"))
                    .GroupBy(x => x)
                    .Select(g => g.First()).ToList();

                completedOrders = orders.Where(x => x.ProductId == val.Value &&
                                                    x.TaskData != null &&
                                                    x.TaskData.StateId == (int)States.ProductionComplete).ToList();
            }
            else if (filter == NCR_FILTERS.Vendor && val != null && val > 0)
            {

                var totalQuantity = orders
                    .Where(x => x.Product != null && x.Product.VendorId == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var local_ncrs =  userType == USER_TYPE.Admin ? NcreportService.FindNCReports()
                        .Where(x => x.Product.VendorId == val.Value) : NcreportService.FindNCReportByVendorId(val.Value);
                
                ncrs = local_ncrs.Where(x => x.CustomerCompany.isEnterprise == false).ToList();
                var totalNcrs = ncrs
                    .Where(x => x.RootCause != null)
                    .GroupBy(x => new { x.Order.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) }).ToList();


                var lst2 = (from ord in totalQuantity
                            join ncr in totalNcrs
                                on ord.Month equals ncr.Month into ord_ncr
                            from nc in ord_ncr.DefaultIfEmpty()
                            select new
                            {
                                Month = ord.Month,
                                Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                            }).ToList();


                model.TotalQuantity = totalQuantity
                     .GroupBy(x => new { x.Month })
                     .Select(g => g.Sum(q => (int)q.Qty)).ToList();


                model.TotalQuantityWithoutNcrs = lst2
                           .GroupBy(x => new { x.Month })
                           .Select(g => g.Sum(q => (int)q.Qty)).ToList();

                model.DateRange = orders
                    .OrderBy(x => x.OrderDate.Month)
                    .Where(x => x.Product != null && x.Product.VendorId == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => g.First().OrderDate.ToString("MMMM")).ToList();

                var tasks = TaskDataService.FindTaskDataByVendorId(val.Value, States.ProductionComplete);
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         ord.Product != null &&
                                         ord.Product.VendorId == val &&
                                         ord.PartNumber != null
                                   select ord).ToList();

            }
            else if (filter == NCR_FILTERS.Customer && val != null && val > 0)
            {

                var totalQuantity = orders
                    .Where(x => x.Product != null && x.Product.CustomerId == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var local_ncrs = userType == USER_TYPE.Admin ? NcreportService.FindNCReports().Where(x => x.Product.CustomerId == val.Value)
                                                             : NcreportService.FindNCReportByCustomerId(val.Value);

                ncrs = local_ncrs.Where(x => x.CustomerCompany.isEnterprise == false).ToList();
                var totalNcrs = ncrs
                    .Where(x => x.RootCause != null)
                    .GroupBy(x => new { x.Order.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) }).ToList();


                var lst2 = (from ord in totalQuantity
                            join ncr in totalNcrs
                                on ord.Month equals ncr.Month into ord_ncr
                            from nc in ord_ncr.DefaultIfEmpty()
                            select new
                            {
                                Month = ord.Month,
                                Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                            }).ToList();

                model.TotalQuantity = totalQuantity
                     .GroupBy(x => new { x.Month })
                     .Select(g => g.Sum(q => (int)q.Qty)).ToList();


                model.TotalQuantityWithoutNcrs = lst2
                           .GroupBy(x => new { x.Month })
                           .Select(g => g.Sum(q => (int)q.Qty)).ToList();

                model.DateRange = orders
                    .OrderBy(x => x.OrderDate.Month)
                    .Where(x => x.Product != null && x.Product.CustomerId == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => g.First().OrderDate.ToString("MMMM")).ToList();

                var tasks = TaskDataService.FindTaskDataByCustomerId(val.Value, States.ProductionComplete);
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         ord.Product != null &&
                                         ord.Product.CustomerId == val &&
                                         ord.PartNumber != null
                                   select ord).ToList();
            }
            else if (filter == NCR_FILTERS.Year && val != null && val > 0)
            {

                var totalQuantity = orders
                    .Where(x => x.OrderDate.Year == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) })
                    .OrderBy(x => x.Month)
                    .ToList();

                var local_ncrs = userType == USER_TYPE.Admin ? NcreportService.FindNCReports().Where(x => x.Order.OrderDate.Year == val.Value)
                                                             : (userType == USER_TYPE.Customer ? NcreportService.FindNCReportByCustomerId(companyId)
                                                                                               : NcreportService.FindNCReportByVendorId(companyId))
                                                            .Where(x => x.Order.OrderDate.Year == val.Value);
 
                ncrs = local_ncrs.Where(x => x.CustomerCompany.isEnterprise == false).ToList();
                var totalNcrs = ncrs
                    .Where(x => x.RootCause != null)
                    .GroupBy(x => new { x.Order.OrderDate.Month })
                    .Select(g => new { Month = g.Key.Month, Qty = g.Sum(q => q.Quantity) }).ToList();

                var lst2 = (from ord in totalQuantity
                            join ncr in totalNcrs
                                on ord.Month equals ncr.Month into ord_ncr
                            from nc in ord_ncr.DefaultIfEmpty()
                            select new
                            {
                                Month = ord.Month,
                                Qty = nc != null ? ord.Qty - nc.Qty : ord.Qty,
                            }).ToList();

                model.TotalQuantity = totalQuantity
                    .GroupBy(x => new { x.Month })
                    .Select(g => g.Sum(q => (int)q.Qty)).ToList();


                model.TotalQuantityWithoutNcrs = lst2
                           .GroupBy(x => new { x.Month })
                           .Select(g => g.Sum(q => (int)q.Qty)).ToList();

                model.DateRange = orders
                    .OrderBy(x => x.OrderDate.Month)
                    .Where(x => x.OrderDate.Year == val)
                    .GroupBy(x => new { x.OrderDate.Month })
                    .Select(g => g.First().OrderDate.ToString("MMMM")).ToList();

                var tasks = TaskDataService.GetTaskDataAll().Where(x => x.TaskId == (int)States.ProductionComplete);
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         ord.Product != null &&
                                         ord.OrderDate.Year == val &&
                                         ord.PartNumber != null
                                   select ord).ToList();
            }

            var TotalCustomerNcrs = ncrs
                .Where(x => x.RootCause == NC_ROOT_CAUSE.CUSTOMER)
                .GroupBy(x => new { x.Order.OrderDate.Month })
                .Select(g => new Tuple<int, int?>(g.Key.Month, g.Sum(q => q.Quantity != null ? (int)q.Quantity : 0))).ToList();

            var TotalVendorNcrs = ncrs
                .Where(x => x.RootCause == NC_ROOT_CAUSE.VENDOR)
                .GroupBy(x => new { x.Order.OrderDate.Month })
                .Select(g => new Tuple<int, int?>(g.Key.Month, g.Sum(q => q.Quantity != null ? (int)q.Quantity : 0))).ToList();



            model.TotalCustomerNcrs = new List<int>();
            model.TotalVendorNcrs = new List<int>();

            foreach (string month in model.DateRange)
            {
                var list = TotalCustomerNcrs.Where(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Item1).Equals(month));
                if (list != null && list.Any())
                {
                    model.TotalCustomerNcrs.Add(list.Select(x => (x.Item2 != null ? x.Item2.Value : 0)).First());
                }
                else
                {
                    model.TotalCustomerNcrs.Add(0);
                }
                list = TotalVendorNcrs.Where(x => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Item1).Equals(month));
                if (list != null && list.Any())
                {
                    model.TotalVendorNcrs.Add(list.Select(x => (x.Item2 != null ? x.Item2.Value : 0)).First());
                }
                else
                {
                    model.TotalVendorNcrs.Add(0);
                }
            }
        }

        public ChartTypeViewModel GetVendorQAChart(int vendorId, int i)
        {
            var tasks = TaskDataService.FindTaskDataByVendorId(vendorId);
            var orders = OrderService.FindOrderByVendorId(vendorId).Where(x => x.TaskId != null).ToList();
            var completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         td.StateId == (int)States.ProductionComplete &&
                                         ord.Product != null &&
                                         ord.Product.VendorId == vendorId &&
                                         ord.PartNumber != null
                                   select ord).ToList();

            ChartInfoViewModel entity = new ChartInfoViewModel();
            List<NCReport> ncrs = new List<NCReport>();
            SetChartData(ref entity, ref ncrs, vendorId, orders, null, null, USER_TYPE.Vendor, ref completedOrders);

            int totalNCRs = entity.TotalCustomerNcrs.Sum() + entity.TotalVendorNcrs.Sum();
            int totalQty = entity.TotalQuantity.Sum();

            var model = new ChartTypeViewModel()
            {
                Index = i,
                ChartType = ChartType.BarChart,
                ConformanceRate = (totalQty - totalNCRs) / (float)totalQty,
                PctgNCRsOrders = ncrs.Count() / (float)orders.Count(),
                PctgCustomerNCRsQuantities = entity.TotalCustomerNcrs.Sum() / (float)totalNCRs,
                PctgVendorNCRsQuantities = entity.TotalVendorNcrs.Sum() / (float)totalNCRs,
                TotalOrderQty = totalQty,
                TotalOrders = completedOrders.Count(),
            };

            model.chartData = new ChartData()
            {
                DateRange = entity.DateRange.ToArray(),
                TotalQuantity = entity.TotalQuantity.ToArray(),
                TotalCustomerNcrs = entity.TotalCustomerNcrs.ToArray(),
                TotalVendorNcrs = entity.TotalVendorNcrs.ToArray(),
            };
            return model;
        }

        public NCRViewModel NCRCharts(ChartType chartType = ChartType.BarChart, NCR_FILTERS? filter = null, int? val = null, DateTime? StartDateTime = null, DateTime? EndDateTime = null)
        {
            Company company = null;
            if (UserContext.UserId != null)
            {
                company = CompanyService.FindCompanyByUserId(UserContext.UserId);
            }
            else
            {
                throw new BLException("You are not logged in.");
            }


            if (filter != null && filter != NCR_FILTERS.NoFilter && val == null || filter == null && val != null)
            {
                throw new BLException("filter or filter value must be provide as pair or both are null!");
            }

            NCRViewModel model = new NCRViewModel();
            List<NCReport> ncrs = new List<NCReport>();
            List<Product> products = new List<Product>();
            List<Order> orders = OrderService.FindOrderList().ToList();
            List<NCReport> allNcrs = NCReportService.FindNCReports().ToList();

            if (UserContext.UserType == USER_TYPE.Admin)
            {
                products = orders.Select(x => x.Product).OrderBy(x => x.PartNumber).ToList();
            }
            else
            {
#if false
                orders = UserContext.UserType == USER_TYPE.Customer ? OrderService.FindOrderByCustomerId(company.Id).ToList()
                                                                    : OrderService.FindOrderByVendorId(company.Id).ToList();

                products = UserContext.UserType == USER_TYPE.Customer ? ProductService.FindProductListByCompanyId(company.Id).ToList()
                                                                      : ProductService.FindProductListByVendorId(company.Id).ToList();

#else
                orders = UserContext.UserType == USER_TYPE.Customer ? orders.Where(x => x.Product.CustomerId == company.Id && x.ProductSharingId == null ||
                                                                                        x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == company.Id)
                                                                            .OrderBy(x => x.OrderDate)
                                                                            .ToList()
                                                                    : orders.Where(x => x.Product.VendorId == company.Id).OrderBy(x => x.OrderDate).ToList();

                products = UserContext.UserType == USER_TYPE.Customer ? orders.Select(x => x.Product).Where(x => x.CustomerId == company.Id).ToList()
                                                                      : orders.Select(x => x.Product).Where(x => x.VendorId == company.Id).ToList();
#endif

            }
            //orders = orders.Where(x => x.TaskId != null).ToList();            
            List<Order> completedOrders = new List<Order>();
            if (UserContext.UserType == USER_TYPE.Customer)
            {
                var tasks = TaskDataService.FindTaskDataByCustomerId(company.Id);
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         td.StateId == (int)States.ProductionComplete &&
                                         ord.Product != null &&
                                         ord.Product.CustomerId == company.Id &&
                                         ord.PartNumber != null
                                   select ord).ToList();
            }
            else if (UserContext.UserType == USER_TYPE.Vendor)
            {
                var tasks = TaskDataService.FindTaskDataByVendorId(company.Id);
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         td.StateId == (int)States.ProductionComplete &&
                                         ord.Product != null &&
                                         ord.Product.VendorId == company.Id &&
                                         ord.PartNumber != null
                                   select ord).ToList();
            }
            else if (UserContext.UserType == USER_TYPE.Admin)
            {
                var tasks = TaskDataService.GetTaskDataAll();
                completedOrders = (from td in tasks
                                   from ord in orders
                                   where td.TaskId == ord.TaskId &&
                                         td.StateId == (int)States.ProductionComplete &&
                                         ord.Product != null &&
                                         ord.Product.CustomerId != null && ord.Product.VendorId != null &&
                                         ord.PartNumber != null && ord.Quantity > 0
                                   select ord).ToList();
            }

            products = (from pd in products
                        from ord in orders
                        where pd.Id == ord.ProductId && ord.Quantity > 0 && pd.PartNumber != null
                        select pd)
                        .GroupBy(x => x.PartNumber)
                        .Select(x => x.First())
                        .ToList();

            model.Orders = orders;
            model.Products = products;

            ChartInfoViewModel entity = new ChartInfoViewModel();

            var dicFilter = new Dictionary<string, string>();

            switch (filter)
            {
                case NCR_FILTERS.Year:
                    dicFilter = new Dictionary<string, string> { { "Order Year", val.ToString() } };
                    break;
                case NCR_FILTERS.Product:
                    dicFilter = new Dictionary<string, string> { { "Part#", ProductService.FindProductById(val.Value).PartNumber } };
                    break;
                case NCR_FILTERS.Customer:
                    dicFilter = new Dictionary<string, string> { { "Customer", CompanyService.FindCompanyById(val.Value).Name } };
                    break;
                case NCR_FILTERS.Vendor:
                    dicFilter = new Dictionary<string, string> { { "Vendor", CompanyService.FindCompanyById(val.Value).Name } };
                    break;

                case NCR_FILTERS.NoFilter:
                    dicFilter = new Dictionary<string, string> { { "NoFilter", String.Empty } };
                    filter = null;
                    val = null;
                    break;

                default:
                    break;

            }

            SetChartData(ref entity, ref ncrs, company.Id, orders, filter, val, UserContext.UserType, ref completedOrders);

            int totalNCRs = entity.TotalCustomerNcrs.Sum() + entity.TotalVendorNcrs.Sum();
            int totalOrderQty = entity.TotalQuantity.Sum();
            int totalQuantityWithoutNcrs = entity.TotalQuantityWithoutNcrs.Sum();

            

            model.ChartType = new ChartTypeViewModel()
            {
                Index = 0,
                ChartType = chartType,
                ConformanceRate = totalQuantityWithoutNcrs / (float)totalOrderQty,
                PctgNCRsOrders = ncrs.Count() / (float)orders.Count(),
                PctgCustomerNCRsQuantities = entity.TotalCustomerNcrs.Sum() / (float)totalNCRs,
                PctgVendorNCRsQuantities = entity.TotalVendorNcrs.Sum() / (float)totalNCRs,
                TotalOrders = completedOrders.Count(),
                TotalOrderQty = totalOrderQty,
                DicFilter = dicFilter,
            };

            model.NcrInfoList = ncrs.Select(g => new NcrInfoViewModel
            {
                Id = g.Id,
                NCRNumber = g.NCRNumber,
                NCRNumberForVendor = g.NCRNumberForVendor,
                DateInitiated = g.NCDetectedDate,
                RootCause = g.RootCause != null ? Enum.GetName(typeof(NC_ROOT_CAUSE), g.RootCause) : null,
                Vendor = g.Product?.VendorCompany?.Name,
                Cost = g.Cost,
                DateClosed = g.DateNcrClosed,
                UserType = UserContext.UserType,
            }).ToList();

            model.ChartType.chartData = new ChartData
            {
                DateRange = entity.DateRange.ToArray(),
                TotalQuantity = entity.TotalQuantityWithoutNcrs.ToArray(),
                TotalCustomerNcrs = entity.TotalCustomerNcrs.ToArray(),
                TotalVendorNcrs = entity.TotalVendorNcrs.ToArray(),
            };

            return model;
        }

        public RFQChartDataViewModel GetChartData(int id, UserMode mode, NCR_CHART_FILTERS? filter = null, int? val = null, int? vendorId = null, DateTime? StartDatetime = null, DateTime? EndDatetime = null)
        {
            var model = NCRChartsApi(id, mode, filter, val, vendorId, StartDatetime, EndDatetime);
            return model;
        }

        public class ActualCompleteOrder
        {
            public int Month { get; set; }
            public int? Qty { get; set; }
        }


        public class GetChartDataQueryDTO
        {
            public int Id { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public DateTime? ShippedDate { get; set; }
            public decimal Quantity { get; set; }
            public DateTime? EstimateCompletionDate { get; set; }
            public int? ProductionLeadTime { get; set; }

            public override string ToString()
            {
                return $"{{ Id = {Id}, Year = {Year}, Month = {Month}, ShippedDate = {ShippedDate}, Quantity = {Quantity}, EstimateCompletionDate = {EstimateCompletionDate}, ProductionLeadTime = {ProductionLeadTime} }}";
            }

            public override bool Equals(object value)
            {
                var type = value as GetChartDataQueryDTO;
                return (type != null) && EqualityComparer<int>.Default.Equals(type.Id, Id) && EqualityComparer<int>.Default.Equals(type.Year, Year) && EqualityComparer<int>.Default.Equals(type.Month, Month) && EqualityComparer<DateTime?>.Default.Equals(type.ShippedDate, ShippedDate) && EqualityComparer<decimal>.Default.Equals(type.Quantity, Quantity) && EqualityComparer<DateTime?>.Default.Equals(type.EstimateCompletionDate, EstimateCompletionDate) && EqualityComparer<int?>.Default.Equals(type.ProductionLeadTime, ProductionLeadTime);
            }

            public override int GetHashCode()
            {
                int num = 0x7a2f0b42;
                num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Id);
                num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Year);
                num = (-1521134295 * num) + EqualityComparer<int>.Default.GetHashCode(Month);
                num = (-1521134295 * num) + EqualityComparer<DateTime?>.Default.GetHashCode(ShippedDate);
                num = (-1521134295 * num) + EqualityComparer<decimal>.Default.GetHashCode(Quantity);
                num = (-1521134295 * num) + EqualityComparer<DateTime?>.Default.GetHashCode(EstimateCompletionDate);
                return (-1521134295 * num) + EqualityComparer<int?>.Default.GetHashCode(ProductionLeadTime);
            }
        }

        private RFQChartDataViewModel NCRChartsApi(int companyId, UserMode mode, NCR_CHART_FILTERS? filter = null, int? val = null, int? vendorId = null, DateTime? StartDatetime = null, DateTime? EndDatetime = null)
        {
            Company company = CompanyService.FindCompanyById(companyId);

            var orders = (mode == UserMode.Vendor ?
                                  OrderService.FindOrdersByVendorId(companyId) :
                                  OrderService.FindOrdersByCustomerId(companyId));
            orders = orders.Where(x => vendorId != null ? x.Product.VendorId == vendorId : true);

            var totalNcrs = (mode == UserMode.Vendor ?
                                     NCReportService.FindNCReportByVendorId(companyId) :
                                     NCReportService.FindNCReportByCustomerId(companyId));
            totalNcrs = totalNcrs.Where(x => vendorId != null ? x.VendorId == vendorId : true);

            if (mode == UserMode.Sharer)
            {
                orders = orders.Where(x => x.ProductSharingId != null && x.ProductSharing.OwnerCompanyId == companyId);
                totalNcrs = totalNcrs.Where(x => x.Order.ProductSharingId != null && x.Order.ProductSharing.OwnerCompanyId == companyId);
            }

            int ordCounts = 0;
            int diff = 0;
            int shippedParts = 0;
            int ncrParts = 0;

            switch (filter)
            {
                case NCR_CHART_FILTERS.Product:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductId == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.ProductId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Vendor:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.Product.VendorId == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.VendorId == val.Value);
                    }
                    break;

                case NCR_CHART_FILTERS.Customer:
                    if (val != null)
                    {
                        orders = orders
                            .Where(x => x.CustomerId != null ? x.CustomerId == val.Value : true)
                            .Where(x => x.Product.CustomerId == val.Value || x.ProductSharingId != null && x.ProductSharing.SharingCompanyId == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.CustomerId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Year:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.OrderDate.Year == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.Order.OrderDate.Year == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Sharee:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductSharing.SharingCompanyId == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.Order.ProductSharing.SharingCompanyId == val.Value);
                    }
                    break;
                case NCR_CHART_FILTERS.Sharer:
                    if (val != null)
                    {
                        orders = orders.Where(x => x.ProductSharing.OwnerCompanyId == val.Value);
                        totalNcrs = totalNcrs.Where(x => x.Order.ProductSharing.OwnerCompanyId == val.Value);
                    }
                    break;
                default:
                    break;
            }

            IEnumerable<GetChartDataQueryDTO> ords;
            IEnumerable<GetChartDataQueryDTO> ordersOnTime;
            IQueryable<NCReport> ncrs;
            List<GetChartDataQueryDTO> orderlist = new List<GetChartDataQueryDTO>();

            if (StartDatetime != null && EndDatetime != null)
            {
                orderlist = (from ord in orders.Distinct()
                                 join ncr in totalNcrs
                                 on ord.Id equals ncr.OrderId into ord_ncr
                             where ord.OrderDate >= StartDatetime && ord.OrderDate <= EndDatetime
                             from nc in ord_ncr.DefaultIfEmpty()
                             select new GetChartDataQueryDTO
                             {
                                 Id = ord.Id,
                                 Year = ord.OrderDate.Year,
                                 Month = ord.OrderDate.Month,
                                 ShippedDate = ord.ShippedDate,
                                 Quantity = nc != null && nc.Quantity != null ? ord.Quantity - nc.Quantity.Value : ord.Quantity,
                                 EstimateCompletionDate = ord.EstimateCompletionDate,
                                 ProductionLeadTime = ord.Product.ProductionLeadTime
                             }).OrderBy(x => x.Id).ToList();
            }
            else
            {
                orderlist = (from ord in orders.Distinct()
                             join ncr in totalNcrs
                             on ord.Id equals ncr.OrderId into ord_ncr
                             from nc in ord_ncr.DefaultIfEmpty()
                             select new GetChartDataQueryDTO
                             {
                                 Id = ord.Id,
                                 Year = ord.OrderDate.Year,
                                 Month = ord.OrderDate.Month,
                                 ShippedDate = ord.ShippedDate,
                                 Quantity = nc != null && nc.Quantity != null ? ord.Quantity - nc.Quantity.Value : ord.Quantity,
                                 EstimateCompletionDate = ord.EstimateCompletionDate,
                                 ProductionLeadTime = ord.Product.ProductionLeadTime
                             }).OrderBy(x => x.Id).ToList();
            }

            var chartDataList = orderlist
                   .GroupBy(x => new { x.Year, x.Month })
                   .Select(g => new RFQChartData
                   {
                       At = $"{g.Key.Year}-{g.Key.Month}",
                       ShippedParts = shippedParts = (int)(ords = orderlist.Where(o => o.Year == g.Key.Year && o.Month == g.Key.Month && o.ShippedDate != null)).Sum(x => x.Quantity),
                       ShippedOrders = g.Count(x => x.ShippedDate != null),
                       OrdersOnTime = (ordersOnTime = g.Where(x => x.EstimateCompletionDate != null && x.ShippedDate != null && x.ShippedDate <= x.EstimateCompletionDate)).Count(),
                       NcrsByVendor = (ncrs = totalNcrs.Where(x => x.Order != null &&
                                                                   x.Order.OrderDate != null &&
                                                                   x.Order.OrderDate.Year == g.Key.Year &&
                                                                   x.Order.OrderDate.Month == g.Key.Month)).Count(x => x.RootCause == NC_ROOT_CAUSE.VENDOR),
                       NCRParts = ncrs != null && ncrs.Count() > 0 ? (ncrParts = ncrs.Sum(x => (int)x.Quantity)) : 0,
                       PartsConformance = (shippedParts + ncrParts) > 0 ? shippedParts / (float)(shippedParts + ncrParts) : 0,
                       OrderConformance = ((diff = ((ordCounts = ords.Count()) - ncrs.Select(x => x.OrderId).Distinct().Count())) > 0 ? diff : 0) /
                                          (float)(ordCounts == 0 ? 1 : ordCounts),
                       OnTimeConformance = ordCounts > 0 ? ordersOnTime.Count() / (float)ordCounts : 0,
                       AvrLeadTime = (ords.Sum(x => x.ProductionLeadTime) ?? 0) / (ordCounts > 0 ? ordCounts : 1),
                       Month = g.Key.Month,
                       Year = g.Key.Year,
                   }).ToList();

            RFQChartDataViewModel model = new RFQChartDataViewModel
            {
                Company = new CompanyInfo
                {
                    Id = companyId,
                    Name = company.Name,
                    City = company.Address?.City,
                    Country = company.Address?.Country?.CountryName,
                },

                ChartData = chartDataList,
            };

            //Fix Nan's
            //foreach (var chartData in chartDataList)
            //{
            //    if (float.IsNaN(chartData.PartsConformance))
            //    {
            //        chartData.PartsConformance = 0;
            //    }
            //    if (float.IsNaN(chartData.OrderConformance))
            //    {
            //        chartData.OrderConformance = 0;
            //    }
            //    if (float.IsNaN(chartData.OnTimeConformance))
            //    {
            //        chartData.OnTimeConformance = 0;
            //    }
            //}

            //if (StartDatetime != null && EndDatetime != null)
            //{
            //    var dataList = model.ChartData.Where(x => x.Year >= StartDatetime.Value.Year &&
            //                                      x.Month >= StartDatetime.Value.Month &&
            //                                      x.Year <= EndDatetime.Value.Year &&
            //                                      x.Month <= EndDatetime.Value.Month).Select(x => x).ToList();
            //    model.ChartData = dataList;
            //}
            return model;
        }

        public VendorStatsViewModel NCRStatsApi(int vendorId)
        {
            Company company = CompanyService.FindCompanyById(vendorId);
            var orders = OrderService.FindOrderByVendorId(vendorId);

            var totalNcrs = NCReportService.FindNCReportByVendorId(vendorId).ToList();
            int ncrs = totalNcrs.Count();

            var shippedParts = (int) orders.Where(x => x.ShippedDate != null).Sum(x => x.Quantity);
            var shippedOrders = orders.Where(x => x.ShippedDate != null).Count();
            var ordersOnTime = orders.Where(x => x.EstimateCompletionDate != null && x.ShippedDate != null && x.ShippedDate <= x.EstimateCompletionDate).Count();
            var ncrsByVendor = totalNcrs.Count(x => x.RootCause == NC_ROOT_CAUSE.VENDOR);
            var partsConformance = shippedParts > 0 ? (float)(shippedParts - totalNcrs.Sum(x => x.Quantity)) / (float)shippedParts : 0;
            var orderConformance = shippedOrders > 0 ? (shippedOrders - ncrs) / (float)shippedOrders : 0;
            var onTimeConformance = shippedOrders > 0 ? ordersOnTime / (float)shippedOrders : 0;
            var avrLeadTime = (int)orders.Where(x => x.ShippedDate != null).Sum(x => x.Product.ProductionLeadTime) / (shippedOrders > 0 ? shippedOrders : 1);

            RFQChartData stats = new RFQChartData
            {
                ShippedParts = shippedParts,
                ShippedOrders = shippedOrders,
                OrdersOnTime = ordersOnTime,
                NcrsByVendor = ncrs,
                PartsConformance = partsConformance,
                OrderConformance = orderConformance,
                OnTimeConformance = onTimeConformance,
                AvrLeadTime = avrLeadTime,
            };


            VendorStatsViewModel model = new VendorStatsViewModel
            {
                Id = vendorId,
                Name = company.Name,
                Country = company.Address?.Country?.CountryName,
                Company = new CompanyInfo
                {
                    Id = vendorId,
                    Name = company.Name,
                    City = company.Address?.City,
                    Country = company.Address?.Country?.CountryName,
                },
                Stats = stats,
            };
            return model;
        }

        // Chart js
        [HttpPost]
        public JsonResult MultiBarChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            Chart _chart = new Chart();
            _chart.labels = DateRange;

            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByCustomer,
                data = TotalCustomerNcrs,
                backgroundColor = ChartColors.TotalCustomerNcrs,
                borderColor = ChartColors.TotalCustomerNcrs,
                borderWidth = "1"
            });
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByVendor,
                data = TotalVendorNcrs,
                backgroundColor = ChartColors.TotalVendorNcrs,
                borderColor = ChartColors.TotalVendorNcrs,
                borderWidth = "1"
            });
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.TotalQuantity,
                data = TotalQuantity,
                backgroundColor = ChartColors.TotalQuantity,
                borderColor = ChartColors.TotalQuantity,
                borderWidth = "1"
            });
            _chart.datasets = _dataSet;
            //return Json(_chart, JsonRequestBehavior.AllowGet);

            return new JsonResult()
            {
                Data = _chart,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult MultiLineChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            Chart _chart = new Chart();
            _chart.labels = DateRange;

            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();

            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByCustomer,
                data = TotalCustomerNcrs,
                backgroundColor = ChartColors.TotalCustomerNcrs,
                borderColor = ChartColors.TotalCustomerNcrs,
                borderWidth = "1",
            });
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByVendor,
                data = TotalVendorNcrs,
                backgroundColor = ChartColors.TotalVendorNcrs,
                borderColor = ChartColors.TotalVendorNcrs,
                borderWidth = "1"
            });
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.TotalQuantity,
                data = TotalQuantity,
                backgroundColor = ChartColors.TotalQuantity,
                borderColor = ChartColors.TotalQuantity,
                borderWidth = "1"
            });

            _chart.datasets = _dataSet;
            //return Json(_chart, JsonRequestBehavior.AllowGet);

            return new JsonResult()
            {
                Data = _chart,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }


        public JsonResult MultiPieChartData(string[] DateRange, int[] TotalQuantity, int[] TotalCustomerNcrs, int[] TotalVendorNcrs)
        {
            Chart _chart = new Chart();
            _chart.labels = DateRange;

            List<string> bColor = new List<string>();
            for (int i = 0; i < DateRange.Length; i++)
            {
                string color = ChartColors.PieChartMonthColors[DateRange[i]];
                bColor.Add(color);
            }

            _chart.datasets = new List<Datasets>();
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByCustomer,
                data = TotalCustomerNcrs,
                backgroundColor = bColor.ToArray(),
                borderColor = bColor.ToArray(),
                borderWidth = "1",

            });
            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.NCRCausedByVendor,
                data = TotalVendorNcrs,
                backgroundColor = bColor.ToArray(),
                borderColor = bColor.ToArray(),
                borderWidth = "1",
            });

            _dataSet.Add(new Datasets()
            {
                label = IndicatingMessages.TotalQuantity,
                data = TotalQuantity,
                backgroundColor = bColor.ToArray(),
                borderColor = bColor.ToArray(),
                borderWidth = "1",
            });


            _chart.datasets = _dataSet;

            //return Json(_chart, JsonRequestBehavior.AllowGet);

            return new JsonResult()
            {
                Data = _chart,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}