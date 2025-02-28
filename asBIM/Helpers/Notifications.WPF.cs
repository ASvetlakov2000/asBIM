using System;
using Notifications.Wpf;


namespace asBIM
{
    public static class NotificationManagerWPF
    {
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Сообщение </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void MessageSucces(string title, string description, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = description,
                Type = notificationType
            }, expirationTime: ts);
        }
        
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Сообщение </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void MessageInfo(string title, string description, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = description,
                Type = notificationType
            }, expirationTime: ts);
        }
        
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Сообщение </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void MessageInfoSmile(string title, string description, string smile, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = description + smile,
                Type = notificationType
            }, expirationTime: ts);
        }
        
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Сообщение </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void MessageSmileInfo(string title,  string smile, string description, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = smile + description,
                Type = notificationType
            }, expirationTime: ts);
        }
        
        public static void MessageSmileInfoTm(string title,  string smile, string description, NotificationType notificationType, int seconds)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, seconds);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = smile + description,
                Type = notificationType
            }, expirationTime: ts);
        }

        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "timeInSec" > Время работы команды </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void TimeOfWork(string title, string timeInSec, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = timeInSec,
                // Message = "Время выполнения " + timeInSec + "сек.",
                Type = notificationType
            }, expirationTime: ts);

        }

        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "title" > Заголовок </param>
        /// <param name = "elementCount" > Количество элементов </param>
        /// <param name = "notificationType" > Тип уведомления </param>
        /// </summary>
        public static void ElemCount(string title, string elementCount, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = elementCount,
                // Message = "\n\nКоличество обработанных элементов: " + elementCount,
                Type = notificationType
            }, expirationTime: ts);

        }
    }
}