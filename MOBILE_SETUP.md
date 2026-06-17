# 📱 NSC Winlator Edition - Setup untuk HP/Mobile

Panduan lengkap untuk setup dan build di HP tanpa PC.

## 🎯 Opsi untuk HP

Ada 3 cara untuk build di HP:

### ✅ Opsi 1: Termux (Android - RECOMMENDED)
- Gratis
- Tidak perlu root
- Full .NET 8 support
- Paling powerful

### ✅ Opsi 2: Online Compiler
- Tidak perlu install
- Cloud-based
- Instant
- Terbatas storage

### ✅ Opsi 3: GitHub Web Editor
- Di browser
- Edit & push
- GitHub Actions build otomatis
- Tidak perlu terminal

---

## 🔧 OPSI 1: Termux (TERBAIK)

### Step 1: Install Termux

1. Buka **Google Play Store**
2. Search: "Termux"
3. Install (developer: Fredrik Fornwall)
4. Buka Termux

### Step 2: Setup Terminal

Paste command ini di Termux:

```bash
# Update package manager
pkg update && pkg upgrade -y

# Install required packages
pkg install -y git wget unzip

# Install .NET 8 SDK
pkg install -y dotnet-runtime dotnet-sdk
```

(Takes ~5-10 menit, tergantung internet)

### Step 3: Download Project

```bash
# Create directory
mkdir ~/projects
cd ~/projects

# Download dan extract
wget https://github.com/YOUR_USERNAME/NSC-Winlator-Edition/archive/main.zip
unzip main.zip
cd NSC-Winlator-Edition-main
```

**Atau clone via git:**

```bash
git clone https://github.com/YOUR_USERNAME/NSC-Winlator-Edition.git
cd NSC-Winlator-Edition
```

### Step 4: Build

```bash
cd src/NSC.Winlator
dotnet restore
dotnet build -c Release
```

**Output:** `bin/Release/net8.0-windows/NSC.Winlator.exe`

### Step 5: Transfer ke PC (atau Winlator)

```bash
# Lihat lokasi file
ls -la bin/Release/net8.0-windows/

# Copy ke folder shared (dapat di akses dari PC)
cp -r bin/Release/net8.0-windows ~/Download/
```

Transfer via:
- Google Drive
- OneDrive
- Telegram
- Email

---

## 💻 OPSI 2: Online Compiler

Tanpa install apapun, langsung code di browser!

### Replit (Gratis)

1. Buka https://replit.com
2. Sign up (GitHub atau email)
3. Create new Repl → C# / .NET
4. Paste kode
5. Click "Run"

**Kelebihan:**
- Instant
- No installation
- Collaborative

**Kekurangan:**
- Storage terbatas
- Execution time terbatas
- Untuk testing saja

### Step-by-Step:

1. Go to https://replit.com/new/csharp
2. Delete template code
3. Copy-paste file-file dari project
4. Click "Run"

---

## 🌐 OPSI 3: GitHub Web Editor (TERMUDAH)

Langsung di browser, GitHub handle build!

### Step 1: Upload ke GitHub

```bash
# Di Termux:
cd ~/projects/NSC-Winlator-Edition
git init
git add .
git commit -m "Initial"
git remote add origin https://github.com/USERNAME/NSC-Winlator-Edition.git
git push -u origin main
```

### Step 2: Edit di GitHub Web

1. Buka GitHub repo di browser
2. Click file untuk edit
3. Click pencil icon (Edit)
4. Make changes
5. Commit

### Step 3: GitHub Actions Build Otomatis

1. Push changes
2. Go to **Actions** tab
3. Wait for build
4. Download artifact

**Tidak perlu terminal!** Semua via browser! 🎉

---

## 📱 Best Practices untuk Mobile

### 1. Gunakan File Manager
- Organize files dengan baik
- Buat backup
- Transfer via cloud

### 2. Version Control
```bash
# Check status
git status

# Commit changes
git add .
git commit -m "description"

# Push to GitHub
git push origin main
```

### 3. Text Editor for Mobile
Pilih satu:
- **Acode** - Best for coding (Play Store)
- **VS Code Online** - Web version
- **GitHub Codespaces** - Full IDE online

### 4. Terminal Tips
```bash
# Clear screen
clear

# List files
ls -la

# Navigate
cd folder_name

# Create directory
mkdir new_folder

# View file
cat filename.cs
```

---

## 🎯 Quick Workflow for Mobile

### If Using Termux:

```bash
# 1. Open Termux
termux

# 2. Navigate
cd ~/projects/NSC-Winlator-Edition

# 3. Update code
git pull origin main

# 4. Build
cd src/NSC.Winlator
dotnet build

# 5. View output
ls -la bin/Release/
```

### If Using GitHub Web:

