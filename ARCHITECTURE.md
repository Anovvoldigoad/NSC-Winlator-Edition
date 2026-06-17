# NSC Winlator Edition - Architecture & Design

## Executive Summary

NSC Winlator Edition is a lightweight, modular WinForms application built on .NET 8. It implements a service-oriented architecture with dependency injection, designed specifically for Winlator compatibility while maintaining cross-platform compatibility with Wine, Proton, and FEXCore.

## Core Design Principles

### 1. Managed Code Only
All code uses managed .NET libraries exclusively:
- No P/Invoke or native DLL calls
- No DirectX or graphics APIs
- No Registry access (JSON-based configuration)
- No kernel32 or system DLLs

**Why:** Ensures compatibility across Wine, Proton, Winlator, and FEXCore environments.

### 2. Lightweight Architecture
- WinForms + GDI+ rendering (lighter than WPF/MAUI)
- Minimal external dependencies
- Lazy loading where appropriate
- Memory-conscious design

**Why:** Critical for performance on Android containers (Winlator).

### 3. Modular Services
- Single Responsibility Principle
- Service abstraction pattern
- Dependency injection (manual DI in AppBootstrap)
- Clear separation of concerns

**Why:** Easy to test, extend, and maintain individual components.

### 4. Error Resilience
- Comprehensive try/catch blocks
- User-friendly error messages
- Logging at all critical points
- Graceful degradation

**Why:** Application should never crash; users informed of issues.

## Architecture Layers

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│      (WinForms MainForm)                │
│  - UI Components                        │
│  - User Interactions                    │
│  - Display Management                   │
└─────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│        Service Layer                    │
│   - Business Logic                      │
│   - Operations Orchestration            │
│   - External Tool Integration           │
└─────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│     Infrastructure Layer                │
│   - Storage Management                  │
│   - Configuration                       │
│   - Logging                             │
│   - Bootstrap/DI                        │
└─────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│        Data Layer                       │
│   - File System                         │
│   - JSON/INI Serialization              │
│   - Archive Extraction                  │
└─────────────────────────────────────────┘
```

## Service Architecture

### Service Hierarchy

```
AppBootstrap (Entry Point)
├── StorageInitializer
│   └── Initializes directory structure
├── LoggerService (Singleton)
│   └── Manages all logging
├── ModScanner
│   └── Mod detection
├── ModConfigService
│   └── mod_config.ini parsing
├── ProfileManager
│   └── Profile persistence
├── LoadOrderManager
│   └── Load order management
├── BackupService
│   └── Game backup/restore
├── ConflictDetector
│   └── File conflict analysis
├── GameSettingsService
│   └── Game configuration
├── LaunchService
│   └── Game launching
├── ModInstaller
│   └── Package installation
├── CompilerService
│   ├── YacpkWrapper
│   └── Compilation orchestration
└── UI (MainForm)
    └── All services interact through this
