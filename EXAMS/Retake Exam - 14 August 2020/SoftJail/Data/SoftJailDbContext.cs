namespace SoftJail.Data
{
	using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
	{
		public SoftJailDbContext()
		{
		}

		public SoftJailDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Prisoner> Prisoners { get; set; }

        public DbSet<Officer> Officers { get; set; }

        public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }

        public DbSet<Mail> Mails { get; set; }

        public DbSet<Cell> Cells { get; set; }

        public DbSet<Department> Departments { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            //builder.Entity<Prisoner>(prisoner =>
            //{
            //    prisoner.HasOne(p => p.Cell)
            //        .WithMany(c => c.Prisoners)
            //        .HasForeignKey(p => p.CellId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});


            //builder.Entity<Cell>(cell =>
            //{
            //    cell.HasOne(c => c.Department)
            //        .WithMany(d => d.Cells)
            //        .HasForeignKey(c => c.DepartmentId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            //builder.Entity<Mail>(mail =>
            //{
            //    mail.HasOne(m => m.Prisoner)
            //        .WithMany(p => p.Mails)
            //        .HasForeignKey(m => m.PrisonerId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //});

            builder.Entity<OfficerPrisoner>()
                .HasKey(of => new { of.PrisonerId, of.OfficerId });

        }


    }
}