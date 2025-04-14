using System.Globalization;

namespace Mehran.SmartGlobalExceptionHandling.Core.Localizations;

public class LocalizedErrorMessageLocalizer : IErrorMessageLocalizer
{
    // پیام‌های چند زبانه برای کلیدهای خطا
    private static readonly Dictionary<string, Dictionary<string, string>> _messages = new()
    {
        ["fa"] = new()
        {
            ["NotFound"] = "موردی یافت نشد.",
            ["Validation"] = "ورودی نامعتبر است.",
            ["Unauthorized"] = "دسترسی غیرمجاز.",
            ["Conflict"] = "تعارض در داده‌ها وجود دارد.",
            ["Business"] = "خطای منطقی رخ داد.",
            ["Unhandled"] = "خطای غیرمنتظره‌ای رخ داد.",
            ["Forbidden"] = "شما اجازه دسترسی به این بخش را ندارید.",
            ["TooManyRequests"] = "درخواست‌های بیش‌ازحد ارسال شده است.",
            ["Timeout"] = "مهلت انجام درخواست به پایان رسید.",
            ["InvalidArgument"] = "پارامتر ارسالی نامعتبر است.",
            ["InvalidOperation"] = "عملیات در وضعیت فعلی مجاز نیست.",
            ["DbUpdate"] = "خطا در ذخیره‌سازی اطلاعات در پایگاه داده."
        },
        ["en"] = new()
        {
            ["NotFound"] = "Item not found.",
            ["Validation"] = "Invalid input.",
            ["Unauthorized"] = "Unauthorized access.",
            ["Conflict"] = "Data conflict occurred.",
            ["Business"] = "A business rule violation occurred.",
            ["Unhandled"] = "An unexpected error occurred.",
            ["Forbidden"] = "You do not have permission to access this resource.",
            ["TooManyRequests"] = "Too many requests.",
            ["Timeout"] = "Request timed out.",
            ["InvalidArgument"] = "Invalid argument provided.",
            ["InvalidOperation"] = "Operation not allowed.",
            ["DbUpdate"] = "Database update failed."
        },
        ["ar"] = new()
        {
            ["NotFound"] = "العنصر غير موجود.",
            ["Validation"] = "إدخال غير صالح.",
            ["Unauthorized"] = "وصول غير مصرح به.",
            ["Conflict"] = "حدث تعارض في البيانات.",
            ["Business"] = "حدث خطأ في منطق العمل.",
            ["Unhandled"] = "حدث خطأ غير متوقع.",
            ["Forbidden"] = "ليس لديك إذن للوصول إلى هذا المورد.",
            ["TooManyRequests"] = "طلبات كثيرة جداً.",
            ["Timeout"] = "انتهت مهلة الطلب.",
            ["InvalidArgument"] = "معامل غير صالح.",
            ["InvalidOperation"] = "العملية غير مسموح بها.",
            ["DbUpdate"] = "فشل تحديث قاعدة البيانات."
        }
    };

    public string Get(string key)
    {
        // دریافت زبان از HttpContext
        var lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        if (_messages.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out var msg))
            return msg;

        // زبان پیش‌فرض فارسی
        return _messages["fa"].TryGetValue(key, out var fallback) ? fallback : "خطا رخ داد.";
    }
}

