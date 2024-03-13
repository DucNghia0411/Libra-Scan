using LIBRA.Scan.API.Entities.Requests;
using Microsoft.AspNetCore.Http;
﻿using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IImageService
    {
        Task<long> Create(ImageCreateRequest request);
        Task<IEnumerable<Image>> Get(Expression<Func<Image, bool>> predicate);
    }
}
