namespace TestDocker01.Models
{
    /// <summary>
    /// An DTO class that represents base search criteria.
    /// </summary>
    public class BaseSearchCriteria
    {
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page index.
        /// </summary>
        /// <value>
        /// The page index.
        /// </value>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the sort by property.
        /// </summary>
        /// <value>
        /// The sort by property.
        /// </value>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the type of the sort.
        /// </summary>
        /// <value>
        /// The type of the sort.
        /// </value>
        public SortType SortType { get; set; }
    }
}
