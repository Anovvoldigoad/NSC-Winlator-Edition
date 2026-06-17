using NSC.Winlator.Infrastructure;
using NSC.Winlator.Models;
using NSC.Winlator.Services;

namespace NSC.Winlator.Forms
{
    public partial class MainForm : Form
    {
        private List<ModInfo> _allMods = new();
        private ProfileInfo _currentProfile = new();
        private bool _isLoading = false;

        public MainForm()
        {
            InitializeComponent();
            Text = "NSC Winlator Edition - Mod Manager";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(900, 700);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeUI();
            LoadMods();
            LoggerService.LogAdded += LoggerService_LogAdded;
        }

        private void InitializeUI()
        {
            // Setup table layout
            TableLayoutPanel mainLayout = new();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.RowCount = 4;
            mainLayout.ColumnCount = 2;

            // Row heights
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

            // Toolbar
            Panel toolbarPanel = CreateToolbar();
            mainLayout.Controls.Add(toolbarPanel, 0, 0);
            mainLayout.SetColumnSpan(toolbarPanel, 2);

            // Left: Mod List
            mainLayout.Controls.Add(CreateModListPanel(), 0, 1);

            // Right: Metadata Viewer
            mainLayout.Controls.Add(CreateMetadataPanel(), 1, 1);

            // Log Viewer
            mainLayout.Controls.Add(CreateLogViewerPanel(), 0, 2);
            mainLayout.SetColumnSpan(mainLayout.Controls[mainLayout.Controls.Count - 1], 2);

            // Action Buttons
            mainLayout.Controls.Add(CreateActionButtonsPanel(), 0, 3);
            mainLayout.SetColumnSpan(mainLayout.Controls[mainLayout.Controls.Count - 1], 2);

            Controls.Add(mainLayout);
        }

        private Panel CreateToolbar()
        {
            Panel toolbar = new();
            toolbar.Dock = DockStyle.Fill;
            toolbar.BackColor = SystemColors.ButtonFace;
            toolbar.Padding = new Padding(5);

            FlowLayoutPanel flow = new();
            flow.Dock = DockStyle.Fill;
            flow.WrapContents = false;

            Button refreshBtn = new Button { Text = "Refresh", Width = 80 };
            refreshBtn.Click += (s, e) => LoadMods();

            Button profileBtn = new Button { Text = "Profiles", Width = 80 };
            profileBtn.Click += (s, e) => ShowProfilesDialog();

            Button settingsBtn = new Button { Text = "Settings", Width = 80 };
            settingsBtn.Click += (s, e) => ShowSettingsDialog();

            flow.Controls.Add(refreshBtn);
            flow.Controls.Add(profileBtn);
            flow.Controls.Add(settingsBtn);

            toolbar.Controls.Add(flow);
            return toolbar;
        }

        private Panel CreateModListPanel()
        {
            Panel panel = new();
            panel.Dock = DockStyle.Fill;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(5);

            Label label = new Label { Text = "Installed Mods", Dock = DockStyle.Top, Height = 20 };
            panel.Controls.Add(label);

            CheckedListBox modList = new();
            modList.Name = "ModList";
            modList.Dock = DockStyle.Fill;
            modList.ItemCheck += ModList_ItemCheck;
            modList.SelectedIndexChanged += ModList_SelectedIndexChanged;

            panel.Controls.Add(modList);
            return panel;
        }

        private Panel CreateMetadataPanel()
        {
            Panel panel = new();
            panel.Dock = DockStyle.Fill;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(5);

            Label titleLabel = new Label { Text = "Mod Details", Dock = DockStyle.Top, Height = 20, Font = new Font(Font, FontStyle.Bold) };
            panel.Controls.Add(titleLabel);

            TextBox metadataBox = new();
            metadataBox.Name = "MetadataBox";
            metadataBox.Dock = DockStyle.Fill;
            metadataBox.ReadOnly = true;
            metadataBox.Multiline = true;
            metadataBox.ScrollBars = ScrollBars.Both;

            panel.Controls.Add(metadataBox);
            return panel;
        }

        private Panel CreateLogViewerPanel()
        {
            Panel panel = new();
            panel.Dock = DockStyle.Fill;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(5);
            panel.Height = 150;

            Label label = new Label { Text = "Activity Log", Dock = DockStyle.Top, Height = 20, Font = new Font(Font, FontStyle.Bold) };
            panel.Controls.Add(label);

            TextBox logBox = new();
            logBox.Name = "LogBox";
            logBox.Dock = DockStyle.Fill;
            logBox.ReadOnly = true;
            logBox.Multiline = true;
            logBox.ScrollBars = ScrollBars.Vertical;

            panel.Controls.Add(logBox);
            return panel;
        }

