using ScanProject.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Media.Imaging;

namespace ScanProject.Model
{
    public class Image : ViewModelBase
    {
        public Image() { }

        public Image(string name, long pageId, string imagePath, string Icode, int order)
        {
            Id = 0;
            PageID = pageId;
            Name = name;
            ImagePath = imagePath;
            Order = order;
            PageIcode = Icode;
            IsSelected = false;
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        private string _Icode;
        public string PageIcode
        {
            get { return _Icode; }
            set
            {
                _Icode = value;
                OnPropertyChanged("PageIcode");
            }
        }
        public string? Name { get; set; }
        public int Id { get; set; }

        private int _order;
        public int Order
        {
            get { return _order; }
            set
            {
                _order = value;
                OnPropertyChanged("Order");
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        private long _pageID;
        public long PageID
        {
            get { return _pageID; }
            set
            {
                _pageID = value;
                OnPropertyChanged("PageID");
            }
        }

        private int _pageNum;
        public int PageNum
        {
            get { return _pageNum; }
            set
            {
                _pageNum = value;
                OnPropertyChanged("PageNum");
            }
        }

        private BitmapImage _bitmapImage;
        public BitmapImage bitmapImage
        {
            get { return _bitmapImage; }
            set
            {
                _bitmapImage = value;
                OnPropertyChanged("bitmapImage");
            }
        }
    }
}
