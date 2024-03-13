using ScanProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProject.Model
{
    public class Page : ViewModelBase
    {
        public string PageName { get; set; }
        public int PageID { get; set; }
        public long? DocID { get; set; }
        public string PagePath { get; set; }
        public int PageNum { get; set; }
        public string Icode { get; set; }

        public ObservableCollection<Image> _Images;

        public ObservableCollection<Image> Images
        {
            get { return _Images; }
            set
            {
                _Images = value;
                OnPropertyChanged("Images");
            }
        }
    }
}