1. Open browser → GitHub.com
2. Click repo
3. Click "." (opens web editor)
4. Edit files
5. Commit
6. GitHub Actions builds
7. Download from Artifacts

---

## 📁 File Organization on Mobile

```
Downloads/
├── NSC-Winlator-Edition/     ← Main project
│   ├── src/
│   │   └── NSC.Winlator/
│   ├── .github/
│   └── docs/
└── Builds/                    ← Output
    ├── v1.0.0/
    └── latest/
```

---

## 🚀 Fastest Way (5 Minutes)

### For GitHub Release Only:

1. **Download source** from GitHub as ZIP
2. **Extract** in HP
3. **View files** in file manager
4. **Upload to GitHub** via web
5. **GitHub Actions builds** automatically
6. **Download artifact** from GitHub
7. **Done!**

No coding needed if you just want binary!

---

## 🔧 Troubleshooting Mobile

### Termux Issues

**Problem:** Command not found
```bash
pkg install package_name
```

**Problem:** Storage full
```bash
# Check storage
df -h

# Clear cache
rm -rf ~/.cache/*
```

**Problem:** .NET not installing
```bash
# Try alternate method
pkg install -y clang make

# Or use online compiler instead
```

### Network Issues

- Use WiFi (more stable)
- Download smaller files first
- Break into parts
- Use Google Drive sync

---

## 💡 Mobile Development Tips

### 1. Use Lightweight Editor
- **Acode** - lightweight, fast
- **Quickedit** - simple interface
- **VS Code Web** - full IDE in browser

### 2. Organize Code
```
project/
├── Models/
├── Services/
├── Forms/
└── docs/
```

### 3. Git without Terminal
- GitHub Desktop (if available)
- GitKraken (mobile-friendly)
- GitHub Web Editor
- Gitpod

### 4. File Transfer
- Google Drive
- OneDrive
- Telegram
- Syncthing
- Tresorit

---

## ✅ Recommended Mobile Setup

### For Pure Mobile Users:

**Best Option: GitHub + Termux**

1. **Termux** for building locally
2. **GitHub Web** for easy editing
3. **GitHub Actions** for CI/CD
4. **GitHub Releases** for downloads

**Workflow:**
- Edit in GitHub Web → Commit
- Or edit in Termux → Push via git
- GitHub Actions auto-builds
- Download from Artifacts/Releases

### Minimal Setup:

Just need:
- Browser
- GitHub account
- Internet

That's it! GitHub handles everything!

---

## 🎓 Learning Path for Mobile

### If You Want to Learn Building:

1. Install Termux
2. Learn basic commands
3. Learn git
4. Learn .NET CLI
5. Build locally
6. Push to GitHub

### If You Just Want Binary:

1. Go to GitHub
2. Click Actions or Releases
3. Download executable
4. Use Winlator to run

---

## 📊 Comparison

| Method | Setup | Speed | Learning | Control |
|--------|-------|-------|----------|---------|
| Termux | Medium | Slow | High | Full |
| Online Compiler | Easy | Medium | Low | Limited |
| GitHub Web | Very Easy | Fast | None | Limited |
| GitHub Actions | Easy | Fast | Low | Medium |

---

## 🎉 Mobile Build Success!

After building:

1. File akan ada di:
   ```
   ~/projects/NSC-Winlator-Edition/src/NSC.Winlator/bin/Release/
   ```

2. Transfer ke PC:
   - Google Drive
   - OneDrive
   - Telegram
   - Email

3. Extract di PC

4. Run di Windows:
   ```
   NSC.Winlator.exe
   ```

5. Run di Winlator (Android):
   - Copy file ke Winlator
   - Run dari Winlator

---

## ⚠️ Important Notes

### Storage Requirements

- .NET SDK: ~500 MB
- Project: ~200 MB
- Build output: ~100 MB
- **Total: ~1 GB recommended**

### RAM Requirements

- Termux: 2 GB+
- Build: 1-2 GB RAM
- Running: 256 MB

### Network

- Download SDK: ~200 MB
- Clone repo: ~5 MB
- Upload release: ~50-100 MB

---

## 🆘 Support for Mobile

### Common Issues

**"pkg install failed"**
- Check internet
- Try smaller package
- Restart Termux

**"dotnet not found"**
- Reinstall: `pkg install dotnet-sdk`
- Check PATH: `echo $PATH`

**"No space left"**
- Clear cache: `rm -rf ~/.cache`
- Delete old builds
- Check storage: `df -h`

**"Build failed"**
- Check error message
- Update packages: `pkg update`
- Try offline if online compiler

---

## 🚀 Start Today!

Choose your method:
1. **Termux** - Full control
2. **GitHub Web** - Easiest
3. **Online Compiler** - Quickest

All work on mobile! 📱

---

**Happy mobile development!** 🎉

Anda bisa build & manage project langsung dari HP!