```

## Core Models

### ModInfo
Represents a single mod installation:
```csharp
public class ModInfo
{
    public string Name { get; set; }           // Mod display name
    public string Author { get; set; }         // Mod creator
    public string Version { get; set; }        // Semantic version
    public string Description { get; set; }    // Mod description
    public string ModFolder { get; set; }      // Installation path
    public string? IconPath { get; set; }      // Optional icon
    public bool Enabled { get; set; }          // Current state
    public DateTime LastModified { get; set; } // Metadata timestamp
}
```

### ProfileInfo
Represents a saved mod configuration:
```csharp
public class ProfileInfo
{
    public string Name { get; set; }                    // Profile name
    public List<string> EnabledMods { get; set; }      // Enabled mod list
}
```

### GameSettings
Stores game configuration:
```csharp
public class GameSettings
{
    public string GameExecutable { get; set; }  // Path to game.exe
    public string GameDirectory { get; set; }   // Game folder
}
```

## Data Persistence

### Configuration Files

**Config/game.json** - Game Settings
```json
{
  "GameExecutable": "C:\\Games\\NarutoSC\\game.exe",
  "GameDirectory": "C:\\Games\\NarutoSC"
}
```

**Config/loadorder.json** - Mod Load Order
```json
[
  "ModA",
  "ModB",
  "ModC"
]
```

**Profiles/*.json** - Saved Profiles
```json
{
  "Name": "Profile Name",
  "EnabledMods": ["ModA", "ModC"]
}
```

**Mods/*/mod_config.ini** - Mod Metadata
```ini
[ModManager]
ModName=Awesome Mod
Author=Creator Name
Version=1.0.0
Description=Does awesome things
EnableMod=true
```

**Logs/*.log** - Application Logs
```
[2025-01-15 14:30:45] [Info] Application started
[2025-01-15 14:30:46] [Info] Loaded 5 mods
[2025-01-15 14:31:00] [Success] Mod installed: NewMod
```

## Service Details

### LoggerService (Singleton Pattern)

**Responsibilities:**
- Centralized logging for all components
- Multiple log file categories
- In-memory buffer for UI display
- Event notification for real-time UI updates

**Key Methods:**
```csharp
LoggerService.LogInfo(message)          // General info
LoggerService.LogWarning(message)       // Warnings
LoggerService.LogError(message, ex)     // Errors with exceptions
LoggerService.LogSuccess(message)       // Success messages
LoggerService.LogCompile(message)       // Compilation-specific
LoggerService.LogInstall(message)       // Installation-specific
LoggerService.LogLaunch(message)        // Launch-specific
```

**Architecture:**
- Event-based notification: `event EventHandler<LogEventArgs> LogAdded`
- Concurrent buffer: `ConcurrentBag<string> _logBuffer`
- File-based persistence: Multiple `.log` files
- Thread-safe operations with `lock` statements

### ModScanner

**Responsibilities:**
- Detect installed mods
- Recursively scan for mod_config.ini files
- Return structured mod list

**Key Methods:**
```csharp
ScanMods(modsFolder)              // Scan immediate directory
ScanModsRecursive(modsFolder)     // Scan all subdirectories
```

**Algorithm:**
1. Check if folder exists
2. For each subdirectory/mod_config.ini:
   - Read mod metadata via ModConfigService
   - Create ModInfo object
   - Add to list
3. Return list of ModInfo

### ModConfigService

**Responsibilities:**
- Parse INI file format
- Extract mod metadata
- Write configuration changes
- Handle missing/invalid configs gracefully

**Supported INI Fields:**
```ini
[ModManager]
ModName=          # Mod display name
Author=           # Creator name
Version=          # Version string
Description=      # Text description
EnableMod=true    # Boolean: enabled status
```

**Key Methods:**
```csharp
ReadModConfig(modFolder)          // Parse config from folder
WriteModConfig(modInfo)           // Save mod configuration
```

### ProfileManager

**Responsibilities:**
- Save mod profiles as JSON
- Load saved profiles
- Delete profiles
- Manage profile folder

**Persistence Format:**
```json
{
  "Name": "Gaming Profile",
  "EnabledMods": ["Mod1", "Mod2"]
}
```

**Key Methods:**
```csharp
SaveProfile(profile)      // Persist to JSON
LoadProfile(name)         // Load from JSON
DeleteProfile(name)       // Remove profile
GetAllProfiles()          // List all profiles
ProfileExists(name)       // Check existence
```

### LoadOrderManager

**Responsibilities:**
- Maintain mod load order
- Provide reordering operations
- Persist load order to disk
- Integrate with mod list

**Persistence Format:**
```json
["Mod1", "Mod2", "Mod3"]
```

**Key Methods:**
```csharp
SetLoadOrder(modNames)    // Set complete order
GetLoadOrder()            // Get current order
MoveUp(modName)           // Move mod up by 1
MoveDown(modName)         // Move mod down by 1
AddMod(modName)           // Add to order
RemoveMod(modName)        // Remove from order
GetPosition(modName)      // Get current index
```

### BackupService

**Responsibilities:**
- Create game backups with timestamps
- Restore from backups
- List available backups
- Delete backups

**Backup Structure:**
```
Backups/
├── 20250115_143000/     # Backup timestamp
│   ├── [game files]
│   └── [game structure]
├── 20250115_150000/
│   └── ...
```

**Key Methods:**
```csharp
BackupGameFiles(gamePath)                    // Create backup
RestoreBackup(backupPath, gamePath)          // Restore files
GetAvailableBackups()                        // List backups
DeleteBackup(backupName)                     // Remove backup
```

**Implementation Details:**
- Asynchronous file copying for large backups
- Recursive directory copying
- Timestamp-based naming: `yyyyMMdd_HHmmss`
- Overwrites handled in RestoreBackup

### ConflictDetector

**Responsibilities:**
- Analyze enabled mods for file conflicts
- Report conflicting files
- Track which mods conflict
- Generate human-readable reports

**Monitored Paths:**
- `Resources/`
- `Characters/`
- `CPKs/`

**Key Methods:**
```csharp
DetectConflicts(enabledMods)       // Analyze conflicts
GenerateConflictReport(conflicts)  // Create report
```

**Algorithm:**
1. For each enabled mod:
   - Scan monitored directories
   - Extract relative file paths
   - Build file → mods map
2. Find files with multiple owners
3. Return ConflictReport list

### ModInstaller

**Responsibilities:**
- Support multiple archive formats
- Extract packages to temporary folder
- Detect mod name from metadata
- Handle conflicts and backups
- Clean up temporary files

**Supported Formats:**
- `.nsc` - NSC package (SharpCompress)
- `.ensc` - Enhanced NSC (SharpCompress)
- `.uns` - UNS package (SharpCompress)
- `.unse` - Enhanced UNS (SharpCompress)
- `.zip` - ZIP archive (System.IO.Compression)
- `.7z` - 7-Zip archive (SharpCompress)
- `.rar` - RAR archive (SharpCompress)
- `.nus4` - NUS4 package (SharpCompress)

**Key Methods:**
```csharp
InstallMod(packagePath, modsFolder)    // Install package
UninstallMod(modFolder)                // Remove mod
IsModPackage(filePath)                 // Validate format
```

**Installation Workflow:**
1. Validate package exists
2. Create temporary extraction folder
3. Extract based on file format
4. Find mod_config.ini in extracted files
5. Read mod name from config
6. Back up existing mod if present
7. Move extracted mod to mods folder
8. Clean up temporary files

### GameSettingsService

**Responsibilities:**
- Store game executable path
- Store game directory path
- Validate paths existence
- Check game configuration status
- Persist to JSON configuration

**Persistence Format:**
```json
{
  "GameExecutable": "C:\\Games\\game.exe",
  "GameDirectory": "C:\\Games"
}
```

**Key Methods:**
```csharp
SetGameExecutable(path)    // Set executable and directory
SetGameDirectory(path)     // Set game directory
IsGameConfigured()         // Check configuration validity
GetSettings()              // Get current settings
```

**Validation:**
- Executable must exist
- Directory must exist
- Both must be configured for valid state

### LaunchService

**Responsibilities:**
- Execute game executable
- Support command-line arguments
- Capture process information
- Handle launch errors

**Key Methods:**
```csharp
LaunchGame(gameSettings)                      // Launch game
LaunchGameWithArguments(gameSettings, args)   // Launch with args
```

**Implementation:**
- Uses `ProcessStartInfo` for launching
- Preserves working directory
- Logs process ID on success
- Captures error output on failure

### YacpkWrapper

**Responsibilities:**
- Wrap YACpkTool.exe executable
- Validate tool availability
- Execute compilation
- Capture tool output
- Handle compilation errors

**Key Methods:**
```csharp
SetToolPath(toolPath)                      // Set tool location
ValidateTool()                             // Check tool exists
ExecuteCompile(input, output, params)      // Run compilation
```

**Tool Discovery:**
- Located at: `Tools/YACpkTool.exe`
- Validates existence before execution
- Captures stdout/stderr
- Checks exit code for success

### CompilerService

**Responsibilities:**
- Orchestrate mod compilation workflow
- Create temporary workspace
- Merge enabled mod files
- Execute YACpk compilation
- Clean up temporary files
- Report compilation progress

**Key Methods:**
```csharp
CompileMods(enabledMods, outputPath)    // Compile workflow
```

**Compilation Workflow:**
```
Enabled Mods List
↓
Create Temporary Workspace
↓
For Each Enabled Mod:
  - Copy files (exclude mod_config.ini)
  - Preserve directory structure
