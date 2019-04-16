using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Data
{
    public class P480000_CreateCalendarTables : IAppMigration
    {
        private CalendarDbContext _dbContext;

        public string Id { get; } = "P480000";

        public string Migration { get; } = "Create Initial Calendar Tables";

        public P480000_CreateCalendarTables(CalendarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            // If any calendar table exists then skip 
            if (_dbContext.Database.TableExists("CalendarEvent", "plugin"))
                return MigrationResult.Skipped();

            // CalendarEvent table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarEvent](
	                [EventId] [nvarchar](50) NOT NULL,
                    [DocumentId] [nvarchar](50) NOT NULL,
	                [Title] [nvarchar](500) NOT NULL,
                    [Posted] [datetime] NOT NULL,
                    [Style] [nvarchar] (50),
                    [BackgroundColor] [nvarchar] (50),
                    [Description] [nvarchar] (max),
                    [EventStart] [datetime] NOT NULL,
                    [EventEnd] [datetime] NOT NULL,
                    [SiteId] [nvarchar](450) NOT NULL,
                    [UserId] [nvarchar] (450) NULL,
                    [Phone] [nvarchar] (20) NULL,
                    [AllDayEvent] [bit] NULL,
                    [Url] [nvarchar] (450) NULL,
                    [IsRecurrent] [bit] NOT NULL,
                    [Location] [nvarchar] (450) NULL,
                    [ShowOrganizerName] [bit] NULL,
                    [ShowPhoneNumber] [bit] NULL,
	                CONSTRAINT [PK_CalendarEvent] PRIMARY KEY CLUSTERED ([EventId] ASC)
                )
            ");

            // Calendar Widget Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarWidgetSetting](
	                [Id] [nvarchar](50) NOT NULL,
                    [CalendarId] [nvarchar](50),
                    [Title] [nvarchar] (200) NULL,
                    [DefaultView] [nvarchar](100) NOT NULL,
                    [StartDayOfWeek] [nvarchar] (50),
                    [Format12] [bit],
                    [SiteId] [nvarchar](50) NOT NULL,
                    [HideWeekends] [bit] NOT NULL,                     
	                CONSTRAINT [PK_CalendarWidgetSetting] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            // CalendarEventGroup Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarEventGroup](
	                [EventGroupId] [nvarchar](50) NOT NULL,
                    [SiteId] [nvarchar](450) NOT NULL,
                    [UserId] [nvarchar] (450) NULL,
                    [Title] [nvarchar](200) NULL,
                        
	                CONSTRAINT [PK_CalendarEventGroup] PRIMARY KEY CLUSTERED ([EventGroupId] ASC)
                )
            ");


            // CalendarEvent to CalendarEventGroup Mapping Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarEventGroupEvent](
	                [EventId] [nvarchar](450) NOT NULL,
                    [EventGroupId] [nvarchar] (450) NOT NULL,
                                                
	                CONSTRAINT [PK_CalendarEventGroupEvent] PRIMARY KEY CLUSTERED ([EventId], [EventGroupId])
                )
            ");


            // CalendarWidget to CalendarEventGroup Mapping Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarWidgetEventGroup](
	                [Id] [nvarchar](50) NOT NULL,
                    [WidgetId] [nvarchar](450) NOT NULL,
                    [EventGroupId] [nvarchar] (450) NULL,
                    [Color] [nvarchar] (20) NULL,
                    [Title] [nvarchar] (20) NULL,                    
	                CONSTRAINT [PK_CalendarWidgetEventGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            // CalendarEventTag Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarEventTag](
	                [Id] [nvarchar](50) NOT NULL,
                    [EventId] [nvarchar](50),
                    [TagId] [nvarchar](50) NULL,
                                  
	                CONSTRAINT [PK_CalendarEventTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            // CalendarEventRecurrence Table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[CalendarEventRecurrence](
                    [Id] [nvarchar](50) NOT NULL,
                    [EventId] [nvarchar](50),
	                [Frequency] [nvarchar](50) NOT NULL,
                    [Interval] [int] NOT NULL,
                    [EndDate] [datetime] NULL,
                    [Count] [int] NULL,
                    [DaysOfWeek] [nvarchar](50) NULL,
                    [DayOfMonth] [int] NULL,
                    [Months] [nvarchar] (50) NULL,

	                CONSTRAINT [PK_CalendarEventRecurrence] PRIMARY KEY CLUSTERED ([EventId] ASC)
                )
            ");

            return MigrationResult.Success();
        }
    }
}
