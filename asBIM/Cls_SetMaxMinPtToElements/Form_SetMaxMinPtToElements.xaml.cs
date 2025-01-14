using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using asBIM.ViewModel;
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using MaterialDesignThemes;
using MaterialDesignColors;
using Notifications.Wpf;
using Document = Autodesk.Revit.DB.Document;
using CommunityToolkit.Mvvm.Input;

namespace asBIM
{
    /// <summary>
    /// Логика взаимодействия для Form_SetMaxMinPtToElements.xaml
    /// </summary>
    public partial class Form_SetMaxMinPtToElements : Window
    {
        // Было
        // Form_SetMaxMinPtToElements(SetMaxMinPtToElements_ViewModel vm);
        
        // GPT
        // Form_SetMaxMinPtToElements(UIApplication uiApp)
        
        
        public Form_SetMaxMinPtToElements(SetMaxMinPtToElements_ViewModel vm)
        {
            // Тест для MVVM
            InitializeComponent();
            // DataContext = vm;
            
            // Тест для настройки окон
            // InitializeComponent();
        }
        
        public void Bt_Elem_Click(object sender, RoutedEventArgs e)
        {
            // Тест для MVVM
            SetMaxMinPtToElements_ViewModel vm = (SetMaxMinPtToElements_ViewModel)sender;
            Close();
        }
        
        public void Bt_Linear_Click(object sender, RoutedEventArgs e)
        {
            // Тест для MVVM
            SetMaxMinPtToElements_ViewModel vm = (SetMaxMinPtToElements_ViewModel)sender;
            Close();
        }
        
        public void Bt_Info_Click(object sender, RoutedEventArgs e)
        {
            // Тест для MVVM
            Form_SetMaxMinPtToElements_Info info = new Form_SetMaxMinPtToElements_Info();
            info.Show();
            
            // Тест для настройки окон
            // Form_SetMaxMinPtToElements_Info info = new Form_SetMaxMinPtToElements_Info();
            // info.Show();
        }

        public void Bt_ОК_Click(object sender, RoutedEventArgs e)
        {
            // Тест для настройки окон
            Close();
        }
    }
}
