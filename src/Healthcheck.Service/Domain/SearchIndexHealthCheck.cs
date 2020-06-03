namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Models;
    using Sitecore.Configuration;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.ContentSearch.Utilities;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Search Index Heatlh check component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class SearchIndexHealthCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        /// <value>
        /// The name of the index.
        /// </value>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the minimum expected documents count.
        /// </summary>
        /// <value>
        /// The minimum expected documents count.
        /// </value>
        public int MinimumExpectedDocumentsCount { get; set; }

        /// <summary>
        /// Gets or sets the custom query.
        /// </summary>
        /// <value>
        /// The custom query.
        /// </value>
        public string CustomQuery { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchIndexHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public SearchIndexHealthCheck(Item item) : base(item)
        {
            this.IndexName = item["Indexname"];
            this.MinimumExpectedDocumentsCount = Sitecore.MainUtil.GetInt(item["Minimum Expected Documents Count"], 0);
            this.CustomQuery = item["Custom Query"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = "Search index contain no errors";

            if (string.IsNullOrEmpty(IndexName))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Search Index Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            ISearchIndex index;
            try
            {
                index = ContentSearchManager.GetIndex(IndexName);
            }
            catch(Exception ex)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Search Index does not exist or cannot be loaded: " + IndexName,
                    Exception = null
                });

                return;
            }

            var totalResults = 0;

            try
            {
                IQueryable<SearchResultItem> query = null;

                using (var context = index.CreateSearchContext())
                {
                    if (string.IsNullOrEmpty(CustomQuery))
                    {
                        query = context.GetQueryable<SearchResultItem>();
                    }
                    else
                    {
                        var stringModel = SearchStringModel.ExtractSearchQuery(CustomQuery);
                        using (var switcher = new DatabaseSwitcher(Factory.GetDatabase("master")))
                        {
                            query = LinqHelper.CreateQuery<SearchResultItem>(context, stringModel);
                        }
                    }

                    totalResults = query.Count();
                }

                if (MinimumExpectedDocumentsCount > totalResults)
                {
                    this.Status = HealthcheckStatus.Error;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("Search Index returned less items than expected: {0}, actual: {1} ", this.MinimumExpectedDocumentsCount, totalResults),
                        Exception = null
                    });

                    return;
                }
            }   
            catch (Exception ex)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Search Index error",
                    Exception = ex
                });

                return;
            }
        }
    }
}