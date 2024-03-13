using ScanProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProject.Model
{
    public class Document : ViewModelBase
    {
        public Document()
        {
            Pages = new ObservableCollection<Page>();
        }

        public long? _Id;
        public string _Name;
        public ObservableCollection<Page> _Pages;

        public long? Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        public string DocumentPath { get; set; }

        public ObservableCollection<Page> Pages
        {
            get { return _Pages; }
            set
            {
                _Pages = value;
                OnPropertyChanged("Pages");
            }
        }
    }
}
