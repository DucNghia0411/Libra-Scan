using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class ImageService : IImageService
    {
        private readonly ScanAppContext _context;
        private readonly IImageRepo _imageRepo;

        public ImageService
        (
            ScanAppContext context
            , IImageRepo imageRepo
        )
        {
            this._context = context;
            this._imageRepo = imageRepo;
        }

        public async Task<long> Create(ImageCreateRequest request)
        {
            string directoryPath = "D:\\ScanProject\\ImagesHost";
            string filePath = null;

            try
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
                directorySecurity.AddAccessRule(new FileSystemAccessRule("ADMIN", FileSystemRights.FullControl, AccessControlType.Allow));
                directoryInfo.SetAccessControl(directorySecurity);

                filePath = Path.Combine(directoryPath, request.file.FileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.file.CopyToAsync(fileStream);
                }
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }

            Image image = new Image()
            {
                PageId = request.PageId,
                Name = request.Name,
                Path = filePath,
                NumberOrder = request.NumberOrder,
                Deleted = request.Deleted,
                Note = request.Note
            };
            try
            {
                await _context.Images.AddAsync(image);
                await _context.SaveChangesAsync();
                return image.Id;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }

        public async Task<IEnumerable<Image>> Get(Expression<Func<Image, bool>> predicate)
        {
            IEnumerable<Image> image = await _imageRepo.ListAsync(predicate);
            return image;
        }
    }
}
