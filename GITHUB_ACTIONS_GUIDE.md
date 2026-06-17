# ⚡ GitHub Actions - Quick Start Guide

Build otomatis sudah siap! Ikuti panduan ini untuk setup.

## 🚀 Quick Setup (5 Menit)

### 1. Push ke GitHub

```bash
# Inisialisasi git
git init
git add .
git commit -m "Initial commit"
git branch -M main

# Ganti USERNAME dan REPO dengan milik Anda
git remote add origin https://github.com/USERNAME/NSC-Winlator-Edition.git
git push -u origin main
```

### 2. GitHub Actions Langsung Aktif!

Setelah push:
1. Buka GitHub repository
2. Klik tab **"Actions"**
3. Lihat workflow "Build NSC Winlator Edition" berjalan
4. Tunggu sampai selesai (±2 menit)
5. Download artifact

**Selesai!** Build otomatis sudah bekerja! ✅

---

## 📥 Download Build Results

### Dari GitHub Actions Artifacts

1. **Actions** tab → Click build terbaru
2. Scroll ke **Artifacts**
3. Download `NSC-Winlator-win-x64` (±50-100 MB)
4. Extract dan run `NSC.Winlator.exe`

### Dari GitHub Releases

1. **Releases** tab → Click release
2. Download binary siap pakai
3. Extract dan jalankan

---

## 📦 Cara Buat Release

Setiap tag akan auto-create release dengan binary!

```bash
# 1. Commit perubahan
git add .
git commit -m "Version 1.0.0"

# 2. Create tag
git tag -a v1.0.0 -m "Release 1.0.0"

# 3. Push tag
git push origin v1.0.0
```

**Hasilnya:**
- GitHub akan auto-build
- Auto-create Release page
- Upload 3 variants (x64, x86, framework)
- Release notes otomatis

---

## 🔄 Workflow yang Ada

### 1. **build.yml** - Build Otomatis

**Trigger saat:**
- Push ke `main` atau `develop`
- Pull request dibuat
- Manual run

**Hasil:**
- Executable self-contained
- Download dari Artifacts

### 2. **release.yml** - Release Build

**Trigger saat:**
- Tag di-push (format: `v1.0.0`)

**Hasil:**
- 3 build variants
- GitHub Release page
- All downloadable

---

## 📊 Monitor Build

1. **Actions** tab
2. Lihat workflow history
3. Click untuk details
4. View build logs

### Status Indicators

- 🟢 **Success** - Build berhasil
- 🔴 **Failed** - Ada error
- 🟡 **In Progress** - Sedang build

---

## 🎯 Workflow Actions

### Manual Trigger Build

1. Actions tab
2. "Build NSC Winlator Edition"
3. "Run workflow" button
4. Confirm

Build dimulai!

---

## 📋 File Structure untuk GitHub

```
NSC-Winlator-Edition/
├── .github/
│   └── workflows/
│       ├── build.yml          ← Automatic build
│       └── release.yml        ← Release build
├── .gitignore                 ← Ignore patterns
├── src/
│   └── NSC.Winlator/
│       ├── Models/
│       ├── Services/
│       ├── Infrastructure/
│       ├── Forms/
│       ├── Program.cs
│       └── NSC.Winlator.csproj
├── README.md
├── BUILD_INSTRUCTIONS.md
└── ... (other docs)
```

---

## 🛠️ Customize Workflows

### Change Build Platforms

Edit `build.yml`:
```yaml
dotnet publish -c Release -r win-x64 --self-contained -o publish
```

Options:
- `win-x64` - Windows 64-bit
- `win-x86` - Windows 32-bit
- `linux-x64` - Linux 64-bit
- `osx-x64` - macOS

### Change Trigger Branches

Edit `on` section:
```yaml
on:
  push:
    branches: [ main, develop, feature/* ]
```

---

## 🔐 Secrets (Tidak perlu!)

Workflow menggunakan `GITHUB_TOKEN` built-in. 
**Tidak perlu setup secret apapun.**

---

## 📈 Performance

- Build time: ~2-3 menit
- Artifact size: ~50-100 MB (self-contained)
- Storage: 90 hari retention (free tier)

---

## ❓ Troubleshooting

### Build Gagal

1. Check **Actions** tab
2. Click failed workflow
3. View build logs
4. Common issues:
   - .NET SDK version (harus 8.0+)
   - Source code syntax errors
   - Missing dependencies

### Artifact Tidak Ada

1. Check workflow completed
2. Verify `publish/` folder created
3. Check workflow file path correct

### Action Tidak Trigger

1. Push ke correct branch (`main` atau `develop`)
2. Check file `.github/workflows/build.yml` exists
3. Manual trigger via Actions tab

---

## 🚀 Next Steps

1. ✅ Push repository ke GitHub
2. ✅ Wait untuk workflow run
3. ✅ Download artifact
4. ✅ Test executable
5. ✅ Create tag untuk release
6. ✅ GitHub Release auto-created

---

## 💡 Pro Tips

### Auto-notify Build Status

Add to workflow:
```yaml
- name: Create Badge
  run: echo "Build Status: Success"
```

### Different Artifacts Per Platform

```yaml
- name: Publish x64
  run: dotnet publish -r win-x64

- name: Publish x86
  run: dotnet publish -r win-x86
```

### Conditional Steps

```yaml
- name: Step Name
  if: success()
  run: echo "Build succeeded"
```

---

## 📚 Resources

- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [.NET Setup Action](https://github.com/actions/setup-dotnet)
- [Upload Artifact](https://github.com/actions/upload-artifact)

---

## ✅ Checklist

- [ ] Repository created on GitHub
- [ ] Code pushed to main branch
- [ ] `.github/workflows/` folder exists
- [ ] `build.yml` exists
- [ ] `release.yml` exists
- [ ] First build completed successfully
- [ ] Artifact downloaded and tested
- [ ] Ready to create releases!

---

**🎉 Workflow is ready! Just push and let GitHub Actions build for you!**

Setiap push akan automatically build dan create artifact. Setiap tag push akan automatic release!
