using System.Configuration;

namespace Fanex.NAP.SPU.Notification.Services
{
    public static class AppSetting
    {
        public static string TelegramToken => ConfigurationManager.AppSettings["telegram-token"] ?? "";

        public static string TelegramId => ConfigurationManager.AppSettings["telegram-id"] ?? "";

    }
}
