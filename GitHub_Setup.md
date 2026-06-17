# Setup GitHub Repository untuk CI/CD Build Otomatis

Panduan lengkap untuk setup NSC Winlator Edition di GitHub dengan **GitHub Actions** untuk build otomatis.

## 📋 Langkah Setup

### 1. Create GitHub Repository

```bash
# Inisialisasi git di folder project
cd NSC-Winlator-Edition
git init

# Add semua file
git add .

# Commit
git commit -m "Initial commit: NSC Winlator Edition MVP"

# Add remote repository
git remote add origin https://github.com/YOUR_USERNAME/NSC-Winlator-Edition.git

# Push ke GitHub
git branch -M main
git push -u origin main
```

### 2. GitHub Actions Already Configured

File workflow sudah tersedia di:
- `.github/workflows/build.yml` - Build otomatis setiap push
- `.github/workflows/release.yml` - Release build saat tag push

**Tidak perlu setup tambahan!** Workflow akan langsung aktif setelah push ke GitHub.

---

## 🔧 Bagaimana Workflow Bekerja

### Build Workflow (`build.yml`)

Berjalan otomatis setiap kali:
- Push ke branch `main` atau `develop`
- Pull request dibuat
- Manual trigger via GitHub UI

**Yang dilakukan:**
1. ✅ Checkout kode
2. ✅ Setup .NET 8 SDK
3. ✅ Restore dependencies
4. ✅ Build project (Release mode)
5. ✅ Publish win-x64 self-contained
6. ✅ Upload artifact

**Output:**
- Artifact: `NSC-Winlator-win-x64` (executable + dependencies)
- Tersedia di: Actions → [Run name] → Artifacts

### Release Workflow (`release.yml`)

Berjalan otomatis saat:
- Tag baru di-push (format: `v1.0.0`)

**Yang dilakukan:**
1. ✅ Checkout kode
2. ✅ Setup .NET 8 SDK
3. ✅ Build Release
4. ✅ Publish x64 self-contained
5. ✅ Publish x86 self-contained
6. ✅ Publish framework-dependent
7. ✅ Create GitHub Release
8. ✅ Upload binaries ke release

**Output:**
- GitHub Release dengan:
  - Release notes otomatis
  - 3 download options (x64, x86, framework-dependent)
  - Semua binaries siap download

---

## 📦 Bagaimana Cara Membuat Release

### Manual Step-by-Step

```bash
# 1. Commit semua perubahan
git add .
git commit -m "Version 1.0.0 ready for release"

# 2. Create tag
git tag -a v1.0.0 -m "NSC Winlator Edition v1.0.0"

# 3. Push tag ke GitHub
git push origin v1.0.0
```

**GitHub Actions akan:**
- Otomatis detect tag
- Build release
- Create GitHub Release
- Upload binaries

### Atau Via GitHub UI

1. Go to Releases
2. Click "Create a new release"
3. Enter tag: `v1.0.0`
4. Enter title: `NSC Winlator Edition v1.0.0`
5. Enter description (optional)
6. Click "Publish release"

---

## 📥 Download Built Files

### Dari Build Artifacts

1. Go to **Actions** tab di GitHub
2. Click workflow run terbaru
3. Scroll down ke "Artifacts"
4. Download `NSC-Winlator-win-x64`
5. Extract dan run `NSC.Winlator.exe`

### Dari Releases

1. Go to **Releases** tab
2. Click release yang ingin didownload
3. Download binary yang sesuai:
   - **win-x64** - Untuk Windows 64-bit (recommended)
   - **win-x86** - Untuk Windows 32-bit
   - **framework-dependent** - Butuh .NET 8 terinstall

---

## 🔐 Permissions & Secrets

**Tidak ada secret yang diperlukan!** Workflow menggunakan:
- `${{ secrets.GITHUB_TOKEN }}` (built-in default)

Token ini otomatis tersedia dan cukup untuk:
- Upload artifacts
- Create releases
- Push ke repository

---

