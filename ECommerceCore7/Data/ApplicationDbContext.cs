using ECommerceCore7.Areas.Identity.Models;
using ECommerceCore7.Models.Entities;
using ECommerceCore7.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore7.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        int,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken> // Mehdi: Class declaration modified
    {

        // Table Names
        private string usersTableName = "Users";
        private string userClaimsTableName = "UserClaims";
        private string userLoginsTableName = "UserLogins";
        private string userTokensTableName = "UserTokens";
        private string rolesTableName = "Roles";
        private string roleClaimsTableName = "RoleClaims";
        private string userRolesTableName = "UserRoles";


        // Column Names
        private string userIDColumnName = "UserID";
        private string userClaimIDColumnName = "UserClaimID";
        private string userNameColumnName = "Username";
        private string normalizedUserNameColumnName = "NormalizedUsername";
        private string roleIDColumnName = "RoleID";
        private string roleClaimIDColumnName = "RoleClaimID";
        private string roleNameColumnName = "RoleName";
        private string roleNormalizedNameColumnName = "RoleNormalizedName";

        // Mehdi:Start: Added Following Lines in ApplicationDbContext
        public DbSet<UserLoginWithSms> UserLoginWithSms { get; set; }
        public DbSet<UserPreRegisteration> PreRegisterations { get; set; }
        // Mehdi:End

        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<PageGroup> PageGroups { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<InventoryActionType> InventoryActionTypes { get; set; }
        public DbSet<InventoryAction> InventoryActions { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<VideoType> VideoTypes { get; set; }
        //public DbSet<AuthenticationTicket> AuthenticationTickets { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<PageTag> PageTags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.ToTable(usersTableName);
                b.Property(e => e.Id).HasColumnName(userIDColumnName);
                b.Property(e => e.UserName).HasColumnName(userNameColumnName);
                b.Property(e => e.NormalizedUserName).HasColumnName(normalizedUserNameColumnName);
            });

            modelBuilder.Entity<ApplicationUserClaim>(b =>
            {
                b.ToTable(userClaimsTableName);
                b.Property(e => e.Id).HasColumnName(userClaimIDColumnName);
                b.Property(e => e.UserId).HasColumnName(userIDColumnName);
            });

            modelBuilder.Entity<ApplicationUserLogin>(b =>
            {
                b.ToTable(userLoginsTableName);
                b.Property(e => e.UserId).HasColumnName(userIDColumnName);
            });

            modelBuilder.Entity<ApplicationUserToken>(b =>
            {
                b.ToTable(userTokensTableName);
                b.Property(e => e.UserId).HasColumnName(userIDColumnName);
            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                b.ToTable(rolesTableName);
                b.Property(e => e.Id).HasColumnName(roleIDColumnName);
                b.Property(e => e.Name).HasColumnName(roleNameColumnName);
                b.Property(e => e.NormalizedName).HasColumnName(roleNormalizedNameColumnName);
            });

            modelBuilder.Entity<ApplicationRoleClaim>(b =>
            {
                b.ToTable(roleClaimsTableName);
                b.Property(e => e.Id).HasColumnName(roleClaimIDColumnName);
                b.Property(e => e.RoleId).HasColumnName(roleIDColumnName);
            });

            modelBuilder.Entity<ApplicationUserRole>(b =>
            {
                b.ToTable(userRolesTableName);
            });

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole() { Id = 1, Name = "Admin", NormalizedName = "مدیران سایت" }
            );

            //modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser() {Id = 1,UserName = "SuperAdmin",PasswordHash = "lsdmvlksdvmlskdmdslvk",Email = "novinbox@gmail.com",EmailConfirmed = true,PhoneNumber = "09031466281",PhoneNumberConfirmed = true});
            /*
            modelBuilder.Entity<ProductGroup>().HasData(
                new ProductGroup() { ProductGroupID = 1, ProductGroupTitle = "گوشی موبایل"},
                new ProductGroup() { ProductGroupID = 2, ProductGroupTitle = "تبلت"},
                new ProductGroup() { ProductGroupID = 3, ProductGroupTitle = "لپتاپ"}
            );
            */

            //var pageGroups = this.PageGroups.ToList();
            //if (!pageGroups.Any())
            /*
            {
                modelBuilder.Entity<PageGroup>().HasData(
                    new PageGroup() { PageGroupID = 1, PageGroupTitle = "خبرها", PageGroupIcon = "fa-solid fa-newspaper" },
                    new PageGroup() { PageGroupID = 2, PageGroupTitle = "مقاله ها", PageGroupIcon = "fa-light fa-memo" },
                    new PageGroup() { PageGroupID = 3, PageGroupTitle = "تخفیف ها", PageGroupIcon = "fa-sharp fa-solid fa-tag" }
                );
            }
            */

            modelBuilder.Entity<InventoryActionType>().HasData(
                new InventoryActionType() { InventoryActionID = 1, InventoryActionTypeTitle = "ورود به انبار" },
                new InventoryActionType() { InventoryActionID = 2, InventoryActionTypeTitle = "خروج از انبار" }
            );

            modelBuilder.Entity<VideoType>().HasData(
                new VideoType() { VideoTypeID = 1, VideoTypeTitle = "ویدئو برای محصول" },
                new VideoType() { VideoTypeID = 2, VideoTypeTitle = "ویدئو برای صفحه" }
            );

        }





    }
}