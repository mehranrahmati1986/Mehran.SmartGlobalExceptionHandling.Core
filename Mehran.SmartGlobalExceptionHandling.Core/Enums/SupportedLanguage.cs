using System.ComponentModel.DataAnnotations;

namespace Mehran.SmartGlobalExceptionHandling.Core.Enums;

public enum SupportedLanguage : byte
{
    [Display(Name = "فارسی")]
    Fa = 1,

    [Display(Name = "انگلیسی")]
    En = 2,

    [Display(Name = "عربی")]
    Ar = 3
}

