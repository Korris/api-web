namespace Mcsg.Lib.Data.Entities.Common
{
    public class PagedResults<T>
    {
        public PagedResults(int totalItems,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Init(totalItems, pageNumber, pageSize);
            Items = new List<T>();
        }
        public PagedResults(List<T> items,
            int totalItems,
            int pageNumber = 1,
            int pageSize = 10)
        {
            Init(totalItems, pageNumber, pageSize);
            Items = items;
        }

        private void Init(int totalItems,
            int pageNumber,
            int pageSize)
        {
            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);

            // Ensure actual page isn't out of range
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            else if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Total number of items to be paged
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Maximum number of page navigation links to display, default is 5
        /// </summary>
        public int MaxNavigationPages { get; private set; } = 5;

        /// <summary>
        /// Current active page
        /// </summary>
        public int PageNumber { get; private set; } = 1;

        /// <summary>
        /// Number of items per page, default is 10
        /// </summary>
        public int PageSize { get; private set; } = 10;

        public int TotalPages { get; private set; }
    }
}