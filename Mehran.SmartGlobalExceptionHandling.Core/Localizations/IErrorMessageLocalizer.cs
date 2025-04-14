namespace Mehran.SmartGlobalExceptionHandling.Core.Localizations;

/// <summary>
/// // اینترفیس عمومی لوکالایزر پیام‌های خطا
/// </summary>
public interface IErrorMessageLocalizer
{
    string Get(string key);
}