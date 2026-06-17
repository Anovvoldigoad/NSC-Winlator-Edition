# NSC Winlator Edition MVP

A lightweight, Winlator-compatible mod manager for Naruto Storm Connections.

## Overview

NSC Winlator Edition is a .NET 8 WinForms application designed to manage mods for Naruto Storm Connections. Built with Winlator compatibility as a primary requirement, it uses managed code exclusively and avoids heavy dependencies.

## Features

### Core Features
- **Mod Detection**: Automatically scans and detects installed mods
- **Mod Management**: Enable/Disable mods with persistent configuration
- **Profile Support**: Save and load mod profiles
- **Load Order Management**: Organize mod loading priority
- **Conflict Detection**: Identifies conflicting files between mods
- **Backup/Restore**: Creates and restores game backups
- **Mod Installation**: Supports multiple package formats
- **Compilation**: YACpk integration for mod compilation
- **Game Launching**: Direct game executable launching
- **Logging**: Comprehensive activity logging

### Supported Formats
- .nsc (NSC Package)
- .ensc (Enhanced NSC Package)
- .uns (UNS Package)
- .unse (Enhanced UNS Package)
- .zip (ZIP Archive)
- .7z (7-Zip Archive)
- .rar (RAR Archive)
- .nus4 (NUS4 Package)

## Project Structure

```
NSC.Winlator/
├── src/
│   └── NSC.Winlator/
│       ├── Forms/
│       │   ├── MainForm.cs
│       │   └── MainForm.Designer.cs
│       ├── Models/
│       │   ├── ModInfo.cs
│       │   ├── GameProfile.cs
│       │   ├── ProfileInfo.cs
│       │   ├── AppConfig.cs
│       │   └── GameSettings.cs
│       ├── Services/
│       │   ├── IniParser.cs
│       │   ├── LoggerService.cs
│       │   ├── ModConfigService.cs
│       │   ├── ModScanner.cs
│       │   ├── ModInstaller.cs
│       │   ├── ProfileManager.cs
│       │   ├── LoadOrderManager.cs
│       │   ├── BackupService.cs
│       │   ├── ConflictDetector.cs
│       │   ├── GameSettingsService.cs
│       │   ├── LaunchService.cs
│       │   ├── YacpkWrapper.cs
│       │   └── CompilerService.cs
│       ├── Infrastructure/
│       │   ├── StorageInitializer.cs
│       │   └── AppBootstrap.cs
│       ├── Program.cs
│       └── NSC.Winlator.csproj
└── README.md
```

## Building

### Requirements
- .NET 8 SDK
- Visual Studio 2022 or Visual Studio Code

### Build Instructions

```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Publish for distribution
dotnet publish -c Release -r win-x64
```

## Installation

### User Installation
1. Download the latest release
2. Extract to desired location
3. Run `NSC.Winlator.exe`

