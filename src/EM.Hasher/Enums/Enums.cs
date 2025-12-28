
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace EM.Hasher;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class Enums
{
    /// <summary>
    /// EM Hasher Theme Settings
    /// These values have always been used when setting the selected theme.
    /// Now decoupled from the actual implementation to allow UI flexibility.
    /// </summary>
    public enum AppThemeSetting
    {
        Default,    // 0
        Dark,       // 1
        Light       // 2
    }
}
