using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Angelo.Connect.Logging;
using Microsoft.Extensions.Logging;

namespace Angelo.Connect.Web.Migrations
{
    [DbContext(typeof(DbLogContext))]
    [Migration("20180126195329_DbLog_Migration20180126")]
    partial class DbLog_Migration20180126
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Angelo.Connect.Logging.DbLogEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Category");

                    b.Property<DateTime>("Created");

                    b.Property<int>("EventCode");

                    b.Property<string>("EventName");

                    b.Property<int>("LogLevel");

                    b.Property<string>("Message");

                    b.Property<string>("ResourceId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("LogEvent","log");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DbLogEventId");

                    b.Property<string>("DocumentId");

                    b.Property<int?>("EventId");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("DocumentEvent","log");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentEvent", b =>
                {
                    b.HasOne("Angelo.Connect.Logging.DbLogEvent", "Event")
                        .WithMany()
                        .HasForeignKey("EventId");
                });
        }
    }
}
