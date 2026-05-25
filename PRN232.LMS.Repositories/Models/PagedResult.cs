namespace PRN232.LMS.Repositories.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public PagedResult(List<T> Items, int Page, int PageSize, int TotalItems)
        {
            this.Items = Items;
            this.Page = Page;
            this.PageSize = PageSize;
            this.TotalItems = TotalItems;
        }

    }
}
