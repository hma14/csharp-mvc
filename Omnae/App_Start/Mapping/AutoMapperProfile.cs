using AutoMapper;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Model.ViewModels;
using Omnae.ShippingAPI.DHL.Models;
using Omnae.ViewModels;
using Address = Omnae.Model.Models.Address;

namespace Omnae.App_Start.Mapping
{
    class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile() : base()
        {

            CreateMap<Product, RFQViewModel>(MemberList.Destination);
            CreateMap<RFQViewModel, Product>(MemberList.Source);

            CreateMap<RequestQuote, ShippingQuoteRequestViewModel>(MemberList.Destination);
            CreateMap<ShippingQuoteRequestViewModel, RequestQuote>(MemberList.Source);

            CreateMap<NCReport, NcrDescriptionViewModel>(MemberList.Destination);
            CreateMap<NcrDescriptionViewModel, NCReport>(MemberList.Source);

            CreateMap<TaskData, TaskDataViewModel>(MemberList.Destination);
            CreateMap<TaskDataViewModel, TaskData>(MemberList.Source);

            CreateMap<Product, ProductViewModel>(MemberList.Destination);
            CreateMap<ProductViewModel, Product>(MemberList.Source);

            CreateMap<Order, OrderViewModel>(MemberList.Destination);
            CreateMap<OrderViewModel, Order>(MemberList.Source);

            CreateMap<Address, BillingAddressViewModel>(MemberList.Destination);
            CreateMap<BillingAddressViewModel, Address>(MemberList.Source);


            CreateMap<Product, ProductListViewModel>();
            CreateMap<ProductViewModel, Product>();
            CreateMap<Product, UpdateRFQViewModel>();
            CreateMap<Order, OrderViewModel>();

            CreateMap<CompaniesCreditRelationship, CompaniesCreditRelationshipViewModel>();
            CreateMap<CompaniesCreditRelationshipViewModel, CompaniesCreditRelationship>();

        }
    }
}