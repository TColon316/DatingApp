namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        #region User Filters
        public string? Gender { get; set; }

        // Prevents user from seeing themselves in the results
        public string? CurrentUsername { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 125;

        public string OrderBy { get; set; } = "lastActive";
        #endregion
    }
}