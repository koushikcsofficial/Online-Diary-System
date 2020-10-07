namespace DiarySystemWebApp.Models
{
    using System.Data.Entity;

    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }

        public virtual DbSet<AccountDetail> AccountDetails { get; set; }
        public virtual DbSet<DiaryDetail> DiaryDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_Email)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_Password)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_LastName)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_Address)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .Property(e => e.User_Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<AccountDetail>()
                .HasMany(e => e.SubmittedDiaryDetails)
                .WithRequired(e => e.SubmitionAccountDetail)
                .HasForeignKey(e => e.Account_Id);

            modelBuilder.Entity<AccountDetail>()
                .HasMany(e => e.AcceptedDiaryDetails)
                .WithOptional(e => e.AcceptanceAccountDetail)
                .HasForeignKey(e => e.Diary_AcceptedBy);

            modelBuilder.Entity<DiaryDetail>()
                .Property(e => e.Diary_Subject)
                .IsUnicode(false);

            modelBuilder.Entity<DiaryDetail>()
                .Property(e => e.Diary_Content)
                .IsUnicode(false);
        }
    }
}
