using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SamLearnsAzure.Models;

namespace SamLearnsAzure.Service.EFCore
{
    public partial class SamsAppDBContext : DbContext
    {
        public SamsAppDBContext()
        {
        }

        public SamsAppDBContext(DbContextOptions<SamsAppDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<Inventories> Inventories { get; set; }
        public virtual DbSet<InventoryParts> InventoryParts { get; set; }
        public virtual DbSet<InventorySets> InventorySets { get; set; }
        public virtual DbSet<Owners> Owners { get; set; }
        public virtual DbSet<OwnerSets> OwnerSets { get; set; }
        public virtual DbSet<PartCategories> PartCategories { get; set; }
        public virtual DbSet<PartRelationships> PartRelationships { get; set; }
        public virtual DbSet<Parts> Parts { get; set; }
        public virtual DbSet<Sets> Sets { get; set; }
        public virtual DbSet<SetImages> SetImages { get; set; }
        public virtual DbSet<Themes> Themes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //The generated models
            modelBuilder.Entity<Colors>(entity =>
            {
                entity.ToTable("colors");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.IsTrans).HasColumnName("is_trans");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Rgb)
                    .HasColumnName("rgb")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Inventories>(entity =>
            {
                entity.ToTable("inventories");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.SetNum)
                    .HasColumnName("set_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Version).HasColumnName("version");

                entity.HasOne(d => d.Set)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.SetNum)
                    .HasConstraintName("FK_inventories_sets");
            });

            modelBuilder.Entity<InventoryParts>(entity =>
            {
                entity.HasKey(e => e.InventoryPartId);

                entity.ToTable("inventory_parts");

                entity.Property(e => e.InventoryPartId).HasColumnName("inventory_part_id");

                entity.Property(e => e.ColorId).HasColumnName("color_id");

                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");

                entity.Property(e => e.IsSpare).HasColumnName("is_spare");

                entity.Property(e => e.PartNum)
                    .IsRequired()
                    .HasColumnName("part_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.InventoryParts)
                    .HasForeignKey(d => d.ColorId)
                    .HasConstraintName("FK_inventory_parts_colors");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryParts)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventory_parts_inventories");

                entity.HasOne(d => d.Part)
                    .WithMany(p => p.InventoryParts)
                    .HasForeignKey(d => d.PartNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventory_parts_parts");
            });

            modelBuilder.Entity<InventorySets>(entity =>
            {
                entity.HasKey(e => e.InventorySetId);

                entity.ToTable("inventory_sets");

                entity.Property(e => e.InventorySetId).HasColumnName("inventory_set_id");

                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.SetNum)
                    .IsRequired()
                    .HasColumnName("set_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventorySets)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventory_sets_inventories");

                entity.HasOne(d => d.Set)
                    .WithMany(p => p.InventorySets)
                    .HasForeignKey(d => d.SetNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_inventory_sets_sets");
            });

            modelBuilder.Entity<Owners>(entity =>
            {
                entity.ToTable("owners");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.OwnerName)
                    .IsRequired()
                    .HasColumnName("owner_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OwnerSets>(entity =>
            {
                entity.HasKey(e => e.OwnerSetId);

                entity.ToTable("owner_sets");

                entity.Property(e => e.OwnerSetId).HasColumnName("owner_set_id");

                entity.Property(e => e.Owned).HasColumnName("owned");

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");

                entity.Property(e => e.SetNum)
                    .IsRequired()
                    .HasColumnName("set_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Wanted).HasColumnName("wanted");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.OwnerSets)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_owner_sets_owner");

                entity.HasOne(d => d.Set)
                    .WithMany(p => p.OwnerSets)
                    .HasForeignKey(d => d.SetNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_owner_sets_sets");
            });

            modelBuilder.Entity<PartCategories>(entity =>
            {
                entity.ToTable("part_categories");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<PartRelationships>(entity =>
            {
                entity.HasKey(e => e.PartRelationshipId);

                entity.ToTable("part_relationships");

                entity.Property(e => e.PartRelationshipId).HasColumnName("part_relationship_id");

                entity.Property(e => e.ChildPartNum)
                    .HasColumnName("child_part_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ParentPartNum)
                    .HasColumnName("parent_part_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RelType)
                    .IsRequired()
                    .HasColumnName("rel_type")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Parts>(entity =>
            {
                entity.HasKey(e => e.PartNum);

                entity.ToTable("parts");

                entity.Property(e => e.PartNum)
                    .HasColumnName("part_num")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PartCatId).HasColumnName("part_cat_id");
                entity.Property(e => e.PartMaterialId).HasColumnName("part_material_id");
            });

            modelBuilder.Entity<Sets>(entity =>
            {
                entity.HasKey(e => e.SetNum);

                entity.ToTable("sets");

                entity.Property(e => e.SetNum)
                    .HasColumnName("set_num")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NumParts).HasColumnName("num_parts");

                entity.Property(e => e.ThemeId).HasColumnName("theme_id");

                entity.Property(e => e.Year).HasColumnName("year");

            });

            modelBuilder.Entity<SetImages>(entity =>
            {
                entity.HasKey(e => e.SetImageId);

                entity.ToTable("set_images");

                entity.Property(e => e.SetImageId)
                    .HasColumnName("set_image_id")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.SetImage).HasColumnName("set_image");

                entity.Property(e => e.SetNum)
                    .IsRequired()
                    .HasColumnName("set_num")
                    .HasMaxLength(100)
                    .IsUnicode(false);

            });

            modelBuilder.Entity<Themes>(entity =>
            {
                entity.ToTable("themes");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");
            });

            //Create a custom query
            modelBuilder.Query<SetParts>();
        }
    }
}
