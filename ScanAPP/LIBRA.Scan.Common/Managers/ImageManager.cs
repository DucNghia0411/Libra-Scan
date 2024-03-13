using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class ImageManager
    {
        public static long? imageId { get; set; }
        public static string? imageName { get; set; }
        public static string? imagePath { get; set; }
        public static long? imageOrder { get; set; }

        public static void SetImageId(long id)
        {
            imageId = id;
        }

        public static void SetImageOrder(long order)
        {
            imageOrder = order;
        }

        public static void SetImageName(string name)
        {
            imageName = name;
        }
    }
}
