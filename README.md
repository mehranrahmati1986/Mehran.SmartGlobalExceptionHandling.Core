# Mehran.SmartGlobalExceptionHandling.Core - .NET Core Exception Management Package

---

## 🌐 English Documentation

### Overview

**Mehran.SmartGlobalExceptionHandling.Core** is a robust and extensible .NET Core middleware for capturing, handling, logging, and notifying unhandled exceptions. It is multilingual, customizable, and notification-friendly.

---

### ✅ Features

- 🚨 **Centralized Error Handling:** Capture system, business, network, and other exceptions.
- 🌍 **Built-in Localization:** Supports English, Persian, and Arabic, with an option to use custom localizers.
- 🔔 **Notification System:** Send error notifications via **Email**, **Slack**, and **Telegram**.
- 🧾 **Pluggable Logging Support:** Works with Console, Serilog, ELK, and other logging frameworks.
- 💾 **Optional Error Storage:** Save errors in SQL database.
- ⚙️ **Easy Setup and Extensibility:** Rapid integration and customization.
- ⭐ **Advanced FluentValidation Support**  
  Optional automatic language configuration for validation messages is provided.  
  *(Note: The Mehran.SmartGlobalExceptionHandling.Core version must be 1.0.6 or later to enable this feature.)*
- Since version 1.0.7, support for Chinese, Russian, French, German, Hindi, Japanese, Korean, Urdu, and Spanish has been added, along with many new exception types to make things easier for you.
- 🧠 This package will soon be equipped with artificial intelligence capabilities to assist you more intelligently in error analysis and diagnostics.
- Handling the following exceptions:
- ValidationException, BusinessException, NotFoundException, ArgumentNullException, UnauthorizedAccessException, ValidationException, PaymentRequiredException, TooManyRequestsException, RequestTimeoutException, InvalidOperationException, DatabaseUpdateException, DbUpdateException, MethodNotAllowedException, NotAcceptableException, ProxyAuthenticationRequiredException, GoneException, LengthRequiredException, PreconditionFailedException, PayloadTooLargeException, UriTooLongException, UnsupportedMediaTypeException, RangeNotSatisfiableException, ExpectationFailedException, ImATeapotException, AuthenticationTimeoutException, MisdirectedRequestException, UnprocessableEntityException, LockedException, FailedDependencyException, UpgradeRequiredException, PreconditionRequiredException, RequestHeaderFieldsTooLargeException, UnavailableForLegalReasonsException, ClientClosedRequestException, NotImplementedHttpException, BadGatewayException, ServiceUnavailableException, GatewayTimeoutException, HttpVersionNotSupportedException, VariantAlsoNegotiatesException, InsufficientStorageException, LoopDetectedException, NotExtendedException, NetworkAuthenticationRequiredException
- All exceptions can handle metadata
---

### 🛠 Installation

Install via NuGet:

```bash
Install-Package Mehran.SmartGlobalExceptionHandling.Core
```

---


### ⚙️ Configuration

#### Program.cs / Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // Register the exception handling services with custom options
    services.AddMehranExceptionHandling(options =>
    {
        options.ShowDetails = true;
        options.LogExceptions = true;
        options.StackTrace = false;
        options.Language = SupportedLanguage.En; // Change language (En, Fa, Ar, ...)
        options.HandleFluentValidationErrors = true;      // Enable FluentValidation error handling
        options.ConfigureFluentValidationLanguage = true;   // Automatically configure FluentValidation language
    });

    // Register your preferred notifiers
    services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
    services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
    services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

    services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
}

