# PetCareX - Hệ Thống Quản Lý Trung Tâm Chăm Sóc Thú Cưng

**Môn học:** CSC12002 - Cơ sở dữ liệu nâng cao
**Trường:** Đại học Khoa học Tự nhiên - ĐHQG TP.HCM
**Khoa:** Công nghệ Thông tin
**Nhóm:** 16

## Giới thiệu

PetCareX là hệ thống quản lý chuỗi trung tâm chăm sóc thú cưng gồm 10 chi nhánh, cung cấp các dịch vụ khám bệnh, tiêm phòng và bán lẻ thức ăn/phụ kiện cho thú cưng. Đồ án tập trung vào thiết kế cơ sở dữ liệu nâng cao (mức quan niệm, logic, vật lý) và áp dụng các giải pháp tối ưu hiệu suất truy vấn như chỉ mục (indexing), phân vùng (partitioning) trên tập dữ liệu lớn.

## Tính năng chính

### Quản lý khách hàng & thú cưng
- Đăng ký hội viên, quản lý thông tin thú cưng
- Chương trình thành viên 3 cấp: Cơ bản – Thân thiết – VIP (dựa trên chi tiêu năm)
- Tích điểm loyalty theo hóa đơn thanh toán

### Dịch vụ
- **Khám bệnh:** Ghi nhận bác sĩ phụ trách, triệu chứng, chẩn đoán, toa thuốc, lịch tái khám
- **Tiêm phòng:** Quản lý vắc-xin, liều lượng, gói tiêm theo tháng kèm ưu đãi giảm giá
- **Mua hàng:** Quản lý sản phẩm (thức ăn, thuốc, phụ kiện), tồn kho

### Quản lý vận hành
- Lập hóa đơn, tính khuyến mãi và điểm tích lũy
- Quản lý nhân sự (bác sĩ thú y, nhân viên bán hàng, tiếp tân, quản lý chi nhánh)
- Lịch sử điều động nhân viên giữa các chi nhánh
- Đánh giá chất lượng dịch vụ từ khách hàng

### Báo cáo & thống kê
- Doanh thu theo ngày/tháng/quý/năm, theo chi nhánh và toàn hệ thống
- Thống kê vắc-xin được đặt nhiều nhất, dịch vụ mang lại doanh thu cao nhất
- Thống kê thú cưng theo loài/giống, tình hình hội viên theo hạng
- Hiệu suất nhân viên, tra cứu lịch sử khám/tiêm chủng

## Công nghệ sử dụng

- **Ngôn ngữ:** C#
- **Giao diện:** WinForms
- **Cơ sở dữ liệu:** SQL Server
- **Tối ưu hiệu suất:** Index, Partitioning, phân tích tần suất truy vấn

## Vai trò người dùng

Hệ thống hỗ trợ 3 vai trò chính:
- **Khách hàng:** Đặt lịch khám, tiêm phòng, mua hàng, xem lịch sử, đánh giá dịch vụ
- **Bác sĩ:** Quản lý lịch khám, cập nhật chẩn đoán và toa thuốc
- **Quản trị:** Quản lý nhân sự, chi nhánh, sản phẩm, xem báo cáo thống kê

## Hướng dẫn cài đặt & chạy dự án

### Yêu cầu
- Visual Studio (2019 trở lên)
- SQL Server đã được cài đặt và cấu hình sẵn
- Kết nối Internet (để tải NuGet packages lần đầu)

### Các bước thực hiện

1. **Clone repository về máy:**
```bash
git clone https://github.com/BachKhaVan/PetCareX.git
```

2. **Mở project bằng Visual Studio:**
   - Mở file `PETCAREX.slnx`

3. **Restore NuGet Packages:**
   Chuột phải vào **Solution** trong Solution Explorer → **Restore NuGet Packages**.
   Nếu không tự chạy, vào **Tools → NuGet Package Manager → Package Manager Console**, gõ:
   ```powershell
   Update-Package -reinstall
   ```

4. **Cấu hình kết nối SQL Server:**
   - Mở file `App.config`
   - Chỉnh sửa connection string phù hợp với SQL Server trên máy bạn (server name, tên database, thông tin đăng nhập)

5. **Chạy script tạo cơ sở dữ liệu** (nếu có script `.sql` đính kèm trong repo) trên SQL Server trước khi chạy ứng dụng.

6. **Build và chạy:**
   - `Ctrl+Shift+B` để build
   - `F5` để chạy ứng dụng
