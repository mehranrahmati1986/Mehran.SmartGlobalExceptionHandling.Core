using System.ComponentModel.DataAnnotations;

namespace Mehran.SmartGlobalExceptionHandling.Core.Enums;

public enum SupportedLanguage : byte
{
    [Display(Name = "فارسی")]
    Fa = 1,

    [Display(Name = "انگلیسی")]
    En = 2,

    [Display(Name = "عربی")]
    Ar = 3,

    [Display(Name = "روسی")]
    ru = 4,

    [Display(Name = "چینی")]
    zh = 5,

    [Display(Name = "آلمانمی")]
    de = 6,

    [Display(Name = "فرانسوی")]
    fr = 7,

    [Display(Name = "ژاپنی")]
    ja = 8,

    [Display(Name = "کره ای")]
    ko = 9,

    [Display(Name = "هندی")]
    hi = 10,

    [Display(Name = "اردو")]
    ur = 11,

    [Display(Name = "اسپانیایی")]
    es = 12,
}

