using System;
using Microsoft.EntityFrameworkCore;
using RockBotPanel.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RockBotPanel.Data
{
    public partial class d940mhn2jd7mllContext : DbContext
    {
        public d940mhn2jd7mllContext()
        {
        }

        public d940mhn2jd7mllContext(DbContextOptions<d940mhn2jd7mllContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Allusers> Allusers { get; set; }
        public virtual DbSet<Chatinfo> Chatinfo { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("connect");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Allusers>(entity =>
            {
                entity.ToTable("allusers");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Username).HasColumnName("username");
            });

            modelBuilder.Entity<Chatinfo>(entity =>
            {
                entity.ToTable("chatinfo");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DisabledCommands).HasColumnName("disabled_commands");

                entity.Property(e => e.Rules).HasColumnName("rules");

                entity.Property(e => e.WarnsQuantity).HasColumnName("warns_quantity");

                entity.Property(e => e.Welcome).HasColumnName("welcome");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Chatid).HasColumnName("chatid");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.Property(e => e.Warns).HasColumnName("warns");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
