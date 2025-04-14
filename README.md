راهنما به 3 زبان انگلیسی فارسی و عربی

---

## 🌐 English Documentation

# Mehran.SmartGlobalExceptionHandling.Core - .NET Core Exception Management Package

### Overview

**Mehran.SmartGlobalExceptionHandling.Core** is a robust and extensible .NET Core middleware for capturing, handling, logging, and notifying unhandled exceptions. It is multilingual, customizable, and notification-friendly.

---

### ✅ Features

- 🚨 Centralized error handling for system, business, and network exceptions
- 🌍 Built-in localization (English, Persian, Arabic) and support for custom localizers
- 🔔 Notification system via **Email**, **Slack**, and **Telegram**
- 🧾 Pluggable logging support (Console, Serilog, ELK, etc.)
- 💾 Optional storage of errors in SQL database
- ⚙️ Easy setup and extensibility

---

### 🛠 Installation

```bash
Install-Package Mehran.SmartGlobalExceptionHandling.Core
```

---

### ⚙️ Configuration

#### Program.cs / Startup.cs

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddExceptionHandling(options =>
    {
        options.ShowDetails = true;
        options.LogExceptions = true;
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
}
```

### 📩 Notification Samples

#### 📧 Email Notifier

```csharp
### 🔧 appsettings.json

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
```
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

## 🌐 🇮🇷 راهنمای فارسی

### معرفی

پکیج **Mehran.SmartGlobalExceptionHandling.Core** برای مدیریت و کنترل خطاها در پروژه‌های .NET Core طراحی شده است. این پکیج قابلیت ثبت، گزارش، و ارسال اعلان در صورت وقوع خطا را دارد.

---

### ✅ ویژگی‌ها

- 🚨 پشتیبانی از خطاهای سیستمی، تجاری، شبکه‌ای و...
- 🌍 پشتیبانی از سه زبان (فارسی، انگلیسی، عربی)
- 🔔 ارسال اعلان از طریق ایمیل، تلگرام و Slack
- 🧾 امکان لاگ‌گیری با سری لاگ، ELK و لاگر سفارشی
- 💾 ذخیره خطاها در دیتابیس
- ⚙️ قابل توسعه و سفارشی‌سازی

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
});

services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
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

```csharp
public class SmtpEmailNotifier : IExceptionNotifier
{
    // مشابه انگلیسی
}
```

💬 **Slack**:

```csharp
public class SlackNotificationNotifier : IExceptionNotifier
{
    // مشابه انگلیسی
}
```

📲 **تلگرام**:

```csharp
public class TelegramNotificationNotifier : IExceptionNotifier
{
    // مشابه انگلیسی
}
```

---

## 🌐 🇸🇦 الوثائق باللغة العربية

### مقدمة

حزمة **Mehran.SmartGlobalExceptionHandling.Core** توفر إدارة مركزية وشاملة للأخطاء في تطبيقات .NET Core مع دعم متعدد اللغات وإرسال التنبيهات.

---

### ✅ الميزات

- 🚨 إدارة جميع أنواع الاستثناءات
- 🌍 دعم للغة العربية والفارسية والإنجليزية
- 🔔 تنبيهات عبر البريد الإلكتروني، Slack، Telegram
- 🧾 دعم أنظمة تسجيل مختلفة
- ⚙️ قابل للتخصيص والتوسيع

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
});

services.AddSingleton<IExceptionNotifier, SmtpEmailNotifier>();
services.AddSingleton<IExceptionNotifier, SlackNotificationNotifier>();
services.AddSingleton<IExceptionNotifier, TelegramNotificationNotifier>();

services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
```

---

### إعدادات `appsettings.json`

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

📧 **البريد الإلكتروني**:

```csharp
public class SmtpEmailNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```

💬 **Slack**:

```csharp
public class SlackNotificationNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```

📲 **Telegram**:

```csharp
public class TelegramNotificationNotifier : IExceptionNotifier
{
    // كما هو موضح في القسم الإنجليزي
}
```
Made with ❤️ by Mehran Ghaederahmat
---