        private Panel CreateActionButtonsPanel()
        {
            Panel panel = new();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = SystemColors.ButtonFace;
            panel.Padding = new Padding(5);

            FlowLayoutPanel flow = new();
            flow.Dock = DockStyle.Top;
            flow.WrapContents = true;
            flow.Height = 80;

            // Define buttons
            CreateButton("Install Mod", (s, e) => InstallMod());
            CreateButton("Remove Mod", (s, e) => RemoveMod());
            CreateButton("Backup Game", (s, e) => BackupGame());
            CreateButton("Restore Backup", (s, e) => RestoreBackup());
            CreateButton("Check Conflicts", (s, e) => CheckConflicts());
            CreateButton("Compile Mods", (s, e) => CompileMods());
            CreateButton("Launch Game", (s, e) => LaunchGame());
            CreateButton("Compile & Launch", (s, e) => CompileAndLaunch());

            void CreateButton(string text, EventHandler click)
            {
                Button btn = new Button { Text = text, Width = 100, Height = 35 };
                btn.Click += click;
                flow.Controls.Add(btn);
            }

            panel.Controls.Add(flow);
            return panel;
        }

        private void LoadMods()
        {
            _isLoading = true;
            _allMods = AppBootstrap.ModScanner?.ScanMods(AppBootstrap.ModsFolder) ?? new();

            CheckedListBox modList = FindControl("ModList") as CheckedListBox;
            if (modList != null)
            {
                modList.Items.Clear();
                foreach (var mod in _allMods)
                {
                    modList.Items.Add(mod.Name, mod.Enabled);
                }
            }

            _isLoading = false;
            Log($"Loaded {_allMods.Count} mods");
        }

        private void ModList_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_isLoading) return;

