namespace CodePulse.API.Models.Domain
{
    public class Category // Lớp đại diện cho danh mục
    {
        public Guid Id { get; set; } // Mã định danh duy nhất
        public string Name { get; set; } // Tên danh mục
        public string UrlHandle { get; set; } // Đường dẫn URL cho danh mục
    }

}
