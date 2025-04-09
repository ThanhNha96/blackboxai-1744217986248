# Giả Lập Trộm Chó (Dog Theft Simulator)

## Yêu Cầu Hệ Thống

- Unity 2022.3 LTS hoặc mới hơn
- Visual Studio 2019/2022 hoặc VS Code với Unity extension
- Git (để clone repository)

## Các Bước Cài Đặt

1. Clone Repository:
```bash
git clone https://github.com/your-username/dog-theft-simulator.git
cd dog-theft-simulator
```

2. Cài Đặt Unity Hub:
- Tải Unity Hub từ: https://unity.com/download
- Cài đặt Unity Hub
- Cài đặt Unity 2022.3 LTS thông qua Unity Hub

3. Cài Đặt Các Package Cần Thiết:
- TextMeshPro
- Cinemachine
- Input System
- Universal Render Pipeline
- Post Processing

4. Mở Project:
- Mở Unity Hub
- Chọn "Add" và chọn thư mục project
- Chờ Unity import toàn bộ assets

## Build Game

### Build cho Windows:
1. Mở Unity Editor
2. File > Build Settings
3. Chọn Platform: PC, Mac & Linux Standalone
4. Target Platform: Windows
5. Architecture: x86_64
6. Chọn "Build" hoặc "Build And Run"

### Build cho Mac:
1. Mở Unity Editor
2. File > Build Settings
3. Chọn Platform: PC, Mac & Linux Standalone
4. Target Platform: macOS
5. Architecture: Universal
6. Chọn "Build" hoặc "Build And Run"

### Build cho Android:
1. Cài đặt Android Studio và Android SDK
2. Unity Editor > Edit > Preferences > External Tools
3. Cấu hình SDK, JDK
4. File > Build Settings
5. Switch Platform sang Android
6. Player Settings:
   - Package name: com.yourstudio.dogtheft
   - Minimum API Level: Android 7.0
   - Target API Level: Latest
7. Chọn "Build" hoặc "Build And Run"

## Cấu Trúc Project

```
GameProject/
├── Scripts/                 # Mã nguồn C#
│   ├── Combat/             # Hệ thống chiến đấu
│   ├── Character/          # Hệ thống nhân vật
│   ├── Police/             # AI cảnh sát
│   ├── Quest/              # Nhiệm vụ
│   ├── Shop/               # Hệ thống cửa hàng
│   ├── UI/                 # Giao diện người dùng
│   └── Environment/        # Môi trường game
├── Prefabs/                # Các prefab
├── Scenes/                 # Các scene
└── ProjectSettings/        # Cấu hình project
```

## Các Scene Chính

1. MainMenu.unity: Màn hình chính
2. GameScene.unity: Scene chơi game chính

## Cấu Hình Build

1. Thêm scenes vào Build Settings theo thứ tự:
   - MainMenu
   - GameScene

2. Player Settings cần thiết:
   - Company Name: Your Studio
   - Product Name: Giả Lập Trộm Chó
   - Version: 1.0
   - Default Icon: Thêm icon game
   - Splash Image: Thêm logo studio

3. Quality Settings:
   - Đặt Preset mặc định
   - Cấu hình đồ họa cho từng platform

## Tối Ưu Hóa

1. Nén Texture:
   - Windows/Mac: Normal quality
   - Android: Compressed

2. Audio Settings:
   - Format: Vorbis
   - Quality: 70%
   - Load Type: Streaming

3. Lighting:
   - Mixed Lighting
   - Bake lighting maps

## Kiểm Tra Trước Khi Build

1. Xóa các debug logs
2. Kiểm tra các scene trong build
3. Kiểm tra các reference
4. Test performance
5. Chạy thử các tính năng chính

## Phát Hành

1. Tạo installer cho Windows/Mac
2. Đóng gói APK cho Android
3. Tạo trang web download
4. Viết hướng dẫn cài đặt

## Hỗ Trợ

Nếu gặp vấn đề khi build:
1. Kiểm tra Unity Console
2. Xem logs trong Editor.log
3. Kiểm tra Build Report
4. Liên hệ support qua Discord/Email

## Credits

Game được phát triển bởi [Your Studio]
Version: 1.0
Copyright © 2024
