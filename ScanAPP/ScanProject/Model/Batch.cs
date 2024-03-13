using ScanProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProject.Model
{
    public class Batch : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ObservableCollection<Model.Document> _Documents;

        public ObservableCollection<Model.Document> Documents
        {
            get { return _Documents; }
            set
            {
                _Documents = value;
                OnPropertyChanged("Documents");
            }
        }
    }
}
