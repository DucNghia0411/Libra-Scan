using ScanProject.ViewModels;
using LIBRA.Scan.Services;
using NTwain;
using NTwain.Data;
using ScanProject.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DragDropEffects = System.Windows.DragDropEffects;
using ListView = System.Windows.Controls.ListView;
using Point = System.Windows.Point;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;
using DragEventArgs = System.Windows.DragEventArgs;
using ScanProject.Views;
using LIBRA.Scan.Services.Constracts;
using System.Windows.Forms;
using LIBRA.Scan.Common.Managers;

using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Common.Setting;
using System.Threading.Tasks;
using LIBRA.Scan.Common.Enumerator;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.Common.Status;
using LIBRA.Scan.Data.EFs;
using Notification.Wpf;
using static System.Windows.Forms.ImageList;
using Application = System.Windows.Application;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Net.Http;
using Microsoft.Data.Sqlite;

namespace ScanProject
{
    public partial class MainWindow : Window
    {
        //config
        private ScanAppContext _context;
        public ISetUp _setUp;
        public DataSource _dataSource;
        public TwainSession _twain;

        //api client
        public IRoleApiClient _roleApiClient;

        //service
        public IPageService pageService;
        public IImageService imageService;
        private readonly NotificationManager _notificationManager;

        //declare
        public double ImageWidth;
        public double ImageHeight;

        public int ImageNum = 1;
        public int PageNum = 1;
        public int PageOrder = 1;
        public int NumPageChoosed = 0;
        public int NumberSideScan = 2;

        public bool isRescan = false;
        public bool isInsert = false;

        public MainViewModel _mainViewModel;
        private Point _lastMouseDown;

        private Model.Page draggedItem;
        private Model.Page _target;
        public Model.Document document = new Model.Document();

        public ObservableCollection<Model.Page> col = new ObservableCollection<Model.Page>();
        public ObservableCollection<BitmapImage> scannedImages = new ObservableCollection<BitmapImage>();
        public ObservableCollection<Model.Image> ListIMGSelected = new ObservableCollection<Model.Image>();

        private List<Bitmap> _scannedImages = new List<Bitmap>();

        private Point startPoint;

        public Model.Image SelectedImage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _context = new ScanAppContext();
            _setUp = new SetUp();
            pageService = new PageService();
            imageService = new ImageService();
            _notificationManager = new NotificationManager();

            _roleApiClient = new RoleApiClient();
            _mainViewModel = new MainViewModel();

            //GetTwainSession();
            //GetDataSource();
            //if (_dataSource != null)
            //{
            //    GetLastedInfor();
            //}

            System.Windows.Application.Current.MainWindow = this;
            DataContext = _mainViewModel;
            CurrentDocument();
            CurrentPage();
            SelectedImagesCount();
            Loaded += Window_Loaded;
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500);