public void Configure(IApplicationBuilder app)
{
    app.UseCustomExceptionHandling();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

---

### 🚀 Usage Example

Below is an example of a controller triggering unhandled exceptions:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        throw new UnauthorizedAccessException("You are not authorized to access this resource.");
    }

    [HttpGet("custom")]
    public IActionResult GetCustomError()
    {
        throw new ApplicationException("This is a custom application exception.");
    }
}
```

#### Example Response (if ShowDetails = true)

```json
{
  "statusCode": 400,
  "message": "Validation failed.",
  "details": "Some required fields are missing.",
  "stackTrace": null,
  "errors": [
    {
      "field": "Email",
      "error": "Email is required."
    },
    {
      "field": "Password",
      "error": "Password must be at least 6 characters."
    }
  ],
  "traceId": "e7fa2bcf-4724-43de-9d5b-9e2c4d44473c",
  "timestamp": "2025-04-14T18:24:12.345Z",
  "metaData": {
    "userId": 123,
    "userName": "john_doe"
  },
 "fluentValidationErrors": {
    "Code": [
      "The country code must consist of numbers only."
    ]
  }
}
```

---

### 🔍 What is MetaData?

The **MetaData** property in the `ErrorResponse<T>` class is a generic container for including additional custom information with your error response.  
**Use Cases:**  
- Returning user information on authentication errors  
- Providing debug identifiers or environment details  
- Sending extra data related to a failed operation

For example:

```csharp
public class ErrorResponse<UserInfo>
{
    public UserInfo MetaData { get; set; }
}
```

---

### 🔎 ElasticSearch or Database Logging

Example of a custom logger:

```csharp
public class MyCustomLogger : IExceptionLogger
{
    public void Log(Exception exception)
    {
        // Send logs to ElasticSearch, file, or any other destination
    }
}
```

Register it:

```csharp
builder.Services.AddSingleton<IExceptionLogger, MyCustomLogger>();
```

---

### 📩 Notification Samples

#### 📧 Email Notifier

```json
// appsettings.json
{
  "EmailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "your_email@example.com",
    "Password": "your_password",
    "To": "alerts@yourdomain.com"
  }
}
```

```csharp
public class SmtpEmailNotifier : IExceptionNotifier
{
    private readonly EmailSettings _settings;

    public SmtpEmailNotifier(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task NotifyAsync(Exception exception, ErrorResponse response)
    {
        if (response.StatusCode == 500)
        {
            var message = new MailMessage();
            message.To.Add(_settings.To);
            message.Subject = "❗ Critical system error";
            message.Body = $"TraceId: {response.TraceId}\n{exception.Message}";

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.UseSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            await client.SendMailAsync(message);
        }
    }
}
```

#### 💬 Slack Notifier

```csharp
public class SlackNotificationNotifier : IExceptionNotifier
{
    private readonly string _webhookUrl;

    public SlackNotificationNotifier(string webhookUrl)
    {
        _webhookUrl = webhookUrl;
    }

    public async Task NotifyAsync(Exception exception, ErrorResponse response)
    {
        if (response.StatusCode == 500)
        {
            var message = new
            {
                text = $"❗ Critical system error\nMessage: {exception.Message}\nTraceId: {response.TraceId}\nTime: {response.Timestamp}"
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
            await client.PostAsync(_webhookUrl, content);
        }
    }
}
```

#### 📲 Telegram Notifier

```csharp
public class TelegramNotificationNotifier : IExceptionNotifier
{
    private readonly string _telegramToken;
    private readonly string _chatId;

    public TelegramNotificationNotifier(string telegramToken, string chatId)
    {
        _telegramToken = telegramToken;
        _chatId = chatId;
    }

