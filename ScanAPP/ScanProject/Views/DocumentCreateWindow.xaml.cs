using Azure;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Common.Setting;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.ViewModels;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using ScanProject.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for DocumentCreateWindow.xaml
    /// </summary>
    public partial class DocumentCreateWindow : Window
    {
        private readonly IDocumentService _documentService;
        private readonly IDocumentTypeService _documentTypeService;

        public DocumentVM _document { get; set; }

        public DocumentCreateWindow()
        {
            InitializeComponent();
            _documentService = new DocumentService();
            _documentTypeService = new DocumentTypeService();

            //setting
            this.ResizeMode = ResizeMode.NoResize;

            //get data
            GetListType();
            CheckAction();
        }

        private async void CreateDocument_Click(object sender, RoutedEventArgs e)
        {
            if(DocumentManager._documentIsEdit == true)
                UpdateDocument();
            else
                CreateDocument();

            BatchManagerWindow batchManagerWindow = System.Windows.Application.Current.Windows.OfType<BatchManagerWindow>().FirstOrDefault();
            if (batchManagerWindow != null)
            {
                batchManagerWindow.GetListDocumentsById((long)BatchManager._batchId);
            }
        }

        private async void GetListType()
        {
            IEnumerable<DocumentType> documentTypes = await _documentTypeService.GetList();
            cboDocumentType.ItemsSource = documentTypes;
            cboDocumentType.SelectedValuePath = "Id";
        }

        private string CheckDocumentCreateField()
        {
            string notification = string.Empty;
            if (txtDocumentName.Text.Trim() == "")
                notification += "Tên tài liệu không được để trống! \n";
            if (txtAdministrativeDivision.Text.Trim() == "")
                notification += "Tên đơn vị không được để trống! \n";
            if (txtDocumentNo.Text.Trim() == "")
                notification += "Số tài liệu không được để trống! \n";
            if (txtDocumentYear.Text.Trim() == "")
                notification += "Năm tài liệu không được để trống! \n";
            if (cboDocumentType.SelectedValue == null)
                notification += "Vui lòng chọn loại tài liệu! \n";
            return notification;
        }

        private void btnEditDocument_Click(object sender, RoutedEventArgs e)
        {
            cboDocumentType.IsEnabled = true;
            txtDocumentName.IsEnabled = true;
            txtAdministrativeDivision.IsEnabled = true;
            txtDocumentNo.IsEnabled = true;
            txtDocumentYear.IsEnabled = true;
            txtScannedImages.IsEnabled = true;
            txtDocumentNote.IsEnabled = true;

            btnCreateDocument.IsEnabled = true;
            btnEditDocument.IsEnabled = false;
            btnCancelDocument.IsEnabled = true;
            btnCloseDocument.IsEnabled = false;
        }

        private void btnCancelDocument_Click(object sender, RoutedEventArgs e)
        {
            cboDocumentType.IsEnabled = false;
            txtDocumentName.IsEnabled = false;
            txtAdministrativeDivision.IsEnabled = false;
            txtDocumentNo.IsEnabled = false;
            txtDocumentYear.IsEnabled = false;
            txtScannedImages.IsEnabled = false;
            txtDocumentNote.IsEnabled = false;

            btnCreateDocument.IsEnabled = false;
            btnEditDocument.IsEnabled = true;
            btnCancelDocument.IsEnabled = false;
            btnCloseDocument.IsEnabled = true;
        }

        private void btnCloseDocumnet_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public void CheckAction()
        {
            if(DocumentManager._documentIsEdit == true && _document != null)
            {
                cboDocumentType.IsEnabled = false;
                txtDocumentName.IsEnabled = false;
                txtAdministrativeDivision.IsEnabled = false;
                txtDocumentNo.IsEnabled = false;
                txtDocumentYear.IsEnabled = false;
                txtScannedImages.IsEnabled = false;
                txtDocumentNote.IsEnabled = false;

                btnCreateDocument.IsEnabled = false;
                btnEditDocument.IsEnabled = true;
                btnCancelDocument.IsEnabled = false;
                btnCloseDocument.IsEnabled = true;

                cboDocumentType.SelectedValue = _document.DocumentTypeId;
                txtDocumentName.Text = _document.Name;
                txtAdministrativeDivision.Text = _document.AdministrativeDivision;
                txtDocumentNo.Text = _document.DocumentNo.ToString();
                txtDocumentYear.Text = _document.DocumentYear.ToString();
                txtDocumentNote.Text = _document.Note;
                txtScannedImages.Text = _document.ScannedImages.ToString();
            }
            else
            {
                cboDocumentType.IsEnabled = true;
                txtDocumentName.IsEnabled = true;
                txtAdministrativeDivision.IsEnabled = true;
                txtDocumentNo.IsEnabled = true;
                txtDocumentYear.IsEnabled = true;
                txtScannedImages.IsEnabled = false;
                txtDocumentNote.IsEnabled = true;

                btnCreateDocument.IsEnabled = true;
                btnEditDocument.IsEnabled = false;
                btnCancelDocument.IsEnabled = false;
                btnCloseDocument.IsEnabled = true;
            }
        }

        public void UpdateDocument()
        {
            if(_document == null)
            {
                System.Windows.MessageBox.Show($"Tài liệu bạn muốn cập nhật không tồn tại!", "Thông báo!", MessageBoxButton.OK);
                return;
            }

            DocumentUpdateRequest request = new DocumentUpdateRequest()
            {
                Id = Convert.ToInt64(_document.Id),
                Name = txtDocumentName.Text,
                AdministrativeDivision = txtAdministrativeDivision.Text,
                DocumentTypeId = Convert.ToInt64(cboDocumentType.SelectedValue.ToString()),
                DocumentNo = Convert.ToInt64(txtDocumentNo.Text),
                DocumentYear = Convert.ToInt64(txtDocumentYear.Text),
                ScannedImages = !String.IsNullOrWhiteSpace(txtScannedImages.Text) || !String.IsNullOrEmpty(txtScannedImages.Text) ? Convert.ToInt32(txtScannedImages.Text) : 0,
                Note = txtDocumentNote.Text.ToString()
            };

            long updatedId = _documentService.Update(request);

            if (updatedId == 0)
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật tài liệu thất bại!", "Thông báo!!");
                return;
            }

            System.Windows.Forms.MessageBox.Show("Cập nhật tài liệu thành công!", "Thông báo!!");

            this.Close();
        }

        public async void CreateDocument()
        {
            long? batchId = BatchManager._batchId;

            if (batchId == null)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng chọn gói trước khi tạo tài liệu mới!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            if (CheckDocumentCreateField() != "")
            {
                System.Windows.Forms.MessageBox.Show(CheckDocumentCreateField(), "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            DateTime now = DateTime.Now;

            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string systemPath = System.IO.Path.Combine(BatchManager._batchPath, $"{txtDocumentName.Text}_{now.ToString("yyyyMMdd")}");
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

            DocumentCreateRequest request = new DocumentCreateRequest()
            {
                Name = txtDocumentName.Text,
                BatchId = (long)batchId,
                UserId = UserManager._userId,
                AdministrativeDivision = txtAdministrativeDivision.Text,
                DocumentNo = Convert.ToInt64(txtDocumentNo.Text),
                DocumentYear = Convert.ToInt64(txtDocumentYear.Text),
                DocumentTypeId = Convert.ToInt64(cboDocumentType.SelectedValue),
                ScannedImages = 0,
                Note = txtDocumentNote.Text,
                Path = systemPath,
            };

            long documentId = await _documentService.Create(request);
            if (documentId == 0)
            {
                System.Windows.Forms.MessageBox.Show("Tạo tài liệu mới thất bại!", "Thông báo!!");
                return;
            }

            System.Windows.Forms.MessageBox.Show("Tạo tài liệu mới thành công!", "Thông báo!!");

            this.Close();
        }
    }
}
