# NSC Winlator Edition - Quick Start Guide

## For Developers

### 30-Second Setup

```bash
# 1. Navigate to project folder
cd src/NSC.Winlator

# 2. Restore dependencies
dotnet restore

# 3. Build and run
dotnet run
```

Application starts in ~5 seconds on modern hardware.

## Project Files Overview

### Essential Files

| File | Purpose |
|------|---------|
| `NSC.Winlator.csproj` | Project configuration, dependencies |
| `Program.cs` | Application entry point |
| `Forms/MainForm.cs` | Main UI window |
| `Infrastructure/AppBootstrap.cs` | Service initialization |

### Critical Services

| Service | File | Purpose |
|---------|------|---------|
| ModScanner | `Services/ModScanner.cs` | Detect installed mods |
| ModInstaller | `Services/ModInstaller.cs` | Install mod packages |
| ProfileManager | `Services/ProfileManager.cs` | Save/load mod profiles |
| CompilerService | `Services/CompilerService.cs` | Orchestrate compilation |
| LoggerService | `Services/LoggerService.cs` | Centralized logging |

## Common Tasks

### Adding a New Service

**1. Create service file** in `Services/NewService.cs`:
```csharp
namespace NSC.Winlator.Services
{
    public class NewService
    {
        public void DoSomething()
        {
            LoggerService.LogInfo("Doing something...");
        }
    }
}
```

**2. Register in** `Infrastructure/AppBootstrap.cs`:
```csharp
public static NewService? NewService { get; private set; }

public static void Initialize()
{
    // ... existing services
    NewService = new NewService();
}
```

**3. Use in UI**:
```csharp
AppBootstrap.NewService?.DoSomething();
```

### Adding New Mod Format

**Update** `Services/ModInstaller.cs`:
```csharp
private static readonly string[] SupportedExtensions = 
{
    ".nsc", ".ensc", ".uns", ".unse", ".zip", ".7z", ".rar", ".nus4",
    ".newformat"  // Add here
};

// Add extraction logic:
if (extension == ".newformat")
{
    ExtractNewFormat(packagePath, tempFolder);
}
```

### Debugging

**Enable Debug Output:**
```csharp
System.Diagnostics.Debug.WriteLine("Variable: " + value);
```

**Check Logs:**
```
%AppData%\NSC.Winlator\Logs\application.log
```

**Breakpoints:**
- F5 to start with debugger
- F10 to step over
- F11 to step into
- F9 to toggle breakpoint

## Build Commands

```bash
# Debug build (default)
dotnet build

# Release build (optimized)
dotnet build -c Release

# Run application
dotnet run

# Run tests
dotnet test

# Clean build
dotnet clean && dotnet build

# Publish standalone
dotnet publish -c Release -r win-x64 --self-contained
```

## Folder Structure Reference

```
src/NSC.Winlator/
├── Models/           ← Data models (no logic)
├── Services/         ← Business logic
├── Forms/            ← UI components
├── Infrastructure/   ← Setup and DI
├── Program.cs        ← Entry point
└── NSC.Winlator.csproj  ← Project file
```

## Key Concepts

### Service Pattern
Each service has single responsibility:
- **ModScanner**: Just scanning
- **ModInstaller**: Just installation
- **ProfileManager**: Just profile storage
- **LoggerService**: Just logging

### Dependency Injection
```csharp
// Access any service via AppBootstrap
AppBootstrap.ModScanner?.ScanMods(path);
AppBootstrap.ProfileManager?.SaveProfile(profile);
```

### Logging
```csharp
LoggerService.LogInfo("Normal message");
LoggerService.LogWarning("Caution");
LoggerService.LogError("Problem", exception);
LoggerService.LogSuccess("Completed!");
```

### Async Operations
```csharp
// Long-running operation on background thread
Task.Run(async () => 
{
    bool result = await Service.LongOperation();
    // Update UI on main thread
    Invoke(() => UpdateUI());
});
```

## Configuration Files

### Game Settings
```json
// %AppData%\NSC.Winlator\Config\game.json
{
  "GameExecutable": "C:\\Games\\game.exe",
  "GameDirectory": "C:\\Games"
}
```

