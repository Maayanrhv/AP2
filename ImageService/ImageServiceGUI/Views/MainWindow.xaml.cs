using ImageServiceGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace ImageServiceGUI.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            Button btn = new Button();
            btn.Name = "btn1";
            btn.Click += btn1_Click;
        }
        public void btn1_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("aAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }
}
