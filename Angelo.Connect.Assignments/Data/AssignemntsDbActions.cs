using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Assignments.Models;
using System.Collections.Generic;
using Angelo.Connect.Security;
using Angelo.Connect.Services;

namespace Angelo.Connect.Assignments.Data
{
    public static class AssignmentsDbActions
    {

        public static void CreateSchemas(AssignmentsDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(AssignmentsDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.AssignmentWidget"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[AssignmentWidget](
	                    [Id] [nvarchar](50) NOT NULL,
	                    [Title] [nvarchar](500) NULL,
	                    CONSTRAINT [PK_AssignmentWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.Assignment"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[Assignment](
                        [Id]               NVARCHAR (450) NOT NULL,
                        [AllowComments]    BIT            NOT NULL,
                        [AssignmentBody]   NVARCHAR (MAX) NULL,
                        [CreatedBy]        NVARCHAR (MAX) NULL,
                        [CreatedUTC]       DATETIME2 (7)  NOT NULL,
                        [DueUTC]           DATETIME2 (7)  NOT NULL,
                        [NotificationId]   NVARCHAR (MAX) NULL,
                        [OwnerId]          NVARCHAR (MAX) NULL,
                        [OwnerLevel]       INT            NOT NULL,
                        [SendNotification] BIT            NOT NULL,
                        [Status]           NVARCHAR (10)  NULL,
                        [TimeZoneId]       NVARCHAR (MAX) NULL,
                        [Title]            NVARCHAR (MAX) NULL,
	                    CONSTRAINT [PK_Assignment] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.AssignmentCategory"))
            {

                dbContext.Database.ExecuteSqlCommand(@"
                        CREATE TABLE [plugin].[AssignmentCategory] (
                            [Id]                   NVARCHAR (450) NOT NULL,
                            [AssignmentCategoryId] NVARCHAR (450) NULL,
                            [OwnerId]              NVARCHAR (MAX) NULL,
                            [OwnerLevel]           INT            NOT NULL,
                            [Title]                NVARCHAR (MAX) NULL,
	                        CONSTRAINT [PK_AssignmentCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                        );
                ");
            }

            if (!dbContext.Database.TableExists("plugin.AssignmentCategoryLink"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[AssignmentCategoryLink] (
                        [AssignmentId] NVARCHAR (450) NOT NULL,
                        [CategoryId]   NVARCHAR (450) NOT NULL
                        CONSTRAINT [PK_AssignmentCategoryLink] PRIMARY KEY CLUSTERED ([AssignmentId] ASC, [CategoryId] ASC)
                    );

                    CREATE NONCLUSTERED INDEX [IX_AssignmentCategoryLink_1]
                        ON [plugin].[AssignmentCategoryLink]([AssignmentId] ASC);

                    CREATE NONCLUSTERED INDEX [IX_AssignmentCategoryLink_2]
                        ON [plugin].[AssignmentCategoryLink]([CategoryId] ASC);
                ");
            }

            if (!dbContext.Database.TableExists("plugin.AssignmentUserGroup"))
            {

                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[AssignmentUserGroup] (
                        [AssignmentId] NVARCHAR (450) NOT NULL,
                        [UserGroupId]  NVARCHAR (450) NOT NULL
                        CONSTRAINT [PK_AssignmentUserGroup] PRIMARY KEY CLUSTERED ([AssignmentId] ASC, [UserGroupId] ASC)
                    );                

                    CREATE NONCLUSTERED INDEX [IX_AssignmentUserGroup_1]
                        ON [plugin].[AssignmentUserGroup]([AssignmentId] ASC);

                    CREATE NONCLUSTERED INDEX [IX_AssignmentUserGroup_2]
                        ON [plugin].[AssignmentUserGroup]([UserGroupId] ASC);
                ");
            }
        }

        public static void InsertSeedData(AssignmentsDbContext dbContext)
        {
            string adminId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73";
            string janeId = "EECEFCC1-8050-4A0F-A5A5-D7ED19A078A8";

            var assignmentCategories = new List<AssignmentCategory>();

            assignmentCategories.Add(AddAssignmentCategory(dbContext, "10th Grade English", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "10th Grade Science", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "10th Grade Math", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "10th Grade History", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "11th Grade English", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "11th Grade Science", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "11th Grade Math", adminId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "11th Grade History", adminId));

            assignmentCategories.Add(AddAssignmentCategory(dbContext, "3rd Grade English", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "3rd Grade Science", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "3rd Grade Math", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "3rd Grade History", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "4th Grade English", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "4th Grade Science", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "4th Grade Math", janeId));
            assignmentCategories.Add(AddAssignmentCategory(dbContext, "4th Grade History", janeId));

            AddAssignment(dbContext, assignmentCategories[3], "History Chapter 3", "Read Chapter 3 (pg. 20-28) of your History book and answer questions 1-7 on page 29.", 10);
            AddAssignment(dbContext, assignmentCategories[3], "History Chapter 4", "Read Chapter 4 (pg. 30-37) of your History book and answer questions 1-12 on page 38.", 11);
            AddAssignment(dbContext, assignmentCategories[3], "History Chapter 5", "Read Chapter 5 (pg. 39-45) of your History book and answer questions 1-5,7,9, and 11 on page 46.", 14);
            AddAssignment(dbContext, assignmentCategories[2], "Math Chapter 2", "Read Chapter 2 (pg. 10-17) of your Algebra book and answer questions 1-5,8, and 10.", 10);
            AddAssignment(dbContext, assignmentCategories[2], "Math Chapter 3", "Read Chapter 3 (pg. 19-28) of your Algebra book and answer questions 2-8 and 10-20.", 12);

            dbContext.SaveChanges();
        }

        private static AssignmentCategory AddAssignmentCategory(AssignmentsDbContext db, string title, string userId)
        {
            var ac = new AssignmentCategory()
            {
                Id = (title + "-" + userId).ToUpper().Replace(' ', '-'),
                OwnerLevel = OwnerLevel.User,
                OwnerId = userId,
                Title = title
            };
            if (!db.AssignmentCategories.Any(c => c.Id == ac.Id))
                db.AssignmentCategories.Add(ac);
            return ac;
        }

        private static Assignment AddAssignment(AssignmentsDbContext db, AssignmentCategory category, string title, string body, int dayTillDue)
        {
            var assignment = new Assignment()
            {
                Id = (title + "-" + category.OwnerId).ToUpper().Replace(' ', '-'),
                OwnerId = category.OwnerId,
                OwnerLevel = OwnerLevel.User,
                CreatedUTC = DateTime.UtcNow,
                Status = AssignmentStatus.Draft,
                Title = title,
                AssignmentBody = body,
                AllowComments = false,
                DueUTC = DateTime.Now.AddDays(dayTillDue),
                TimeZoneId = TimeZoneHelper.DefaultTimeZoneId,
                SendNotification = false,
                CreatedBy = category.OwnerId
            };
            if (!db.Assignments.Any(a => a.Id == assignment.Id))
                db.Assignments.Add(assignment);
            if (!db.AssignmentCategoryLinks.Any(l => l.AssignmentId == assignment.Id && l.CategoryId == category.Id))
                db.AssignmentCategoryLinks.Add(new AssignmentCategoryLink() { AssignmentId = assignment.Id, CategoryId = category.Id });
            return assignment;
        }

    }
}