### First Run
On first run, the application will:
1. Create necessary folders in `%AppData%\NSC.Winlator\`
2. Initialize logging system
3. Scan for existing mods

## Architecture

### Design Principles
- **Managed Code Only**: No P/Invoke or native DLL calls
- **Lightweight**: Minimal dependencies, WinForms for performance
- **Modular Services**: Dependency injection pattern
- **Error Handling**: Comprehensive try/catch with user-friendly messages
- **Cross-Platform Path**: Works on Windows, Wine, Proton, Winlator, FEXCore

### Service Architecture

**StorageInitializer**
- Initializes required directory structure
- Creates Config, Mods, Profiles, Backups, Logs, Cache folders

**LoggerService**
- Centralized logging system
- Writes to timestamped log files
- Supports multiple log categories
- In-memory log buffer for UI display

**ModScanner**
- Recursively scans for mod_config.ini files
- Returns list of detected mods
- Handles missing/invalid configs gracefully

**ModConfigService**
- Parses mod_config.ini files
- Reads mod metadata (name, author, version, description)
- Supports EnableMod flag for mod state persistence
- Writes configuration changes back to disk

**ProfileManager**
- Persists profiles as JSON files
- Tracks enabled mods per profile
- Supports CRUD operations on profiles
- Automatically manages profile folder structure

**LoadOrderManager**
- Stores mod load order in loadorder.json
- Provides move up/down functionality
- Automatically saves changes
- Integrates with mod list display

**BackupService**
- Creates timestamped backups of game folders
- Asynchronous backup/restore operations
- Lists available backups with timestamps
- Supports backup deletion

**ConflictDetector**
- Scans enabled mods for duplicate files
- Checks Resources/, Characters/, CPKs/ paths
- Generates human-readable conflict reports
- Identifies which mods conflict

**GameSettingsService**
- Persists game configuration as JSON
- Validates executable and directory existence
- Provides IsGameConfigured() check
- Supports custom game paths

**LaunchService**
- Launches game executable via ProcessStartInfo
- Supports command-line arguments
- Captures exit codes and errors
- Logs all launch attempts

**ModInstaller**
- Supports 8 package formats (nsc, ensc, uns, unse, zip, 7z, rar, nus4)
- Extracts packages to temporary folder
- Locates mod_config.ini for naming
- Backs up existing mods before overwrite
- Handles cleanup of failed installations

**CompilerService**
- Orchestrates mod compilation workflow
- Creates temporary workspace
- Merges enabled mod files
- Executes YACpk compilation
- Provides progress callbacks
- Cleans up temporary files

**YacpkWrapper**
- Wraps YACpkTool.exe executable
- Validates tool availability
- Executes compilation with error handling
- Captures and logs tool output

### Data Storage

**Profiles/** - JSON-formatted mod profiles
```json
{
  "Name": "Profile Name",
  "EnabledMods": ["Mod1", "Mod2", "Mod3"]
}
```

**Config/game.json** - Game configuration
```json
{
  "GameExecutable": "C:\\Games\\NarutoSC\\game.exe",
  "GameDirectory": "C:\\Games\\NarutoSC"
}
```

**Config/loadorder.json** - Mod load order
```json
["Mod1", "Mod2", "Mod3"]
```

**Mods/*/mod_config.ini** - Individual mod configuration
```ini
[ModManager]
ModName=My Awesome Mod
Author=Author Name
Version=1.0
Description=A description of the mod
EnableMod=true
```

**Logs/** - Application activity logs
- application.log - General app activity
- compile.log - Compilation details
- install.log - Installation details
- launch.log - Game launch details

## Usage

### Basic Workflow

1. **Configure Game**
   - Click Settings
   - Select game executable
   - Application detects game directory

2. **Install Mods**
   - Click Install Mod
   - Select package file (.nsc, .zip, etc.)
   - Application extracts and registers mod

3. **Manage Mods**
   - Check/Uncheck mods to enable/disable
   - View mod details in right panel
   - Use Up/Down arrows to reorder (if implemented)

4. **Save Profile**
   - Configure desired mods
   - Click Profiles → Save Profile
   - Name your configuration

5. **Compile & Launch**
   - Enable desired mods
   - Click "Compile & Launch"
   - Application compiles and launches game

### Advanced Features

**Conflict Detection**
- Click "Check Conflicts"
- View mods with overlapping files
- Load order matters for conflict resolution

**Backup Management**
- Click "Backup Game" to create backup
- Click "Restore Backup" to restore from timestamp
- Backups stored with full timestamp

**Load Order**
- Mods later in load order override earlier ones
- Use Move Up/Down buttons to adjust
- Order affects conflict resolution

## Winlator Compatibility

### Why WinForms?
- Lighter rendering with GDI+
- Lower CPU/memory overhead
- Better compatibility with Wine/Proton
- No heavy framework dependencies

### Requirements for Winlator
- .NET 8 Runtime installed in container
- Wine environment configured
- Sufficient storage for mods and backups

### Restrictions
- No Registry access (uses JSON instead)
- No P/Invoke calls (pure managed code)
- No DirectX/Graphics APIs
- No native hooks or injections

## Configuration

### First Run Setup
Application creates folder structure:
```
%AppData%\NSC.Winlator\
├── Mods/              (user mod installations)
├── Profiles/          (saved profiles)
├── Backups/           (game backups)
├── Config/            (game.json, loadorder.json)
├── Logs/              (activity logs)
├── Tools/             (YACpkTool.exe location)
└── Cache/             (temporary files)
```

### Manual Configuration
Edit `%AppData%\NSC.Winlator\Config\game.json`:
```json
{
  "GameExecutable": "path/to/game.exe",
  "GameDirectory": "path/to/game/folder"
}
```

## Troubleshooting

### Application Won't Start
1. Check .NET 8 is installed: `dotnet --version`
2. Check logs in `%AppData%\NSC.Winlator\Logs\application.log`
3. Verify write permissions to AppData folder

### Mods Not Detected
1. Ensure mods are in `%AppData%\NSC.Winlator\Mods\`
2. Each mod must have `mod_config.ini` file
3. Check logs for scanning errors
4. Click Refresh button

### Compilation Fails
1. Verify YACpkTool.exe is in `Tools/` folder
2. Check compilation logs in `Logs/compile.log`
3. Ensure at least one mod is enabled
4. Verify temporary workspace can be created

### Game Won't Launch
1. Verify game executable path in Settings
2. Check game directory exists and is readable
3. Review logs in `Logs/launch.log`
4. Ensure game isn't already running

## Performance Notes

- Mod scanning is cached until Refresh is clicked
- Large backups may take time on first run
- Compilation speed depends on mod count and file size
- Log buffer limited to 100 entries in UI

## Future Enhancements (V2)

Planned features not in MVP:
- XFBIN Editor
- Character Injector
- Param Editor
- Custom Roster Builder
- Managed CPK Compiler
- Advanced profile scheduling
- Mod auto-update checking

## License

This project is provided as-is for educational and personal use.

## Support

For issues and questions:
1. Check application logs in `%AppData%\NSC.Winlator\Logs\`
2. Review README troubleshooting section
3. Ensure all dependencies are met
4. Verify Winlator/Wine configuration

## Technical Details

### Dependencies
- SharpCompress (for archive extraction)
- System.Text.Json (for JSON serialization)
- System.IO.Compression (for ZIP handling)
- WinForms (Windows Desktop UI framework)

### Platform Support
- Windows 10/11 (native)
- Winlator (Android via Wine)
- Wine (Linux/Mac via compatibility layer)
- Proton (Steam Compatibility Tool)
- FEXCore (ARM translation)

### Minimum Requirements
- .NET 8 Runtime
- 256MB RAM minimum
- 1GB storage for mods/backups
- Windows 10 or equivalent

## Development

### Adding New Services
1. Create service class in `Services/`
2. Register in `AppBootstrap.cs`
3. Use dependency injection pattern
4. Add logging for debugging

### Extending Mod Formats
1. Update `ModInstaller.cs` supported extensions
2. Add format-specific extraction logic
3. Test with sample packages
4. Document in README

### Custom Logging
```csharp
LoggerService.LogInfo("General message");
LoggerService.LogWarning("Warning message");
LoggerService.LogError("Error message", exception);
LoggerService.LogSuccess("Success message");
LoggerService.LogCompile("Compile-specific message");
LoggerService.LogInstall("Install-specific message");
LoggerService.LogLaunch("Launch-specific message");
```

## Changelog

### v1.0.0 (MVP Release)
- Initial release
- Core mod management
- Profile support
- Load order management
- Backup/Restore
- Conflict detection
- YACpk compilation
- Game launching
- Comprehensive logging
- Winlator compatibility

---

**NSC Winlator Edition** - Lightweight Mod Manager for Naruto Storm Connections
