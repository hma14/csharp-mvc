using AutoMapper;
using Omnae.BlobStorage;
using Omnae.BusinessLayer.Models;
using Omnae.Common;
using Omnae.Model.Models;
using Omnae.Model.ViewModels;
using Omnae.ShippingAPI.DHL.Models;
using Omnae.WebApi.DTO;
using Omnae.WebApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Omnae.Data.Query.OrderQuery;
using Address = Omnae.Model.Models.Address;

namespace Omnae.WebApi.App_Start.Mapping
{
    class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile() : base()
        {
            CreateMap<Company, CompanyDTO>(MemberList.Destination)
                .ForMember(dto => dto.Email, m => m.MapFrom(db => db.Shipping.EmailAddress))
                .ForMember(db => db.CompanyAddedAt, m => m.MapFrom(db => db._createdAt))
                .ForMember(db => db.ContactFirstName, m => m.MapFrom(db => db.Users.FirstOrDefault().FirstName))
                .ForMember(db => db.ContactLastName, m => m.MapFrom(db => db.Users.FirstOrDefault().LastName))
                .ForMember(dto => dto.BillingAddress, m => m.MapFrom(db => db.BillAddress));
            CreateMap<CompanyDTO, Company>(MemberList.Source)
                .ForMember(db => db.CompanyType, m => m.MapFrom(dto => CompanyType.None))
                .ForMember(db => db.BillAddress, m => m.MapFrom(dto => dto.BillingAddress))
                .ForMember(db => db.Currency, m => m.MapFrom(dto => dto.CurrencyCode))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Address, AddressDTO>(MemberList.Destination)
                .ForMember(vm => vm.StateOrProvinceName, m => m.MapFrom(db => db.StateProvince.Name))
                .ForMember(vm => vm.Country, m => m.MapFrom(db => db.Country.CountryName));
            CreateMap<AddressDTO, Address>(MemberList.Source)
                .ForPath(db => db.StateProvince.Name, m => m.MapFrom(dto => dto.StateOrProvinceName))
                .ForPath(db => db.Country.CountryName, m => m.MapFrom(dto => dto.Country));

            CreateMap<Shipping, ShippingDTO>(MemberList.Destination);
            CreateMap<ShippingDTO, Shipping>(MemberList.Source);

            CreateMap<ShippingProfile, ShippingProfileDTO>(MemberList.Destination)
                .ForMember(vm => vm.ProfileDescription, m => m.MapFrom(db => db.Description))
                .ForMember(vm => vm.Attention_FreeText, m => m.MapFrom(db => db.Shipping.Attention_FreeText))
                .ForMember(vm => vm.PhoneNumber, m => m.MapFrom(db => db.Shipping.PhoneNumber))
                .ForMember(vm => vm.EmailAddress, m => m.MapFrom(db => db.Shipping.EmailAddress))
                .ForMember(vm => vm.Address, m => m.MapFrom(db => db.Shipping.Address));
            CreateMap<ShippingProfileDTO, ShippingProfile>(MemberList.Source)
                .ForPath(db => db.Shipping.CompanyId, m => m.MapFrom(dto => dto.CompanyId))
                .ForPath(db => db.Shipping.Address.CompanyId, m => m.MapFrom(dto => dto.CompanyId))
                .ForPath(db => db.Shipping.Attention_FreeText, m => m.MapFrom(dto => dto.Attention_FreeText))
                .ForPath(db => db.Shipping.PhoneNumber, m => m.MapFrom(dto => dto.PhoneNumber))
                .ForPath(db => db.Shipping.EmailAddress, m => m.MapFrom(dto => dto.EmailAddress))
                .ForPath(db => db.Shipping.Address, m => m.MapFrom(dto => dto.Address))
                .ForPath(db => db.Description, m => m.MapFrom(dto => dto.ProfileDescription));
            CreateMap<ShippingProfileDTO, Shipping>(MemberList.Source)
                .ForPath(db => db.Address.CompanyId, m => m.MapFrom(dto => dto.CompanyId))
                .ForPath(db => db.Address.CompanyId, m => m.MapFrom(dto => dto.CompanyId))
                .ForPath(db => db.Attention_FreeText, m => m.MapFrom(dto => dto.Attention_FreeText))
                .ForPath(db => db.PhoneNumber, m => m.MapFrom(dto => dto.PhoneNumber))
                .ForPath(db => db.EmailAddress, m => m.MapFrom(dto => dto.EmailAddress))
                .ForPath(db => db.Address, m => m.MapFrom(dto => dto.Address));

            CreateMap<ShippingAccount, ShippingAccountDTO>(MemberList.Destination);

            CreateMap<ShippingAccountDTO, ShippingAccount>(MemberList.Source);


            CreateMap<Product, ProductDTO>(MemberList.Destination)
                .ForMember(vm => vm.ModifiedUtc, m => m.MapFrom(db => db._updatedAt))
                .ForMember(vm => vm.VendorName, m => m.MapFrom(db => db.VendorCompany.Name));
            CreateMap<ProductDTO, Product>(MemberList.Source);

