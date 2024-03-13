using LIBRA.Scan.Common.Status;
using LIBRA.Scan.Data.EFs;

using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services.Constracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class ImageService : IImageService
    {
        private readonly ScanAppContext _context;

        public ImageService() 
        {
            _context = new ScanAppContext();
        }

        public IEnumerable<Image> GetByPageId(long Id)
        {
            return _context.images
                 .Where(x => x.Page_Id == Id && x.Deleted != 1).OrderBy(x => x.Number_Order).AsNoTracking().ToList();
        }

        public async Task<long> Create(ImageCreateRequest request)
        {
            Image image = new Image()
            {
                Name = request.Name,
                Page_Id = request.PageId,
                Number_Order = request.NumberOrder,
                Note = request.Note,
                Path = request.Path,
                Icode = request.PageIcode
            };
            try
            {
                await _context.images.AddAsync(image);
                await _context.SaveChangesAsync();
                return image.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CheckExists(string imageName)
        {
            return _context.images.Any(a => a.Name.Contains(imageName));
        }

        public bool UpdatePathById(long id, string filePath)
        {
            var image = _context.images.Find(id);

            if (image == null)
                return false;

            image.Path = filePath;

            _context.images.Update(image);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(ImageUpdateRequest request)
        {
            var image = _context.images.Find(request.Id);

            if (image == null)
                return false;

            image.Name = request.Name;
            image.Path = request.Path;
            image.Page_Id = request.PageId;
            image.Number_Order = request.NumberOrder;
            image.Deleted = request.Deleted ? 1 : 0;
            image.Note = request.Note;

            _context.images.Update(image);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckIdImages(long id)
        {
            var image = _context.images.Find(id);

            if (image == null)
                return false;

            return true;
        }

        public bool UpdateOrder(long idFirstImg, long idSecondImg)
        {
            var firstImage = _context.images.Find(idFirstImg);

            if (firstImage == null)
                return false;

            var secondImage = _context.images.Find(idSecondImg);

            if (secondImage == null)
                return false;

            long? fisrtOrder = firstImage.Number_Order;
            long? secondOrder = secondImage.Number_Order;

            firstImage.Number_Order = secondOrder;
            secondImage.Number_Order = fisrtOrder;

            _context.images.Update(firstImage);
            _context.images.Update(secondImage);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(long id)
        {
            var image = _context.images.Find(id);

            if (image == null)
                return false;

            _context.images.Remove(image);

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public long GetCurrentOrderByPageId(long? pageId)
        {
            var image = _context.images.Where(x => x.Page_Id == pageId).OrderBy(x => x.Number_Order).LastOrDefault();

            if (image == null || image.Number_Order == null)
                return 0;

            return (long)image.Number_Order;
        }

        public bool ReSort(long? idImage)
        {
            var image = _context.images.Find(idImage);
            if (image == null || image.Number_Order == null)
                return false;

            List<Image> listImage = _context.images.Where(x => x.Page_Id == image.Page_Id).OrderBy(x => x.Number_Order).ToList();

            int orderToRemove = (int)image.Number_Order;

            for (int i = orderToRemove; i < listImage.Count(); i++)
            {
                listImage[i].Number_Order = listImage[i].Number_Order - 1;
                _context.images.Update(listImage[i]);
            }

            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