↓
Execute YACpk Compile
↓
Return Output CPK
↓
Clean Temporary Workspace
```

**Progress Reporting:**
- Event-based: `event EventHandler<string> CompileProgress`
- Fired at major steps
- Logged to compile.log

## User Interface Architecture

### MainForm Layout

**Grid Structure:**
- 4 rows, 2 columns
- Row 1 (35px): Toolbar
- Row 2 (flexible): Mod list + Metadata
- Row 3 (30%): Log viewer
- Row 4 (100px): Action buttons

### Components

**Toolbar:**
- Refresh (reload mod list)
- Profiles (manage profiles)
- Settings (configure game)

**Mod List (Left Panel):**
- CheckedListBox control
- Displays installed mods
- Checkbox for enable/disable
- Selection triggers metadata display

**Metadata Display (Right Panel):**
- TextBox (read-only)
- Shows mod details:
  - Name, Author, Version
  - Description, Enabled status
  - Installation location

**Log Viewer (Bottom):**
- TextBox (read-only, multiline)
- Real-time activity display
- Auto-scroll to latest
- Max 100K characters before clear

**Action Buttons (Bottom):**
```
Install Mod    | Remove Mod      | Backup Game | Restore Backup
Check Conflicts| Compile Mods    | Launch Game | Compile & Launch
```

### Event Flow

```
User Interaction
↓
Button Click → Event Handler
↓
Determine Action
↓
Call Service Method (often async)
↓
Service Performs Operation
↓
Log Results
↓
Update UI (Invoke if cross-thread)
↓
User Feedback (Message Box / Log Display)
```

## Dependency Injection

### Manual DI Pattern

Instead of external DI framework, uses `AppBootstrap.cs`:

```csharp
public static class AppBootstrap
{
    public static ModScanner? ModScanner { get; private set; }
    public static ModConfigService? ModConfigService { get; private set; }
    // ... other services
    
