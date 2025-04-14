using Mehran.SmartGlobalExceptionHandling.Core.Enums;

namespace Mehran.SmartGlobalExceptionHandling.Core.Config;

/// <summary>
/// کلاس تنظیمات هندلینگ خطاها
/// </summary>
public class ExceptionHandlingOption
{
    /// <summary>
    /// نمایش جزئیات خطا در پاسخ
    /// </summary>
    public bool ShowDetails { get; set; } = false;

    /// <summary>
    /// فعال‌سازی لاگ خطا
    /// </summary>
    public bool LogExceptions { get; set; } = true;

    /// <summary>
    /// پیام‌های پیش‌ فرض سفارشی
    /// </summary>
    public Dictionary<string, string> DefaultMessages { get; set; } = [];

    /// <summary>
    /// زبان پیش‌فرض برای پیام‌ها
    /// </summary>
    public SupportedLanguage Language { get; set; } = SupportedLanguage.Fa;
}
