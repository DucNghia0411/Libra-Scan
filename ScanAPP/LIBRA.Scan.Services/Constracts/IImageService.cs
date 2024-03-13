using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IImageService
    {
        IEnumerable<Image> GetByPageId(long Id);
        Task<long> Create(ImageCreateRequest request);
        bool CheckExists(string imageName);
        bool UpdatePathById(long id, string filePath);
        bool Update(ImageUpdateRequest request);
        bool CheckIdImages(long id);
        bool UpdateOrder(long idFirstImg, long idSecondImg);
        bool Delete(long id);
        long GetCurrentOrderByPageId(long? pageId);
        bool ReSort(long? idImage);
    }
}
