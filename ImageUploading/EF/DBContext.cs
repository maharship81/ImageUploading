using ImageUploading.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ImageUploading.EF
{
    public class DBContext: DbContext
    {
        public DBContext()
            : base("name=EmployeeEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
        public virtual DbSet<ImageUpload> ImageUploads { get; set; }
    }
}