    public async Task NotifyAsync(Exception exception, ErrorResponse response)
    {
        if (response.StatusCode == 500)
        {
            var message = $"❗ Critical error\nMessage: {exception.Message}\nTraceId: {response.TraceId}\nTime: {response.Timestamp}";
            var url = $"https://api.telegram.org/bot{_telegramToken}/sendMessage?chat_id={_chatId}&text={Uri.EscapeDataString(message)}";

            using var client = new HttpClient();
            await client.GetAsync(url);
        }
    }
}
```

---

### 🧩 Custom Localizer

```csharp
public class CustomErrorMessageLocalizer : IErrorMessageLocalizer
{
    public string Get(string key) => key switch
    {
        "NotFound" => "Resource not found.",
        "Validation" => "Invalid input.",
        _ => "An unexpected error occurred."
    };
}
```

Register it:

```csharp
services.AddSingleton<IErrorMessageLocalizer, CustomErrorMessageLocalizer>();
```

---

## 🌐 中文文档 (Chinese Documentation)
概述

Mehran.SmartGlobalExceptionHandling.Core 是一个强大且可扩展的.NET Core中间件，用于捕获、处理、记录和通知未处理的异常。它支持多语言、可定制且便于通知。
✅ 功能特点

    🚨 集中式错误处理： 捕获系统、业务、网络等各类异常。

    🌍 内置本地化： 支持英语、波斯语、阿拉伯语，并可自定义本地化器。

    🔔 通知系统： 通过电子邮件、Slack和Telegram发送错误通知。

    🧾 可插拔日志支持： 兼容Console、Serilog、ELK等日志框架。

    💾 可选错误存储： 将错误保存到SQL数据库。

    ⚙️ 易于设置和扩展： 快速集成和定制。

    ⭐ 高级FluentValidation支持
    可选自动配置验证消息的语言。
    (注意：需使用Mehran.SmartGlobalExceptionHandling.Core 1.0.6或更高版本。)

    自1.0.7版本起，新增支持中文、俄语、法语、德语、印地语、日语、韩语、乌尔都语和西班牙语，并添加了许多新的异常类型以便使用。

    🧠 该包即将加入人工智能功能，以更智能地协助错误分析和诊断。

    支持处理的异常类型：
    ValidationException, BusinessException, NotFoundException等（完整列表见原文）。

    所有异常均可处理元数据。

🛠 安装

通过NuGet安装：
bash

Install-Package Mehran.SmartGlobalExceptionHandling.Core

⚙️ 配置
Program.cs / Startup.cs
csharp

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    // 注册异常处理服务
    services.AddMehranExceptionHandling(options =>
    {
        options.ShowDetails = true;
        options.LogExceptions = true;
        options.StackTrace = false;
        options.Language = SupportedLanguage.Zh; // 设置语言为中文
        options.HandleFluentValidationErrors = true;
        options.ConfigureFluentValidationLanguage = true;
    });

    // 注册通知服务
    services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
    services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
    services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

    services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
}

---

## 🌐 Русская документация (Russian Documentation)
Обзор

Mehran.SmartGlobalExceptionHandling.Core - это мощное и расширяемое промежуточное ПО для .NET Core, предназначенное для захвата, обработки, логирования и уведомления о необработанных исключениях. Поддерживает многоязычность, настройку и систему уведомлений.
✅ Возможности

    🚨 Централизованная обработка ошибок: Захват системных, бизнес, сетевых и других исключений.

    🌍 Встроенная локализация: Поддержка английского, персидского, арабского языков с возможностью добавления кастомных локализаторов.

    🔔 Система уведомлений: Отправка уведомлений через Email, Slack и Telegram.

    🧾 Поддержка логирования: Совместимость с Console, Serilog, ELK и другими фреймворками.

    💾 Хранение ошибок: Возможность сохранения ошибок в SQL базу данных.

    ⚙️ Простая настройка: Быстрая интеграция и кастомизация.

    ⭐ Поддержка FluentValidation
    Автоматическая настройка языка для сообщений валидации.
    (Требуется версия 1.0.6 или выше.)

    Начиная с версии 1.0.7 добавлена поддержка китайского, русского, французского, немецкого, хинди, японского, корейского, урду и испанского языков, а также новые типы исключений.

    🧠 В ближайшее время пакет будет оснащен ИИ-функциями для интеллектуального анализа ошибок.

    Поддерживаемые исключения:
    ValidationException, BusinessException, NotFoundException и другие (полный список в оригинале).

    Все исключения поддерживают метаданные.

🛠 Установка

Через NuGet:
bash

Install-Package Mehran.SmartGlobalExceptionHandling.Core

⚙️ Настройка
Program.cs / Startup.cs
csharp

public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddMehranExceptionHandling(options =>
    {
        options.ShowDetails = true;
        options.LogExceptions = true;
        options.StackTrace = false;
        options.Language = SupportedLanguage.Ru; // Русский язык
        options.HandleFluentValidationErrors = true;
        options.ConfigureFluentValidationLanguage = true;
    });

    services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
    services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
    services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

    services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
}

---

## 🌐 🇮🇷 راهنمای فارسی

### معرفی

**Mehran.SmartGlobalExceptionHandling.Core** یک پکیج قدرتمند برای مدیریت و کنترل خطاها در پروژه‌های .NET Core است. این پکیج قابلیت ثبت، گزارش‌گیری و ارسال اعلان خطاها (با پشتیبانی از چند زبان: فارسی، انگلیسی و عربی) را فراهم می‌کند.

---

### ✅ ویژگی‌ها

- 🚨 **مدیریت متمرکز خطا:** پشتیبانی از خطاهای سیستمی، تجاری، شبکه‌ای و غیره.
- 🌍 **پشتیبانی چندزبانه:** پیام‌های خطا به زبان‌های فارسی، انگلیسی و عربی.  
- 🔔 **سیستم اعلان:** ارسال اعلان خطا از طریق ایمیل، تلگرام و Slack.
- 🧾 **لاگ‌گیری پلاگین‌پذیر:** پشتیبانی از لاگ‌گیری با کنسول، Serilog، ELK و سایر ابزارها.
- 💾 **ذخیره اختیاری:** امکان ذخیره خطاها در پایگاه داده.
- ⚙️ **تنظیم و توسعه آسان:** راه‌اندازی سریع و تنظیمات سفارشی.
- ⭐ **پشتیبانی پیشرفته از FluentValidation**  
  پیکربندی خودکار اختیاری زبان پیام‌های اعتبارسنجی بر مبنای تنظیمات انتخاب‌شده.  
  *(توجه: پکیج Mehran.SmartGlobalExceptionHandling.Core باید نسخه ۱.۰.۶ یا بالاتر باشد.)*
  -  از نسخه 1.0.7 پشتیبانی از زبان های چینی روسی، فرانسه، آلمانی، هندی، ژاپنی، کره ای، اردو و اسپانیایی هم برای شما فراهم شده و کلی اکسپشن های جدید رو بهش اضافه کردم برای راحتی کار شما عزیزان
  - 🧠 به‌زودی این پکیج به قابلیت‌های هوش مصنوعی مجهز خواهد شد تا در تحلیل و بررسی خطاها، بهتر و هوشمندانه‌تر به شما کمک کند.
  - تمامی اکسپشن‌ها قابلیت پشتیبانی از متادیتا را دارند.

---

### 🛠 نصب

```bash
Install-Package Mehran.SmartGlobalExceptionHandling.Core
```

---

### ⚙️ تنظیمات اولیه

```csharp
services.AddExceptionHandling(options =>
{
    options.ShowDetails = true;
    options.LogExceptions = true;
    options.StackTrace = false;
    options.Language = SupportedLanguage.En; // Change language (En, Fa, Ar)
    options.HandleFluentValidationErrors = true;      // Enable FluentValidation error handling
    options.ConfigureFluentValidationLanguage = true;   // Automatically configure FluentValidation language
 });
});

