using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Drive.v3; // Để làm việc với Google Drive API, nếu cần lấy ảnh
using Google.Apis.Util.Store;
using GoogleSheet.Data;

public class GoogleSheetsReader
{
    private static string ApplicationName = "Google Sheets API Example";
    private static string spreadsheetId = "1Oy2Z99kFpRm5EhudTsclvrw2RydZSLcBK3b8JwrOqZo"; // Thay bằng ID Google Sheet của bạn
    private static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly, DriveService.Scope.DriveReadonly }; // Thêm quyền truy cập Google Drive để lấy ảnh

    private SheetsService service;
    private DriveService driveService;  // Thêm DriveService để lấy ảnh

    public GoogleSheetsReader(string jsonCredentialsFilePath)
    {
        // Xác thực bằng Service Account
        GoogleCredential credential;
        using (var stream = new FileStream(jsonCredentialsFilePath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        // Khởi tạo SheetsService
        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        // Khởi tạo DriveService để làm việc với Google Drive
        driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    public List<SanPham> ReadDataFromSheet(string targetSheetName)
    {
        dbWorkHouseDataContext _db = new dbWorkHouseDataContext();
        var sanPhams = new List<SanPham>();

        // Lấy tất cả các sheet trong tài liệu
        var spreadsheetResponse = service.Spreadsheets.Get(spreadsheetId).Execute();

        // Kiểm tra xem sheet có tên cần tìm có tồn tại không
        var sheet = spreadsheetResponse.Sheets.FirstOrDefault(s => s.Properties.Title == targetSheetName);

        if (sheet != null)
        {
            // Nếu tìm thấy sheet có tên
            string sheetName = sheet.Properties.Title;
            Console.WriteLine($"Dữ liệu từ sheet: {sheetName}");

            // Đọc dữ liệu từ sheet
            string range = $"{sheetName}!A:Z";  // Đọc toàn bộ các cột từ A đến Z (hoặc thay đổi phạm vi nếu cần)
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();

            // Kiểm tra nếu có dữ liệu trả về
            if (response.Values != null && response.Values.Count > 1) // Kiểm tra dữ liệu > 1 hàng để bỏ qua hàng đầu tiên
            {
                // Bắt đầu từ hàng thứ 2 (chỉ số 1), bỏ qua hàng đầu tiên (thường là tiêu đề)
                for (int i = 1; i < response.Values.Count; i++)  // Bắt đầu từ i = 1 để bỏ qua hàng đầu tiên
                {
                    if (i == 1)
                        continue;
                    var row = response.Values[i]; // Lấy dòng thứ i

                    // Tạo đối tượng SanPham từ từng hàng dữ liệu
                    SanPham sanPham = new SanPham
                    {
                        SanPhamId = row.Count > 0 ? row[0].ToString() : "0",
                        SanPhamThamChieu = row.Count > 1 ? row[1].ToString() : "",
                        MaCcdc = row.Count > 2 ? row[2].ToString() : "",
                        TenSanPham = row.Count > 3 ? row[3].ToString() : "",
                        HienThi = row.Count > 4 ? row[4].ToString() : "",
                        PartNo = row.Count > 5 ? row[5].ToString() : "",
                        NhomSanPham = row.Count > 6 ? row[6].ToString() : "",
                        QuyCach = row.Count > 7 ? row[7].ToString() : "",
                        ViTri = row.Count > 8 ? row[8].ToString() : "",
                        HinhAnh = row.Count > 9 ? row[9].ToString().Replace("SAN_PHAM_Images/", @"D:\Image\") : "",
                        DiaChi = row.Count > 10 ? row[10].ToString() : "",
                        ThongTin = row.Count > 11 ? row[11].ToString() : "",
                        HanSuDung = row.Count > 12 ? row[12].ToString() : "",
                        Dvt = row.Count > 13 ? row[13].ToString() : "",
                        SlNhap = row.Count > 14 && int.TryParse(row[14].ToString(), out int slNhap) ? slNhap : 0,
                        SlXuat = row.Count > 15 && int.TryParse(row[15].ToString(), out int slXuat) ? slXuat : 0,
                        SlTon = row.Count > 16 && int.TryParse(row[16].ToString(), out int slTon) ? slTon : 0,
                        GiaTriTon = row.Count > 17 && decimal.TryParse(row[17].ToString(), out decimal giaTriTon) ? giaTriTon : 0m,
                        TrangThai = row.Count > 18 ? row[18].ToString() : "",
                        GhiChu = row.Count > 19 ? row[19].ToString() : "",
                        NgayTao = row.Count > 20 ? row[20].ToString() : "",
                        NguoiTao = row.Count > 21 ? row[21].ToString() : "",
                        LastUpdate = row.Count > 22 ? row[22].ToString() : ""


                    };

                    var sanphami = _db.SanPhams.FirstOrDefault(x => x.SanPhamId == sanPham.SanPhamId);
                    if (sanphami != null)
                    {
                        // Nếu tồn tại, cập nhật dữ liệu
                        sanphami.SanPhamThamChieu = sanPham.SanPhamThamChieu;
                        sanphami.MaCcdc = sanPham.MaCcdc;
                        sanphami.TenSanPham = sanPham.TenSanPham;
                        sanphami.HienThi = sanPham.HienThi;
                        sanphami.PartNo = sanPham.PartNo;
                        sanphami.NhomSanPham = sanPham.NhomSanPham;
                        sanphami.QuyCach = sanPham.QuyCach;
                        sanphami.ViTri = sanPham.ViTri;
                        sanphami.HinhAnh = sanPham.HinhAnh;
                        sanphami.DiaChi = sanPham.DiaChi;                   
                        sanphami.ThongTin = sanPham.ThongTin;
                        sanphami.HanSuDung = sanPham.HanSuDung;
                        sanphami.Dvt = sanPham.Dvt;
                        sanphami.SlNhap = sanPham.SlNhap;
                        sanphami.SlXuat = sanPham.SlXuat;
                        sanphami.SlTon = sanPham.SlTon;
                        sanphami.GiaTriTon = sanPham.GiaTriTon;
                        sanphami.TrangThai = sanPham.TrangThai;
                        sanphami.GhiChu = sanPham.GhiChu;
                        sanphami.NgayTao = sanPham.NgayTao;
                        sanphami.NguoiTao = sanPham.NguoiTao;
                        sanphami.LastUpdate = sanPham.LastUpdate;

                    }
                    else
                    {
                        
                        // Nếu không tồn tại, thêm mới
                        _db.SanPhams.InsertOnSubmit(sanPham);
                    }

                    // Lưu lại các thay đổi vào cơ sở dữ liệu
                    _db.SubmitChanges();
                    sanPhams.Add(sanPham);
                }
            }
            else
            {
                Console.WriteLine("Không có dữ liệu trong sheet này.");
            }
        }
        else
        {
            Console.WriteLine($"Không tìm thấy sheet có tên '{targetSheetName}'.");
        }

        return sanPhams;
    }

    // Hàm lấy ảnh từ Google Drive (nếu có liên kết đến ảnh)
    public string GetImageUrlFromDrive(string fileId)
    {
        try
        {
            // Lấy metadata của file từ Google Drive
            var request = driveService.Files.Get(fileId);
            var file = request.Execute();
            return file.WebContentLink; // Link tải về nội dung của file
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy ảnh: {ex.Message}");
            return null;
        }
    }
}

// Sử dụng class GoogleSheetsReader
class Program
{
    static void Main(string[] args)
    {
        string jsonCredentialsFilePath = @"D:\okivnpe-78bf73de6b9f.json"; // Đường dẫn tới file JSON Service Account
        GoogleSheetsReader reader = new GoogleSheetsReader(jsonCredentialsFilePath);
        var sanPhams = reader.ReadDataFromSheet("SAN_PHAM"); // Thay tên sheet theo nhu cầu

        foreach (var sanPham in sanPhams)
        {
            Console.WriteLine($"SanPhamId: {sanPham.SanPhamId}, HinhAnh: {sanPham.HinhAnh}");
        }
    }
}
