# 📰 FUNews Management System — Assignment 01 (ASP.NET Core MVC)

> **PRN222 Assignment 01** — Hệ thống quản lý tin tức trường đại học xây dựng bằng **ASP.NET Core MVC**, Entity Framework Core, SignalR và 3-Layer Architecture.

---

## 🗂️ Mục lục

- [Thông tin dự án](#thông-tin-dự-án)
- [Công nghệ sử dụng](#công-nghệ-sử-dụng)
- [Cài đặt & Chạy dự án](#cài-đặt--chạy-dự-án)
- [Tài khoản mặc định](#tài-khoản-mặc-định)
- [Luồng test từng tính năng](#luồng-test-từng-tính-năng)
  - [1. Xác thực (Auth)](#1-xác-thực-auth)
  - [2. Trang tin tức công khai (News Index)](#2-trang-tin-tức-công-khai-news-index)
  - [3. Chi tiết bài viết (News Details)](#3-chi-tiết-bài-viết-news-details)
  - [4. Quản lý bài viết - Staff (News Manage)](#4-quản-lý-bài-viết---staff-news-manage)
  - [5. Lịch sử bài viết (News History)](#5-lịch-sử-bài-viết-news-history)
  - [6. Bản nháp (Drafts)](#6-bản-nháp-drafts)
  - [7. Thư viện cá nhân (Library)](#7-thư-viện-cá-nhân-library)
  - [8. Quản lý danh mục - Staff (Categories)](#8-quản-lý-danh-mục---staff-categories)
  - [9. Quản lý thẻ - Staff (Tags)](#9-quản-lý-thẻ---staff-tags)
  - [10. Quản lý tài khoản - Admin (Accounts)](#10-quản-lý-tài-khoản---admin-accounts)
  - [11. Báo cáo - Admin (Report)](#11-báo-cáo---admin-report)
  - [12. Nhật ký hoạt động - Admin (Audit Logs)](#12-nhật-ký-hoạt-động---admin-audit-logs)
  - [13. Dashboard](#13-dashboard)
  - [14. Hồ sơ cá nhân (Profile)](#14-hồ-sơ-cá-nhân-profile)
  - [15. Thông báo (Notifications)](#15-thông-báo-notifications)

---

## Thông tin dự án

| Mục | Nội dung |
|---|---|
| **Tên dự án** | FUNews Management System |
| **Assignment** | PRN222 — Assignment 01 |
| **Kiến trúc** | ASP.NET Core MVC + 3-Layer (Controller → Service → Repository) |
| **Pattern** | Repository Pattern, Dependency Injection, Singleton |
| **Database** | SQL Server — `FUNewsManagement` |
| **ORM** | Entity Framework Core 8 |
| **Real-time** | SignalR (thông báo và cập nhật live) |

---

## Công nghệ sử dụng

- **Backend:** ASP.NET Core 8 MVC, C# 12
- **ORM:** Entity Framework Core 8 (Code First với Migration)
- **Real-time:** SignalR
- **Frontend:** Bootstrap 5, Vanilla CSS (Design System riêng), Google Fonts (Inter + Newsreader)
- **Database:** SQL Server 2019+
- **Auth:** Session-based Authentication (không dùng ASP.NET Identity)

---

## Cài đặt & Chạy dự án

### Yêu cầu hệ thống
- .NET 8 SDK
- SQL Server 2019+ (hoặc SQL Server Express / LocalDB)
- Visual Studio 2022 hoặc VS Code

### Bước 1 — Clone & mở dự án
```bash
# Mở solution
cd NguyenCongNhat_SE19BNET_A01/NguyenCongNhatMVC
```

### Bước 2 — Cấu hình database
Mở `appsettings.json` và cập nhật connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=FUNewsManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AdminAccount": {
    "Email": "admin@FUNewsManagementSystem.org",
    "Password": "@@abc123@@"
  }
}
```

### Bước 3 — Tạo database
**Cách 1:** Chạy file SQL có sẵn:
```
FUNewsManagement_FullSetup.sql
```

**Cách 2:** EF Core Migration:
```bash
dotnet ef database update
```

### Bước 4 — Chạy ứng dụng
```bash
dotnet run
# hoặc nhấn F5 trong Visual Studio
```

Ứng dụng chạy tại: `https://localhost:7xxx` hoặc `http://localhost:5xxx`

---

## Tài khoản mặc định

| Vai trò | Email | Mật khẩu | Quyền |
|---|---|---|---|
| **Admin** | `admin@FUNewsManagementSystem.org` | `@@abc123@@` | Quản lý tài khoản, xem báo cáo, audit logs |
| **Staff** | `IsabellaDavid@FUNewsManagement.org` | `@@abc123@@` | CRUD bài viết, danh mục, thẻ |
| **Staff** | `MichaelCharlotte@FUNewsManagement.org` | `@@abc123@@` | CRUD bài viết, danh mục, thẻ |
| **Lecturer** | `EmmaWilliam@FUNewsManagement.org` | `@@abc123@@` | Xem, like, bookmark, comment |
| **Lecturer** | `OliviaJames@FUNewsManagement.org` | `@@abc123@@` | Xem, like, bookmark, comment |

> **Lưu ý:** Admin được cấu hình trong `appsettings.json`, không lưu trong database.

---

## Luồng test từng tính năng

---

### 1. Xác thực (Auth)

**URL:** `/Auth/Login` | `/Auth/Register` | `/Auth/Logout`

#### ✅ Test Login thành công
1. Truy cập `/Auth/Login`
2. Nhập Email: `IsabellaDavid@FUNewsManagement.org` / Password: `@@abc123@@`
3. Nhấn **Sign In**
4. **Kết quả mong đợi:** Chuyển đến `/News/Index`, navbar hiển thị tên và role "Staff"

#### ✅ Test Login Admin
1. Nhập Email: `admin@FUNewsManagementSystem.org` / Password: `@@abc123@@`
2. **Kết quả mong đợi:** Navbar hiển thị menu Admin (Accounts, Report, Audit Logs)

#### ❌ Test Login sai mật khẩu
1. Nhập đúng email, sai password
2. **Kết quả mong đợi:** Hiển thị thông báo lỗi "Invalid email or password"

#### ✅ Test Register
1. Truy cập `/Auth/Register`
2. Điền đầy đủ: Name, Email (chưa tồn tại), Password (≥6 ký tự), Role
3. **Kết quả mong đợi:** Tài khoản được tạo, tự động đăng nhập

#### ✅ Test Logout
1. Khi đang đăng nhập, click avatar → **Sign out**
2. **Kết quả mong đợi:** Xóa session, chuyển về `/Auth/Login`

---

### 2. Trang tin tức công khai (News Index)

**URL:** `/News/Index`

#### ✅ Test xem danh sách bài viết (chưa đăng nhập)
1. Truy cập `/News/Index` khi chưa đăng nhập
2. **Kết quả mong đợi:**
   - Hiển thị Hero section với search bar
   - Bài viết featured (lớn nhất) hiển thị đầu tiên
   - Lưới bài viết (news-grid) bên dưới
   - Ảnh bài viết hiển thị đúng; ảnh null → placeholder SVG

#### ✅ Test tìm kiếm
1. Gõ từ khóa vào thanh tìm kiếm (ví dụ: "AI", "alumni")
2. Nhấn **Search**
3. **Kết quả mong đợi:** Danh sách lọc theo keyword trong title và headline

#### ✅ Test lọc theo Category
1. Chọn một danh mục từ dropdown "Category"
2. **Kết quả mong đợi:** Chỉ hiển thị bài trong danh mục đó

#### ✅ Test lọc theo Tag
1. Chọn tag từ dropdown "Tag"
2. **Kết quả mong đợi:** Chỉ hiển thị bài có tag đó

#### ✅ Test placeholder ảnh
1. Tạo bài viết không có ImageUrl (xem mục #4)
2. Quay lại News Index
3. **Kết quả mong đợi:** Hiển thị ảnh placeholder SVG màu xám thay vì khoảng trống

#### ✅ Test Admin view
1. Đăng nhập Admin → `/News/Index`
2. **Kết quả mong đợi:** Hiển thị dạng danh sách với filter panel và workflow state

---

### 3. Chi tiết bài viết (News Details)

**URL:** `/News/Details/{id}`

#### ✅ Test xem chi tiết
1. Click vào bất kỳ bài viết nào
2. **Kết quả mong đợi:** Hiển thị đầy đủ: tiêu đề, ảnh (nếu có), nội dung, tags, author, ngày đăng

#### ✅ Test Like bài viết (cần đăng nhập)
1. Đăng nhập bất kỳ tài khoản
2. Vào chi tiết bài viết
3. Click nút **Like** (icon tim)
4. **Kết quả mong đợi:** Icon chuyển màu xanh, số like tăng 1

#### ✅ Test Unlike
1. Click lại nút Like đã active
2. **Kết quả mong đợi:** Icon trở về màu xám, số like giảm 1

#### ✅ Test Bookmark
1. Click icon **Bookmark**
2. **Kết quả mong đợi:** Icon bookmark chuyển màu xanh, lưu vào Library

#### ✅ Test Comment
1. Gõ nội dung vào ô comment
2. Nhấn **Post**
3. **Kết quả mong đợi:** Comment hiển thị ngay bên dưới với tên và thời gian

#### ✅ Test Reply comment
1. Click **Reply** dưới một comment
2. Gõ nội dung reply
3. **Kết quả mong đợi:** Reply hiển thị thụt lề bên dưới comment gốc

#### ✅ Test Edit/Delete comment của mình
1. Hover vào comment của chính mình → icon `⋯`
2. Chọn **Edit** → sửa nội dung → **Save**
3. Chọn **Delete** → xác nhận
4. **Kết quả mong đợi:** Comment được cập nhật hoặc bị ẩn (IsDeleted = true)

#### ❌ Test không thể edit comment của người khác
1. Hover vào comment của người khác
2. **Kết quả mong đợi:** Không hiển thị menu edit/delete

---

### 4. Quản lý bài viết — Staff (News Manage)

**URL:** `/News/Manage`
**Yêu cầu:** Đăng nhập tài khoản **Staff**

#### ✅ Test tạo bài viết mới
1. Đăng nhập Staff → `/News/Manage`
2. Click **+ Create**
3. Điền đầy đủ:
   - **Title:** "Test Article 01"
   - **Headline:** "Short summary"
   - **Category:** chọn bất kỳ
   - **Content:** Nội dung bài viết
   - **Image URL:** `https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=450&fit=crop`
   - **Workflow State:** Published
   - **Tags:** chọn 1-3 tags
4. Nhấn **Save Article**
5. **Kết quả mong đợi:** Bài xuất hiện trong danh sách, SignalR thông báo cho các tab khác

#### ✅ Test chỉnh sửa bài viết
1. Click **Edit** trên bài viết muốn sửa
2. Modal mở ra với dữ liệu đã điền sẵn
3. Sửa tiêu đề → **Save Changes**
4. **Kết quả mong đợi:** Bài viết cập nhật, danh sách refresh qua SignalR

#### ✅ Test xóa bài viết
1. Click **Delete** trên bài viết
2. Xác nhận dialog "Delete this article?"
3. **Kết quả mong đợi:** Bài viết bị xóa khỏi danh sách

#### ✅ Test Publish / Unpublish
1. Click **Publish** để kích hoạt bài
2. Click **Unpublish** để ẩn bài
3. **Kết quả mong đợi:** Trạng thái Active/Inactive thay đổi

#### ✅ Test tìm kiếm bài viết
1. Gõ keyword vào ô tìm kiếm
2. Nhấn **Search**
3. **Kết quả mong đợi:** Danh sách lọc theo từ khóa

#### ✅ Test SignalR real-time
1. Mở 2 tab trình duyệt cùng `/News/Manage`
2. Tạo/sửa bài ở tab 1
3. **Kết quả mong đợi:** Tab 2 tự động reload và hiển thị dữ liệu mới

#### ❌ Test Staff không truy cập được Accounts
1. Đăng nhập Staff → truy cập `/Accounts/Index`
2. **Kết quả mong đợi:** Redirect về `/News/Index`

---

### 5. Lịch sử bài viết (News History)

**URL:** `/News/History`
**Yêu cầu:** Đăng nhập **Staff**

#### ✅ Test xem lịch sử bài viết của mình
1. Đăng nhập Staff → `/News/History`
2. **Kết quả mong đợi:** Hiển thị danh sách tất cả bài viết đã tạo bởi tài khoản này, kèm ngày sửa và trạng thái

---

### 6. Bản nháp (Drafts)

**URL:** `/News/Drafts`
**Yêu cầu:** Đăng nhập **Staff**

#### ✅ Test xem danh sách bản nháp
1. Đăng nhập Staff → `/News/Drafts`
2. **Kết quả mong đợi:** Hiển thị các bài có Workflow State = Draft hoặc Pending Review

---

### 7. Thư viện cá nhân (Library)

**URL:** `/News/Library`
**Yêu cầu:** Đăng nhập bất kỳ tài khoản

#### ✅ Test xem bài đã bookmark
1. Bookmark một số bài viết (xem mục #3)
2. Click **Library** trên navbar
3. **Kết quả mong đợi:** Hiển thị danh sách bài đã bookmark

#### ✅ Test xem bài đã like
1. Vào Library → tab **Liked**
2. **Kết quả mong đợi:** Danh sách bài đã like

---

### 8. Quản lý danh mục — Staff (Categories)

**URL:** `/Categories/Index`
**Yêu cầu:** Đăng nhập **Staff**

#### ✅ Test tạo danh mục mới
1. Click **+ Create**
2. Điền:
   - **Name:** "Test Category"
   - **Description:** Mô tả
   - **Parent:** None (Root)
   - **Active:** checked
3. **Save Category**
4. **Kết quả mong đợi:** Danh mục xuất hiện trong danh sách

#### ✅ Test chỉnh sửa
1. Click **Edit** → modal mở, dữ liệu điền sẵn
2. Sửa tên → **Save Changes**

#### ✅ Test xóa danh mục không có bài viết
1. Tạo danh mục mới (chưa có bài)
2. Click **Delete** → xác nhận
3. **Kết quả mong đợi:** Xóa thành công

#### ❌ Test xóa danh mục có bài viết
1. Click **Delete** trên danh mục đang được dùng bởi bài viết
2. **Kết quả mong đợi:** Thông báo lỗi "Cannot delete a category that is used by news articles"

#### ✅ Test tìm kiếm
1. Gõ keyword → nhấn **Search**
2. **Kết quả mong đợi:** Lọc theo tên danh mục

---

### 9. Quản lý thẻ — Staff (Tags)

**URL:** `/Tags/Index`
**Yêu cầu:** Đăng nhập **Staff**

#### ✅ Test tạo tag mới
1. Click **+ Create**
2. Điền **Tag Name:** "TestTag", **Note:** tuỳ chọn
3. **Save Tag**
4. **Kết quả mong đợi:** Tag xuất hiện trong danh sách với số bài = 0

#### ✅ Test chỉnh sửa / Xóa tag
1. Edit → sửa tên → Save
2. Delete trên tag chưa dùng → xác nhận → xóa thành công

#### ❌ Test xóa tag đang được dùng
1. Delete tag đang gắn với bài viết
2. **Kết quả mong đợi:** Thông báo "Cannot delete a tag that is currently in use"

---

### 10. Quản lý tài khoản — Admin (Accounts)

**URL:** `/Accounts/Index`
**Yêu cầu:** Đăng nhập **Admin**

#### ✅ Test xem danh sách tài khoản
1. Đăng nhập Admin → `/Accounts/Index`
2. **Kết quả mong đợi:** Danh sách tất cả tài khoản trong hệ thống (không bao gồm Admin)

#### ✅ Test tạo tài khoản mới
1. Click **+ Create**
2. Điền:
   - **Full Name:** "New User"
   - **Email:** `newuser@fu.edu.vn`
   - **Role:** Staff (1) hoặc Lecturer (2)
   - **Password:** `@@abc123@@`
3. **Save Account**
4. **Kết quả mong đợi:** Tài khoản mới xuất hiện trong danh sách

#### ✅ Test chỉnh sửa tài khoản
1. Click **Edit** → modal mở, field Password để trống nếu không muốn đổi
2. Sửa tên → **Save Changes**

#### ❌ Test xóa tài khoản có bài viết
1. Click **Delete** trên tài khoản đã tạo/cập nhật bài
2. **Kết quả mong đợi:** Lỗi "Cannot delete an account that is attached to news articles"

#### ✅ Test xóa tài khoản mới tạo (không có bài)
1. Tạo tài khoản mới (bước trên)
2. Delete ngay → xác nhận
3. **Kết quả mong đợi:** Xóa thành công

#### ✅ Test tìm kiếm tài khoản
1. Gõ tên hoặc email vào search → **Search**

#### ❌ Test Staff không truy cập được Accounts
1. Đăng nhập Staff → truy cập `/Accounts/Index`
2. **Kết quả mong đợi:** Redirect về `/News/Index`

---

### 11. Báo cáo — Admin (Report)

**URL:** `/Accounts/Report`
**Yêu cầu:** Đăng nhập **Admin**

#### ✅ Test xem báo cáo toàn bộ
1. Đăng nhập Admin → `/Accounts/Report`
2. Để trống ngày → nhấn **Generate**
3. **Kết quả mong đợi:** Hiển thị tất cả bài viết, sắp xếp theo `CreatedDate DESC`

#### ✅ Test lọc theo khoảng thời gian
1. Chọn **Start Date:** `2024-05-01` và **End Date:** `2024-05-15`
2. Nhấn **Generate**
3. **Kết quả mong đợi:** Chỉ hiển thị bài tạo trong khoảng đó

#### ✅ Test xuất CSV
1. Sau khi có kết quả → nhấn **Export CSV**
2. **Kết quả mong đợi:** Tải về file `.csv` với các cột: Title, Category, CreatedBy, CreatedDate, Status

---

### 12. Nhật ký hoạt động — Admin (Audit Logs)

**URL:** `/AuditLogs/Index`
**Yêu cầu:** Đăng nhập **Admin**

#### ✅ Test xem audit logs
1. Thực hiện một số thao tác (tạo/sửa/xóa bài viết)
2. Đăng nhập Admin → `/AuditLogs/Index`
3. **Kết quả mong đợi:** Hiển thị log với: Actor Email, Role, Action, Entity, Timestamp

---

### 13. Dashboard

**URL:** `/Dashboard/Index`
**Yêu cầu:** Đăng nhập **Admin** hoặc **Staff**

#### ✅ Test xem Dashboard
1. Đăng nhập Admin hoặc Staff → click **Dashboard**
2. **Kết quả mong đợi:**
   - Thống kê: tổng bài viết, danh mục, tags, tài khoản
   - Bài viết mới nhất
   - Hoạt động gần đây (Audit Logs)

---

### 14. Hồ sơ cá nhân (Profile)

**URL:** `/Profile/Index`
**Yêu cầu:** Đăng nhập bất kỳ tài khoản

#### ✅ Test xem Profile
1. Click Avatar trên navbar → **Profile**
2. **Kết quả mong đợi:** Hiển thị thông tin: tên, email, role, avatar, bio

#### ✅ Test cập nhật thông tin
1. Sửa tên, số điện thoại, bio
2. Nhấn **Save Changes**
3. **Kết quả mong đợi:** Thông tin được cập nhật

#### ✅ Test đổi mật khẩu
1. Điền **Current Password**, **New Password**, **Confirm Password**
2. Nhấn **Change Password**
3. **Kết quả mong đợi:** Mật khẩu đổi thành công, đăng xuất và yêu cầu đăng nhập lại

#### ✅ Test upload avatar
1. Click **Change Avatar** → chọn ảnh (jpg/png < 2MB)
2. **Kết quả mong đợi:** Avatar mới hiển thị trên navbar

---

### 15. Thông báo (Notifications)

**URL:** Icon chuông 🔔 trên Navbar

#### ✅ Test nhận thông báo
1. Đăng nhập Staff A (tác giả bài viết)
2. Đăng nhập Staff B (trên tab khác) → Like/Comment bài của A
3. Quay lại tab Staff A
4. **Kết quả mong đợi:** Icon chuông có badge đỏ (số thông báo chưa đọc)

#### ✅ Test xem dropdown thông báo
1. Click icon chuông 🔔
2. **Kết quả mong đợi:** Dropdown hiện danh sách thông báo kiểu Facebook (like ❤️, bookmark 🔖, comment 💬)

#### ✅ Test Mark all as read
1. Click **Mark all read** trong dropdown
2. **Kết quả mong đợi:** Badge đỏ biến mất, tất cả thông báo đánh dấu đã đọc

#### ✅ Test trang Notifications đầy đủ
1. Click **See all notifications** trong dropdown
2. **Kết quả mong đợi:** Trang hiển thị toàn bộ lịch sử thông báo

---

## 🏗️ Kiến trúc hệ thống

```
NguyenCongNhatMVC/
├── Controllers/          # Tầng điều khiển (MVC Controller)
│   ├── AuthController.cs
│   ├── NewsController.cs
│   ├── AccountsController.cs
│   ├── CategoriesController.cs
│   ├── TagsController.cs
│   ├── DashboardController.cs
│   ├── ProfileController.cs
│   └── NotificationsController.cs
├── Models/               # Domain models & ViewModels
├── Data/                 # DbContext (Entity Framework Core)
├── Repositories/         # Tầng truy xuất dữ liệu
│   ├── Interfaces/       # IRepository interfaces
│   └── Implementations/  # Concrete implementations
├── Services/             # Tầng nghiệp vụ (Business Logic)
│   ├── Interfaces/       # IService interfaces
│   └── Implementations/  # Service implementations
├── Hubs/                 # SignalR Hub
│   └── NewsHub.cs
├── Views/                # Razor Views (MVC)
└── wwwroot/              # Static files (CSS, JS, images)
```

---

## 📌 Ghi chú quan trọng

- Ứng dụng **mặc định chuyển hướng** về `/Auth/Login` khi chưa đăng nhập
- **Admin account** được cấu hình trong `appsettings.json`, không lưu trong database
- **SignalR** kết nối qua `/newsHub` — khi có thay đổi bài viết, tất cả client tự động reload
- **Ảnh placeholder** (`/images/news-placeholder.svg`) hiển thị khi `ImageUrl` null hoặc bị lỗi
- **Phân quyền:**
  - `Admin`: Accounts, Report, AuditLogs, Dashboard
  - `Staff`: Categories, Tags, News Manage, History, Drafts, Dashboard
  - `Lecturer/Reader`: Chỉ xem, like, comment, bookmark
"# FUNewsManagementSystem_NguyenCongNhat_SE19BNET_A01" 