services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
```

---

### 🚀 نمونه استفاده

یک کنترلر نمونه برای تست مدیریت خطا:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        throw new UnauthorizedAccessException("شما مجاز به دسترسی به این بخش نیستید.");
    }

    [HttpGet("custom")]
    public IActionResult GetCustomError()
    {
        throw new ApplicationException("این یک خطای سفارشی برنامه است.");
    }
}
```

#### مثال پاسخ (در صورت فعال بودن ShowDetails)

```json
{
  "statusCode": 400,
  "message": "اعتبارسنجی انجام نشد.",
  "details": "برخی فیلدهای ضروری وارد نشده‌اند.",
  "stackTrace": null,
  "errors": [
    {
      "field": "Email",
      "error": "ایمیل الزامی است."
    },
    {
      "field": "Password",
      "error": "رمز عبور باید حداقل ۶ کاراکتر باشد."
    }
  ],
  "traceId": "e7fa2bcf-4724-43de-9d5b-9e2c4d44473c",
  "timestamp": "2025-04-14T18:24:12.345Z",
  "metaData": {
    "userId": 123,
    "userName": "john_doe"
  }
},
"fluentValidationErrors": {
    "Code": [
      "کد کشور باید فقط شامل اعداد باشد."
    ]
  }
```

---

### 🔍 متا دیتا چیست؟

