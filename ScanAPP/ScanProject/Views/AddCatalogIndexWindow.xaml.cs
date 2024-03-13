using LIBRA.Scan.Entities.ViewModels;
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
using System.Windows.Shapes;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for AddCatalogIndexWindow.xaml
    /// </summary>
    public partial class AddCatalogIndexWindow : Window
    {
        public AddCatalogIndexWindow()
        {
            InitializeComponent();
            List<CatalogIndexVM> list = new List<CatalogIndexVM>();
            list.Add(new CatalogIndexVM() { Name = "Báo cáo"});
            list.Add(new CatalogIndexVM() { Name = "Công văn" });
            list.Add(new CatalogIndexVM() { Name = "Nghị định" });
            list.Add(new CatalogIndexVM() { Name = "Nghị quyết" });
            list.Add(new CatalogIndexVM() { Name = "Biên bản" });
            list.Add(new CatalogIndexVM() { Name = "Quy chế" });
            lvTempData.ItemsSource = list;
        }
    }
}
