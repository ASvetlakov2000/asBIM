using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Notifications.Wpf.Controls;
 using asBIM;
 using Autodesk.Revit.DB;

 namespace Notifications.Wpf
{
    public static class NotificationManagerWPF
    {
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "message" > Сообщение </param>
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Описание </param>
        /// </summary>
        public static void MessageSucces(string title, string description, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = description,
                Type = NotificationType.Success
            }, expirationTime: ts);
        }
        
        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "message" > Сообщение </param>
        /// <param name = "title" > Заголовок </param>
        /// <param name = "description" > Описание </param>
        /// </summary>
        public static void MessageInfo(string title, string description, NotificationType notificationType)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 10);

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = description,
                Type = NotificationType.Information
            }, expirationTime: ts);
        }

        /// <summary>
        /// Вывод окна Notifications.WPF c Сообщением
        /// <param name = "elementCount" > Количество элементов </param>
        /// <param name = "timeInSec" > Описание </param>
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
                Type = NotificationType.Information
            }, expirationTime: ts);

        }

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