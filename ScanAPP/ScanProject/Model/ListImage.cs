using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProject.Model
{
    public class ListImage
    {
        public ListImage(int docID, string name)
        {
            DocID = docID;
            Name = name;
            Images = new ObservableCollection<Image>();
        }

        public string Name { get; set; }
        public int DocID { get; set; }
        public int PageNum { get; set; }
        public ObservableCollection<Image> Images { get; set; }
    }
}
