using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Đường dẫn đến tệp credentials.json (Service Account JSON)
        var credentialsPath = @"D:\okivnpe-6a56ddc97fdb.json";

        // Tạo credential từ tệp JSON của Service Account
        var credential = GoogleCredential.FromFile(credentialsPath)
            .CreateScoped(DriveService.Scope.DriveReadonly); // Điều chỉnh phạm vi nếu cần

        // Khởi tạo DriveService sử dụng credential
        var driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Drive Service",
        });

        // ID của thư mục trên Google Drive (cần thay đổi với thư mục của bạn)
        string folderId = "1VnnvLxCmMQ8kVI8G36gH8C5ilPK3hZR1"; // Thay ID thư mục của bạn ở đây

        // Tải tất cả tệp trong thư mục về thư mục D:\
        await DownloadAllFilesInFolder(driveService, folderId, @"D:\Image");

        Console.WriteLine("All files have been downloaded.");
        Console.ReadLine();
    }

    // Hàm tải xuống tất cả các tệp trong thư mục Google Drive
    static async Task DownloadAllFilesInFolder(DriveService driveService, string folderId, string localFolderPath)
    {
        var nextPageToken = string.Empty;

        // Lặp qua các trang tệp cho đến khi không còn tệp nào
        do
        {
            // Lấy danh sách tệp trong thư mục (bao gồm phân trang)
            var request = driveService.Files.List();
            request.Q = $"'{folderId}' in parents"; // Lấy tất cả các tệp trong thư mục
            request.Fields = "nextPageToken, files(id, name)"; // Lấy ID và tên tệp
            request.PageToken = nextPageToken; // Đặt token trang hiện tại

            var files = await request.ExecuteAsync();

            // Duyệt qua từng tệp và tải xuống nếu tệp chưa tồn tại
            foreach (var file in files.Files)
            {
                string localFilePath = Path.Combine(localFolderPath, file.Name);

                // Kiểm tra nếu tệp đã tồn tại
                if (File.Exists(localFilePath))
                {
                    Console.WriteLine($"File {file.Name} already exists. Skipping download.");
                    continue; // Nếu tệp đã tồn tại, bỏ qua việc tải xuống
                }

                Console.WriteLine($"Downloading: {file.Name} ({file.Id})");

                // Lấy luồng dữ liệu tệp từ Google Drive
                var fileRequest = driveService.Files.Get(file.Id);
                var fileStream = new MemoryStream();

                // Tải tệp về bộ nhớ
                await fileRequest.DownloadAsync(fileStream);

                // Lưu tệp xuống thư mục local
                using (var fileOutput = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    await fileStream.CopyToAsync(fileOutput);
                }

                Console.WriteLine($"File {file.Name} downloaded to {localFilePath}");
            }

            // Lấy token của trang tiếp theo (nếu có)
            nextPageToken = files.NextPageToken;

        } while (!string.IsNullOrEmpty(nextPageToken)); // Lặp lại cho đến khi không còn trang nào
    }
}
