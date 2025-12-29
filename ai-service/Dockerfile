# 1. Quay lại dùng bản python slim cho nhẹ hệ điều hành
FROM python:3.11-slim

WORKDIR /app

# 2. Cài đặt các công cụ biên dịch cơ bản (đôi khi cần cho một số thư viện python)
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    && rm -rf /var/lib/apt/lists/*

# 3. BƯỚC QUAN TRỌNG NHẤT: Cài Torch CPU TẠI ĐÂY
# Dùng --index-url trỏ vào kho CPU để nó chỉ tải bản ~200MB
RUN pip install --no-cache-dir torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cpu

# 4. Copy file requirements
COPY requirements.txt .

# 5. Cài các thư viện còn lại
# LƯU Ý: Trong requirements.txt PHẢI XÓA dòng 'torch' đi nhé!
RUN pip install --no-cache-dir -r requirements.txt

# 6. Copy code và chạy
COPY . .
EXPOSE 8000
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]