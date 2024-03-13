using LIBRA.Scan.Common.Common;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.ViewModels;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
using System.Linq;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for ImportDataByFileWindow.xaml
    /// </summary>
    public partial class ImportDataByFileWindow : System.Windows.Window
    {
        private readonly IBatchService _batchService;
        private readonly IDocumentService _documentService;
        private long _batchId;
        private long _documentId;

        public ImportDataByFileWindow()
        {
            _batchService = new BatchService();
            _documentService = new DocumentService();
            InitializeComponent();
            GetBatchs();
        }

        private void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ZIP Folder (.zip)|*.zip";
            DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string zipFilePath = openFileDialog.FileName;
                txtFilePath.Text = zipFilePath;
            }
        }

        private void GetBatchs()
        {
            List<object> return_data = new List<object>();
            IEnumerable<Batch> batchs = _batchService.GetAll();

            foreach (Batch batch in batchs) 
            {
                var obj = new
                {
                    Id = batch.Id,
                    Name = batch.Name,
                };
                return_data.Add(obj);
            }

            cboBatch.Items.Clear();
            cboBatch.ItemsSource = return_data;
            cboBatch.DisplayMemberPath = "Name";
        }

        private void cboBatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox cbo = (System.Windows.Controls.ComboBox)sender;
            BatchVM selectedItem = ValueConvert.ConvertToObject<BatchVM>(cbo.SelectedItem);

            if (selectedItem != null)
            {
                long batchId = Convert.ToInt64(selectedItem.Id);
                _batchId = batchId;
                GetDocumentsByBatch(batchId);
            }
        }

        private void GetDocumentsByBatch(long batchId)
        {
            List<object> return_data = new List<object>();
            IEnumerable<Document> documents = _documentService.GetDocumentsByBatchId(batchId);

            foreach (Document document in documents) 
            {
                var obj = new
                {
                    Id = document.Id,
                    Name = document.Name,
                };
                return_data.Add(obj);
            }

            cboDocument.Items.Clear();
            cboDocument.ItemsSource = return_data;
            cboDocument.DisplayMemberPath = "Name";
        }

        private void cboDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox cbo = (System.Windows.Controls.ComboBox)sender;
            DocumentVM selectedItem = ValueConvert.ConvertToObject<DocumentVM>(cbo.SelectedItem);

            if (selectedItem != null)
            {
                long documentId = Convert.ToInt64(selectedItem.Id);
                _documentId = documentId;
            }
        }

        private void btnDimEdit_Click(object sender, RoutedEventArgs e)
        {
            btnDimEdit.Visibility = Visibility.Collapsed;
            btnDimRefresh.Visibility = Visibility.Collapsed;
            btnDimSave.Visibility = Visibility.Visible;
            btnDimCancel.Visibility = Visibility.Visible;

            txtDimOne.IsEnabled = true;
            txtDimTwo.IsEnabled = true;
        }

        private void btnDimCancel_Click(object sender, RoutedEventArgs e)
        {
            btnDimEdit.Visibility = Visibility.Visible;
            btnDimRefresh.Visibility = Visibility.Visible;
            btnDimSave.Visibility = Visibility.Collapsed;
            btnDimCancel.Visibility = Visibility.Collapsed;

            txtDimOne.IsEnabled = false;
            txtDimTwo.IsEnabled = false;
        }

        private void btnDimRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtDimOne.Text = "3000";
            txtDimTwo.Text = "5000";
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
