# PRN232 - Lab 1: Learning Management System (LMS) API

Dự án này là một RESTful API được xây dựng theo kiến trúc 3 lớp (API, Services, Repositories) sử dụng **.NET 9** và **SQL Server**. Toàn bộ hệ thống đã được cấu hình Docker hóa hoàn chỉnh giúp Mentor có thể chạy thử và chấm bài một cách nhanh chóng nhất mà không cần cài đặt .NET SDK hay SQL Server cục bộ trên máy.

---

## 🚀 Hướng Dẫn Chạy Dự Án (Dành Cho Mentor)

Để khởi chạy toàn bộ ứng dụng và cơ sở dữ liệu, Mentor chỉ cần thực hiện các bước đơn giản sau:

### 1. Yêu cầu hệ thống
* Máy tính đã cài đặt và đang chạy **Docker Desktop** (hoặc Docker Engine + Docker Compose).

### 2. Các bước khởi chạy
1. Giải nén thư mục dự án và mở công cụ dòng lệnh (Terminal / PowerShell / CMD / Git Bash) tại thư mục chứa file `docker-compose.yml`.
2. Chạy lệnh sau để Docker tự động tải image, build source code và khởi chạy hệ thống:
   ```bash
   docker compose up --build
   ```
3. Sau khi Docker khởi động hoàn tất (thường mất khoảng 15-30 giây ở lần đầu tiên để SQL Server khởi tạo):
   * **Truy cập Swagger UI (API Documentation & Testing):** Mở trình duyệt và truy cập địa chỉ:
     [http://localhost:5000](http://localhost:5000) (hoặc [http://localhost:5000/index.html](http://localhost:5000/index.html))
   * **Kết nối Database (MSSQL):**
     * **Server:** `localhost,1433`
     * **User:** `sa`
     * **Password:** `YourStrongPassword123!`
     * *Lưu ý:* Cơ sở dữ liệu sẽ tự động tạo cấu trúc bảng và seeding đầy đủ dữ liệu mẫu (**5 học kỳ, 10 môn học, 20 khóa học, 50 sinh viên và 500 lượt đăng ký**) ngay trong lần chạy đầu tiên nhờ cơ chế `EnsureCreated()`.

---

## 🛠️ Xử Lý Sự Cố Thường Gặp (Troubleshooting)

### 📌 Lỗi trùng cổng `1433` (Cổng SQL Server mặc định)
Nếu máy của Mentor đã cài đặt SQL Server cục bộ và đang chạy service SQL Server trên cổng `1433`, lệnh `docker compose up` sẽ báo lỗi:
> *Bind for 0.0.0.0:1433 failed: port is already allocated.*

**👉 Cách khắc phục rất đơn giản:**
1. Mở file `docker-compose.yml`.
2. Tìm đến cấu hình của service `db`, phần `ports` ở dòng 10-11:
   ```yaml
   ports:
     - "1433:1433"
   ```
3. Đổi cổng bên ngoài host thành một cổng khác bất kỳ (ví dụ `14301`), giữ nguyên cổng container phía sau:
   ```yaml
   ports:
     - "14301:1433"
   ```
4. Lưu file và chạy lại lệnh: `docker compose up --build`
5. Lúc này ứng dụng API vẫn hoạt động bình thường (vì API kết nối trực tiếp với DB qua mạng ảo nội bộ của Docker bằng tên service `Server=db` chứ không qua cổng host). Nếu muốn kết nối DB từ SSMS cục bộ, Mentor sẽ dùng Server: `localhost,14301`.

### 📌 Dọn dẹp dữ liệu cũ (Reset Database)
Nếu muốn xóa toàn bộ database và container để chạy lại từ đầu với dữ liệu sạch:
```bash
docker compose down -v
```
*(Tham số `-v` sẽ xóa volume lưu trữ dữ liệu `mssql_data` để SQL Server tạo lại DB mới ở lần khởi động sau).*

---

## 📂 Cấu Trúc Các File Docker Trong Dự Án

1. **`docker-compose.yml` (ở thư mục gốc):**
   * Định nghĩa 2 dịch vụ: `db` (SQL Server 2022) và `api` (ASP.NET Core API).
   * Có cơ chế `healthcheck` cho SQL Server, đảm bảo database khởi động hoàn tất trước khi API khởi chạy (`depends_on` với `condition: service_healthy`), tránh lỗi nghẽn hoặc lỗi kết nối lúc khởi động.
   * Truyền chuỗi kết nối vào API thông qua Environment Variable ghi đè file cấu hình.
2. **`PRN232.LMS.API/Dockerfile`:**
   * Sử dụng kỹ thuật **Multi-stage build** tối ưu hóa kích thước image.
   * Stage 1: Dùng `dotnet/sdk:9.0` để khôi phục NuGet packages và biên dịch ứng dụng.
   * Stage 2: Chỉ lấy sản phẩm sau khi publish và chạy trên môi trường runtime `dotnet/aspnet:9.0` gọn nhẹ và bảo mật.
3. **`.dockerignore` (ở thư mục gốc):**
   * Loại bỏ các file tạm thời, thư mục `bin/`, `obj/`, `.git/`, `.vs/` khỏi Docker Build Context để tăng tốc độ build hình ảnh và tránh xung đột thư viện giữa các hệ điều hành.
