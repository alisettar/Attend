using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Attend.Infrastructure.Persistence;

#nullable disable

namespace Attend.Infrastructure.Migrations
{
    [DbContext(typeof(AttendDbContext))]
    partial class AttendDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Attend.Domain.Entities.Attendance", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("TEXT");

                b.Property<bool>("CheckedIn")
                    .HasColumnType("INTEGER");

                b.Property<DateTime?>("CheckedInAt")
                    .HasColumnType("TEXT");

                b.Property<Guid>("EventId")
                    .HasColumnType("TEXT");

                b.Property<int>("Status")
                    .HasColumnType("INTEGER");

                b.Property<Guid>("UserId")
                    .HasColumnType("TEXT");

                b.HasKey("Id");
                b.HasIndex("EventId");
                b.HasIndex("UserId", "EventId").IsUnique();

                b.ToTable("Attendances");
            });

            modelBuilder.Entity("Attend.Domain.Entities.Event", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("TEXT");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<DateTime>("Date")
                    .HasColumnType("TEXT");

                b.Property<string>("Description")
                    .HasMaxLength(1000)
                    .HasColumnType("TEXT");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("TEXT");

                b.HasKey("Id");
                b.ToTable("Events");
            });

            modelBuilder.Entity("Attend.Domain.Entities.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("TEXT");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("TEXT");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("TEXT");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("TEXT");

                b.Property<string>("Phone")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("TEXT");

                b.Property<string>("QRCode")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("TEXT");

                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.HasIndex("QRCode").IsUnique();

                b.ToTable("Users");
            });

            modelBuilder.Entity("Attend.Domain.Entities.Attendance", b =>
            {
                b.HasOne("Attend.Domain.Entities.Event", "Event")
                    .WithMany("Attendances")
                    .HasForeignKey("EventId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Attend.Domain.Entities.User", "User")
                    .WithMany("Attendances")
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Event");
                b.Navigation("User");
            });

            modelBuilder.Entity("Attend.Domain.Entities.Event", b =>
            {
                b.Navigation("Attendances");
            });

            modelBuilder.Entity("Attend.Domain.Entities.User", b =>
            {
                b.Navigation("Attendances");
            });
#pragma warning restore 612, 618
        }
    }
}