            CreateMap<Order, OrderDTO>(MemberList.Destination)
                .ForMember(vm => vm.State, m => m.MapFrom(db => (States)db.TaskData.StateId))
                .ForMember(vm => vm.PartName, m => m.MapFrom(db => db.Product.Name))
                .ForMember(vm => vm.IsRiskBuild, m => m.MapFrom(db => db.TaskData.IsRiskBuild))
                .ForMember(vm => vm.RejectReason, m => m.MapFrom(db => db.TaskData.RejectReason))
                .ForMember(vm => vm.NewDesireShippingDate, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.NewDesireShippingDate : (DateTime?)null))
                .ForMember(vm => vm.ExpeditedShipmentRequestId, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequestId : null))
                .ForMember(vm => vm.InitiateCompanyId, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.InitiateCompanyId : (int?)null))
                .ForMember(vm => vm.ExpeditedShipmentType, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.ExpeditedShipmentType : (EXPEDITED_SHIPMENT_TYPE?)null))
                .ForMember(vm => vm.IsExpeditedShipmentAccepted, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.IsAccepted : (bool?)null))
                .ForMember(vm => vm.IsRequestedByCustomer, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.IsRequestedByCustomer : (bool?)null))
                .ForMember(vm => vm.IsRequestedByVendor, m => m.MapFrom(db => db.ExpeditedShipmentRequestId != null ? db.ExpeditedShipmentRequest.IsRequestedByVendor : (bool?)null))
                .ForMember(vm => vm.CancelOrderRequest, m => m.MapFrom(db => db.CancelOrderReason != null));

            CreateMap<OrderDTO, Order>(MemberList.Source);

            CreateMap<Product, ProductViewModel>(MemberList.Destination)
                .ForMember(vm => vm.PreferredCurrencyText, m => m.MapFrom(db => db.PreferredCurrency));
            CreateMap<ProductViewModel, Product>(MemberList.Source)
                .ForMember(vm => vm.PreferredCurrency, m => m.MapFrom(db => db.PreferredCurrencyText));

            CreateMap<NcrCreationApiViewModel, NcrDescriptionViewModel>(MemberList.Source);
            CreateMap<NCReport, NcrDescriptionViewModel>(MemberList.Destination);
            CreateMap<NcrDescriptionViewModel, NCReport>(MemberList.Source)
                .ForMember(vm => vm.NCDetectedby, m => m.Ignore());
            CreateMap<NCReport, NCReportDTO>(MemberList.Destination);
            CreateMap<NCReportDTO, NCReport>(MemberList.Source);

            CreateMap<NCRViewModel, ChartDataDTO>(MemberList.Destination);
            CreateMap<ChartDataDTO, NCRViewModel>(MemberList.Source);

            CreateMap<Product, ProductListViewModel>();
            CreateMap<ProductViewModel, Product>();

            CreateMap<Product, RFQViewModel>();
            CreateMap<RFQViewModel, Product>();

            CreateMap<RequestQuote, ShippingQuoteRequestViewModel>();
            CreateMap<ShippingQuoteRequestViewModel, RequestQuote>();

            CreateMap<PriceBreak, PriceBreakDTO>();
            CreateMap<PriceBreakDTO, PriceBreak>();

            CreateMap<UserPerformance, UserPerformanceViewModel>();
            CreateMap<UserPerformanceViewModel, UserPerformance>();

            CreateMap<ProductViewModel, ProductApiViewModel>();
            CreateMap<ProductApiViewModel, ProductViewModel>();

            CreateMap<NcrDescriptionViewModel, NcrDescriptionDTO>();
            CreateMap<NcrDescriptionDTO, NcrDescriptionViewModel>();

            CreateMap<RFQViewModel, RFQApiViewModel>();
            CreateMap<RFQApiViewModel, RFQViewModel>();

            CreateMap<CompaniesCreditRelationship, CompaniesCreditRelationshipViewModel>();
            CreateMap<CompaniesCreditRelationshipViewModel, CompaniesCreditRelationship>();

            CreateMap<PartRevision, PartRevisionDTO>()
                .ForMember(vm => vm.ProductId, m => m.MapFrom(db => db.TaskData.ProductId));
            CreateMap<PartRevisionDTO, PartRevision>();

            CreateMap<CompanyBankInfo, CompanyBankInfoDTO>(MemberList.Destination)
                .ForMember(vm => vm.AddressLine1, m => m.MapFrom(db => db.BankAddress.AddressLine1))
                .ForMember(vm => vm.AddressLine2, m => m.MapFrom(db => db.BankAddress.AddressLine2))
                .ForMember(vm => vm.City, m => m.MapFrom(db => db.BankAddress.City))
                .ForMember(vm => vm.ZipCode, m => m.MapFrom(db => db.BankAddress.ZipCode))
                .ForMember(vm => vm.PostalCode, m => m.MapFrom(db => db.BankAddress.PostalCode))
                .ForMember(vm => vm.StateProvince, m => m.MapFrom(db => db.BankAddress.StateProvince.Name))
                .ForMember(vm => vm.Country, m => m.MapFrom(db => db.BankAddress.Country.CountryName));


            CreateMap<CompanyBankInfoDTO, CompanyBankInfo>();

            CreateMap<CompanyBankInfo, CompanyBankInfoViewModel>();
            CreateMap<CompanyBankInfoViewModel, CompanyBankInfo>(MemberList.Destination);

            CreateMap<NCRImages, NCRImagesDTO>();
            CreateMap<NCRImagesDTO, NCRImages>();

            CreateMap<ProductSharing, ProductShareDto>()
                .ForMember(vm => vm.Id, m => m.MapFrom(db => db.Id))
                .ForMember(vm => vm.SharerCompanyId, m => m.MapFrom(db => db.OwnerCompanyId))
                .ForMember(vm => vm.ShareeCompanyId, m => m.MapFrom(db => db.SharingCompanyId))
                .ForMember(vm => vm.ShareeCompanyName, m => m.MapFrom(db => db.ProductSharingCompany.Name))
                .ForMember(vm => vm.ShareeCompanyEmail, m => m.MapFrom(db => db.ProductSharingCompany.Shipping.EmailAddress))
                .ForMember(vm => vm.CreatedAt, m => m.MapFrom(db => db.CreatedUtc));
            CreateMap<ProductShareDto, ProductSharing>();


            CreateMap<ProductSharing, ProductCustomerDto>()
                .ForMember(vm => vm.Id, m => m.MapFrom(db => db.SharingCompanyId))
                .ForMember(vm => vm.Name, m => m.MapFrom(db => db.ProductSharingCompany.Name))
                .ForMember(vm => vm.Email, m => m.MapFrom(db => db.ProductSharingCompany.Shipping.EmailAddress))
                .ForMember(vm => vm.Logo, m => m.MapFrom(db => db.ProductSharingCompany.CompanyLogoUri))
                .ForMember(vm => vm.BecameCustomerAt, m => m.MapFrom(db => db.ProductSharingCompany._createdAt));
            CreateMap<ProductCustomerDto, ProductSharing>();


            CreateMap<Document, DocumentDTO>(MemberList.Destination)
                .BeforeMap<DocumentAfterMap>();


            CreateMap<VendorBidRFQStatus, VendorBidRFQStatusDTO>()
                .ForMember(vm => vm.RejectRFQReason, m => m.MapFrom(db => db.RFQActionReasonId != null && db.RFQActionReason.ReasonType == REASON_TYPE.REJECT_RFQ ? db.RFQActionReason.Reason : null))
                .ForMember(vm => vm.RejectRFQDescription, m => m.MapFrom(db => db.RFQActionReasonId != null && db.RFQActionReason.ReasonType == REASON_TYPE.REJECT_RFQ ? db.RFQActionReason.Description : null))
                .ForMember(vm => vm.TimeStamp, m => m.MapFrom(db => db._createdAt));
            CreateMap<VendorBidRFQStatusDTO, VendorBidRFQStatus>();

            CreateMap<BidRequestRevision, BidRequestRevisionDTO>()
                .ForMember(vm => vm.RFQRevisionReason, m => m.MapFrom(db => db.RFQActionReasonId != null && db.RFQActionReason.ReasonType == REASON_TYPE.RFQ_REVISION_REQUEST ? db.RFQActionReason.Reason : null))
                .ForMember(vm => vm.RFQRevisionDescription, m => m.MapFrom(db => db.RFQActionReasonId != null && db.RFQActionReason.ReasonType == REASON_TYPE.RFQ_REVISION_REQUEST ? db.RFQActionReason.Description : null));
            CreateMap<BidRequestRevisionDTO, BidRequestRevision>();

            CreateMap<ProductPriceQuote, ProductPriceQuoteDTO>()
                .ForMember(vm => vm.PriceBreaks, m => m.MapFrom(db => ToISet<PriceBreak, PriceBreakDTO>(db.PriceBreaks)));

            CreateMap<ProductPriceQuoteDTO, ProductPriceQuote>();
        }


        private static ISet<TDestination> ToISet<TSource, TDestination>(IEnumerable<TSource> source)
        {
            ISet<TDestination> set = null;
            if (source != null)
            {
                set = new HashSet<TDestination>();

                foreach (TSource item in source)
                {
                    set.Add(Mapper.Map<TSource, TDestination>(item));
                }
            }
            return set;
        }
    }

    internal class DocumentAfterMap : IMappingAction<Document, DocumentDTO>
    {
        public DocumentAfterMap(DocumentStorageService blobService)
        {
            BlobService = blobService;
        }

        private DocumentStorageService BlobService { get; }

        public void Process(Document source, DocumentDTO destination)
        {
            var newUrl = BlobService.AddSecurityTokenToUrl(source.DocUri);
            destination.DocUri = source.DocUri = newUrl;
        }
    }
}