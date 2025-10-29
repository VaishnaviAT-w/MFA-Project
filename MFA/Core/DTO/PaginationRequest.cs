namespace MFA.Core.DTO
{
    public class AdminSettingsPaginationResponse
    {
        public int TotalCount { get; set; }
        public int Index { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }

    public class PaginationRequest
    {
        public int Index { get; set; }
        public int PageSize { get; set; }
       
    }
}