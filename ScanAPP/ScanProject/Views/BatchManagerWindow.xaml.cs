using LIBRA.Scan.Common.Common;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using Notification.Wpf;
﻿using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.Common.Setting;
using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;

using ScanProject.ViewModels;
using ScanProject.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Media.Imaging;
using Window = System.Windows.Window;
using LIBRA.Scan.Entities.ViewModels;
using Page = LIBRA.Scan.Entities.LiteEntities.Page;
using System.Security.Cryptography.Pkcs;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace ScanProject.View
{
    /// <summary>
    /// Interaction logic for BatchManagerWindow.xaml
    /// </summary>
    public partial class BatchManagerWindow : Window
    {  
        private readonly IBatchService _batchService;
        private readonly IProcessService _processService;
        private readonly IDocumentService _documentService;
        private readonly IPageService _pageService;
        private readonly IImageService _imageService;
        private readonly IPdfService _pdfService;

        private readonly NotificationManager _notificationManager;

        public BatchManagerWindow()
        {
            InitializeComponent();

            //setting
            this.ResizeMode = ResizeMode.NoResize;

            //declare
            _batchService = new BatchService();
            _processService = new ProcessService();
            _notificationManager = new NotificationManager();
            _documentService = new DocumentService();
            _pageService = new PageService();
            _imageService = new ImageService();
            _pdfService = new PdfService();

            //get data
            GetListBatches();
            CurrentBatch();
            CurrentDocument();
        }
        private void CreateDocument_Click(object sender, RoutedEventArgs e)
        {
            long? batchId = BatchManager._batchId;

            if (batchId == null)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng chọn gói trước khi khởi tạo tài liệu mới!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            DocumentCreateWindow documentCreate = new DocumentCreateWindow();
            DocumentManager._documentIsEdit = false;
            documentCreate.ShowDialog();
        }

        public void GetListBatches()
        {
            List<object> return_data = new List<object>();

            long userId = UserManager._userId;

            IEnumerable<Batch> batches = _batchService.GetBatchesByUserId(userId);

            foreach (Batch batch in batches) 
            {
                var obj = new
                {
                    Id = batch.Id,
                    Name = batch.Name,
                    Path = batch.Path,
                    Note = batch.Note,
                    Created = batch.Created_Date,
                    Status = batch.StatusBatch.Status
                };
                return_data.Add(obj);
            }

            lstvBatches.ItemsSource = null;
            lstvBatches.ItemsSource = return_data;

            lstvBatchesFixed.ItemsSource = null;
            lstvBatchesFixed.ItemsSource = return_data;
        }

        private void CurrentBatch()
        {
            var currentBatch = BatchManager._batchName;

            if(string.IsNullOrWhiteSpace(currentBatch))
            {
                txtCurrentBatch.Text = "Bạn chưa lựa chọn gói!";
            }
            else 
            {
                txtCurrentBatch.Text = $"{currentBatch}";
            }
        }

        private void lstvBatches_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstvBatches.SelectedItem == null)
                return;

            LIBRA.Scan.Entities.ViewModels.BatchVM batchVM = ValueConvert.ConvertToObject<LIBRA.Scan.Entities.ViewModels.BatchVM>(lstvBatches.SelectedItem);
            
            MessageBoxResult result = System.Windows.MessageBox.Show($"Bạn muốn truy cập vào gói: {batchVM.Name} ?", "Xác nhận", MessageBoxButton.OKCancel);

            if(result == MessageBoxResult.OK)
            {
                BatchManager.SetBatchId(batchVM.Id);
                BatchManager.SetBatchName(batchVM.Name);

                string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                BatchManager.SetBatchPath(Path.Combine(userFolderPath, batchVM.Path));

                CurrentBatch();
                GetListDocumentsById(batchVM.Id);
            }
        }

        private async void btnFinishScan_Click(object sender, RoutedEventArgs e)
        {
            string? filePath = BatchManager._batchPath;

            if (String.IsNullOrEmpty(filePath))
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng lựa chọn gói tài liệu bạn muốn tiến hành!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            if (!Directory.Exists(filePath))
            {
                System.Windows.Forms.MessageBox.Show("Tệp tin không tồn tại!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            long? documentId = DocumentManager._documentId;

            if (documentId != null)
            {
                if(!_pdfService.ConvertBatchToPdf((long)documentId))
                {
                    System.Windows.Forms.MessageBox.Show("Không thể chuyển về định dạng tệp tin PDF", "Thông báo!!", MessageBoxButtons.OK);
                    return;
                }
            }

            string batchDirectory = filePath.Replace(FolderSetting.TempData, FolderSetting.Transfer);

            string zipPath = batchDirectory + ".zip";

            try
            {
                await _processService.TransferFile(batchDirectory, zipPath);
                await Task.Delay(3000);
            }
            catch (Exception)
            {
                var error = new NotificationContent
                {
                    Title = "Thông báo!!",
                    Message = "Có lỗi xảy ra trong tiến trình!! Hủy tiến trình...",
                    Background = new SolidColorBrush(Colors.Red),
                    Foreground = new SolidColorBrush(Colors.White),
                };

                _notificationManager.Show(error);
                return;
            }

            var content = new NotificationContent
            {
                Title = "Thông báo!!",
                Message = "Hoàn thành tiến trình!!",
                Background = new SolidColorBrush(Colors.Green),
                Foreground = new SolidColorBrush(Colors.White),
            };

            _notificationManager.Show(content);
        }

        private string CheckBatchCreateField()
        {
            string notification = string.Empty;
            if (txtBatchName.Text.Trim() == "")
                notification += "Tên gói không được để trống! \n";
            return notification;
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
            if(lstvDocuments.SelectedItem == null)
                return;

            DocumentVM documentVM = ValueConvert.ConvertToObject<DocumentVM>(lstvDocuments.SelectedItem);
            MessageBoxResult result = System.Windows.MessageBox.Show($"Bạn muốn truy cập vào tài liệu: {documentVM.Name} ?", "Xác nhận", MessageBoxButton.OKCancel);
         
            if (result == MessageBoxResult.OK)
            {
                DocumentManager.SetDocumentId(documentVM.Id);
                DocumentManager.SetDocumentName(documentVM.Name);
                //string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                //DocumentManager.SetDocumentPath(Path.Combine(userFolderPath, documentVM.Path));
                DocumentManager.SetDocumentPath(documentVM.Path);
                CurrentDocument();
                UpdateImagesByDocumentId(documentVM.Id);
            }
        }

        public void UpdateImagesByDocumentId(long documentId)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            var documentCategories = mainWindow._mainViewModel.DocumentCategories.FirstOrDefault(a => a.Id == documentId);

            if (documentCategories != null)
                documentCategories.Pages.Clear();

            mainWindow._mainViewModel.ListImageMain.Clear();
            List<string> errorList = new List<string>();
            var listPages = GetListPageByDocID(documentId);
            foreach (var page in listPages)
            {
                if (documentCategories != null)
                {
                    documentCategories.Pages.Add(page);
                    var documentPages = documentCategories.Pages.FirstOrDefault(x => x.PageID == page.PageID);
                    if (documentPages != null)
                    {
                        var listImages = GetListImageByPageId(page.PageID);
                        foreach (var image in listImages)
                        {
                            Model.Image imgScanned = new Model.Image();
                            imgScanned.Id = image.Id;
                            imgScanned.Name = $"Ảnh {image.Order}";
                            imgScanned.ImagePath = image.ImagePath;
                            imgScanned.PageID = page.PageID;
                            imgScanned.PageIcode = page.Icode;
                            imgScanned.Order = image.Order;
                            try
                            {
                                imgScanned.bitmapImage = ImagePathToBitmap(image.ImagePath);
                            }
                            catch (Exception)
                            {
                                errorList.Add(imgScanned.Name);
                                continue;
                            }
                            documentPages.Images.Add(imgScanned);
                            mainWindow._mainViewModel.ListImageMain.Add(imgScanned);
                        }
                    }
                }
            }

            if(errorList.Count > 0)
            {
                string notification = "Trong quá trình xử lý và hiển thị, có các ảnh bị lỗi hoặc không tồn tại trong hệ thống bao gồm: \n";
                foreach (var error in errorList) 
                {
                    notification += error + "\n";
                }
                System.Windows.Forms.MessageBox.Show(notification, "Thông báo!!", MessageBoxButtons.OK);
            }
        }

        private BitmapImage ImagePathToBitmap(string imagePath)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);
            BitmapImage tempImage = ConvertWriteableBitmapToBitmapImage(writeableBitmap);
            return tempImage;
        }

        private BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap writeableBitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (var stream = new System.IO.MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                encoder.Save(stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }

        private void CurrentDocument()
        {
            var currentDocument = DocumentManager._documentName;
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            if (string.IsNullOrWhiteSpace(currentDocument))
            {
                txtCurrentDocument.Text = "Bạn chưa lựa chọn tài liệu!";
                mainWindow.lblCurrentDocument.Content = String.Empty;
            }
            else
            {
                mainWindow.lblCurrentDocument.Content = currentDocument;
                txtCurrentDocument.Text = $"{currentDocument}";
            }
        }

        public void GetListDocumentsById(long batchId)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;

            List<object> return_data = new List<object>();
            mainWindow._mainViewModel.DocumentCategories.Clear();
            mainWindow._mainViewModel.ListImageMain.Clear();
            IEnumerable<Document> documents = _documentService.GetDocumentsByBatchId(batchId);

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
                    DocumentTypeId = document.DocumentType.Id,
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

                Model.Document document1 = new Model.Document();
                document1.Id = document.Id;
                document1.Name = document.Name;
                document1.Pages = new ObservableCollection<Model.Page>();
                document1.DocumentPath = document.Path;

                mainWindow._mainViewModel.DocumentCategories.Add(document1);
               
            }

            lstvDocuments.ItemsSource = null;
            lstvDocuments.ItemsSource = return_data;

            lstvDocumentsFixed.ItemsSource = null;
            lstvDocumentsFixed.ItemsSource = return_data;
        }

        public List<Model.Page> GetListPageByDocID(long docId)
        {
            IEnumerable<Page> pages = _pageService.GetByDocumentId(docId);
            List<Model.Page> pagesModel = new List<Model.Page>();

            foreach (Page item in pages)
            {
                Model.Page page = new Model.Page()
                {
                    PageName = item.Name,
                    PageID = (int)item.Id,
                    DocID = docId,
                    Icode = item.Icode,
                    PageNum = (int)item.Number_Order,
                    PagePath = item.Path,
                    Images = new ObservableCollection<Model.Image>()
                };
                pagesModel.Add(page);
            }

            return pagesModel;
        }

        public List<Model.Image> GetListImageByPageId(long pageId)
        {
            var images = _imageService.GetByPageId(pageId);
            List<Model.Image> imagesModel = new List<Model.Image>();

            foreach (var image in images)
            {
                Model.Image imageModel = new Model.Image()
                {
                    Id = (int)image.Id,
                    Name = image.Name,
                    ImagePath = image.Path,
                    Order = (int)(image.Number_Order != null ? image.Number_Order : 0),
                };
                imagesModel.Add(imageModel);
            }

            return imagesModel;
        }

        private async void CreateBatch_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (CheckBatchCreateField() != "")
            {
                System.Windows.Forms.MessageBox.Show(CheckBatchCreateField(), "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            DateTime now = DateTime.Now;

            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string systemPath = System.IO.Path.Combine(FolderSetting.AppFolder, FolderSetting.TempData, $"{txtBatchName.Text}_{now.ToString("yyyyMMddHHmmss")}");
            string path = System.IO.Path.Combine(userFolderPath, systemPath);

            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(path);

                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                string currentUser = WindowsIdentity.GetCurrent().Name;
                FileSystemAccessRule accessRule = new FileSystemAccessRule(currentUser, FileSystemRights.Write,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None, AccessControlType.Allow);
                directorySecurity.AddAccessRule(accessRule);
                directoryInfo.SetAccessControl(directorySecurity);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Khởi tạo thư mục thất bại! Vui lòng cấp quyền cho hệ thống.");
            }

            Directory.CreateDirectory(path);

            BatchCreateRequest request = new BatchCreateRequest()
            {
                Name = txtBatchName.Text,
                Note = txtBatchNote.Text,
                UserId = UserManager._userId,
                Path = systemPath,
                CreatedDate = now.ToString(),
                Icode = Guid.NewGuid().ToString()
            };

            long batchId = await _batchService.Create(request);
            if (batchId == 0)
            {
                System.Windows.Forms.MessageBox.Show("Tạo gói mới thất bại!", "Thông báo!!");
                return;
            }

            BatchManager.SetBatchId(batchId);
            BatchManager.SetBatchName(txtBatchName.Text);
            BatchManager.SetBatchPath(systemPath);

            System.Windows.Forms.MessageBox.Show("Tạo gói mới thành công!", "Thông báo!!");

            BatchManagerWindow batchManagerWindow = System.Windows.Application.Current.Windows.OfType<BatchManagerWindow>().FirstOrDefault();
            if (batchManagerWindow != null)
            {
                batchManagerWindow.GetListBatches();
            }
        }

        private void EditDocument_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            Object dataContext = button.DataContext;

            DocumentVM documentVM = ValueConvert.ConvertToObject<DocumentVM>(dataContext);

            if(documentVM != null)
            {
                DocumentCreateWindow documentCreateWindow= new DocumentCreateWindow();
                documentCreateWindow._document = documentVM;
                DocumentManager._documentIsEdit = true;
                documentCreateWindow.CheckAction();
                documentCreateWindow.Show();
            }
        }

        private void lstvDocuments_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                ScrollViewer scrollViewer = GetScrollViewer(lstvDocumentsFixed);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        private void lstvDocumentsFixed_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                ScrollViewer scrollViewer = GetScrollViewer(lstvDocuments);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            Object dataContext = button.DataContext;

            DocumentVM documentVM = ValueConvert.ConvertToObject<DocumentVM>(dataContext);

            if (documentVM != null)
            {
                var result = System.Windows.MessageBox.Show($"Bạn có chắc chắn muốn xóa tài liệu {documentVM.Name}", "Thông báo!!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        _documentService.Delete(documentVM.Id);
                        System.Windows.Forms.MessageBox.Show("Xóa tài liệu thành công!", "Thông báo!!");
                        GetListDocumentsById((long)BatchManager._batchId);
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void lstvBatches_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                ScrollViewer scrollViewer = GetScrollViewer(lstvBatchesFixed);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        private void lstvBatchesFixed_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0)
            {
                ScrollViewer scrollViewer = GetScrollViewer(lstvBatches);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                }
            }
        }

        private void DeleteBatch_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            Object dataContext = button.DataContext;

            BatchVM batchVM = ValueConvert.ConvertToObject<BatchVM>(dataContext);

            if(batchVM != null)
            {
                var result = System.Windows.MessageBox.Show($"Bạn có chắc chắn muốn xóa gói tài liệu {batchVM.Name}", "Thông báo!!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        _batchService.Delete(batchVM.Id);
                        System.Windows.Forms.MessageBox.Show("Xóa gói tài liệu thành công!", "Thông báo!!");
                        GetListBatches();
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void EditBatch_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
            Object dataContext = button.DataContext;

            BatchVM batchVM = ValueConvert.ConvertToObject<BatchVM>(dataContext);

            if(batchVM != null) 
            {
                BatchUpdateWindow batchUpdateWindow = new BatchUpdateWindow();
                batchUpdateWindow._batch = batchVM;
                batchUpdateWindow.GetData();
                batchUpdateWindow.Show();
            }
        }
    }
}
