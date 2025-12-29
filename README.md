🛠️ Hướng dẫn Cài đặt \& Chạy Dự án (Setup Guide)Tài liệu này hướng dẫn cách khởi chạy toàn bộ hệ thống local bao gồm: Infrastructure (Docker), AI Service (Python) và Backend (.NET).📋 Yêu cầu tiên quyết (Prerequisites)Docker Desktop (đã cài đặt và đang chạy)Python 3.xVisual Studio 2022 (hoặc IDE tương đương hỗ trợ .NET)🚀 Bước 1: Khởi tạo Infrastructure (Docker)Bạn cần chạy Qdrant (Vector DB) và Mailpit (Email testing tool). Mở terminal (CMD/PowerShell) và chạy lần lượt các lệnh sau:1. QdrantDịch vụ cơ sở dữ liệu vector cho AI.Bashdocker run -d --name qdrant -p 6333:6333 qdrant/qdrant:latest

2\. MailpitDịch vụ giả lập SMTP server để test gửi mail.Bashdocker run -d --name mailpit -p 1025:1025 -p 8025:8025 axllent/mailpit:latest

💡 Mẹo: Thay vì chạy lệnh lẻ tẻ, bạn có thể tạo file docker-compose.yml và chạy docker-compose up -d. (Xem file mẫu ở cuối tài liệu).🐍 Bước 2: Chạy AI Service (Python)Dịch vụ này chạy trực tiếp trên môi trường local (không qua Docker).Mở terminal và trỏ vào thư mục ai-service:Bashcd ai-service/

Cài đặt các thư viện cần thiết:Bashpip install -r requirements.txt

Khởi chạy server (Uvicorn):Bashuvicorn main:app --host 0.0.0.0 --port 8000

AI Service sẽ hoạt động tại: http://localhost:8000💻 Bước 3: Chạy Backend (Visual Studio)Mở Solution bằng Visual Studio.Đảm bảo Project Backend (API) được set là Startup Project.Nhấn F5 (hoặc nút Run) để khởi chạy Backend.🌐 Bước 4: Truy cập \& Kiểm traSau khi khởi chạy thành công, bạn có thể truy cập các công cụ quản trị qua đường dẫn sau:Dịch vụChức năngĐường dẫn (URL)MailpitXem danh sách email đã gửi (UI)http://localhost:8025/QdrantDashboard quản lý Vector DBhttp://localhost:6333/dashboardAI ServiceAPI Docs (nếu có Swagger)http://localhost:8000/docs