### Load Order
```json
// %AppData%\NSC.Winlator\Config\loadorder.json
["Mod1", "Mod2", "Mod3"]
```

### Mod Profile
```json
// %AppData%\NSC.Winlator\Profiles\MyProfile.json
{
  "Name": "MyProfile",
  "EnabledMods": ["Mod1", "Mod2"]
}
```

## Debugging Common Issues

### Mods Not Loading
1. Check: `%AppData%\NSC.Winlator\Mods\` exists
2. Click **Refresh** button
3. Check `Logs/application.log` for errors

### Installation Fails
- Verify supported format (see ModInstaller.cs)
- Check `Logs/install.log`
- Ensure disk space available

### Compilation Errors
- Check YACpkTool.exe exists in `Tools/`
- Verify at least one mod enabled
- Review `Logs/compile.log`

### Game Won't Launch
- Verify game path in Settings
- Check `Logs/launch.log`
- Ensure game not already running

## Testing Checklist

- [ ] Application starts
- [ ] AppData folder created
- [ ] Can select game executable
- [ ] Can install test mod package
- [ ] Mod appears in list
- [ ] Can enable/disable mod
- [ ] Metadata displays correctly
- [ ] Can create profile
- [ ] Can load profile
- [ ] Backup creates timestamped folder
- [ ] Restore works
- [ ] Logs populate correctly

## Code Standards

### Naming
```csharp
// Public properties: PascalCase
public string ModName { get; set; }

// Private fields: _camelCase
private string _internalValue;

// Methods: PascalCase
public void LoadConfiguration()

// Local variables: camelCase
string folderPath = GetPath();

// Constants: UPPER_CASE
private const string CONFIG_FOLDER = "Config";
```

### Error Handling
```csharp
try
{
    // Operation
    DoSomething();
}
catch (Exception ex)
{
    LoggerService.LogError("Description of what failed", ex);
    return false; // Signal failure to caller
}
```

### Logging
```csharp
// Always log operations
LoggerService.LogInfo("Starting operation");

try
{
    // Do work
}
catch (Exception ex)
{
    LoggerService.LogError("Operation failed", ex);
}

// Log results
LoggerService.LogSuccess("Operation completed");
```

## Performance Tips

### Avoid On UI Thread
```csharp
// Bad: Blocks UI
List<ModInfo> mods = ModScanner.ScanMods(path);

// Good: Background thread
Task.Run(() => 
{
    List<ModInfo> mods = ModScanner.ScanMods(path);
    Invoke(() => LoadModList(mods));
});
```

### Reuse Objects
```csharp
// Create once
ModConfigService configService = new();

// Use multiple times
configService.ReadModConfig(folder1);
configService.ReadModConfig(folder2);
```

### Log Wisely
```csharp
// Good: Useful information
LoggerService.LogInfo($"Scanning {folderCount} folders");

// Bad: Too verbose
foreach(var folder in folders)
    LoggerService.LogInfo($"Checking {folder}");
```

## Resources

- [.NET 8 Docs](https://learn.microsoft.com/dotnet/)
- [C# Language Guide](https://learn.microsoft.com/dotnet/csharp/)
- [WinForms API](https://learn.microsoft.com/dotnet/desktop/winforms/)

## Getting Help

### Check Logs
```
%AppData%\NSC.Winlator\Logs\
├── application.log
├── compile.log
├── install.log
└── launch.log
```

### Review Source
- Search for similar functionality
- Check related service implementations
- Review error handling patterns

### Debug Mode
```csharp
System.Diagnostics.Debugger.Break(); // Breakpoint in code
```

## Common Pitfalls

### ❌ Don't
- Use Registry directly
- Hard-code absolute paths
- Ignore exceptions
- Block UI thread
- Store sensitive data

### ✅ Do
- Use JSON for config
- Use relative/standard paths
- Log and handle errors
- Use async/Task.Run
- Follow service pattern

## Next Steps

1. **Read** ARCHITECTURE.md for design details
2. **Review** BUILD_INSTRUCTIONS.md for compilation
3. **Explore** Services/ folder to understand patterns
4. **Build** and run to verify setup
5. **Start** implementing features

---

**Last Updated:** January 2025
**Version:** 1.0.0
**Framework:** .NET 8
