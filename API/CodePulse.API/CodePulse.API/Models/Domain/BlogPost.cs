namespace CodePulse.API.Models.Domain
{
    public class BlogPost
    {
        public Guid Id { get; set; } // Mã định danh duy nhất
        public string Title { get; set; } // Tiêu đề bài viết
        public string ShortDescription { get; set; } // Mô tả ngắn
        public string Content { get; set; } // Nội dung bài viết
        public string FeatureImageUrl { get; set; } // URL hình ảnh nổi bật
        public string UrlHandle { get; set; } // Đường dẫn URL
        public DateTime PuslishedDate { get; set; } // Ngày xuất bản
        public string Author { get; set; } // Tác giả
        public bool IsVisible { get; set; } // Hiển thị hay không
    }
}
