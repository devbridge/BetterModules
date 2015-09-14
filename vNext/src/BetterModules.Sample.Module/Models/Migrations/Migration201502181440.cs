using BetterModules.Core.DataAccess.DataContext.Migrations;
using BetterModules.Core.Models;
using BetterModules.Core.Modules.Registration;
using FluentMigrator;

namespace BetterModules.Sample.Module.Models.Migrations
{
    /// <summary>
    /// Module database structure update.
    /// </summary>
    [Migration(201502181440)]
    public class Migration201302211227 : DefaultMigration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Migration201302211227"/> class.
        /// </summary>
        public Migration201302211227()
            : base(SampleModuleDescriptor.ModuleName)
        {
        }

        public override void Up()
        {
            Create
                .Table("TestItemChildren").InSchema(SchemaName)
                .WithBaseColumns()
                .WithColumn("Name").AsString(MaxLength.Name).NotNullable()
                .WithColumn("TestItemId").AsGuid().NotNullable()
                .WithColumn("TestItemCategoryId").AsGuid().NotNullable();

            Create
                .ForeignKey("Fk__TestItemChildren__TestItemCategories")
                .FromTable("TestItemChildren").InSchema(SchemaName).ForeignColumn("TestItemCategoryId")
                .ToTable("TestItemCategories").InSchema(SchemaName).PrimaryColumn("Id");

            Create
                .ForeignKey("Fk__TestItemChildren__TestItems")
                .FromTable("TestItemChildren").InSchema(SchemaName).ForeignColumn("TestItemId")
                .ToTable("TestItems").InSchema(SchemaName).PrimaryColumn("Id");

            const string sqlQuery = @"

INSERT INTO module_bettermodulessample.TestItems (Id, Name, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser, 
    CreatedOn, ModifiedOn) 
    VALUES ('{0}', 'Item1', (SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ItemCategory1'), 1, 'Creator', 'Modifier',
    '2015-01-01 01:01:01', '2015-02-02 02:02:02')
INSERT INTO module_bettermodulessample.TestItems (Name, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser) 
    VALUES ('Item2', (SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ItemCategory2'), 1, 'TestAdmin', 'TestAdmin')

INSERT INTO module_bettermodulessample.InheritedTestItems (Id, Description) 
    VALUES ('{0}', 'Inherited Item1')

INSERT INTO module_bettermodulessample.TestItemChildren (Name, TestItemId, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser) 
VALUES ('Item1 Child1', 
	(SELECT Id FROM module_bettermodulessample.TestItems WHERE Name = 'Item1'),
	(SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ChildCategory1'), 1, 'TestAdmin', 'TestAdmin')

INSERT INTO module_bettermodulessample.TestItemChildren (Name, TestItemId, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser) 
VALUES ('Item1 Child2', 
	(SELECT Id FROM module_bettermodulessample.TestItems WHERE Name = 'Item1'),
	(SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ChildCategory2'), 1, 'TestAdmin', 'TestAdmin')

INSERT INTO module_bettermodulessample.TestItemChildren (Name, TestItemId, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser) 
VALUES ('Item2 Child1', 
	(SELECT Id FROM module_bettermodulessample.TestItems WHERE Name = 'Item2'),
	(SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ChildCategory1'), 1, 'TestAdmin', 'TestAdmin')

INSERT INTO module_bettermodulessample.TestItemChildren (Name, TestItemId, TestItemCategoryId, Version, CreatedByUser, ModifiedByUser) 
VALUES ('Item2 Child2', 
	(SELECT Id FROM module_bettermodulessample.TestItems WHERE Name = 'Item2'),
	(SELECT Id FROM module_bettermodulessample.TestItemCategories WHERE Name = 'ChildCategory2'), 1, 'TestAdmin', 'TestAdmin')
";
            Execute.Sql(string.Format(sqlQuery, SampleModuleDescriptor.TestItemModelId));
        }
    }
}