## 📊 Workflow Status & Logs

### View Build Status

1. Go to **Actions** tab
2. Lihat workflow yang sedang/sudah berjalan
3. Click untuk melihat details
4. Lihat logs untuk troubleshooting

### Common Issues

**Build gagal?**
- Check logs di Actions tab
- Lihat error message di build output
- Pastikan .NET 8 compatible

**Artifact tidak ter-upload?**
- Check publish step di logs
- Pastikan folder `publish/` ada setelah build
- Verify path di workflow file

---

## 🚀 Trigger Build Manually

### Via GitHub UI

1. Go to **Actions** tab
2. Click workflow name (Build NSC Winlator Edition)
3. Click "Run workflow"
4. Click "Run workflow" button
5. Wait untuk build selesai

### Via Git Command

```bash
# Push empty commit untuk trigger workflow
git commit --allow-empty -m "Trigger build"
git push origin main
```

---

## 📝 Customize Workflow

### Change Build Output

Di `build.yml`, edit:
```yaml
- name: Publish Release Build
  run: |
    cd src/NSC.Winlator
    dotnet publish -c Release -r win-x64 --self-contained -o publish
```

Ganti `-r win-x64` dengan:
- `-r win-x86` untuk 32-bit
- `-r linux-x64` untuk Linux
- `-r osx-x64` untuk Mac

### Add More Platforms

Tambah step baru di workflow:
```yaml
- name: Publish Linux Build
  run: |
    cd src/NSC.Winlator
    dotnet publish -c Release -r linux-x64 --self-contained -o publish-linux
```

### Change Trigger Conditions

Edit bagian `on` di workflow:
```yaml
on:
  push:
    branches: [ main, develop ]    # Trigger on push
  pull_request:
    branches: [ main ]               # Trigger on PR
  schedule:
    - cron: '0 0 * * *'             # Daily build
  workflow_dispatch:                 # Manual trigger
```

---

## 📦 Artifact Retention

GitHub menyimpan artifacts selama **90 hari** by default.

Untuk extend atau reduce:
```yaml
- name: Upload artifact
  uses: actions/upload-artifact@v3
  with:
    name: NSC-Winlator-win-x64
    path: src/NSC.Winlator/publish/
    retention-days: 30  # Ganti angka sesuai kebutuhan
```

---

## 🔄 Integration dengan Tools

### Release ke Multiple Platforms

Modifikasi workflow untuk support:
- Publish ke NuGet
- Release ke GitHub Releases
- Notify di Discord/Slack
- Auto-tag Docker image

### Example: Discord Notification

```yaml
- name: Notify Discord
  uses: sarisia/actions-status-discord@v1
  if: always()
  with:
    webhook_url: ${{ secrets.DISCORD_WEBHOOK }}
    title: Build Status
    description: NSC Winlator Edition build ${{ job.status }}
```

Setup Discord webhook di Repository Secrets.

---

## 📚 Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Upload Artifact Action](https://github.com/actions/upload-artifact)
- [Create Release Action](https://github.com/softprops/action-gh-release)
- [Setup .NET Action](https://github.com/actions/setup-dotnet)

---

## ✅ Checklist Setup

- [ ] Repository di GitHub sudah create
- [ ] Kode sudah push ke main branch
- [ ] `.github/workflows/` folder ada di root
- [ ] `build.yml` dan `release.yml` ada
- [ ] Actions tab shows workflow running
- [ ] Artifacts ter-upload setelah build

---

## 🎉 Anda Sudah Siap!

Build otomatis sekarang aktif:

1. **Setiap push** → Automatic build
2. **Setiap tag push (v*.*)** → Release build + GitHub Release
3. **Manual trigger** → Via Actions tab

**Next steps:**
1. Push ke GitHub
2. Lihat workflow berjalan di Actions tab
3. Download artifacts
4. Test executable
5. Buat tag dan push untuk release

---

**Happy building! 🚀**

Setiap push sekarang akan automatically build dan create artifact!