    public static void Initialize()
    {
        ModScanner = new ModScanner();
        ModConfigService = new ModConfigService();
        // ... initialize all services
    }
}
```

**Usage in UI:**
```csharp
List<ModInfo> mods = AppBootstrap.ModScanner?.ScanMods(folder);
```

**Advantages:**
- No external dependency
- Full control over initialization order
- Easy to understand
- Lightweight

## Threading Model

### Synchronous Operations
- Mod scanning and display
- Configuration reading/writing
- Profile management
- Load order updates
- Settings updates

### Asynchronous Operations
- Mod installation (file I/O)
- Game backup/restore (large file copies)
- Compilation (YACpk execution)
- Game launching

**Implementation:**
```csharp
// UI Thread
Task.Run(async () => 
{
    // Background thread
    bool result = await Service.LongOperation();
    
    // Back to UI thread
    Invoke(() => 
    {
        UpdateUI();
    });
});
```

## Error Handling Strategy

### Three-Level Approach

**1. Service Level**
```csharp
try
{
    // Operation
}
catch (Exception ex)
{
    LoggerService.LogError("Operation failed", ex);
    return false; // Signal failure
}
```

**2. UI Level**
```csharp
if (!success)
{
    MessageBox.Show("Operation failed", "Error", ...);
}
```

**3. Global Level**
- Application doesn't crash
- Graceful degradation
- User informed of status

## Performance Considerations

### Mod Scanning
- Cached after initial scan
- Refresh only when user clicks button
- Recursive scan handles nested structures
- Handles missing configs gracefully

### File Operations
- Asynchronous for large operations
- Batch operations where possible
- Temporary file cleanup
- No memory leaks (proper disposal)

### Logging
- Concurrent buffer for thread safety
- Limited log buffer in memory (prevents growth)
- Aggregates into files
- Doesn't block main thread

### UI Responsiveness
- Long operations run on background thread
- UI thread only updates display
- Cross-thread calls via Invoke
- No blocking operations on UI thread

## Extensibility Points

### Adding New Services

1. Create service class in `Services/`
2. Implement operation methods
3. Add logging calls
4. Register in `AppBootstrap.Initialize()`
5. Use `AppBootstrap.ServiceName` to access

### Adding New Mod Formats

1. Update `ModInstaller.SupportedExtensions`
2. Add format-specific extraction logic:
   ```csharp
   if (extension == ".custom")
   {
       ExtractCustomFormat(packagePath, tempFolder);
   }
   ```
3. Test with sample packages

### Extending Compilation

1. Add parameters to `CompilerService.CompileMods()`
2. Pass parameters to `YacpkWrapper.ExecuteCompile()`
3. Document new parameters

### Adding UI Features

1. Create form in `Forms/` directory
2. Use service layer for operations
3. Bind to AppBootstrap services
4. Handle cross-thread calls with Invoke

## Testing Strategy

### Unit Testing (Future)

Services can be tested independently:
```csharp
[Test]
public void ModScanner_DetectsValidMod()
{
    var scanner = new ModScanner();
    var mods = scanner.ScanMods(testFolder);
    Assert.AreEqual(1, mods.Count);
}
```

### Integration Testing (Manual)

Test workflows end-to-end:
1. Install mod → Verify in list
2. Enable/Disable → Verify in config
3. Create profile → Load profile
4. Compile and launch

### Performance Testing

Monitor for:
- Memory leaks
- Large backup times
- Compilation speed
- Log file growth

## Security Considerations

### File Operations
- Validates paths before access
- Checks file/directory existence
- Uses safe path operations
- No path traversal vulnerabilities

### External Tools
- Validates YACpk tool existence
- Captures tool output (no pipes to user)
- Checks exit codes
- Logs all operations

### User Data
- Local storage in AppData
- No network transmission
- No telemetry
- User control over all data

## Winlator-Specific Architecture

### Path Management
- Uses standard Windows paths (`%AppData%`)
- Works with Wine path mapping
- Compatible with Proton folder structure
- No absolute hard-coded paths

### No Native Dependencies
- All code is managed .NET
- No P/Invoke calls
- No DirectX or graphics APIs
- No Registry access

### File Format Compatibility
- JSON instead of Registry
- INI parsing via custom code
- Standard ZIP/7z formats
- No proprietary binary formats

### Performance Optimization
- Light UI (WinForms, not WPF)
- Minimal memory footprint
- Efficient file operations
- Smart caching

---

**Version:** 1.0.0
**Last Updated:** January 2025
**Target Framework:** .NET 8
**Platforms:** Windows 10/11, Winlator, Wine, Proton, FEXCore
