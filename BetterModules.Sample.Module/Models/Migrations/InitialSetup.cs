using System;
using BetterModules.Core.DataAccess.DataContext.Migrations;
using FluentMigrator;

namespace BetterModules.Sample.Module.Models.Migrations
{
    /// <summary>
    /// Module initial database structure creation.
    /// </summary>
    [Migration(201502181430)]
    public class InitialSetup : DefaultMigration
    {
        public InitialSetup()
            : base(SampleModuleDescriptor.ModuleName)
        {            
        }

        public override void Up()
        {
            // Test Item Categories
            Create
                 .Table("TestItemCategories").InSchema(SchemaName)
                 .WithBaseColumns()
                 .WithColumn("Name").AsString(100).NotNullable();

            // Test Items
            Create
                .Table("TestItems").InSchema(SchemaName)
                .WithBaseColumns()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("TestItemCategoryId").AsGuid().NotNullable();

            Create
                .ForeignKey("Fk__TestItems__TestItemCategories")
                .FromTable("TestItems").InSchema(SchemaName).ForeignColumn("TestItemCategoryId")
                .ToTable("TestItemCategories").InSchema(SchemaName).PrimaryColumn("Id");

            Insert
                .IntoTable("TestItemCategories").InSchema(SchemaName)
                .Row(new
                {
                    Name = "ItemCategory1", 
                    Version = 5, 
                    CreatedOn = new DateTime(2010, 10, 10, 10, 10, 10),
                    ModifiedOn = new DateTime(2011, 11, 11, 11, 11, 11),
                    CreatedByUser = "TestCreator",
                    ModifiedByUser = "TestModifier",
                    IsDeleted = 0
                })
                .Row(new
                {
                    Name = "ItemCategory2", 
                    Version = 1, 
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedByUser = "TestCreator",
                    ModifiedByUser = "TestCreator",
                    IsDeleted = 0
                })
                .Row(new
                {
                    Name = "ChildCategory1", 
                    Version = 1, 
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedByUser = "TestCreator",
                    ModifiedByUser = "TestCreator",
                    IsDeleted = 0
                })
                .Row(new
                {
                    Name = "ChildCategory2", 
                    Version = 1, 
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedByUser = "TestCreator",
                    ModifiedByUser = "TestCreator",
                    IsDeleted = 0
                });

            // Inherited Test Items
            Create
                 .Table("InheritedTestItems").InSchema(SchemaName)
                 .WithColumn("Id").AsGuid().PrimaryKey()
                 .WithColumn("Description").AsString(100).NotNullable();

            Create
                .ForeignKey("Fk__InheritedTestItems__TestItems")
                .FromTable("InheritedTestItems").InSchema(SchemaName).ForeignColumn("Id")
                .ToTable("TestItems").InSchema(SchemaName).PrimaryColumn("Id");

        }
    }
}