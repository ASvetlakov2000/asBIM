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

namespace asBIM
{
    /// <summary>
    /// Логика взаимодействия для Form_SetParamertToVisibleElementInActiveView.xaml
    /// </summary>
    public partial class Form_SetParamertToVisibleElementInActiveView : Window
    {
        public Form_SetParamertToVisibleElementInActiveView()
        {
            InitializeComponent();
        }
   
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Code_SetParamertToVisibleElementInActiveView code = new Code_SetParamertToVisibleElementInActiveView();
            code.setParameter(tb_ParameterName.Text, tb_ParameterValue.Text);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
