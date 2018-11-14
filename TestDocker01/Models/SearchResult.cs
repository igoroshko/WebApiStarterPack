using System.Collections.Generic;

namespace TestDocker01.Models
{
    /// <summary>
    /// An entity class that represents search result.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the items in the search result.</typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public Metadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<T> Items { get; set; }
    }
}
