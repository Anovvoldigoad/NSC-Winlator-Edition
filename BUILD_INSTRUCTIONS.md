# NSC Winlator Edition - Build Instructions

## Prerequisites

### Required Software
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or later)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community, Professional, or Enterprise)
  - OR [Visual Studio Code](https://code.visualstudio.com/) with C# extension
  - OR any text editor with dotnet CLI

### System Requirements
- Windows 10/11 (for compilation)
- 2GB RAM minimum
- 500MB disk space for SDK and build output

## Project Setup

### 1. Create Project Structure

```bash
mkdir NSC.Winlator
cd NSC.Winlator
mkdir src
cd src
mkdir NSC.Winlator
cd NSC.Winlator
mkdir Forms Models Services Infrastructure
```

### 2. Create Project File

Place `NSC.Winlator.csproj` in the `src/NSC.Winlator/` directory.

### 3. Add Source Files

Copy all `.cs` files to their respective directories:

```
src/NSC.Winlator/
├── NSC.Winlator.csproj
├── Program.cs
├── Forms/
│   ├── MainForm.cs
│   └── MainForm.Designer.cs
├── Models/
│   ├── ModInfo.cs
│   ├── GameProfile.cs
│   ├── ProfileInfo.cs
│   ├── AppConfig.cs
│   └── GameSettings.cs
├── Services/
│   ├── IniParser.cs
│   ├── LoggerService.cs
│   ├── ModConfigService.cs
│   ├── ModScanner.cs
│   ├── ModInstaller.cs
│   ├── ProfileManager.cs
│   ├── LoadOrderManager.cs
│   ├── BackupService.cs
│   ├── ConflictDetector.cs
│   ├── GameSettingsService.cs
│   ├── LaunchService.cs
│   ├── YacpkWrapper.cs
│   └── CompilerService.cs
└── Infrastructure/
    ├── StorageInitializer.cs
    └── AppBootstrap.cs
```

## Building with .NET CLI

### 1. Restore Dependencies

```bash
cd src/NSC.Winlator
dotnet restore
```

Expected output:
```
Restore completed for [project path]\NSC.Winlator.csproj
```

### 2. Build Debug Version

```bash
dotnet build
```

Output location: `bin/Debug/net8.0-windows/NSC.Winlator.exe`

### 3. Build Release Version

```bash
dotnet build -c Release
```

Output location: `bin/Release/net8.0-windows/NSC.Winlator.exe`

### 4. Run Application

```bash
dotnet run
```

## Building with Visual Studio 2022

### 1. Create Solution
- Open Visual Studio 2022
- Create new solution folder
- Add new WinForms .NET 8 project
- Name: `NSC.Winlator`

### 2. Create Project Structure
In Solution Explorer:
- Add folder: `Models`
- Add folder: `Services`
- Add folder: `Forms`
- Add folder: `Infrastructure`

### 3. Add NuGet Packages
- Right-click project → Manage NuGet Packages
- Search and install:
  - `SharpCompress` (version 0.37.2 or later)
  - `System.Text.Json` (version 8.0.0 or later)

### 4. Add Source Files
- Copy each `.cs` file to appropriate folder
- Ensure namespaces match folder structure

### 5. Set as Startup Project
- Right-click project → Set as Startup Project
- Press F5 to run or Ctrl+Shift+B to build

## Publishing

### Self-Contained Build (Recommended for Winlator)

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

Creates complete standalone executable:
```
bin/Release/net8.0-windows/publish/
├── NSC.Winlator.exe (main executable)
├── dotnet.exe
├── *.dll (all dependencies)
└── ... (runtime files)
```

### Framework-Dependent Build (Smaller Size)

```bash
dotnet publish -c Release -r win-x64
```

Requires .NET 8 Runtime to be installed on target system.

### Create Portable Release

```bash
# Create release directory
mkdir NSC.Winlator_v1.0.0_Release
cd NSC.Winlator_v1.0.0_Release

# Publish self-contained
dotnet publish -c Release -r win-x64 --self-contained -o .

# Create archive
tar -czf NSC.Winlator_v1.0.0_win-x64.tar.gz *
# or on Windows:
# Compress-Archive -Path * -DestinationPath NSC.Winlator_v1.0.0_win-x64.zip
```

## Testing the Build

### Unit Testing (Add Later)

Create test project:
```bash
dotnet new xunit -n NSC.Winlator.Tests
dotnet add NSC.Winlator.Tests reference src/NSC.Winlator/NSC.Winlator.csproj
```

Run tests:
```bash
dotnet test
```

### Manual Testing Checklist

- [ ] Application starts without errors
- [ ] Application creates AppData folder structure
- [ ] Logging works (check Logs/ folder)
- [ ] Can select game executable
- [ ] Can install mod package
- [ ] Mod appears in list after refresh
- [ ] Mod can be enabled/disabled
- [ ] Mod metadata displays correctly
- [ ] Backup creates folder with timestamp
- [ ] Restore backup works
- [ ] Compile button works (if YACpk present)
- [ ] Game launches

## Deployment to Winlator

### 1. Prepare Application

```bash
# Build self-contained release
dotnet publish -c Release -r win-x64 --self-contained -o publish/
```

### 2. Package for Winlator

Create folder structure on Android device:
```
/storage/emulated/0/Winlator/
└── NSC.Winlator/
    ├── NSC.Winlator.exe
    ├── dotnet.exe
    ├── *.dll (dependencies)
    └── ... (other runtime files)
```

### 3. Configure Winlator Container

In Winlator:
1. Create new container
2. Install .NET 8 runtime (if needed)
3. Copy application files to container
4. Set C:/NSC.Winlator/ as app directory
5. Run: `NSC.Winlator.exe`

### 4. First Run on Winlator

Application will create:
```
C:/Users/[User]/AppData/Roaming/NSC.Winlator/
├── Mods/
├── Profiles/
├── Backups/
├── Config/
├── Logs/
├── Tools/
└── Cache/
```

## Troubleshooting Build Issues

### Error: "No suitable projects found"
- Ensure you're in correct directory
- Check project file exists: `NSC.Winlator.csproj`
- Verify file names and paths

### Error: "The type or namespace name X could not be found"
- Check using statements at top of file
- Verify namespace paths match folder structure
- Ensure all files are in correct folders

### Error: "SharpCompress not found"
```bash
dotnet add package SharpCompress
```

### Error: "Project file not compatible"
- Verify .NET 8 SDK is installed: `dotnet --version`
- Update TargetFramework in .csproj to `net8.0-windows`

### Warning: "Platform not recognized"
- Build with specific runtime identifier:
```bash
dotnet build -r win-x64
```

## Performance Optimization

### Release Build Settings

In `NSC.Winlator.csproj`:
```xml
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|AnyCPU'">
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishTrimmed>true</PublishTrimmed>
  <PublishSingleFile>true</PublishSingleFile>
  <IncludeAllContentForSelfExtract>true</IncludeAllContentForSelfExtract>
</PropertyGroup>
```

### Trimming (Advanced)
Add to `.csproj` for reduced binary size:
```xml
<PropertyGroup>
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>link</TrimMode>
</PropertyGroup>
```

**Note:** Trimming may affect reflection-based features. Test thoroughly.

## Continuous Integration

### GitHub Actions Example

Create `.github/workflows/build.yml`:
```yaml
name: Build NSC Winlator

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build -c Release
      - run: dotnet publish -c Release -r win-x64 --self-contained
      - uses: actions/upload-artifact@v3
        with:
          name: NSC.Winlator
          path: src/NSC.Winlator/bin/Release/net8.0-windows/publish/
```

## Troubleshooting at Runtime

### Application crashes on startup
- Check `%AppData%\NSC.Winlator\Logs\application.log`
- Ensure .NET 8 runtime is installed
- Check Windows Event Viewer for exception details

### Cannot write to AppData
- Verify user has permissions to `%AppData%\NSC.Winlator\`
- Check disk space availability
- Run as Administrator if necessary

### Mods not detected
- Verify mods are in `%AppData%\NSC.Winlator\Mods\`
- Check each mod has `mod_config.ini`
- Click Refresh button
- Check logs for parsing errors

## Version Management

### Increment Version

Update in `NSC.Winlator.csproj`:
```xml
<Version>1.0.1</Version>
```

### Build Versioned Release

```bash
dotnet publish -c Release -r win-x64 --self-contained \
  -p:Version=1.0.1 \
  -o release/v1.0.1/
```

## Additional Resources

- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/core/)
- [WinForms Documentation](https://learn.microsoft.com/dotnet/desktop/winforms/)
- [C# Language Features](https://learn.microsoft.com/dotnet/csharp/)

## Support

For build issues:
1. Check error message carefully
2. Verify all prerequisites installed
3. Try clean build: `dotnet clean && dotnet build`
4. Check project structure matches specification
5. Review project file syntax

---

**Last Updated:** 2025
**Target Framework:** .NET 8
**Platform:** Windows 10/11 + Winlator
