using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Omnae.WebApi.Models;

namespace Omnae.WebApi.Util
{
    /// <summary>
    /// Pagging class
    /// </summary>
    public static class Pagging
    {
        /// <summary>
        /// Creates a paged set of results.
        /// </summary>
        /// <typeparam name="T">The type of the source IQueryable.</typeparam>
        /// <typeparam name="TReturn">The type of the returned paged results.</typeparam>
        /// <param name="queryable">The source IQueryable.</param>
        /// <param name="requestContext">The context associated with request.</param>
        /// <param name="page">The page number you want to retrieve.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="orderBy">The field or property to order by.</param>
        /// <param name="ascending">Indicates whether or not the order should be ascending (true) or descending (false.)</param>
        /// <param name="routeName">Route name</param>
        /// <returns>Returns a paged set of results.</returns>
        public static async Task<PagedResultSet<TReturn>> ToPagedResultSetAsync<T, TReturn>(
            this IQueryable<T> queryable, HttpRequestContext requestContext, int page, int pageSize = 50,
            string orderBy = null, bool ascending = false, string routeName = "DefaultApi")
        {
            page = Math.Max(1, page);
            var skipAmount = pageSize * (page - 1);

            if (orderBy != null)
            {
                queryable = queryable.OrderByPropertyOrField(orderBy, ascending);
            }

            var projection = queryable
                .Skip(skipAmount)
                .Take(pageSize)
                .ProjectTo<TReturn>();

            var totalNumberOfRecords = await queryable.CountAsync();
            var results = await projection.ToListAsync();

            var totalPageCount = (totalNumberOfRecords + pageSize - 1) / pageSize;
            var nextPageUrl = page < totalPageCount
                ? requestContext.Url?.Link(routeName, new {page = page + 1, pageSize, orderBy, ascending})
                : null;

            return new PagedResultSet<TReturn>
            {
                Results = results,
                Metadata = new PagedResultSet<TReturn>.PageMetadata()
                {
                    PageNumber = page,
                    PageSize = results.Count,
                    TotalNumberOfPages = totalPageCount,
                    TotalNumberOfRecords = totalNumberOfRecords,
                    NextPageUrl = nextPageUrl,
                }
            };
        }
        public static async Task<PagedResultSet<TReturn>> ToPagedResultSetAlternativeMapperAsync<T, TReturn>(
            this IQueryable<T> queryable, HttpRequestContext requestContext, int page, int pageSize = 50,
            string orderBy = null, bool ascending = false, string routeName = "DefaultApi")
        {
            page = Math.Max(1, page);
            var skipAmount = pageSize * (page - 1);

            if (orderBy != null)
            {
                queryable = queryable.OrderByPropertyOrField(orderBy, ascending);
            }

            var projection = await queryable
                .Skip(skipAmount)
                .Take(pageSize)
                .ToListAsync();

            var totalNumberOfRecords = await queryable.CountAsync();
            var results = projection.Select(source => Mapper.Map<TReturn>(source)).ToList();

            var totalPageCount = (totalNumberOfRecords + pageSize - 1) / pageSize;
            var nextPageUrl = page < totalPageCount
                ? requestContext.Url?.Link(routeName, new {page = page + 1, pageSize, orderBy, ascending})
                : null;

            return new PagedResultSet<TReturn>
            {
                Results = results,
                Metadata = new PagedResultSet<TReturn>.PageMetadata()
                {
                    PageNumber = page,
                    PageSize = results.Count,
                    TotalNumberOfPages = totalPageCount,
                    TotalNumberOfRecords = totalNumberOfRecords,
                    NextPageUrl = nextPageUrl,
                }
            };
        }
        /// <summary>
        /// Creates a paged set of results.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable.</typeparam>
        /// <param name="queryable">The input IQueryable.</param>
        /// <param name="requestContext">The context associated with request.</param>
        /// <param name="page">The page number you want to retrieve.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="orderBy">The field or property to order by.</param>
        /// <param name="ascending">Indicates whether or not the order should be ascending (true) or descending (false.)</param>
        /// <param name="routeName">Route name</param>
        /// <returns>Returns a paged set of results.</returns>
        public static async Task<PagedResultSet<T>> ToPagedResultSetAsync<T>(
           this IQueryable<T> queryable, HttpRequestContext requestContext, int page, int pageSize = 50,
           string orderBy = null, bool ascending = false, string routeName = "DefaultApi")
        {
            page = Math.Max(1, page);
            var skipAmount = pageSize * (page - 1);

            if (orderBy != null)
            {
                queryable = queryable.OrderByPropertyOrField(orderBy, ascending);
            }

            var projection = queryable
                .Skip(skipAmount)
                .Take(pageSize);

            int totalNumberOfRecords = 0;
            List<T> results = null;
            try
            {
                totalNumberOfRecords = await queryable.CountAsync();
                results = await projection.ToListAsync();
            }
            catch (Exception ex)
            {
                totalNumberOfRecords = queryable.Count();
                results = projection.ToList();
            }
            
            

            var totalPageCount = (totalNumberOfRecords + pageSize - 1) / pageSize;
            var nextPageUrl = page < totalPageCount
                ? requestContext.Url?.Link(routeName, new { page = page + 1, pageSize, orderBy, ascending })
                : null;

            return new PagedResultSet<T>
            {
                Results = results,
                Metadata = new PagedResultSet<T>.PageMetadata()
                {
                    PageNumber = page,
                    PageSize = results.Count,
                    TotalNumberOfPages = totalPageCount,
                    TotalNumberOfRecords = totalNumberOfRecords,
                    NextPageUrl = nextPageUrl,
                }
            };
        }


        /// <summary>
        /// Overload method for creating a paged set of results.
        /// </summary>
        /// <typeparam name="T">The type of the List.</typeparam>
        /// <param name="queryable">The input IQueryable.</param>
        /// <param name="requestContext">The context associated with request.</param>
        /// <param name="page">The page number you want to retrieve.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="orderBy">The field or property to order by.</param>
        /// <param name="ascending">Indicates whether or not the order should be ascending (true) or descending (false.)</param>
        /// <param name="routeName">Route name</param>
        /// <returns>Returns a paged set of results.</returns>
        public static PagedResultSet<T> ToPagedResultSet<T>(
           this List<T> queryable, HttpRequestContext requestContext, int page, int pageSize = 50,
           string orderBy = null, bool ascending = false, string routeName = "DefaultApi")
        {
            page = Math.Max(1, page);
            var skipAmount = pageSize * (page - 1);

            if (orderBy != null)
            {
                queryable = queryable.OrderByPropertyOrField(orderBy, ascending);
            }

            var totalNumberOfRecords = queryable.Count;
            var results = queryable
                .Skip(skipAmount)
                .Take(pageSize);

            var totalPageCount = (totalNumberOfRecords + pageSize - 1) / pageSize;
            var nextPageUrl = page < totalPageCount
                ? requestContext.Url?.Link(routeName, new { page = page + 1, pageSize, orderBy, ascending })
                : null;

            return new PagedResultSet<T>
            {
                Results = results,
                Metadata = new PagedResultSet<T>.PageMetadata()
                {
                    PageNumber = page,
                    PageSize = results.Count(),
                    TotalNumberOfPages = totalPageCount,
                    TotalNumberOfRecords = totalNumberOfRecords,
                    NextPageUrl = nextPageUrl,
                }
            };
        }
    }
}
