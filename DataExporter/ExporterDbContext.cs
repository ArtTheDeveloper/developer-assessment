using DataExporter.Model;
using Microsoft.EntityFrameworkCore;


namespace DataExporter
{
    public class ExporterDbContext : DbContext
    {
        public DbSet<Policy> Policies { get; set; }

        public ExporterDbContext(DbContextOptions<ExporterDbContext> options) : base(options)
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("ExporterDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // one- to - many relationship between Policy and Notes
            modelBuilder.Entity<Policy>()
                .HasMany<Note>()
                .WithOne()
                .HasForeignKey(i => i.PolicyId);

            modelBuilder.Entity<Policy>().HasData(
                new Policy() { Id = 1, PolicyNumber = "HSCX1001", Premium = 200, StartDate = new DateTime(2024, 4, 1) },
                new Policy() { Id = 2, PolicyNumber = "HSCX1002", Premium = 153, StartDate = new DateTime(2024, 4, 5) },
                new Policy() { Id = 3, PolicyNumber = "HSCX1003", Premium = 220, StartDate = new DateTime(2024, 3, 10) },
                new Policy() { Id = 4, PolicyNumber = "HSCX1004", Premium = 200, StartDate = new DateTime(2024, 5, 1) },
                new Policy() { Id = 5, PolicyNumber = "HSCX1005", Premium = 100, StartDate = new DateTime(2024, 4, 1) });


            // Seed Note data
            modelBuilder.Entity<Note>().HasData(
                new Note { Id = 101, Text = "Note for Policy 1", PolicyId = 1 },
                new Note { Id = 102, Text = "Note2 for Policy 1", PolicyId = 1 },
                new Note { Id = 103, Text = "Note for Policy 2", PolicyId = 2 },
                new Note { Id = 104, Text = "Note for Policy 3", PolicyId = 3 },
                new Note { Id = 105, Text = "Note2 for Policy 3", PolicyId = 3 },
                new Note { Id = 106, Text = "Note for Policy 4", PolicyId = 4 },
                new Note { Id = 107, Text = "Note for Policy 5", PolicyId = 5 },
                new Note { Id = 108, Text = "Note2 for Policy 5", PolicyId = 5 },
                new Note { Id = 109, Text = "Note3 for Policy 5", PolicyId = 5 });

            base.OnModelCreating(modelBuilder);
        }
    }
}
