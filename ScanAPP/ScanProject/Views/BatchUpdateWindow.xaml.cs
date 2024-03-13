using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.ViewModels;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using ScanProject.View;
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
    /// Interaction logic for BatchUpdateWindow.xaml
    /// </summary>
    public partial class BatchUpdateWindow : Window
    {
        private readonly IBatchService _batchService;
        public BatchVM _batch { get; set; }

        public BatchUpdateWindow()
        {
            InitializeComponent();

            _batchService = new BatchService();
        }

        public void UpdateBatch()
        {
            if (_batch == null)
            {
                System.Windows.MessageBox.Show($"Gói tài liệu bạn muốn cập nhật không tồn tại!", "Thông báo!", MessageBoxButton.OK);
                return;
            }

            BatchUpdateRequest request = new BatchUpdateRequest()
            {
                Id = _batch.Id,
                Name = txtBatchName.Text,
                Note = txtBatchNote.Text,
            };

            if (!_batchService.Update(request))
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật tài liệu thất bại!", "Thông báo!!");
                return;
            }

            System.Windows.Forms.MessageBox.Show("Cập nhật tài liệu thành công!", "Thông báo!!");

            BatchManagerWindow batchManagerWindow = System.Windows.Application.Current.Windows.OfType<BatchManagerWindow>().FirstOrDefault();
            if (batchManagerWindow != null)
            {
                batchManagerWindow.GetListBatches();
            }
        }

        private void btnConfirmBatch_Click(object sender, EventArgs e) 
        {
            UpdateBatch();
            EnableField(false, false);
            EnableButton(false, true, false, true);
        }

        private void EnableButton(bool confirm, bool edit, bool cancel, bool close)
        {
            btnConfirmBatch.IsEnabled = confirm;
            btnEditBatch.IsEnabled = edit;
            btnCancelBatch.IsEnabled = cancel;
            btnCloseBatch.IsEnabled = close;
        }

        private void EnableField(bool batchName, bool batchNote)
        {
            txtBatchName.IsEnabled = batchName;
            txtBatchNote.IsEnabled = batchNote; 
        }

        private void btnEditBatch_Click(Object sender, EventArgs e)
        {
            EnableField(true, true);
            EnableButton(true, false, true, false);
        }

        private void btnCancelBatch_Click(Object sender, EventArgs e)
        {
            EnableField(false, false);
            EnableButton(false, true, false, true);
        }

        private void btnCloseBatch_Click(Object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public void GetData()
        {
            if( _batch != null)
            {
                txtBatchName.Text = _batch.Name;
                txtBatchNote.Text = _batch.Note;

                EnableField(false, false);
                EnableButton(false, true, false, true);
            }
        }
    }
}
