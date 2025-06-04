using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmplStruct.Models;

public partial class User15Context : DbContext
{
    public User15Context()
    {
    }

    public User15Context(DbContextOptions<User15Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentSubdivision> DepartmentSubdivisions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeRelation> EmployeeRelations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5421;Database=user15;Username=user15;Password=3XkvwMOb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Departmentid).HasName("new_department_pkey");

            entity.ToTable("department", "public3");

            entity.Property(e => e.Departmentid)
                .HasDefaultValueSql("nextval('public3.new_department_departmentid_seq'::regclass)")
                .HasColumnName("departmentid");
            entity.Property(e => e.Departmentname)
                .HasMaxLength(100)
                .HasColumnName("departmentname");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<DepartmentSubdivision>(entity =>
        {
            entity.HasKey(e => e.Subdivisionid).HasName("subdivision_pkey");

            entity.ToTable("department_subdivision", "public3");

            entity.Property(e => e.Subdivisionid)
                .HasDefaultValueSql("nextval('public3.subdivision_subdivisionid_seq'::regclass)")
                .HasColumnName("subdivisionid");
            entity.Property(e => e.Departmentid).HasColumnName("departmentid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Headpositionid).HasColumnName("headpositionid");
            entity.Property(e => e.Subdivisionname)
                .HasMaxLength(100)
                .HasColumnName("subdivisionname");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentSubdivisions)
                .HasForeignKey(d => d.Departmentid)
                .HasConstraintName("subdivision_departmentid_fkey");

            entity.HasOne(d => d.Headposition).WithMany(p => p.DepartmentSubdivisions)
                .HasForeignKey(d => d.Headpositionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("subdivision_headpositionid_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Employeeid).HasName("employee_pkey");

            entity.ToTable("employee", "public3");

            entity.HasIndex(e => e.Personalnumber, "employee_personalnumber_key").IsUnique();

            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.IsManager)
                .HasDefaultValue(false)
                .HasColumnName("is_manager");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .HasColumnName("middlename");
            entity.Property(e => e.Mobilephone)
                .HasMaxLength(20)
                .HasColumnName("mobilephone");
            entity.Property(e => e.Office)
                .HasMaxLength(10)
                .HasColumnName("office");
            entity.Property(e => e.Personalnumber)
                .HasMaxLength(20)
                .HasColumnName("personalnumber");
            entity.Property(e => e.Positionid).HasColumnName("positionid");
            entity.Property(e => e.Subdivisionid).HasColumnName("subdivisionid");
            entity.Property(e => e.Workphone)
                .HasMaxLength(20)
                .HasColumnName("workphone");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Positionid)
                .HasConstraintName("employee_positionid_fkey");

            entity.HasOne(d => d.Subdivision).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Subdivisionid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_subdivisionid_fkey");
        });

        modelBuilder.Entity<EmployeeRelation>(entity =>
        {
            entity.HasKey(e => e.Relationid).HasName("employee_relations_pkey");

            entity.ToTable("employee_relations", "public3");

            entity.Property(e => e.Relationid).HasColumnName("relationid");
            entity.Property(e => e.Assistantid).HasColumnName("assistantid");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Managerid).HasColumnName("managerid");

            entity.HasOne(d => d.Assistant).WithMany(p => p.EmployeeRelationAssistants)
                .HasForeignKey(d => d.Assistantid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_relations_assistantid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeRelationEmployees)
                .HasForeignKey(d => d.Employeeid)
                .HasConstraintName("employee_relations_employeeid_fkey");

            entity.HasOne(d => d.Manager).WithMany(p => p.EmployeeRelationManagers)
                .HasForeignKey(d => d.Managerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_relations_managerid_fkey");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Positionid).HasName("position_pkey");

            entity.ToTable("position", "public3");

            entity.Property(e => e.Positionid).HasColumnName("positionid");
            entity.Property(e => e.Ismanager)
                .HasDefaultValue(false)
                .HasColumnName("ismanager");
            entity.Property(e => e.Positionname)
                .HasMaxLength(100)
                .HasColumnName("positionname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
