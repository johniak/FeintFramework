using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using FeintSite;

namespace FeintSite.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20170405065036_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("FeintSDK.SessionKey", b =>
                {
                    b.Property<int>("SessionKeyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.HasKey("SessionKeyId");

                    b.ToTable("SessionKey");
                });

            modelBuilder.Entity("FeintSDK.SessionProperty", b =>
                {
                    b.Property<int>("SessionPropertyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("OwnerSessionKeyId");

                    b.Property<string>("Value");

                    b.HasKey("SessionPropertyId");

                    b.HasIndex("OwnerSessionKeyId");

                    b.ToTable("SessionProperty");
                });

            modelBuilder.Entity("FeintSDK.SessionProperty", b =>
                {
                    b.HasOne("FeintSDK.SessionKey", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerSessionKeyId");
                });
        }
    }
}
