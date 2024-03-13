using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProject.ViewModels
{
    public class ShowImageViewModel
    {
        public Model.Image SelectedImage { get; set;}
        public string ImagePath { get; set;}
        public ShowImageViewModel()
        {
            //ImagePath = SelectedImage.ImagePath;
        }
    }
}
