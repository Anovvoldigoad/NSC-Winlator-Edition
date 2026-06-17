# 📱 Upload ke GitHub dari HP - Step by Step

Panduan detail upload project ke GitHub langsung dari HP tanpa PC.

## 🎯 Yang Anda Butuhkan

- ✅ HP/Tablet
- ✅ Internet (WiFi atau mobile data)
- ✅ Browser (Chrome, Firefox, dll)
- ✅ GitHub account (daftar gratis)
- ✅ File project (dari archive)

## 📋 Step-by-Step Upload

### STEP 1: Login ke GitHub

1. Buka: **https://github.com**
2. Click "Sign in" (kanan atas)
3. Enter username dan password
4. Login

### STEP 2: Buat Repository Baru

1. Click **"+"** di kanan atas
2. Pilih **"New repository"**
3. Isi:
   - **Repository name**: `NSC-Winlator-Edition`
   - **Description**: `Mod Manager for Naruto Storm Connections`
   - **Public** ✓ (pilih public biar siapa saja bisa akses)
   - Jangan centang "Initialize with README" (kita upload nanti)

4. Click **"Create repository"**

### STEP 3: Upload Files (CARA 1 - Via Browser Upload)

**PALING MUDAH!**

1. Setelah membuat repo, halaman baru akan muncul
2. Click **"uploading an existing file"** (atau "Add files" → "Upload files")
3. File manager HP akan terbuka
4. Navigate ke folder yang sudah diekstrak
5. Pilih files:
   - **Bisa upload 1 file atau multiple files**
   - Tap file → pilih "Upload"
   - Atau drag-drop jika browser support

6. GitHub akan proses upload
7. Tunggu sampai semua selesai (lihat progress bar)
8. Click **"Commit changes"**

**Done!** Files sudah di GitHub!

### STEP 4: GitHub Actions Runs Automatically!

1. Buka tab **"Actions"** di repo
2. Lihat workflow **"Build NSC Winlator Edition"** sedang berjalan
3. Status akan berubah dari:
   - 🟡 **In Progress** → 🟢 **Completed**
   - Ini terjadi dalam 2-3 menit

4. Workflow akan:
   - Checkout code
   - Setup .NET 8
   - Restore dependencies
   - Build project
   - Publish executable
   - Upload artifact

### STEP 5: Download Build Artifact

1. Buka tab **"Actions"**
2. Click workflow yang paling baru
3. Scroll ke bawah → section **"Artifacts"**
4. Download: **NSC-Winlator-win-x64** (50-100 MB)
5. File akan download sebagai ZIP

### STEP 6: Extract dan Gunakan

1. Extract ZIP yang didownload
2. Isi akan ada:
   - `NSC.Winlator.exe` (main executable)
   - `*.dll` files (dependencies)
   - `.NET runtime files`

3. Folder ini bisa:
   - Dikirim ke PC via Telegram/Drive
   - Dijalankan di Winlator (Android)
   - Dibagikan ke orang lain

---

## 🔄 Cara Upload via Git (Jika Tahu Git)

Jika Anda sudah punya Termux + git:

```bash
# 1. Clone repo ke HP
git clone https://github.com/USERNAME/NSC-Winlator-Edition.git
cd NSC-Winlator-Edition

# 2. Copy files ke sini
# (gunakan file manager, drag-drop ke Termux folder)

# 3. Add dan commit
git add .
git commit -m "Initial commit"

# 4. Push ke GitHub
git push origin main
```

GitHub Actions langsung run!

---

## 📱 Tips untuk Upload di HP

### 1. Internet Stabil

- Gunakan WiFi jika available
- Jangan upload saat signal lemah
- Data lebih besar → lebih lama

### 2. Browser Tips

- Gunakan Chrome atau Firefox
- Jangan close browser saat upload
- Refresh page jika stuck

### 3. File Organization

Sebelum upload, pastikan:
```
Folder yang akan upload:
├── .github/
├── .gitignore
├── src/
├── Models/
├── Services/
├── Infrastructure/
├── Forms/
├── Program.cs
├── NSC.Winlator.csproj
├── README.md
└── (semua file lainnya)
```

### 4. Upload Strategy

**Opsi A: Upload semua sekaligus**
- Lebih cepat jika internet stabil
- Bisa fail jika disconnect

**Opsi B: Upload per folder**
```
1. Upload .github/ dulu
2. Upload src/ dengan subfolder
3. Upload docs
4. Upload root files
```

Lebih aman jika sering disconnect!

### 5. Kalau Gagal

- Refresh halaman
- Coba upload lagi
- Gunakan folder/batch yang lebih kecil
- Cek internet connection

---

## ✅ Checklist

- [ ] Download archive
- [ ] Extract di HP
- [ ] GitHub account ready
- [ ] Create new repo
- [ ] Upload files
- [ ] Wait for Actions (2-3 min)
- [ ] Download artifact
- [ ] Extract executable
- [ ] Ready to use!

---

## 🎯 Waktu Estimasi

| Step | Time |
|------|------|
| GitHub login | 1 min |
| Create repo | 1 min |
| Extract archive | 2 min |
| Upload files | 5-10 min |
| GitHub build | 2-3 min |
| Download artifact | 2-5 min |
| Extract artifact | 1 min |
| **TOTAL** | **15-20 min** |

---

## 🔐 Privacy & Security

- GitHub Public = anyone can see
- For private: GitHub → Settings → Private
- Free account: unlimited public repos
- Source code visible (tapi source code is open-source)

---

## 📊 Upload File Size Reference

| Component | Size |
|-----------|------|
| .github/ | <10 KB |
| src/ | ~90 KB |
| docs | ~100 KB |
| Total | ~200 KB |

Sangat kecil! Upload cepat! ⚡

---

## 🎁 Setelah Upload - Apa Bisa Dilakukan?

1. **Edit files**
   - Click file di GitHub
   - Click pencil (edit)
   - Make changes
   - Commit

2. **See build status**
   - Actions tab
   - Watch workflow run
   - See logs

3. **Download builds**
   - Every commit → builds
   - Every tag → releases
   - Always available

4. **Share with others**
   - Give them repo link
   - They can download
   - They can fork

---

## 💡 Pro Tips

### 1. Always Commit Messages
```
git commit -m "Fix bug in ModScanner"
git commit -m "Add new feature"
git commit -m "Update documentation"
```

Clear messages = easy to track!

### 2. Use Tags for Releases
```
git tag -a v1.0.0 -m "First release"
git push origin v1.0.0
```

Triggers release workflow!

### 3. Pull Request Practice
```
1. Create new branch
2. Make changes
3. Create PR
4. Review
5. Merge
```

Good for practice!

---

## 🆘 Troubleshooting

### File Upload Stuck
- Check internet
- Try smaller file
- Try different browser
- Use GitHub Desktop (desktop app)

### Workflow Not Running
- Check .github/workflows/ folder uploaded
- Check file names correct
- Wait 1 minute (sometimes delayed)
- Check Actions tab

### Build Failed
- Check logs in Actions
- Look for error message
- Fix in code
- Commit and push again

### Download Artifact Slow
- Check file size (50-100 MB)
- WiFi faster than mobile data
- Try again later if network busy

---

## 🎉 You're Done!

Congratulations! Anda sudah:
- ✅ Upload project ke GitHub
- ✅ Trigger automatic build
- ✅ Download executable
- ✅ Ready to use!

**All dari HP! Tanpa PC!** 📱

---

**Next Step:** Download artifact dan jalankan di Winlator atau PC! 🚀