            await Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    LoadSessionWindow load = new LoadSessionWindow();
                    load.ShowDialog();
                });
            });

        }
        #region Setting Menu
        private void ScanManager_Click(object s, RoutedEventArgs e)
        {
            ScanSettingManagerWindow scanSettingManagerWindow = new ScanSettingManagerWindow();
            scanSettingManagerWindow.Show();
        }

        private void CreateNewBatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchManagerWindow batchManagerWindow = new BatchManagerWindow();
                batchManagerWindow.Show();
            }
            catch (SqliteException)
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu.", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion

        #region Scan
        private void ScanButton_Click(object sender, EventArgs e)
        {
            try
            {
                isInsert = false;
                isRescan = false;
                ProcessScan();

                _twain.DataTransferred -= DataTransferred;
                _twain.DataTransferred += DataTransferred;
            }
            catch (TwainException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ProcessScan()
        {
            long? batchId = BatchManager._batchId;
            long? documentId = DocumentManager._documentId;

            if (batchId == null)
            {
                MessageBox.Show("Vui lòng chọn Batch trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            if (documentId == null)
            {
                MessageBox.Show("Vui lòng chọn Document trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            //start scan
            if (!_dataSource.IsOpen)
            {
                _dataSource.Open();
            }

            if (!_twain.IsSourceOpen)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thiết bị trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            if (ScanSettingManager._IsDefault == false)
            {
                BoolType isDuplex;

                if (ScanSettingManager._IsDuplex == true)
                    isDuplex = BoolType.True;
                else
                    isDuplex = BoolType.False;

                _dataSource.Capabilities.CapDuplexEnabled.SetValue(isDuplex);

                if (ScanSettingManager._size != null)
                {
                    SupportedSize size = (SupportedSize)ScanSettingManager._size;
                    _twain.CurrentSource.Capabilities.ICapSupportedSizes.SetValue(size);
                }

                if (ScanSettingManager._depth != null)
                {
                    PixelType depth = (PixelType)ScanSettingManager._depth;
                    _twain.CurrentSource.Capabilities.ICapPixelType.SetValue(depth);
                }

                if (ScanSettingManager._dpi != null)
                {
                    TWFix32 dpi = (TWFix32)ScanSettingManager._dpi;
                    _twain.CurrentSource.Capabilities.ICapXResolution.SetValue(dpi);
                    _twain.CurrentSource.Capabilities.ICapYResolution.SetValue(dpi);
                }
            }

            _dataSource.Enable(SourceEnableMode.NoUI, true, IntPtr.Zero);
        }

        private async void DataTransferred(object s, DataTransferredEventArgs e)
        {
            if (isInsert || isRescan)
                return;

            DateTime CreatedDate = DateTime.Now;
            if(DocumentManager._documentPath == null)
            {
                MessageBox.Show("Vui lòng khởi tạo lại tài liệu", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            var documentId = DocumentManager._documentId;
            if(documentId == null)
            {
                MessageBox.Show("Vui lòng truy cập vào tài liệu trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            var documentCategories = _mainViewModel.DocumentCategories;
            var documentCate = documentCategories.FirstOrDefault(a => a.Id == documentId);
            if (documentCate == null)
            {
                MessageBox.Show("Tài liệu bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            long currentPageId = (long)(PageManager.pageId != null ? PageManager.pageId : 0);
            var documentPage = currentPageId != 0 ? documentCate.Pages.FirstOrDefault(x => x.PageID == currentPageId) : documentCate.Pages.FirstOrDefault();

            currentPageId = documentPage != null ? documentPage.PageID : 0;
            var currentOrder = currentPageId != 0 ? pageService.GetCurrentOrderByDocumentId((long)documentId) : 0;

            string folderPath = String.Empty;
            string systemPath = String.Empty;
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Guid iCode = Guid.NewGuid();
            while (await pageService.CheckExists(iCode.ToString()))
            {
                iCode = Guid.NewGuid();
            };

            if(currentOrder == 0)
            {
                systemPath = System.IO.Path.Combine(
                    FolderSetting.AppFolder,
                    FolderSetting.TempData, $"{BatchManager._batchPath}",
                    DocumentManager._documentPath,
                    $"{currentOrder + 1}_{CreatedDate.ToString("yyyyMMdd")}");
                folderPath = Path.Combine(userFolderPath, systemPath);
                CreateFolder(folderPath);
            }

            if (documentPage == null)
            {
                currentOrder++;
                systemPath = System.IO.Path.Combine(
                    FolderSetting.AppFolder,
                    FolderSetting.TempData, $"{BatchManager._batchPath}",
                    DocumentManager._documentPath,
                    $"{currentOrder}_{CreatedDate.ToString("yyyyMMdd")}");
                folderPath = Path.Combine(userFolderPath, systemPath);
                CreateFolder(folderPath);

                PageCreateRequest request = new PageCreateRequest 
                { 
                    Name = "Trang" + currentOrder,
                    Icode = iCode.ToString(),
                    DocumentId = (long)documentId,
                    Path = systemPath,
                    NumberOrder = (int?)currentOrder,
                    Deleted = false
                };

                currentPageId = await pageService.Create(request);

                PageManager.SetPageId((long)currentPageId);
                PageManager.SetPageName("Trang" + currentOrder);
                PageManager.SetPageOrder(currentOrder);

                documentPage = new Model.Page
                {
                    PageID = (int)currentPageId,
                    DocID = documentId,
                    PageNum = (int)currentOrder,
                    Icode = iCode.ToString(),
                    PagePath = folderPath,
                    PageName = "Trang" + currentOrder,
                    Images = new ObservableCollection<Model.Image>()
                };

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    documentCate.Pages.Add(documentPage);
                });
            }

            // Handle image data
            if (e.NativeData != IntPtr.Zero)
            {
                var stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    _scannedImages.Clear();
                    _scannedImages.Add(new Bitmap(stream));
                }

                foreach (var img in _scannedImages)
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    using (MemoryStream memory = new MemoryStream())
                    {
                        img.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }

                    string fileName = BatchManager._batchName + "_" + DocumentManager._documentId + "_" + currentPageId + "_" + Guid.NewGuid() + ".png";
                    string filePath = Path.Combine(folderPath, fileName);
                    img.Save(filePath);

                    var currentImageOrder = ImageManager.imageOrder;

                    long? imageOrder = currentImageOrder == null ? imageService.GetCurrentOrderByPageId(currentPageId) : currentImageOrder;

                    imageOrder++;

                    Model.Image imgScanned = new Model.Image(fileName, currentPageId, filePath, iCode.ToString(), (int)imageOrder);
                    imgScanned.bitmapImage = bitmapImage;

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        _mainViewModel.ListImageMain.Add(imgScanned);
                        documentPage.Images.Add(imgScanned);
                        ImageManager.SetImageOrder((long)imageOrder);
                    });
                }
            }
        }
        #endregion

        #region Device
        private void SelectDevice_Click(object sender, RoutedEventArgs e)
        {
            DeviceWindow deviceWindow = new DeviceWindow();
            deviceWindow._twain = _twain;
            deviceWindow._mainWindow = this;
            deviceWindow.GetListDevice();
            deviceWindow.ShowDialog();
        }
        #endregion

        #region ReScan Image
        private void RescanButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListIMGSelected.Count == 0 || ListIMGSelected.Count > 2)
            {
                MessageBox.Show("Vui lòng chọn 1 hoặc 2 trang để quét lại", "Lưu ý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isRescan = true;
            isInsert = false;
            ProcessScan();

            _twain.DataTransferred -= RescanImage;
            _twain.DataTransferred += RescanImage;
        }

        private void RescanImage(object s, DataTransferredEventArgs e)
        {
            if (!isRescan)
                return;

            if (e.NativeData != IntPtr.Zero)
            {
                var stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    _scannedImages.Clear();
                    _scannedImages.Add(new Bitmap(stream));
                }
            }

            if (ListIMGSelected.Count == 1)
            {
                var selectedImage = ListIMGSelected.First();
                var scannedImage = _scannedImages.FirstOrDefault();
                if (scannedImage == null)
                {
                    return;
                }

                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream memory = new MemoryStream())
                {
                    scannedImage.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }

                scannedImage.Save(selectedImage.ImagePath);
                selectedImage.bitmapImage = bitmapImage;
            }

            if (ListIMGSelected.Count == 2)
            {
                ScanSettingManager.UpdateRescanIndex();
                foreach (var img in _scannedImages)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream memory = new MemoryStream())
                    {
                        img.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }

                    var selectedImg = ListIMGSelected[ScanSettingManager.rescanIndex];
                    selectedImg.bitmapImage = bitmapImage;
                    img.Save(selectedImg.ImagePath);
                    ScanSettingManager.rescanIndex++;
                }
            }
        }
        #endregion

        #region Insert Scan Image
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListIMGSelected.Count != 1)
            {
                MessageBox.Show("Vui lòng chọn 1 trang để chèn vào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            isRescan = false;
            isInsert = true;

            ProcessScan();

            _twain.DataTransferred -= InsertScanImage;
            _twain.DataTransferred += InsertScanImage;
        }

        private void InsertScanImage(object s, DataTransferredEventArgs e)
        {
            if (!isInsert)
                return;

            var currentImage = ListIMGSelected.FirstOrDefault();
            if(currentImage == null)
            {
                MessageBox.Show("Không tìm thấy vị trí được chọn, vui lòng chọn lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var documentId = DocumentManager._documentId;
            if (documentId == null)
            {
                MessageBox.Show("Vui lòng truy cập vào tài liệu trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }
            var documentCategories = _mainViewModel.DocumentCategories;
            var documentCate = documentCategories.FirstOrDefault(a => a.Id == documentId);
            if (documentCate == null)
            {
                MessageBox.Show("Tài liệu bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            long currentPageId = currentImage.PageID;
            var documentPage = documentCate.Pages.FirstOrDefault(x => x.PageID == currentPageId);
            if (documentPage == null)
            {
                MessageBox.Show("Vui lòng chọn trang để Insert trước khi thực hiện!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            int currentOrder = documentPage.PageNum;
            string? directoryImage = Path.GetDirectoryName(currentImage.ImagePath);
            if (directoryImage == null)
            {
                MessageBox.Show("Vui lòng kiểm tra lại đường dẫn!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            string iCode = currentImage.PageIcode;

            var currentIndex = _mainViewModel.ListImageMain.IndexOf(currentImage);

            // Handle image data
            if (e.NativeData != IntPtr.Zero)
            {
                var stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    _scannedImages.Clear();
                    _scannedImages.Add(new Bitmap(stream));
                }

                foreach (var img in _scannedImages)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    using (MemoryStream memory = new MemoryStream())
                    {
                        img.Save(memory, ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }

                    string fileName = BatchManager._batchName + "_" + DocumentManager._documentId + "_" + currentPageId + "_" + Guid.NewGuid() + ".png";
                    string filePath = Path.Combine(directoryImage, fileName);
                    img.Save(filePath);

                    var currentImageOrder = ImageManager.imageOrder;
                    long? imageOrder = currentImageOrder == null ? imageService.GetCurrentOrderByPageId(currentPageId) : currentImageOrder;
                    imageOrder++;

                    Model.Image imgScanned = new Model.Image(fileName, currentPageId, filePath, iCode.ToString(), (int)imageOrder);
                    imgScanned.bitmapImage = bitmapImage;

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        _mainViewModel.ListImageMain.Insert(currentIndex, imgScanned);
                        documentPage.Images.Insert(currentIndex, imgScanned);

                        for (int i = currentIndex + 1; i < _mainViewModel.ListImageMain.Count; i++)
                        {
                            _mainViewModel.ListImageMain[i].Order = i + 1;
                        }

                        for (int i = currentIndex; i < documentPage.Images.Count; i++)
                        {
                            documentPage.Images[i].Order = i + 1;
                        }

                        ImageManager.SetImageOrder((long)imageOrder);
                    });
                }
            }
        }
        #endregion

        #region Delete Image
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ListIMGSelected.Any())
            {
                MessageBox.Show("Vui lòng chọn hình ảnh để xóa", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xoá hình ảnh đã chọn?", "Lưu ý", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != System.Windows.Forms.DialogResult.Yes)
                return;

            foreach (Model.Image image in ListIMGSelected.ToList())
            {
                if (image.Id != 0 && imageService.CheckIdImages(image.Id))
                {
                    if (!imageService.ReSort(image.Id))
                    {
                        MessageBox.Show("Có lỗi xảy ra trong quá trình xử lý", "Thông báo!!", MessageBoxButtons.OK);
                        return;
                    }

                    if (!imageService.Delete(image.Id))
                    {
                        MessageBox.Show("Xóa hình ảnh thất bại", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                
                long? documentId = DocumentManager._documentId;

                if (documentId == null)
                {
                    MessageBox.Show("Vui lòng chọn Document trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                    return;
                }

                var documentCategories = _mainViewModel.DocumentCategories.FirstOrDefault(x => x.Id == documentId);
                if (documentCategories == null)
                {
                    MessageBox.Show("Vui lòng truy cập lại tài liệu bạn muốn thực hiện!", "Thông báo!!", MessageBoxButtons.OK);
                    return;
                }

                var documentPages = documentCategories.Pages.FirstOrDefault(x => x.PageID == image.PageID);

                if(PageManager.pageId != null && documentPages == null)
                {
                    documentPages = documentCategories.Pages.FirstOrDefault(x => x.PageID == PageManager.pageId);
                }

                if (PageManager.pageId == null && documentPages == null)
                {
                    MessageBox.Show("Vui lòng truy cập lại trang tài liệu bạn muốn thực hiện!", "Thông báo!!", MessageBoxButtons.OK);
                    return;
                }

                if (documentPages == null)
                {
                    MessageBox.Show("Vui lòng truy cập lại trang tài liệu bạn muốn thực hiện!", "Thông báo!!", MessageBoxButtons.OK);
                    return;
                }

                if (image.Id == 0)
                    ReSort(image.Order);

                var filePath = image.ImagePath;
                _mainViewModel.ListImageMain.Remove(image);

                documentPages.Images.Remove(image);

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Ảnh đang được sử dụng ở một quy trình khác! Vui lòng tắt ảnh trước khi thực hiện!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
            ListIMGSelected.Clear();

            var content = new NotificationContent
            {
                Title = "Thông báo!!",
                Message = "Xóa ảnh thành công!!",
                Background = new SolidColorBrush(Colors.Green),
                Foreground = new SolidColorBrush(Colors.White),
            };
            _notificationManager.Show(content);
        }

        private void ReSort(int order)
        {
            List<Model.Image> listImage = _mainViewModel.ListImageMain.ToList();

            int orderToRemove = order - 1;

            listImage.RemoveAt(orderToRemove);

            for (int i = orderToRemove; i < listImage.Count; i++)
            {
                listImage[i].Order = listImage[i].Order - 1;
            }

            var lastImage = listImage.LastOrDefault();
            if(lastImage != null)
                ImageManager.SetImageOrder(lastImage.Order);
        }

        #endregion

        #region Rotate Image
        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;

            if (listSelectedImage == null || listSelectedImage.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hình ảnh để thực hiện chức năng cắt viền!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var image in listSelectedImage)
            {
                BitmapImage originalBitmap = image.bitmapImage;
                RotateTransform rotateTransform = new RotateTransform(90);
                TransformedBitmap rotatedBitmap = new TransformedBitmap(originalBitmap, rotateTransform);

                BitmapEncoder fileEncoder = new PngBitmapEncoder(); 
                fileEncoder.Frames.Add(BitmapFrame.Create(rotatedBitmap));

                using (var fileStream = new FileStream(image.ImagePath, FileMode.Create))
                {
                    fileEncoder.Save(fileStream);
                }

                BitmapEncoder memoryEncoder = new PngBitmapEncoder(); 
                memoryEncoder.Frames.Add(BitmapFrame.Create(rotatedBitmap));

                BitmapImage rotatedBitmapImage = new BitmapImage();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryEncoder.Save(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    rotatedBitmapImage.BeginInit();
                    rotatedBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    rotatedBitmapImage.StreamSource = memoryStream;
                    rotatedBitmapImage.EndInit();
                }

                rotatedBitmapImage.Freeze();
                image.bitmapImage = rotatedBitmapImage;
            }
        }
        #endregion

        #region Merge Image
        private void OpenMergeImageWindow_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;
            if(listSelectedImage.Count !=  2)
            {
                MessageBox.Show("Vui lòng chỉ chọn 2 hình ảnh để thực hiện chức năng ghép!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var firstImage = listSelectedImage[0];
            var secondImage = listSelectedImage[1];

            MergeImageWindow mergeWindow = new MergeImageWindow(firstImage, secondImage);
            mergeWindow.ShowDialog();
        }
        #endregion

        #region Save Image
        private async void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long documentId = (long)(DocumentManager._documentId != null ? DocumentManager._documentId : 0);
                if (documentId == 0)
                {
                    MessageBox.Show("Vui lòng chọn tài liệu để thực hiện!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var documentCategory = _mainViewModel.DocumentCategories.FirstOrDefault(c => c.Id == documentId);
                if (documentCategory == null)
                {
                    MessageBox.Show("Tài liệu bạn chọn chưa được hiển thị", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var pageCategories = documentCategory.Pages;
                if(pageCategories.Count() < 1)
                {
                    MessageBox.Show("Vui lòng tạo trang để thực hiện!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var page in pageCategories)
                {
                    foreach (var image in page.Images)
                    {
                        if (image.ImagePath == null)
                        {
                            MessageBox.Show("Có lỗi trong quá trình lưu trữ dữ liệu!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (imageService.CheckIdImages(image.Id) && image.Id != 0)
                        {
                            ImageUpdateRequest request = new ImageUpdateRequest();
                            request.Id = image.Id;
                            request.Name = image.Name;
                            request.PageIcode = page.Icode;
                            request.PageId = page.PageID;
                            request.Path = image.ImagePath;
                            request.NumberOrder = image.Order;

                            if (!imageService.Update(request))
                            {
                                MessageBox.Show("Có lỗi trong quá trình lưu trữ dữ liệu!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        if (!imageService.CheckExists(Path.GetFileNameWithoutExtension(image.ImagePath)))
                        {
                            if (!imageService.CheckIdImages(image.Id) && image.Id == 0)
                            {
                                ImageCreateRequest imageNew = new ImageCreateRequest();
                                imageNew.Name = image.Name;
                                imageNew.Path = image.ImagePath;
                                imageNew.PageIcode = page.Icode;
                                imageNew.PageId = page.PageID;
                                imageNew.NumberOrder = image.Order;
                                var imageId = await imageService.Create(imageNew);

                                if (imageId == 0)
                                {
                                    MessageBox.Show("Có lỗi trong quá trình lưu trữ dữ liệu!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }
                }

                pageCategories.Clear();
                _mainViewModel.ListImageMain.Clear();
                ListIMGSelected.Clear();

                BatchManagerWindow batchManagerWindow = new BatchManagerWindow();
                batchManagerWindow.UpdateImagesByDocumentId(documentId);

                MessageBox.Show("Lưu thông tin thành công!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message.ToString() , "Lỗi!");
                return;
            }
        }
        #endregion

        #region Treeview
        private async void AddPage_Click(object sender, RoutedEventArgs e)
        {
            DateTime CreatedDate = DateTime.Now;
            if (DocumentManager._documentPath == null)
            {
                MessageBox.Show("Vui lòng khởi tạo lại tài liệu", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            var documentId = DocumentManager._documentId;
            if (documentId == null)
            {
                MessageBox.Show("Vui lòng truy cập vào tài liệu trước khi thực hiện Scan!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            var documentCategories = _mainViewModel.DocumentCategories;
            var documentCate = documentCategories.FirstOrDefault(a => a.Id == documentId);
            if (documentCate == null)
            {
                MessageBox.Show("Tài liệu bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            long currentPageId = (long)(PageManager.pageId != null ? PageManager.pageId : 0);
            var currentOrder = pageService.GetCurrentOrderByDocumentId((long)documentId);
            currentOrder++;

            string folderPath = String.Empty;
            string systemPath = String.Empty;
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            systemPath = System.IO.Path.Combine(
                FolderSetting.AppFolder,
                FolderSetting.TempData, $"{BatchManager._batchPath}",
                DocumentManager._documentPath,
                $"{currentOrder}_{CreatedDate.ToString("yyyyMMdd")}");
            folderPath = Path.Combine(userFolderPath, systemPath);
            CreateFolder(folderPath);

            Guid iCode = Guid.NewGuid();
            while (await pageService.CheckExists(iCode.ToString()))
            {
                iCode = Guid.NewGuid();
            };

            PageCreateRequest request = new PageCreateRequest
            {
                Name = "Trang" + currentOrder,
                Icode = iCode.ToString(),
                DocumentId = (long)documentId,
                Path = systemPath,
                NumberOrder = (int?)currentOrder,
                Deleted = false
            };

            currentPageId = await pageService.Create(request);

            PageManager.SetPageId(currentPageId);
            PageManager.SetPageName("Trang" + currentOrder);
            PageManager.SetPageOrder(currentOrder);

            var documentPage = new Model.Page
            {
                PageID = (int)currentPageId,
                DocID = documentId,
                PageNum = (int)currentOrder,
                Icode = iCode.ToString(),
                PagePath = folderPath,
                PageName = "Trang" + currentOrder,
                Images = new ObservableCollection<Model.Image>()
            };

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                documentCate.Pages.Add(documentPage);
            });

            string pageName = $"Trang {documentPage.PageNum}";
            PageManager.SetPageId(documentPage.PageID);
            PageManager.SetPageName(pageName);
            PageManager.SetPageOrder(documentPage.PageNum);
            CurrentPage();

            _mainViewModel.ListImageMain.Clear();
        }

        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext != null)
            {
                if (element.DataContext is Model.Image clickedImage)
                {
                    SelectedImage = clickedImage;
                    if (SelectedImage != null)
                    {
                        if (!SelectedImage.IsSelected)
                        {
                            SelectedImage.IsSelected = true;
                            ListIMGSelected.Add(SelectedImage);
                        }
                        else
                        {
                            SelectedImage.IsSelected = false;
                            ListIMGSelected.Remove(SelectedImage);
                        }
                    }
                    SelectedImagesCount();
                }
                else if (element.DataContext is Model.Page clickedPage)
                {
                    string pageName = $"Trang {clickedPage.PageNum}";

                    PageManager.SetPageId(clickedPage.PageID);
                    PageManager.SetPageName(pageName);
                    PageManager.SetPageOrder(clickedPage.PageNum);
                    CurrentPage();
                }
                //else if (element.DataContext is Model.Document clickedDocument)
                //{
                //    MessageBox.Show($"Document double-clicked: {clickedDocument.Name}");
                //}
            }
        }

        private void CurrentDocument()
        {
            var currentDocument = DocumentManager._documentName;

            if (string.IsNullOrWhiteSpace(currentDocument))
            {
                lblCurrentDocument.Content = "";
            }
            else
            {
                lblCurrentDocument.Content = $"{currentDocument}";
            }
        }

        private void CurrentPage()
        {
            var currentPage = PageManager.pageName;

            if (string.IsNullOrWhiteSpace(currentPage))
                lblCurrentPage.Content = String.Empty;
            else
                lblCurrentPage.Content = $"{currentPage}";
        }
        #endregion

        #region Move Image
        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
            ListView? listView = sender as ListView;

            if (listView == null)
                return;

            System.Windows.Controls.ListViewItem listViewItem = FindAncestor<System.Windows.Controls.ListViewItem>((DependencyObject)e.OriginalSource);

            // Allow normal selection if the click occurs on a ListViewItem.
            if (listViewItem != null)
            {
                listView.SelectedItem = listViewItem.DataContext;
            }
        }

        private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                            Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ListView listView = sender as ListView;
                    System.Windows.Controls.ListViewItem listViewItem = FindAncestor<System.Windows.Controls.ListViewItem>((DependencyObject)e.OriginalSource);

                    if (listViewItem != null)
                    {
                        // Perform drag-and-drop operation.
                        DragDrop.DoDragDrop(listViewItem, listViewItem.DataContext, DragDropEffects.Move);
                    }
                }
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Model.Image)))
            {
                Model.Image droppedData = e.Data.GetData(typeof(Model.Image)) as Model.Image;
                ListView listView = sender as ListView;
                Model.Image target = ((FrameworkElement)e.OriginalSource).DataContext as Model.Image;

                int oldIndex = listView.Items.IndexOf(droppedData);
                int newIndex = listView.Items.IndexOf(target);

                var listImage = _mainViewModel.ListImageMain;

                if (oldIndex != newIndex)
                {

                    var oldImg = listImage.FirstOrDefault(x => x.Id == droppedData.Id);
                    var newImg = listImage.FirstOrDefault(x => x.Id == target.Id);

                    if(oldImg.Id != 0 || newImg.Id != 0)
                    {
                        if (!imageService.UpdateOrder(oldImg.Id, newImg.Id))
                        {
                            MessageBox.Show("Có lỗi trong quá trình di chuyển hình ảnh!", "Lỗi!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if(oldImg.Id == 0 || newImg.Id == 0)
                    {
                        droppedData.Order = target.Order;
                        target.Order = droppedData.Order;
                    }

                    listImage.Move(oldIndex, newIndex);
                }

                CollectionViewSource.GetDefaultView(listImage).Refresh();
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T ancestor)
                {
                    return ancestor;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
        #endregion

        #region Choose Image
        private void listView_Click(object sender, RoutedEventArgs e)
        {
            SelectedImage = (Model.Image)((ListView)sender).SelectedItem;
            if (SelectedImage != null)
            {
                if (!SelectedImage.IsSelected)
                {
                    SelectedImage.IsSelected = true;
                    ListIMGSelected.Add(SelectedImage);
                }
                else
                {
                    SelectedImage.IsSelected = false;
                    ListIMGSelected.Remove(SelectedImage);
                    ((ListView)sender).SelectedItem = null;
                }
            }
            SelectedImagesCount();
        }

        private void SelectedImagesCount()
        {
            var total = ListIMGSelected.Count;

            lblSelectedImages.Content = $"Đang chọn {total} hình";
        }
             
        #endregion

        #region View Image
        private void ListImage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedImage = (Model.Image)((ListView)sender).SelectedItem;
            ViewImageWindow imageWindow = new ViewImageWindow(SelectedImage);
            imageWindow.Show();
        }
        #endregion

        #region Crop
        private void CropViewImage_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;

            if (listSelectedImage == null) 
            {
                MessageBox.Show("Vui lòng chọn 1 hình ảnh để thực hiện chức năng cắt viền!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listSelectedImage.Count != 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 hình ảnh để thực hiện chức năng cắt viền!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedImage = listSelectedImage.First();
            if(SelectedImage == null)
            {
                MessageBox.Show("Hình bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            CropImageWindow cropImageWindow = new CropImageWindow(SelectedImage);
            cropImageWindow.ShowDialog();
        }
        #endregion

        #region Others
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if(_twain != null)
                _twain.Close();
        }

        private void CreateFolder(string folderPath)
        {
            try
            {
                // Check if the folder already exists
                if (!Directory.Exists(folderPath))
                {
                    // Create the folder
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating folder: " + ex.Message);
            }
        }

        private void CleanupTwain()
        {
            if (_twain.State == (int)ScanState.TransferReady)
            {
                _twain.CurrentSource.Close();
            }
            if (_twain.State == (int)ScanState.SourceEnable)
            {
                _twain.Close();
            }

            if (_twain.State > (int)ScanState.SourceOpen)
            {
                _twain.ForceStepDown(2);
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            CleanupTwain();
            this.Close();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            ScanSettingManagerWindow window = new ScanSettingManagerWindow();
            window.ShowDialog();
        }

        private void AdjustBrightnessConstant_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;

            if (listSelectedImage == null)
            {
                MessageBox.Show("Vui lòng chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listSelectedImage.Count != 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedImage = listSelectedImage.First();
            if (SelectedImage == null)
            {
                MessageBox.Show("Hình bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AdjustBrightnessContrastWindow window = new AdjustBrightnessContrastWindow(SelectedImage);
            window.ShowDialog();
        }

        private void AdjustHueSaturation_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;

            if (listSelectedImage == null)
            {
                MessageBox.Show("Vui lòng chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listSelectedImage.Count != 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedImage = listSelectedImage.First();
            if (SelectedImage == null)
            {
                MessageBox.Show("Hình bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AdjustHueSaturationWindow window = new AdjustHueSaturationWindow(SelectedImage);
            window.ShowDialog();
        }

        private void CropPartOfImageClick_Click(object sender, RoutedEventArgs e)
        {
            var listSelectedImage = ListIMGSelected;

            if (listSelectedImage == null)
            {
                MessageBox.Show("Vui lòng chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listSelectedImage.Count != 1)
            {
                MessageBox.Show("Vui lòng chỉ chọn 1 hình ảnh để thực hiện chức năng!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SelectedImage = listSelectedImage.First();
            if (SelectedImage == null)
            {
                MessageBox.Show("Hình bạn chọn không tồn tại!", "Thông báo!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CropPartOfImageWindow window = new CropPartOfImageWindow(SelectedImage);
            window.ShowDialog();
        }

        private void ImportDataByFile_Click(object sender, RoutedEventArgs e)
        {
            ImportDataByFileWindow window = new ImportDataByFileWindow();
            window.ShowDialog();
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            DocumentTypeWindow window = new DocumentTypeWindow();
            window.ShowDialog();
        }
        #endregion
    }
}

