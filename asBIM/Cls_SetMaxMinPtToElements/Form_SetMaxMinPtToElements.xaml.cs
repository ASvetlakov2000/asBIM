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
using Document = Autodesk.Revit.DB.Document;

namespace asBIM
{
    /// <summary>
    /// Логика взаимодействия для Form_SetMaxMinPtToElements.xaml
    /// </summary>
    public partial class Form_SetMaxMinPtToElements : Window
    {
        public bool Tuner(bool showMessage = true)
        {
            if (DialogResult == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Form_SetMaxMinPtToElements()
        {
            InitializeComponent();
        }

        public void Bt_Elem_Click(object sender, RoutedEventArgs e)
        {
            // Метод SetElementsTBPoints
            Code_SetMaxMinPtToElements code = new Code_SetMaxMinPtToElements();
            DialogResult = true;
            Close();
        }
        public void Bt_Linear_Click(object sender, RoutedEventArgs e)
        {
            // Метод SetElementsTBPoints_Null
            Tuner(false);
            //DialogResult = false;
            Close();
        }
        

        private void Bt_Info_Click(object sender, RoutedEventArgs e)
        {
            Form_SetMaxMinPtToElements_Info info = new Form_SetMaxMinPtToElements_Info();
            info.ShowDialog();
            //DialogResult = false;
        }

        private void Bt_ОК_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = false;
            Close();
        }
    }
}
