using Newtonsoft.Json;
using RetroDriven;

namespace Pocket_Updater.UI
{
    internal sealed class PocketTarget
    {
        public string DisplayName { get; init; } = string.Empty;
        public string Path { get; init; } = string.Empty;
        public bool IsRemovable { get; init; }

        public override string ToString() => DisplayName;
    }

    internal static class PocketTargetContext
    {
        private const string PreferencesFile = "updater_preferences.json";

        private static readonly (string LocationType, string DrivePath) _initialPreference = ReadPreference();
        private static string _selectedLocationType = _initialPreference.LocationType;
        private static string _selectedDrivePath = _initialPreference.DrivePath;
        private static string _selectedPath = ResolveInitialPath(_initialPreference);

        public static event EventHandler? TargetChanged;

        public static string SelectedPath => _selectedPath;
        public static string SelectedLocationType => _selectedLocationType;
        public static string SelectedDrivePath => _selectedDrivePath;

        public static IReadOnlyList<PocketTarget> GetTargets()
        {
            var result = new List<PocketTarget>
            {
                new PocketTarget
                {
                    DisplayName = "Current Directory",
                    Path = Directory.GetCurrentDirectory(),
                    IsRemovable = false
                }
            };

            result.AddRange(GetRemovableTargets());
            return result;
        }

        public static IReadOnlyList<PocketTarget> GetRemovableTargets()
        {
            var result = new List<PocketTarget>();

            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives()
                             .Where(d => d.DriveType == DriveType.Removable && d.IsReady))
                {
                    string label = string.IsNullOrWhiteSpace(drive.VolumeLabel)
                        ? "Removable Storage"
                        : drive.VolumeLabel;

                    result.Add(new PocketTarget
                    {

                        DisplayName = $"{drive.Name.TrimEnd('\\')}  {label}",
                        Path = drive.RootDirectory.FullName,
                        IsRemovable = true
                    });
                }
            }
            catch
            {

            }

            return result;
        }

        public static void SelectCurrentDirectory(bool persist = true)
        {
            string path = Normalize(Directory.GetCurrentDirectory());
            bool changed = !string.Equals(_selectedLocationType, "Current Directory", StringComparison.OrdinalIgnoreCase)
                           || !string.Equals(Normalize(_selectedPath), path, StringComparison.OrdinalIgnoreCase);

            _selectedLocationType = "Current Directory";
            _selectedDrivePath = string.Empty;
            _selectedPath = path;

            if (persist)
                PersistPreference();

            if (changed)
                TargetChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SelectRemovablePath(string path, bool persist = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            string normalized = Normalize(path);
            bool changed = !string.Equals(_selectedLocationType, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                           || !string.Equals(Normalize(_selectedPath), normalized, StringComparison.OrdinalIgnoreCase);

            _selectedLocationType = "Removable Storage";
            _selectedDrivePath = normalized;
            _selectedPath = normalized;

            if (persist)
                PersistPreference();

            if (changed)
                TargetChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SelectRemovableModeWithoutDrive(bool persist = true)
        {
            bool changed = !string.Equals(_selectedLocationType, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                           || !string.IsNullOrEmpty(_selectedDrivePath);

            _selectedLocationType = "Removable Storage";
            _selectedDrivePath = string.Empty;

            if (persist)
                PersistPreference();

            if (changed)
                TargetChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetSelectedPath(string path, bool persist = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            string normalized = Normalize(path);
            string current = Normalize(Directory.GetCurrentDirectory());

            if (string.Equals(normalized, current, StringComparison.OrdinalIgnoreCase))
                SelectCurrentDirectory(persist);
            else
                SelectRemovablePath(normalized, persist);
        }

        private static void PersistPreference()
        {
            try
            {
                string drive = string.Equals(_selectedLocationType, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                    ? _selectedDrivePath
                    : string.Empty;

                Updater_Preferences.Save_Updater_Json(
                    new[] { _selectedLocationType, drive },
                    PreferencesFile);
            }
            catch
            {

            }
        }

        private static (string LocationType, string DrivePath) ReadPreference()
        {
            try
            {
                if (File.Exists(PreferencesFile))
                {
                    var value = JsonConvert.DeserializeObject<Updater_Preferences>(File.ReadAllText(PreferencesFile));
                    if (value != null)
                    {
                        string location = string.Equals(value.update_location, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                            ? "Removable Storage"
                            : "Current Directory";

                        string drive = string.IsNullOrWhiteSpace(value.update_drive_letter)
                            ? string.Empty
                            : Normalize(value.update_drive_letter);

                        return (location, drive);
                    }
                }
            }
            catch
            {

            }

            return ("Current Directory", string.Empty);
        }

        private static string ResolveInitialPath((string LocationType, string DrivePath) preference)
        {
            if (string.Equals(preference.LocationType, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(preference.DrivePath)
                && Directory.Exists(preference.DrivePath))
            {
                return Normalize(preference.DrivePath);
            }

            return Normalize(Directory.GetCurrentDirectory());
        }

        private static string Normalize(string path)
        {
            try
            {
                string root = Path.GetPathRoot(path) ?? string.Empty;
                if (!string.IsNullOrEmpty(root)
                    && string.Equals(path.TrimEnd('\\', '/'), root.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
                {
                    return root;
                }

                return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch
            {
                return path.Trim();
            }
        }
    }
}
