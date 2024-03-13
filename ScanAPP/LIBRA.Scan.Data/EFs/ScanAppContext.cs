using LIBRA.Scan.Entities.LiteEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Data.EFs
{
    public class ScanAppContext : DbContext
    {
        public virtual DbSet<Batch> batch { get; set; }

        public virtual DbSet<Document> document { get; set; }

        public virtual DbSet<DocumentType> document_type { get; set; }

        public virtual DbSet<History> histories { get; set; }

        public virtual DbSet<Image> images { get; set; }

        public virtual DbSet<Page> pages { get; set; }

        public virtual DbSet<StatusProcess> status_process { get; set; }

        public virtual DbSet<StatusScan> status_scan { get; set; }

        public virtual DbSet<StatusBatch> status_batch { get; set; }

        public virtual DbSet<SftpConfig> sftp_config { get; set; }

        public virtual DbSet<Token> tokens { get; set; }

        public virtual DbSet<DeviceSetting> device_setting { get; set; }

        public virtual DbSet<UrlConfig> url_config { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=scan_app.sqlite;");
            //optionsBuilder.UseSqlite("Data Source=..\\..\\..\\scan_app.sqlite;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Batch>(entity => {
                entity.HasOne(d => d.StatusBatch).WithMany(p => p.Batches)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_batchstatus_status");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasOne(d => d.Batch).WithMany(d => d.Documents)
                .HasForeignKey(d => d.Batch_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_document_batch");

                entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Document_Type_Id)
                .HasConstraintName("fk_document_documentype");

                entity.HasOne(d => d.StatusProcess).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Doc_Process_Status)
                .HasConstraintName("fk_docprocessstatus_status");

                entity.HasOne(d => d.StatusScan).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Doc_Scan_Status)
                .HasConstraintName("fk_docscanstatus_status");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.HasOne(d => d.Document).WithMany(p => p.Histories)
                .HasForeignKey(d => d.DocumentId)
                .HasConstraintName("fk_histories_document");

                entity.HasOne(d => d.Image).WithMany(p => p.Histories)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("fk_histories_images");

                entity.HasOne(d => d.Page).WithMany(p => p.Histories)
                .HasForeignKey(d => d.PageId)
                .HasConstraintName("fk_histories_pages");
            });

            modelBuilder.Entity<Image>(entity => 
            {
                entity.HasOne(d => d.Page).WithMany(p => p.Images)
                .HasForeignKey(d => d.Page_Id)
                .HasConstraintName("fk_images_pages");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasOne(d => d.Document).WithMany(p => p.Pages)
                .HasForeignKey(d => d.Document_Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pages_document");
            });
        }
    }
}
