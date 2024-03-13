using LIBRA.Scan.Common.Common;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.ViewModels;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for DocumentManagerWindow.xaml
    /// </summary>
    public partial class DocumentManagerWindow : Window
    {
        private readonly IDocumentService _documentService;
        private readonly IPageService _pageService;

        public DocumentManagerWindow()
        {
            InitializeComponent();
            _documentService = new DocumentService();
            _pageService = new PageService();

            //get data
            GetListDocuments();
            CurrentDocument();
        }

        public void GetListDocuments()
        {
            List<object> return_data = new List<object>();

            IEnumerable<Document> documents = _documentService.GetDocuments();

            foreach (Document document in documents) 
            {
                var obj = new
                {
                    Id = document.Id,
                    Name = document.Name,
                    BatchId = document.Batch_Id,
                    UserId = document.User_Id,
                    AdministrativeDivision = document.Administrative_Division,
                    DocumentNo = document.Document_No,
                    DocumentYear = document.Document_Year,
                    DocumentType = document.DocumentType.Type,
                    DocScanStatus = document.StatusScan.Status,
                    ScannedDate = document.Scanned_Date,
                    RenderedDate = document.Rendered_Date,
                    ScannedImages = document.Scanned_Images,
                    Note = document.Note,
                    DocProcessStatus = document.StatusProcess.Status,
                    Path = document.Path,
                    Icode = document.Icode
                };
                return_data.Add(obj);
            }

            lstvDocuments.ItemsSource = null;
            lstvDocuments.ItemsSource = return_data;
        }

        private void lstvDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DocumentVM documentVM = ValueConvert.ConvertToObject<DocumentVM>(lstvDocuments.SelectedItem);

            MessageBoxResult result = System.Windows.MessageBox.Show($"Bạn muốn truy cập vào tài liệu: {documentVM.Name} ?", "Xác nhận", MessageBoxButton.OKCancel);
            
            if (result == MessageBoxResult.OK)
            {
                DocumentManager.SetDocumentId(documentVM.Id);
                DocumentManager.SetDocumentName(documentVM.Name);
                DocumentManager.SetDocumentPath(documentVM.Path);

                CurrentDocument();

              //  //get page 
              //LIBRA.Scan.Entities.LiteEntities.Document _document = _documentService.GetDocumentsByID(DocumentManager._documentId);
              //  // Get a reference to window2 by type
              //  var window2 = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
              //  int order = 1;
              //  // Access the property of window2
              //  var _mainViewModel = window2._mainViewModel;
              //  _mainViewModel.DocumentCategories = new ObservableCollection<Model.Document>();
              //  _mainViewModel.ListImageMain.Clear();
              //  window2.ListIMGSelected.Clear();
               
              //      ScanProject.Model.Document document = new ScanProject.Model.Document()
              //      {
              //          Id = _document.Id,
              //          Name = _document.Name,
              //          Pages = new ObservableCollection<Model.Page>()
              //      };
              //      var listPages = _pageService.GetByDocumentId((long)document.Id);
              //      foreach (var page in listPages)
              //      {
              //          var listimginfolder = new List<string>();
              //          ScanProject.Model.Page pagetemp = new Model.Page()
              //          {
              //              DocID = document.Id,
              //              PageID = (int)page.Id,
              //              PageName = page.Name,
              //              Images = new ObservableCollection<Model.Image>()
              //          };
              //          listimginfolder.AddRange(Directory.GetFiles(page.Path, "*.png", SearchOption.TopDirectoryOnly).ToList());
              //          var listIMG = imageService.GetByPageId(page.Id).ToList();

              //          foreach (var img in listIMG)
              //          {

              //              string[] temp = Path.GetFileNameWithoutExtension(img.Path).Split('_');
              //              if (temp[temp.Length - 1] == "3")
              //                  continue;
              //              Model.Image imgtemp = new Model.Image(img.Name, img.Path, page.Id, order);
              //              order++;

              //              _mainViewModel.ListImageMain.Add(imgtemp);

              //              BitmapImage image = new BitmapImage();
              //              image.BeginInit();
              //              image.CacheOption = BitmapCacheOption.OnLoad;
              //              image.UriSource = new Uri(img.Path);
              //              image.EndInit();
              //              imgtemp.bitmapImage = image;
              //              pagetemp.Images.Add(imgtemp);
              //          }
              //          foreach (string path in listimginfolder)
              //          {
              //              if (listIMG.Any(a => a.Path == path))
              //                  continue;
              //              else
              //              {
              //                  var img = new Model.Image(Path.GetFileName(path), path, page.Id, order);
              //                  pagetemp.Images.Add(img);
              //                  _mainViewModel.ListImageMain.Add(img);
              //                  BitmapImage image = new BitmapImage();
              //                  image.BeginInit();
              //                  image.CacheOption = BitmapCacheOption.OnLoad;
              //                  image.UriSource = new Uri(img.ImagePath);
              //                  image.EndInit();
              //                  img.bitmapImage = image;
              //              }
              //              order++;
              //          }
              //          document.Pages.Add(pagetemp);
              //      }
              //      _mainViewModel.DocumentCategories.Add(document);

                
            }

        }

        private void CurrentDocument()
        {
            var currentDocument = DocumentManager._documentName;

            if (string.IsNullOrWhiteSpace(currentDocument))
            {
                txtCurrentDocument.Text = "Bạn chưa lựa chọn tài liệu!";
            }
            else
            {
                txtCurrentDocument.Text = $"Tài liệu hiện tại: {currentDocument}";
            }
        }
    }
}
