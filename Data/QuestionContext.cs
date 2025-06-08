using Microsoft.EntityFrameworkCore;
using Web6.Models;

namespace Web6.Data
{
    public class QuestionContext : DbContext
    {
        public QuestionContext(DbContextOptions<QuestionContext> options)
            : base(options)
        { }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                        .HasMany(c => c.Questions)
                        .WithOne(q => q.Category)
                        .HasForeignKey(q => q.CategoryId)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
