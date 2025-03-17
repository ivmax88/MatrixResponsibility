using Microsoft.EntityFrameworkCore;
using MatrixResponsibility.Common;
using System.Data;

namespace MatrixResponsibility.Data
{
    public class AppDbContext: DbContext
    {
        /// <summary>
        /// Коллекция проектов.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Коллекция пользователей.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Коллекция блоков контроля проектов.
        /// </summary>
        public DbSet<BKP> BKPs { get; set; }

        /// <summary>
        /// Коллекция корректировок проектов.
        /// </summary>
        public DbSet<ProjectCorrection> ProjectCorrections { get; set; }

        /// <summary>
        /// Коллекция ролей.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// Коллекция связей пользователей и ролей.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Начальные данные для пользователя
                entity.HasData(
                    new User { Id = 1,Login = "ivanov" , Email="ivanov@olimproekt.ru", FIO = "Иванов Максим Леонидович" },
                    new User { Id = 2,Login = "tsarev" , Email="tsarev@olimproekt.ru", FIO = "Царев Михаил Александрович" },
                    new User { Id = 3,Login = "vladimir", Email="vladimir@olimproekt.ru", FIO = "Ковалёв Владимир Александрович" },
                    new User { Id = 4,Login = "empty", Email="empty@olimproekt.ru", FIO = "empty empty empty" },
                    new User { Id = 5,Login = "not_found", Email="not_found@olimproekt.ru", FIO = "not_found not_found not_found" }
                );
            });

            modelBuilder.Entity<BKP>(entity =>
            {
                entity.HasOne(x => x.Director)
                .WithMany()
                .HasForeignKey(x => x.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

                // Начальные данные для 7 БКП
                entity.HasData(
                    new BKP { Id = 1, Name = "БКП №1", DirectorId = 2 },
                    new BKP { Id = 2, Name = "БКП №2", DirectorId = 2 },
                    new BKP { Id = 3, Name = "БКП №3", DirectorId = 2 },
                    new BKP { Id = 4, Name = "БКП №4", DirectorId = 2 },
                    new BKP { Id = 5, Name = "БКП №5", DirectorId = 2 },
                    new BKP { Id = 6, Name = "БКП №6", DirectorId = 2 },
                    new BKP { Id = 7, Name = "БКП №7", DirectorId = 2 }
                );
            });


            modelBuilder.Entity<Role>(entity =>
            {
                // Начальные данные для ролей
                entity.HasData(
                    new Role { Id = 1, Name = "admin", Description = "Администратор" },
                    new Role { Id = 2, Name = "dbkp", Description = "Директор БКП" },
                    new Role { Id = 3, Name = "gip", Description = "Главный инженер проекта" }
                );
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(x => new { x.UserId, x.RoleId });

                entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

                // Начальные данные для связи пользователя и роли
                entity.HasData(
                    new UserRole { UserId = 1, RoleId = 1 },
                    new UserRole { UserId = 2, RoleId = 1 },
                    new UserRole { UserId = 3, RoleId = 1 }
                );
            });


            #region Project
            modelBuilder.Entity<Project>()
                .HasOne(p => p.BKP)
                .WithMany(b => b.Projects)
                .HasForeignKey(p => p.BKPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.GIP)
                .WithMany()
                .HasForeignKey(p => p.GIPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.AssistantGIP)
                .WithMany()
                .HasForeignKey(p => p.AssistantGIPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.GAP)
                .WithMany()
                .HasForeignKey(p => p.GAPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.GKP)
                .WithMany()
                .HasForeignKey(p => p.GKPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.GP)
                .WithMany()
                .HasForeignKey(p => p.GPId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.EOM)
                .WithMany()
                .HasForeignKey(p => p.EOMId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.SS)
                .WithMany()
                .HasForeignKey(p => p.SSId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.AK)
                .WithMany()
                .HasForeignKey(p => p.AKId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Responsible)
                .WithMany()
                .HasForeignKey(p => p.ResponsibleId)
                .OnDelete(DeleteBehavior.SetNull);
            #endregion

            // Связь один-ко-многим между Project и ProjectCorrection
            modelBuilder.Entity<ProjectCorrection>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.Corrections)
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении Project удаляются все Corrections
        }
    }
}
