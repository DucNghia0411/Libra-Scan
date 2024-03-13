using Azure.Core;
﻿using AutoMapper;
using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly ScanAppContext _context;
        private readonly IDocumentRepo _documentRepo;

        public DocumentService
        (
            ScanAppContext context
            , IDocumentRepo documentRepo
        )
        {
            this._context = context;
            this._documentRepo = documentRepo;
        }

        public async Task<long> Create(DocumentCreateRequest request)
        {
            Document document = new Document()
            {
                Name = request.Name,
                BatchId = request.BatchId,
                UserId = request.UserId,
                AdministrativeDivision = request.AdministrativeDivision,
                DocumentNo = request.DocumentNo,
                DocumentTypeId = request.DocumentTypeId,
                DocScanStatus = request.DocScanStatus,
                ScannedDate = request.ScannedDate,
                RenderedDate = request.RenderedDate,
                SucceedDate = request.SucceedDate,
                Note = request.Note,
                IsValid = request.IsValid,
                UserQc = request.UserQc,
                IsQc = request.IsQc,
                QcDate = request.QcDate,
                Path = request.Path,
                Deleted = request.Deleted,
                DocProcessStatus = request.DocProcessStatus,
                DocumentYear = request.DocumentYear,
                ScannedImages = request.ScannedImages
            };
            try
            {
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();
                return document.Id;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }

        public async Task<Document> GetById(long documentId)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(x => x.Id == documentId);

            Document documentDetails = new Document()
            {
                Id = documentId,
                Name = document.Name,
                BatchId = document.BatchId,
                UserId = document.UserId,
                AdministrativeDivision = document.AdministrativeDivision,
                DocumentNo = document.DocumentNo,
                DocumentTypeId = document.DocumentTypeId,
                DocScanStatus = document.DocScanStatus,
                ScannedDate = document.ScannedDate,
                RenderedDate = document.RenderedDate,
                SucceedDate = document.SucceedDate,
                Note = document.Note,
                IsValid = document.IsValid,
                UserQc = document.UserQc,
                IsQc = document.IsQc,
                QcDate = document.QcDate,
                Path = document.Path,
                Deleted = document.Deleted,
                DocProcessStatus = document.DocProcessStatus,
                DocumentYear = document.DocumentYear,
                ScannedImages = document.ScannedImages
            };

            return documentDetails;
        }
    }
}