ویژگی **MetaData** در کلاس `ErrorResponse<T>` به‌عنوان یک پارامتر جنریک به شما اجازه می‌دهد اطلاعات اضافی یا سفارشی را همراه با پاسخ خطا ارسال کنید.  
**کاربردها:**  
- ارسال اطلاعات کاربر هنگام خطای ورود  
- افزودن اطلاعات اشکال‌زدایی یا محیط اجرا  
- ارسال داده‌های مرتبط با عملیاتی که با خطا مواجه شده است

به عنوان مثال:

```csharp
public class ErrorResponse<UserInfo>
{
    public UserInfo MetaData { get; set; }
}
```

---

### 🔎 ElasticSearch یا پایگاه داده

مثال یک لاگر سفارشی:

```csharp
public class MyCustomLogger : IExceptionLogger
{
    public void Log(Exception exception)
    {
        // ارسال لاگ به ElasticSearch یا هر سرویس دیگری
    }
}
```

ثبت لاگر:

```csharp
builder.Services.AddSingleton<IExceptionLogger, MyCustomLogger>();
```

---

### فایل تنظیمات appsettings.json

```json
{
  "EmailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "your_email@example.com",
    "Password": "your_password",
    "To": "alerts@yourdomain.com"
  },
  "SlackSettings": {
    "WebhookUrl": "https://hooks.slack.com/services/xxxx/yyyy/zzzz"
  },
  "TelegramSettings": {
    "TelegramToken": "your_telegram_token",
    "ChatId": "your_telegram_chat_id"
  }
}
```

---

### نمونه‌های اعلان

#### 📧 ایمیل

```csharp
public class SmtpEmailNotifier : IExceptionNotifier
{
    // مشابه نسخه انگلیسی
}
```

#### 💬 Slack

```csharp
public class SlackNotificationNotifier : IExceptionNotifier
{
    // مشابه نسخه انگلیسی
}
```

#### 📲 تلگرام

```csharp
public class TelegramNotificationNotifier : IExceptionNotifier
{
    // مشابه نسخه انگلیسی
}
```

Made with ❤️ by Mehran Ghaederahmat

---

## 🌐 🇸🇦 الوثائق باللغة العربية

### مقدمة

حزمة **Mehran.SmartGlobalExceptionHandling.Core** توفر إدارة مركزية وشاملة للأخطاء في تطبيقات .NET Core مع دعم متعدد اللغات وإرسال التنبيهات.

---

### ✅ الميزات

- 🚨 **إدارة مركزية للاستثناءات:** التقاط وإدارة جميع أنواع الأخطاء (النظامية، التجارية، الشبكية، وغيرها).
- 🌍 **دعم متعدد اللغات:** رسائل خطأ بلغات عربية، فارسية وإنجليزية.
- 🔔 **نظام التنبيهات:** إرسال تنبيهات عبر البريد الإلكتروني، Telegram و Slack.
- 🧾 **دعم تسجيل مرن:** متوافق مع أنظمة تسجيل متعددة مثل Serilog، ELK والمزيد.
- 💾 **تخزين اختياري:** إمكانية حفظ الأخطاء في قواعد البيانات.
- ⚙️ **تكوين وتوسعة سهلة:** إعداد سريع مع إمكانية تعديل الإعدادات.
- ⭐ **دعم متقدم لـ FluentValidation**  
  يحتوي على تكوين تلقائي اختياري للغة رسائل التحقق بناءً على الإعدادات.  
  *(تنبيه: يجب أن تكون نسخة Mehran.SmartGlobalExceptionHandling.Core المكتبة 1.0.6 أو أحدث.)*
  - اعتبارًا من الإصدار 1.0.7، تمت إضافة دعم للغات الصينية، الروسية، الفرنسية، الألمانية، الهندية، اليابانية، الكورية، الأردية، والإسبانية، بالإضافة إلى العديد من أنواع الاستثناءات الجديدة لتسهيل الأمور عليكم.
  - 🧠 قريبًا سيتم تزويد هذه الحزمة بقدرات الذكاء الاصطناعي لمساعدتك بشكل أكثر ذكاءً في تحليل وتشخيص الأخطاء.
  - يمكن لجميع الاستثناءات التعامل مع البيانات الوصفية

