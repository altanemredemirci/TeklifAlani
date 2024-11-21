using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeklifAlani.Core.Identity;
using TeklifAlani.Core.Models;

namespace TeklifAlani.DAL.Context
{
    public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
        {
        }

        public ApplicationIdentityDbContext() : base(new DbContextOptions<ApplicationIdentityDbContext>())
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Product Fluent API
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductCode);

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductCode)
                .HasColumnType("varchar(30)")
                .HasAnnotation("Display", "Ürün Kodu");

            modelBuilder.Entity<Product>()
                .Property(p => p.Description)
                .HasColumnType("varchar(300)")
                .HasAnnotation("Display", "Açýklama");

            modelBuilder.Entity<Product>()
                .Property(p => p.ListPrice)
                .HasColumnType("decimal(18,2)")
                .HasAnnotation("Display", "Liste Fiyatý");

            modelBuilder.Entity<Product>()
                .Property(p => p.Currency)
                .HasColumnType("nvarchar(5)")
                .HasAnnotation("Display", "Para Birimi");

            modelBuilder.Entity<Product>()
               .Property(p => p.Link)
               .HasColumnType("varchar(300)");

            modelBuilder.Entity<Product>()
               .Property(p => p.BrandId)
               .HasAnnotation("Display", "Marka Id");

            modelBuilder.Entity<Product>()
               .HasOne(p => p.Brand)
               .WithMany(b => b.Products)
               .HasForeignKey(p => p.BrandId);
            #endregion

            #region City District Fluent API
            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .HasAnnotation("Display", "Þehir")
                .HasColumnType("varchar(20)");

            modelBuilder.Entity<City>()
                .HasMany(c => c.Districts)
                .WithOne(d => d.City)
                .HasForeignKey(d => d.CityId);

            modelBuilder.Entity<District>()
                .Property(d => d.Name)
                .HasAnnotation("Display", "Ýlçe")
                .HasColumnType("varchar(30)");

            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(d => d.CityId);
            #endregion

            #region Supplier Fluent API
            modelBuilder.Entity<Supplier>()
                .Property(e => e.CompanyName)
                .HasMaxLength(150)
                .HasAnnotation("Display", "Firma Ünvaný");

            modelBuilder.Entity<Supplier>()
                .Property(e => e.ContactName)
                .HasMaxLength(100)
                .HasAnnotation("Display", "Sorumlu Kiþi");

            modelBuilder.Entity<Supplier>()
                .Property(e => e.Phone)
                .HasMaxLength(11)
                .HasAnnotation("DataType", "PhoneNumber")
                .HasAnnotation("Display", "Telefon");

            modelBuilder.Entity<Supplier>()
               .Property(e => e.Email)
               .HasMaxLength(250)
               .HasAnnotation("DataType", "EmailAddress");

            modelBuilder.Entity<Supplier>()
               .HasOne(s => s.ApplicationUser)
               .WithMany(u => u.Suppliers)
               .HasForeignKey(s => s.ApplicationUserId);

            modelBuilder.Entity<Supplier>()
                .Property(c => c.BrandId)
                .HasAnnotation("Display", "Marka Id");
            #endregion

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

            #region Brand Fluent API
            modelBuilder.Entity<Brand>()
                .Property(b => b.Id)
                .HasJsonPropertyName("id");

            modelBuilder.Entity<Brand>()
                .Property(b => b.Name)
                .HasAnnotation("Display", "Marka")
                .HasColumnType("varchar(30)");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);
            #endregion

        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Brand> Brands { get; set; }
    }
}
