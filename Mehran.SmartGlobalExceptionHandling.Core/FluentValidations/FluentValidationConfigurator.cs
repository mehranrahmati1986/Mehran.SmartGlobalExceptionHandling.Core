using FluentValidation;
using Mehran.SmartGlobalExceptionHandling.Core.Enums;
using System.Globalization;

namespace Mehran.SmartGlobalExceptionHandling.Core.FluentValidations;

internal static class FluentValidationConfigurator
{
    public static void ConfigureLanguage(SupportedLanguage language)
    {
        var cultureCode = language switch
        {
            SupportedLanguage.Fa => "fa",
            SupportedLanguage.En => "en",
            SupportedLanguage.Ar => "ar",
            _ => "fa"
        };

        var currentCulture = ValidatorOptions.Global.LanguageManager.Culture?.Name;

        if (!ValidatorOptions.Global.LanguageManager.Enabled || currentCulture != cultureCode)
        {
            ValidatorOptions.Global.LanguageManager.Enabled = true;
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo(cultureCode);
        }
    }
}