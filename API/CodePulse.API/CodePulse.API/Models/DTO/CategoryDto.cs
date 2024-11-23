namespace CodePulse.API.Models.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; } // Mã định danh duy nhất
        public string Name { get; set; } // Tên danh mục
        public string UrlHandle { get; set; } // Đường dẫn URL cho danh mục
    }
}
