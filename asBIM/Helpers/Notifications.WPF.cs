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
    public static class NotificationManagerWPF_SetMaxMinPtToElements
    {
        public static void Success_For_Elem(this string success)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 8);

            notificationManager.Show(new NotificationContent
            {
                Title = "Афигеть, оно работает!", 
                Message = "\n(￢‿￢ )\n\nОтметки Верха и Низа для единичных \nэлементов записаны!", 
                Type = NotificationType.Success
            }, expirationTime: ts);
        }
        
        public static void Success_For_Linear(this string success)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 8);

            notificationManager.Show(new NotificationContent
            {
                Title = "Афигеть, оно работает!", 
                Message = "\n(￢‿￢ )\n\nОтметки в Начале и Отметки в Конце\nдля линейных объектов записаны!", 
                Type = NotificationType.Success
            }, expirationTime: ts);
        }
        
        public static void Error(this string error)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 8);

            notificationManager.Show(new NotificationContent
            {
                Title = "Упс..!", 
                Message = "\n\u00af\\_(ツ)_/\u00af\nЧто-то пошло не так. \n\nТы знаешь кому писать :)", 
                Type = NotificationType.Error
            }, expirationTime: ts);
        }

        public static void TimeOfWork(this string elementCount, string timeInSec)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 8);
            
            notificationManager.Show(new NotificationContent
                {
                    Title = "Время выполнения",
                    Message ="Время выполнения " + timeInSec + "сек" + "\n\nКоличество обработанных элементов: " + elementCount,
                    Type = NotificationType.Information
                }, expirationTime: ts);
                
        }
        
        public static void SychPls (this string sychPls)
        {
            var notificationManager = new NotificationManager();
            var ts = new TimeSpan(0, 0, 8);

            notificationManager.Show(new NotificationContent
            {
                Title = "Синхронизируйся!", 
                Message = "\n\u00af\\_(ツ)_/\u00af\n\nПеред выполнением - синхронизируйся!\nРазработано дилетантом с CHAT GPT :)", 
                Type = NotificationType.Information
            }, expirationTime: ts);
        }
        
    }
}