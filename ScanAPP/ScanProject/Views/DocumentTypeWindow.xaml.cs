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
    /// Interaction logic for DocumentTypeWindow.xaml
    /// </summary>
    public partial class DocumentTypeWindow : Window
    {
        public DocumentTypeWindow()
        {
            InitializeComponent();
            List<DocumentTypeVM> list = new List<DocumentTypeVM>();
            list.Add(new DocumentTypeVM() { Name = "Cơ quan ban hành", DefaultValue = "<ocr:ocrcoquanbanhanh>" });
            list.Add(new DocumentTypeVM() { Name = "Số văn bản", DefaultValue = "<ocr:ocrsovanban>" });
            list.Add(new DocumentTypeVM() { Name = "Ngày văn bản", DefaultValue = "<ocr:ocrngayvanban>" });
            list.Add(new DocumentTypeVM() { Name = "Loại văn bản", DefaultValue = "<ocr:ocrlaoivanban>" });
            list.Add(new DocumentTypeVM() { Name = "Trích yếu", DefaultValue = "<ocr:ocrtrichyeu>" });
            list.Add(new DocumentTypeVM() { Name = "Kính gửi", DefaultValue = "<ocr:ocrkinhgui>" });
            list.Add(new DocumentTypeVM() { Name = "Nơi nhận", DefaultValue = "<ocr:ocrnoinhan>" });
            list.Add(new DocumentTypeVM() { Name = "Người ký", DefaultValue = "<ocr:ocrnguoiky>" });
            list.Add(new DocumentTypeVM() { Name = "Chức vụ", DefaultValue = "<ocr:ocrchucvu>" });
            lvTempData.ItemsSource = list;

            List<CatalogIndexVM> list2 = new List<CatalogIndexVM>();
            list2.Add(new CatalogIndexVM() { Name = "Văn bản hành chính (VBHC)" });
            list2.Add(new CatalogIndexVM() { Name = "Hóa đơn GTGT" });
            list2.Add(new CatalogIndexVM() { Name = "Hợp đồng thuê mặt bằng" });
            cbTemp.ItemsSource = list2;
            cbTemp.DisplayMemberPath = "Name";
        }


        private void AddIndex_Click(object sender, RoutedEventArgs e) 
        {
            AddCatalogIndexWindow addCatalogIndexWindow = new AddCatalogIndexWindow();
            addCatalogIndexWindow.ShowDialog();
        }
    }
}
