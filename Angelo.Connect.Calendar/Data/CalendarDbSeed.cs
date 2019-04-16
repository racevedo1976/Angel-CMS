using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Angelo.Connect.Calendar.Data
{
    public static class CalendarDbSeed
    {
        public static void CreateSchemas(CalendarDbContext dbContext)
        {
            //shouldnt this be taken care of by the plugin framework??
            dbContext.Database.ExecuteSqlCommand(@"
              IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(CalendarDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.CalendarEvent"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
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
            }

            if (!dbContext.Database.TableExists("plugin.CalendarWidgetSetting"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
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
            }


            if (!dbContext.Database.TableExists("plugin.CalendarEventGroup"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[CalendarEventGroup](
	                    [EventGroupId] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](450) NOT NULL,
                        [UserId] [nvarchar] (450) NULL,
                        [Title] [nvarchar](200) NULL,
                        
	                    CONSTRAINT [PK_CalendarEventGroup] PRIMARY KEY CLUSTERED ([EventGroupId] ASC)
                    )
                ");
            }

            //relationships with EventGroups
            if (!dbContext.Database.TableExists("plugin.CalendarEventGroupEvent"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[CalendarEventGroupEvent](
	                    [EventId] [nvarchar](450) NOT NULL,
                        [EventGroupId] [nvarchar] (450) NOT NULL,
                                                
	                    CONSTRAINT [PK_CalendarEventGroupEvent] PRIMARY KEY CLUSTERED ([EventId], [EventGroupId])
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.CalendarWidgetEventGroup"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[CalendarWidgetEventGroup](
	                    [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](450) NOT NULL,
                        [EventGroupId] [nvarchar] (450) NULL,
                        [Color] [nvarchar] (20) NULL,
                        [Title] [nvarchar] (20) NULL,                    
	                    CONSTRAINT [PK_CalendarWidgetEventGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }


            if (!dbContext.Database.TableExists("plugin.CalendarEventTag"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[CalendarEventTag](
	                    [Id] [nvarchar](50) NOT NULL,
                        [EventId] [nvarchar](50),
                        [TagId] [nvarchar](50) NULL,
                                  
	                    CONSTRAINT [PK_CalendarEventTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }


            if (!dbContext.Database.TableExists("plugin.CalendarEventRecurrence"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
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
            }


        }
        
        public static void InsertSeedData(CalendarDbContext dbContext)
        {
            if (dbContext.CalendarEvents.Count() == 0)
            {
                var event1 = new Models.CalendarEvent
                {
                    EventId = Guid.NewGuid().ToString("N"),
                    DocumentId = Guid.NewGuid().ToString("N"),
                    Title = "Health Forms are Due",
                    Posted = DateTime.Now,
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",  //ADMIN
                    Style = "event-info",
                    Description = @"<p><img class='img-responsive' src='/img/SeedImages/cats1_200_200.jpg' /></p>
                                    <h2>How to Be a Godly Man</h2>
                                    <p>Men's Breakfast is our monthly event where all men of the church are invited
                                         to grow in the knowledge and love of Jesus Christ.</p>
                                    <p>We encourage Lifers to invite other men - unconnected, no-believers, friends, neighbors, co-workers - to join them as an outreach
                                    to the community around us</p>",
                    EventStart = DateTime.Parse($"{DateTime.Now.Month.ToString()}/26/{DateTime.Now.Year.ToString()} 2:00:00 PM"),
                    EventEnd = DateTime.Parse($"4/30/{DateTime.Now.Year.ToString()} 10:15:00 PM"),
                    Phone = "919-277-2222",
                    BackgroundColor ="Red",
                    IsRecurrent= true,
                    ShowOrganizerName = true,
                    ShowPhoneNumber = true
                };

                var eventRecurrence = new Models.CalendarEventRecurrence
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EventId = event1.EventId,
                    Frequency = "Daily",
                    Interval = 1,
                    Count = 7
                };

               
                var event2 = new Models.CalendarEvent
                {
                    EventId = Guid.NewGuid().ToString("N"),
                    DocumentId = Guid.NewGuid().ToString("N"),
                    Title = "Women's Soccer Fundraising",
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",  //ADMIN
                    Posted = DateTime.Now,
                    Style = "event-success",
                    Description = "Classes will be held at our main location.",
                    EventStart = DateTime.Parse($"{DateTime.Now.Month.ToString()}/25/2018 2:00:00 PM"),
                    EventEnd = DateTime.Parse($"4/25/2017 3:15:00 PM"),
                    Phone = "919-277-1111",
                    BackgroundColor = "Pink",
                    IsRecurrent = true,
                    ShowOrganizerName = true,
                    ShowPhoneNumber = true
                };

                var eventRecurrence2 = new Models.CalendarEventRecurrence
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EventId = event2.EventId,
                    Frequency = "Weekly",
                    Interval = 1,
                    Count = 10,
                    DaysOfWeek = "0,3"
                };

                var event3 = new Models.CalendarEvent
                {
                    EventId = Guid.NewGuid().ToString("N"),
                    DocumentId = Guid.NewGuid().ToString("N"),
                    Title = "Child Custody Battle",
                    Posted = DateTime.Now,
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",  //ADMIN
                    Style = "event-info",
                    Description = @"<p><img class='img-responsive' src='/img/SeedImages/cats1_200_200.jpg' /></p>
                                    <h2>How to Be a Godly Man</h2>
                                    <p>Men's Breakfast is our monthly event where all men of the church are invited
                                         to grow in the knowledge and love of Jesus Christ.</p>
                                    <p>We encourage Lifers to invite other men - unconnected, no-believers, friends, neighbors, co-workers - to join them as an outreach
                                    to the community around us</p>",
                    EventStart = DateTime.Parse($"3/26/2018 2:00:00 PM"),
                    EventEnd = DateTime.Parse($"12/30/2018 10:15:00 PM"),
                    Phone = "919-277-2222",
                    BackgroundColor = "Purple",
                    IsRecurrent = true,
                    ShowOrganizerName = true,
                    ShowPhoneNumber = true
                };

                var eventRecurrence3 = new Models.CalendarEventRecurrence
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EventId = event3.EventId,
                    Frequency = "Monthly",
                    Interval = 1,
                    EndDate = DateTime.Parse($"12/30/2018 10:15:00 PM"),
                    DayOfMonth = 21
                };

                var event4 = new Models.CalendarEvent
                {
                    EventId = Guid.NewGuid().ToString("N"),
                    DocumentId = Guid.NewGuid().ToString("N"),
                    Title = "My Yearly Event",
                    Posted = DateTime.Now,
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",  //ADMIN
                    Style = "event-info",
                    Description = @"<p><img class='img-responsive' src='/img/SeedImages/cats1_200_200.jpg' /></p>
                                    <h2>How to Be a Godly Man</h2>
                                    <p>Men's Breakfast is our monthly event where all men of the church are invited
                                         to grow in the knowledge and love of Jesus Christ.</p>
                                    <p>We encourage Lifers to invite other men - unconnected, no-believers, friends, neighbors, co-workers - to join them as an outreach
                                    to the community around us</p>",
                    EventStart = DateTime.Parse($"1/27/2018 2:00:00 PM"),
                    EventEnd = DateTime.Parse($"12/30/2020 10:15:00 PM"),
                    Phone = "919-277-2222",
                    BackgroundColor = "Blue",
                    IsRecurrent = true,
                    ShowOrganizerName = true,
                    ShowPhoneNumber = true
                };

                var eventRecurrence4 = new Models.CalendarEventRecurrence
                {
                    Id = Guid.NewGuid().ToString("N"),
                    EventId = event4.EventId,
                    Frequency = "Yearly",
                    Interval = 1,
                    Count = 2,
                    //EndDate = DateTime.Parse($"12/30/2020 10:15:00 PM"),
                    DayOfMonth = 27,
                    Months = "1"
                };


                var group1 = new Models.CalendarEventGroup()
                {
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",
                    Title = "2017 Softball Events",
                    EventGroupId = Guid.NewGuid().ToString("N")

                };

                var group2 = new Models.CalendarEventGroup()
                {
                    SiteId = "b75cbabb-839f-4b20-ba6e-b74241080201",
                    UserId = "AFCF7980-4BA7-4DD2-879D-599D058F7E73",
                    Title = "2017 Soccer Events",
                    EventGroupId = Guid.NewGuid().ToString("N")

                };

                dbContext.Add(group1);
                dbContext.Add(group2);

                dbContext.AddRange(
                    new Models.CalendarEventGroupEvent() { Event = event1, EventGroupId = group1.EventGroupId },
                    new Models.CalendarEventGroupEvent() { Event = event1, EventGroupId = group2.EventGroupId },
                    new Models.CalendarEventGroupEvent() { Event = event2, EventGroupId = group1.EventGroupId },
                    new Models.CalendarEventGroupEvent() { Event = event3, EventGroupId = group2.EventGroupId }
                    );

                dbContext.Add(eventRecurrence);
                dbContext.Add(eventRecurrence2);
                dbContext.Add(eventRecurrence3);
                dbContext.Add(event4);
                dbContext.Add(eventRecurrence4);

                dbContext.SaveChanges();
                
            }
        }
    }
}
