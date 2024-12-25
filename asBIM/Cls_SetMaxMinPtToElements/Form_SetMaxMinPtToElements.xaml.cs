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
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using MaterialDesignThemes;
using MaterialDesignColors;
using Notifications.Wpf;

namespace asBIM
{
    /// <summary>
    /// Логика взаимодействия для Form_SetMaxMinPtToElements.xaml
    /// </summary>
    public partial class Form_SetMaxMinPtToElements : Window
    {
        public Form_SetMaxMinPtToElements()
        {
            InitializeComponent();
        }

        private void Bt_AR_Click(object sender, RoutedEventArgs e)
        {
            // Метод SetElementsTBPoints
            Code_SetMaxMinPtToElements code = new Code_SetMaxMinPtToElements();
            
            // TODO: 3.1 // Метод SetElementsTBPoints по разделам через UI
            //code.SetElementsTBPoints(doc);
            Close();
        }

        private void Bt_EX_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        // TODO: 3. Добавить вывод инфо об плагине T&B

        private void Bt_Info_Click(object sender, RoutedEventArgs e)
        {
            Form_SetMaxMinPtToElements_Info info = new Form_SetMaxMinPtToElements_Info();
            info.ShowDialog();
            Close();
        }
    }
}
