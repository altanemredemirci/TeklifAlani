using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TeklifAlani.Core.Identity;
using TeklifAlani.Core.Models;

namespace TeklifAlani.WEBUI.Context
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region ApplicationUser Fluent API

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.PhoneNumber)
            .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.TaxNumber)
            .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.TC)
            .IsUnique();


            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Logo)
                .HasColumnType("varchar(50)")
                .HasAnnotation("Display", "Firma Logosu");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.CompanyName)
                .HasColumnType("varchar(50)")
                .HasAnnotation("Display", "Firma Ünvaný");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Name)
                .HasColumnType("varchar(30)")
                .HasAnnotation("Display", "Ýsim");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Surname)
                .HasColumnType("varchar(30)")
                .HasAnnotation("Display", "Soyisim");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.UserName)
                .IsRequired(false);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Address)
                .HasColumnType("varchar(250)")
                .HasAnnotation("Display", "Adres");

            modelBuilder.Entity<ApplicationUser>()
               .Property(u => u.ShipmentEmail)
               .HasColumnType("varchar(250)")
               .HasAnnotation("Display", "Sevkiyat Email");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.TaxNumber)
                .HasColumnType("varchar(15)")
                .HasAnnotation("Display", "Vergi No");

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.TC)
                .HasColumnType("varchar(11)")
                .HasAnnotation("Display", "TC Kimlik No");

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.District)
                .WithMany(d => d.ApplicationUsers)
                .HasForeignKey(u => u.DistrictId);
            #endregion


            // Identity tablolarýný özelleþtirmek için ek ayarlar yapabilirsiniz
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser"); 
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");


        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Brand> Brands { get; set; }
    }
}
