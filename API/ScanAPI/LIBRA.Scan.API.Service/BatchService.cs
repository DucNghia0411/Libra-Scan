using AutoMapper;
using Azure.Core;
using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Dtos;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Entities.ViewModels;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LIBRA.Scan.API.Service
{
    public class BatchService : IBatchService
    {
        private readonly ScanAppContext _context;
        private readonly IBatchRepo batchRepo;
        private readonly IMapper mapper;

        public BatchService
        (
            ScanAppContext context
            , IBatchRepo batchRepo
            , IMapper mapper
        )
        {
            this._context = context;
            this.batchRepo = batchRepo;
            this.mapper = mapper;
        }

        public async Task<long> Create(BatchCreateRequest request)
        {
            Batch batch = new Batch()
            {
                Name = request.Name,
                UserId = request.UserId,
                Deleted = request.Deleted,
                Note = request.Note,
                Path = request.Path
            };
            try
            {
                await _context.Batches.AddAsync(batch);
                await _context.SaveChangesAsync();
                return batch.Id;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }

        public async Task<IEnumerable<Batch>> Get(Expression<Func<Batch, bool>> predicate)
        {
            IEnumerable<Batch> batches = await batchRepo.ListAsync(predicate);
            return mapper.Map<IEnumerable<Batch>>(batches);
        }

        public async Task<IEnumerable<Batch>> GetAll()
        {
            IEnumerable<Batch> batches = await batchRepo.ListAsync();
            return mapper.Map<IEnumerable<Batch>>(batches);
        }

        public async Task<BatchVM> GetByName(string batchName)
        {
            var batch = await _context.Batches.FirstOrDefaultAsync(x => x.Name == batchName);

            if (batch == null)
                return null;

            BatchVM batchDetails = new BatchVM();
            batchDetails.Id = batch.Id;
            batchDetails.Name = batch.Name;
            batchDetails.UserId = batch.UserId;
            batchDetails.Deleted = batch.Deleted;
            batchDetails.Note = batch.Note;
            batchDetails.Path = batch.Path;

            return batchDetails;
        }
        //public async Task<IEnumerable<BatchDto>> Get(Expression<Func<Batch, bool>> predicate)
        //{
        //    var batch = await batchRepo.ListAsync(predicate, q => (IOrderedQueryable<Batch>)q.Include(x => x.Job));
        //    return mapper.Map<IEnumerable<BatchDto>>(batch);
        //}

        //public async Task<BatchDto> GetSingle(Expression<Func<Batch, bool>> predicate)
        //{
        //    var batch = await batchRepo.Find(predicate, q => q.Include(x => x.Job).Include(y => y.Status));
        //    var a = batch.Job;
        //    var b = batch.Status;
        //    return mapper.Map<BatchDto>(batch);
        //}
        //public async Task<bool> Add(BatchDto batchDto)
        //{
        //    Batch batch = new Batch()
        //    {
        //        JobId = batchDto.JobId,
        //        CreatedDate = batchDto.CreatedDate,
        //        Name = batchDto.Name,
        //        Deleted = false,
        //        StatusId = batchDto.StatusId
        //    };
        //    try
        //    {
        //        await batchRepo.AddAsync(batch, false);
        //        return true;
        //    }
        //    catch { }
        //    return false;
        //}
    }
}
