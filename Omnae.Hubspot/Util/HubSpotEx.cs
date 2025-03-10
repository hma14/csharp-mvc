using System.Collections.Generic;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using JetBrains.Annotations;
using Omnae.Hubspot.Model;

namespace Omnae.Hubspot.Util
{
    public static class HubSpotEx
    {
        [CanBeNull]
        public static T GetByEmailOrNull<T>(this IHubSpotContactApi api, string email) where T : ContactHubSpotModel, new()
        {
            try
            {
                return api.GetByEmail<T>(email);
            }
            catch (HubSpotException e) when (e.RawJsonResponse.ToLowerInvariant().Contains("contact does not exist"))
            {
                return null;
            }
        }

        [CanBeNull]
        public static T CreateOrIgnore<T>(this IHubSpotContactApi api, T entity) where T : ContactHubSpotModel, new()
        {
            if (api.GetByEmailOrNull<T>(entity.Email) != null)
                return null;
            try
            {
                return api.Create<T>(entity);
            }
            catch (HubSpotException e) when (e.RawJsonResponse.Contains("Contact already exists"))
            {
                return null;
            }
        }

        public static IEnumerable<T> ListAll<T>(this IHubSpotCompanyApi api, List<string> propertiesToInclude = null) where T : CompanyHubSpotModel, new()
        {
            var offset = 0L;
            var haveMoreResults = true;

            do
            {
                var page = api.List<T>(new ListRequestOptions{Offset = offset, Limit = 100, PropertiesToInclude = propertiesToInclude});

                foreach (var company in page.Companies)
                {
                    yield return company;
                }

                offset = page.ContinuationOffset;
                haveMoreResults = page.MoreResultsAvailable;
            } while (haveMoreResults);
        }

        [CanBeNull]
        public static bool? ExistsByDomain<T>(this IHubSpotCompanyApi api, string companyDomain) where T : CompanyHubSpotModel, new()
        {
            return string.IsNullOrWhiteSpace(companyDomain) ? (bool?) null : api.GetByDomain<Company>(companyDomain) != null;
        }

    }
}
