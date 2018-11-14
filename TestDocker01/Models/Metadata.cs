namespace TestDocker01.Models
{
    /// <summary>
    /// An entity class that represents the search result metadata.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total records count.
        /// </summary>
        /// <value>
        /// The total records count.
        /// </value>
        public int TotalCount { get; set; }
    }
}
