﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Notifications.Wpf.Controls;

namespace Notifications.Wpf
{
    public static class NotificationManagerWPF_SetMaxMinPtToElements
    {
        public static void Success(this string success)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 5);

            notificationManager.Show(new NotificationContent
            {
                Title = "Отметки Верха и Низа элементов \nзаписаны!", 
                Message = success, 
                Type = NotificationType.Success
            }, expirationTime: ts);
        }
        
        public static void Error(this string error)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 5);

            notificationManager.Show(new NotificationContent
            {
                Title = "Ошибка!", 
                Message = error, 
                Type = NotificationType.Error
            }, expirationTime: ts);
        }
    }
}