            if (e.Index < _allMods.Count)
            {
                _allMods[e.Index].Enabled = (e.NewValue == CheckState.Checked);
                AppBootstrap.ModConfigService?.WriteModConfig(_allMods[e.Index]);
            }
        }

        private void ModList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            CheckedListBox modList = FindControl("ModList") as CheckedListBox;
            TextBox metadataBox = FindControl("MetadataBox") as TextBox;

            if (modList != null && metadataBox != null && modList.SelectedIndex >= 0)
            {
                var mod = _allMods[modList.SelectedIndex];
                metadataBox.Text = $"Name: {mod.Name}\n" +
                                   $"Author: {mod.Author}\n" +
                                   $"Version: {mod.Version}\n" +
                                   $"Description: {mod.Description}\n" +
                                   $"Enabled: {mod.Enabled}\n" +
                                   $"Location: {mod.ModFolder}";
            }
        }

        private void InstallMod()
        {
            using (OpenFileDialog dialog = new())
            {
                dialog.Title = "Select Mod Package";
                dialog.Filter = "All Supported|*.nsc;*.ensc;*.uns;*.unse;*.zip;*.7z;*.rar;*.nus4|All Files|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _ = Task.Run(async () =>
                    {
                        bool success = await AppBootstrap.ModInstaller?.InstallMod(dialog.FileName, AppBootstrap.ModsFolder);
                        Invoke(() => {
                            if (success.GetValueOrDefault())
                            {
                                LoadMods();
                                Log("Mod installed successfully");
                            }
                            else
                            {
                                MessageBox.Show("Failed to install mod", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        });
                    });
                }
            }
        }

        private void RemoveMod()
        {
            CheckedListBox modList = FindControl("ModList") as CheckedListBox;
            if (modList?.SelectedIndex >= 0 && modList.SelectedIndex < _allMods.Count)
            {
                var mod = _allMods[modList.SelectedIndex];
                if (MessageBox.Show($"Remove mod '{mod.Name}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (AppBootstrap.ModInstaller?.UninstallMod(mod.ModFolder) == true)
                    {
                        LoadMods();
                        Log($"Removed mod: {mod.Name}");
                    }
                }
            }
        }

        private void BackupGame()
        {
            var settings = AppBootstrap.GameSettingsService?.GetSettings();
            if (settings == null || !Directory.Exists(settings.GameDirectory))
            {
                MessageBox.Show("Game directory not configured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ = Task.Run(async () =>
            {
                bool success = await AppBootstrap.BackupService?.BackupGameFiles(settings.GameDirectory);
                Invoke(() =>
                {
                    if (success.GetValueOrDefault())
                        Log("Game backup completed");
                    else
                        MessageBox.Show("Backup failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            });
        }

        private void RestoreBackup()
        {
            var backups = AppBootstrap.BackupService?.GetAvailableBackups() ?? new();
            if (backups.Count == 0)
            {
                MessageBox.Show("No backups available", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Simple selection dialog
            var selected = PromptSelection("Select Backup", backups);
            if (selected != null)
            {
                var settings = AppBootstrap.GameSettingsService?.GetSettings();
                if (settings != null)
                {
                    _ = Task.Run(async () =>
                    {
                        string backupPath = Path.Combine(AppBootstrap.BackupsFolder, selected);
                        bool success = await AppBootstrap.BackupService?.RestoreBackup(backupPath, settings.GameDirectory);
                        Invoke(() =>
                        {
                            if (success.GetValueOrDefault())
                                Log("Game restored from backup");
                            else
                                MessageBox.Show("Restore failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    });
                }
            }
        }

        private void CheckConflicts()
        {
            var enabledMods = _allMods.Where(m => m.Enabled).ToList();
            var conflicts = AppBootstrap.ConflictDetector?.DetectConflicts(enabledMods) ?? new();

            string report = AppBootstrap.ConflictDetector?.GenerateConflictReport(conflicts) ?? "No conflicts detected";
            MessageBox.Show(report, "Conflict Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CompileMods()
        {
            var enabledMods = _allMods.Where(m => m.Enabled).ToList();
            if (!enabledMods.Any())
            {
                MessageBox.Show("No mods enabled for compilation", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new())
            {
                dialog.Title = "Save Compiled Output";
                dialog.Filter = "All Files|*.*";
                dialog.DefaultExt = ".cpk";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _ = Task.Run(async () =>
                    {
                        bool success = await AppBootstrap.CompilerService?.CompileMods(enabledMods, dialog.FileName);
                        Invoke(() =>
                        {
                            if (success.GetValueOrDefault())
                                Log("Compilation completed successfully");
                            else
                                MessageBox.Show("Compilation failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        });
                    });
                }
            }
        }

        private void LaunchGame()
        {
            var settings = AppBootstrap.GameSettingsService?.GetSettings();
            if (settings == null || !AppBootstrap.GameSettingsService.IsGameConfigured())
            {
                MessageBox.Show("Game not configured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowSettingsDialog();
                return;
            }

            if (AppBootstrap.LaunchService?.LaunchGame(settings) == true)
            {
                Log("Game launched");
            }
            else
            {
                MessageBox.Show("Failed to launch game", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CompileAndLaunch()
        {
            var enabledMods = _allMods.Where(m => m.Enabled).ToList();
            var settings = AppBootstrap.GameSettingsService?.GetSettings();

            if (!enabledMods.Any() || settings == null || !AppBootstrap.GameSettingsService.IsGameConfigured())
            {
                MessageBox.Show("No mods enabled or game not configured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ = Task.Run(async () =>
            {
                string tempOutput = Path.Combine(Path.GetTempPath(), "nsc_compiled.cpk");
                bool compiled = await AppBootstrap.CompilerService?.CompileMods(enabledMods, tempOutput);

                if (compiled.GetValueOrDefault() && AppBootstrap.LaunchService?.LaunchGame(settings) == true)
                {
                    Invoke(() => Log("Compiled and launched game successfully"));
                }
                else
                {
                    Invoke(() => MessageBox.Show("Compile and launch failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
            });
        }

        private void ShowProfilesDialog()
        {
            MessageBox.Show("Profile management coming soon", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowSettingsDialog()
        {
            using (SaveFileDialog dialog = new())
            {
                dialog.Title = "Select Game Executable";
                dialog.Filter = "Executable Files|*.exe|All Files|*.*";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AppBootstrap.GameSettingsService?.SetGameExecutable(dialog.FileName);
                    Log("Game settings updated");
                }
            }
        }

        private void Log(string message)
        {
            LoggerService.LogInfo(message);
        }

        private void LoggerService_LogAdded(object? sender, LoggerService.LogEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(() => LoggerService_LogAdded(sender, e));
                return;
            }

            TextBox logBox = FindControl("LogBox") as TextBox;
            if (logBox != null)
            {
                logBox.AppendText($"[{e.Level}] {e.Message}\n");
                if (logBox.TextLength > 100000)
                    logBox.Clear();
            }
        }

        private string? PromptSelection(string title, List<string> items)
        {
            using (Form form = new Form())
            {
                form.Text = title;
                form.Width = 400;
                form.Height = 300;
                form.StartPosition = FormStartPosition.CenterParent;

                ListBox listBox = new ListBox { Dock = DockStyle.Fill };
                foreach (var item in items)
                    listBox.Items.Add(item);

                Button okBtn = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom };
                form.Controls.Add(listBox);
                form.Controls.Add(okBtn);
                form.AcceptButton = okBtn;

                return form.ShowDialog() == DialogResult.OK && listBox.SelectedIndex >= 0
                    ? listBox.SelectedItem.ToString()
                    : null;
            }
        }

        private Panel FindControl(string name) => Controls.OfType<Panel>()
            .SelectMany(p => p.Controls.OfType<Control>())
            .Where(c => c.Name == name)
            .OfType<Panel>()
            .FirstOrDefault()
            ?? Controls.OfType<Control>()
                .Where(c => c.Name == name)
                .OfType<Panel>()
                .FirstOrDefault();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            AppBootstrap.Shutdown();
        }
    }
}
