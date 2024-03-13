using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ScanProject.Model;
using ScanProject.View;
using ScanProject.Views;
using LIBRA.Scan.Entities.Entities;
using LIBRA.Scan.Services.Constracts;
using LIBRA.Scan.Services;
using LIBRA.Scan.Entities.Dtos;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace ScanProject.ViewModels
{
    public class MainViewModel : WorkspaceViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<Model.Page> PageCategories { get; set; }
        private ObservableCollection<Model.Document> _DocumentCategories { get; set; }
        public ObservableCollection<Model.Document> DocumentCategories
        {
            get { return _DocumentCategories; }
            set
            {
                _DocumentCategories = value;
                this.OnPropertyChanged(nameof(DocumentCategories));
            }
        }
        private ObservableCollection<Model.Image> _ListImageMain { get; set; }
        public ObservableCollection<Model.Image> ListImageMain
        {
            get { return _ListImageMain; }
            set
            {
                _ListImageMain = value;
                this.OnPropertyChanged(nameof(ListImageMain));
            }
        }
        private ObservableCollection<BitmapImage> _ListBitmapImages { get; set; }
        public ObservableCollection<BitmapImage> ListBitmapImages
        {
            get { return _ListBitmapImages; }
            set
            {
                _ListBitmapImages = value;
                this.OnPropertyChanged(nameof(ListBitmapImages));
            }
        }
        public MainViewModel()
        {
            ListImageMain = new ObservableCollection<Model.Image>();
            DocumentCategories = new ObservableCollection<Model.Document>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