---

### 🛠 التثبيت

```bash
Install-Package Mehran.SmartGlobalExceptionHandling.Core
```

---

### ⚙️ الإعداد

```csharp
services.AddExceptionHandling(options =>
{
    options.ShowDetails = true;
    options.LogExceptions = true;
    options.StackTrace = false;
    options.Language = SupportedLanguage.En; // Change language (En, Fa, Ar)
    options.HandleFluentValidationErrors = true;      // Enable FluentValidation error handling
    options.ConfigureFluentValidationLanguage = true;   // Automatically configure FluentValidation language
});
});

services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
```

---

### 🚀 مثال للاستخدام

أضف Controller للاختبار كما يلي:

```csharp
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        throw new UnauthorizedAccessException("ليست لديك صلاحية للوصول إلى هذا المورد.");
    }

    [HttpGet("custom")]
    public IActionResult GetCustomError()
    {
        throw new ApplicationException("هذا خطأ مخصص من التطبيق.");
    }
}
```

#### مثال الاستجابة (في حال تفعيل ShowDetails)

```json
{
  "statusCode": 400,
  "message": "فشل التحقق من الصحة.",
  "details": "بعض الحقول المطلوبة مفقودة.",
  "stackTrace": null,
  "errors": [
    {
      "field": "Email",
      "error": "البريد الإلكتروني مطلوب."
    },
    {
      "field": "Password",
      "error": "يجب أن تكون كلمة المرور مكونة من 6 أحرف على الأقل."
    }
  ],
  "traceId": "e7fa2bcf-4724-43de-9d5b-9e2c4d44473c",
  "timestamp": "2025-04-14T18:24:12.345Z",
  "metaData": {
    "userId": 123,
    "userName": "john_doe"
  }
},
"fluentValidationErrors": {
    "Code": [
      "يجب أن يتكون رمز الدولة من أرقام فقط."
    ]
  }
```

---

### 🔍 ما هو MetaData؟

خاصية **MetaData** في الكلاس `ErrorResponse<T>` تُستخدم لإرفاق معلومات إضافية أو مخصصة مع رسالة الخطأ.  
**الاستخدامات:**  
- إرسال معلومات المستخدم عند فشل عملية تسجيل الدخول.  
- تضمين بيانات التصحيح أو معلومات البيئة.  
- إرسال بيانات إضافية متعلقة بالعملية التي فشلت.

على سبيل المثال:

```csharp
public class ErrorResponse<UserInfo>
{
    public UserInfo MetaData { get; set; }
}
```

---

### 🔎 ElasticSearch أو قاعدة بيانات

مثال على كلاس Logger مخصص:

```csharp
public class MyCustomLogger : IExceptionLogger
{
    public void Log(Exception exception)
    {
        // إرسال السجلات إلى ElasticSearch أو أي خدمة أخرى
    }
}
```

وتسجيله:

```csharp
builder.Services.AddSingleton<IExceptionLogger, MyCustomLogger>();
```

---

### إعدادات ملف appsettings.json

```json
{
  "EmailSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "your_email@example.com",
    "Password": "your_password",
    "To": "alerts@yourdomain.com"
  },
  "SlackSettings": {
    "WebhookUrl": "https://hooks.slack.com/services/xxxx/yyyy/zzzz"
  },
  "TelegramSettings": {
    "TelegramToken": "your_telegram_token",
    "ChatId": "your_telegram_chat_id"
  }
}
```

---

### أمثلة على التنبيهات

#### 📧 البريد الإلكتروني

```csharp
public class SmtpEmailNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```

#### 💬 Slack

```csharp
public class SlackNotificationNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```

#### 📲 Telegram

```csharp
public class TelegramNotificationNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```

Made with ❤️ by Mehran Ghaederahmat

---
