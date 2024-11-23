using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using GoogleSheet.Data;

namespace ProductionPE
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        private static readonly HttpClient httpClient = new HttpClient(); // HttpClient tái sử dụng
        private static Dictionary<string, Bitmap> imageCache = new Dictionary<string, Bitmap>(); // Bộ nhớ cache ảnh

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            dbWorkHouseDataContext _db = new dbWorkHouseDataContext();

            // Truy vấn cơ sở dữ liệu và tạo danh sách sản phẩm
            var sanPhams = _db.v_sanphams.Select(sp => new SanPham
            {
                MaCcdc = sp.MaCcdc,
                TenSanPham = sp.TenSanPham,
                HinhAnh = sp.HinhAnh,
                NhomSanPham = sp.NhomSanPham,
                SLNhap = sp.SLNhap,
                SLXuat = sp.SLXuat,
                SlTon = sp.SlTon,
                LastUpdate = sp.LastUpdate,
                HinhAnhBitmap = null // Chưa tải ảnh
            }).ToList();

            // Sau khi lấy xong, tải hình ảnh bất đồng bộ và song song
            var sanPhamsWithImages = await LoadImagesAsync(sanPhams);

            // Gán datasource cho GridControl
            gridControlData.DataSource = sanPhamsWithImages;

            // Khởi tạo RepositoryItemPictureEdit để hiển thị hình ảnh trong GridView
            RepositoryItemPictureEdit pictureEdit = new RepositoryItemPictureEdit();
            gridControlData.RepositoryItems.Add(pictureEdit);

            // Đặt SizingMode của RepositoryItemPictureEdit để hình ảnh tự động vừa ô
            pictureEdit.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

            // Gán RepositoryItemPictureEdit cho cột HinhAnhBitmap
            gridViewData.Columns["HinhAnhBitmap"].ColumnEdit = pictureEdit;
            gridViewData.Columns["HinhAnhBitmap"].Caption = "Hình Ảnh";
            gridViewData.Columns["HinhAnhBitmap"].Visible = true;
            gridViewData.Columns["HinhAnhBitmap"].OptionsColumn.AllowEdit = false;

            gridViewData.Columns["HinhAnh"].Visible = false;

            // Điều chỉnh kích thước cột hình ảnh và chiều cao hàng sao cho hình ảnh vừa vặn
            gridViewData.Columns["HinhAnhBitmap"].Width = 50; // Đặt chiều rộng cột
            gridViewData.RowHeight = 10; // Đặt chiều cao của hàng (lý tưởng cho hình ảnh)
        }

        // Tải hình ảnh bất đồng bộ và song song
        private async Task<List<SanPham>> LoadImagesAsync(List<SanPham> sanPhams)
        {
            var tasks = sanPhams.Select(sp => LoadImageForProductAsync(sp)).ToList();
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        // Tải hình ảnh cho từng sản phẩm
        private async Task<SanPham> LoadImageForProductAsync(SanPham sp)
        {
            sp.HinhAnhBitmap = await LoadImageBitmapAsync(sp.HinhAnh);
            return sp;
        }

        // Tải hình ảnh từ đường dẫn hoặc URL, sử dụng cache nếu đã tải trước đó
        private async Task<Bitmap> LoadImageBitmapAsync(string imagePathOrUrl)
        {
            if (imageCache.ContainsKey(imagePathOrUrl)) // Kiểm tra cache
            {
                return imageCache[imagePathOrUrl];
            }

            try
            {
                Bitmap image = null;

                // Nếu là đường dẫn cục bộ
                if (File.Exists(imagePathOrUrl))
                {
                    image = await Task.Run(() => (Bitmap)Image.FromFile(imagePathOrUrl));
                }
                // Nếu là URL hợp lệ
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(imagePathOrUrl);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        image = (Bitmap)Image.FromStream(ms);
                    }
                }

                if (image != null)
                {
                    imageCache[imagePathOrUrl] = image; // Lưu vào cache
                }

                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tải ảnh: " + ex.Message);
            }
            return null;
        }
    }

    public class SanPham
    {
        public string MaCcdc { get; set; }
        public string TenSanPham { get; set; }
        public string HinhAnh { get; set; }
        public string NhomSanPham { get; set; }
        public int? SLNhap { get; set; }
        public int? SLXuat { get; set; }
        public int? SlTon { get; set; }
        public string LastUpdate { get; set; }
        public Bitmap HinhAnhBitmap { get; set; }  
    }
}
