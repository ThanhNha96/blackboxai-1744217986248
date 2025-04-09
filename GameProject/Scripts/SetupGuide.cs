using UnityEngine;

public class SetupGuide : MonoBehaviour
{
    /*
    HƯỚNG DẪN CÀI ĐẶT VÀ BUILD GAME "GIẢ LẬP TRỘM CHÓ"
    
    A. SETUP MÔI TRƯỜNG:
    
    1. Cài đặt phần mềm:
       - Unity Hub: https://unity.com/download
       - Unity 2022.3 LTS
       - Visual Studio 2022 Community hoặc VS Code
       - Git
    
    2. Cài đặt Unity packages (Window > Package Manager):
       - TextMeshPro (UI)
       - Cinemachine (Camera)
       - Input System (Điều khiển)
       - Universal RP (Đồ họa)
       - Post Processing (Hiệu ứng)
       - DOTween (Animation)
    
    3. Cấu hình project:
       - Edit > Project Settings > Player:
         * Company Name: Your Studio
         * Product Name: Giả Lập Trộm Chó
         * Version: 1.0
       
       - Edit > Project Settings > Quality:
         * Set Ultra cho PC
         * Set Medium cho Mobile
       
       - Edit > Project Settings > Input:
         * Thêm cấu hình chuột
         * Thêm phím Space
    
    B. BUILD GAME:
    
    1. Chuẩn bị scenes:
       - Scenes cần có:
         * MainMenu
         * GameScene
       
       - File > Build Settings:
         * Thêm scenes theo thứ tự
         * MainMenu index 0
         * GameScene index 1
    
    2. Build cho Windows:
       - File > Build Settings
       - Platform: PC, Mac & Linux
       - Target: Windows x64
       - Development Build (tùy chọn)
       - Compression Method: LZ4
       
       Build Steps:
       1. Switch Platform to Windows
       2. Bấm Player Settings kiểm tra cấu hình
       3. Bấm Build
       4. Chọn thư mục output
       5. Đợi build hoàn tất
    
    3. Build cho Android:
       - Cài Android SDK, JDK
       - Switch Platform to Android
       - Player Settings:
         * Package Name
         * Version
         * Min API 24 (Android 7.0)
       - Build APK hoặc AAB
    
    C. KIỂM TRA TRƯỚC KHI BUILD:
    
    1. Scene Setup:
       - Kiểm tra Prefabs:
         * Player
         * NPCs
         * UI Elements
         * Effects
       
       - Kiểm tra References:
         * Không có Missing Scripts
         * Không có null references
         * Audio Sources có clips
         * Animations có clips
    
    2. Code Review:
       - Xóa Debug.Log
       - Kiểm tra Exception handling
       - Tối ưu performance
       - Memory leaks
    
    3. Testing:
       - Test các tính năng chính:
         * Combat system
         * Quest system
         * Shop system
         * Police system
         * Save/Load
       
       - Test hiệu năng:
         * FPS ổn định
         * Memory usage
         * Loading times
    
    D. PHÁT HÀNH:
    
    1. Tạo bản build:
       - Release version
       - Không có debug features
       - Đã test kỹ
    
    2. Đóng gói:
       - Windows: Tạo installer
       - Android: Sign APK
       - Nén files
    
    3. Distribution:
       - Upload lên host
       - Tạo trang download
       - Viết changelog
    
    E. TROUBLESHOOTING:
    
    1. Lỗi build thường gặp:
       - Missing references
       - Scene không được add
       - SDK không đúng version
       - Package conflicts
    
    2. Giải quyết:
       - Xem Console log
       - Kiểm tra Build Report
       - Clean/Rebuild project
       - Reimport All
    
    F. OPTIMIZATION TIPS:
    
    1. Assets:
       - Compress textures
       - Optimize meshes
       - Audio compression
       - Atlas packing
    
    2. Code:
       - Use object pooling
       - Optimize Update calls
       - Batch rendering
       - Reduce garbage collection
    
    3. Build size:
       - Remove unused assets
       - Compress build
       - Strip debug symbols
    
    Contact support:
    - Discord: your-discord
    - Email: support@yourstudio.com
    */
}
