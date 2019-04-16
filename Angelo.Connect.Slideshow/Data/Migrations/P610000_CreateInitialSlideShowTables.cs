using Angelo.Common.Migrations;
using Angelo.Connect.SlideShow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Slideshow.Data.Migrations
{
    public class P610000_CreateInitialSlideShowTables : IAppMigration
    {
        private SlideShowDbContext _dbContext;

        public string Id { get; } = "P610000";

        public string Migration { get; } = "Create Initial SlideShow Tables";

        public P610000_CreateInitialSlideShowTables(SlideShowDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_dbContext.Database.TableExists("SlideShowWidget", "plugin"))
                return MigrationResult.Skipped();
            
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[SlideShowWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](50) NOT NULL,
	                    
                        [Title] [nvarchar](500) NULL,
                        [Description] [nvarchar](MAX) NULL,     
                        [BackgroundColor] [nvarchar] (100) NULL,
                        [Transition] [INT] NULL,
                        [Duration] [INT] NOT NULL,
                        [Height] [nvarchar] (10) NULL,
                        CONSTRAINT [PK_SlideShowWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[SlideShowSlide](
                        [WidgetId] [nvarchar](50) NOT NULL,
	                    [DocumentId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        [Description] [nvarchar](MAX) NULL,
                        [Delay] [INT]  NULL,
                        [State] [INT]  NULL,

                        [VisibleFrom] [datetime2] NULL,
                        [VisibleTo] [datetime2] NULL,
                        [ThumbnailUrl] [nvarchar] (2048) NULL,
                        [ImageUrl] [nvarchar] (2048) NULL,
                        [Color] [nvarchar] (100) NULL,
                        [ImageSourceSize] [nvarchar] (100) NULL,
                        [Position] [INT]  NULL,
                        [Tiling] [INT]  NULL,
                        [BackgroundFit] [INT] NOT NULL,

                        [ParallaxId] [nvarchar] (50) NULL,
                        [KenBurnsEffectId] [nvarchar] (50) NULL,


                        [Transition] [INT]  NULL,
                        [SlotBoxAmount] [INT]  NULL,
                        [SlotRotation] [INT]  NULL,
                        [Direction] [INT]  NULL,
                        [Duration] [INT]  NULL,
                        [IsLinkEnabled] [bit]  NULL,
                        [SlideLinkUrl] [nvarchar] (2048) NULL,
                        [LinkTarget] [INT]  NULL,

                        CONSTRAINT [PK_SlideShowSlide] PRIMARY KEY CLUSTERED ([DocumentId] ASC)
                    )
                ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[SlideShowLayer](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SlideId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](50) NOT NULL,
                        [SourceUrl] [nvarchar](50) NULL,
                        [Target] [nvarchar] (50) NULL,
                        [LayerType] [nvarchar] (50) NULL,
                        [X] [INT] NULL,
                        [Y] [INT] NULL,
                        [Height] [INT] NULL,
                        [Width] [INT] NULL,
                        [Transition] [nvarchar] (50) NULL,
                        [Position] [nvarchar] (50) NULL,
                        [Delay] [nvarchar] (50) NULL,
                        [FontFamily] [nvarchar](100) NULL,
                        [FontSize] [INT] NULL,
                        [Color] [nvarchar](100) NULL,
                        [HorizontalAlignment] [INT] NULL,
                        [VerticalAlignment] [INT] NULL,

                        [FadeInTransition] [INT] NULL,
                        [FadeInDirection] [INT] NULL,
                        [FadeInDuration] [INT] NULL,
                        [FadeInDelay] [INT] NULL,

                        [FadeOutTransition] [INT] NULL,
                        [FadeOutDirection] [INT] NULL,
                        [FadeOutDuration] [INT] NULL,
                        [FadeOutDelay] [INT] NULL,

                        CONSTRAINT [PK_SlideShowLayer] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [FK_SlideShowLayer_SlideShowSlide] FOREIGN KEY (SlideId) REFERENCES [plugin].[SlideShowSlide](DocumentId)
                    )
                ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[SlideShowParallax](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SlideId] [nvarchar](50) NOT NULL,
                        [IsEnabled] [BIT] NOT NULL,

                        [ParallaxEvent] [INT] NOT NULL,
                        [ParallaxOrigin] [INT] NOT NULL,
                        [AnimationSpeed] [INT] NOT NULL,
                        [Level] [INT] NOT NULL,
                        [IsMobileDisabled] [BIT] NOT NULL,

                        [LevelDepth1] [INT] NULL,
                        [LevelDepth2] [INT] NULL,
                        [LevelDepth3] [INT] NULL,
                        [LevelDepth4] [INT] NULL,
                        [LevelDepth5] [INT] NULL,
                        [LevelDepth6] [INT] NULL,
                        [LevelDepth7] [INT] NULL,
                        [LevelDepth8] [INT] NULL,
                        [LevelDepth9] [INT] NULL,
                        [LevelDepth10] [INT] NULL,

                        CONSTRAINT [PK_Parallax] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [FK_Parallax_SlideShowSlide] FOREIGN KEY (SlideId) REFERENCES [plugin].[SlideShowSlide](DocumentId)
                    )
                ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[SlideShowKenBurnsEffect](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SlideId] [nvarchar](50) NOT NULL,
                        [IsEnabled] [BIT] NOT NULL,

                        [ScaleFrom] [INT] NOT NULL,
                        [ScaleTo] [INT] NOT NULL,
                        [HorizontalOffsetFrom] [INT] NOT NULL,
                        [HorizontalOffsetTo] [INT] NOT NULL,
                        [VerticalOffsetFrom] [INT] NOT NULL,
                        [VerticalOffsetTo] [INT] NOT NULL,
                        [RotationFrom] [INT] NOT NULL,
                        [RotationTo] [INT] NOT NULL,
                        [Duration] [INT] NOT NULL,

                        CONSTRAINT [PK_KenBurnsEffect] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [FK_KenBurnsEffect_SlideShowSlide] FOREIGN KEY (SlideId) REFERENCES [plugin].[SlideShowSlide](DocumentId)
                    )
                ");


            return MigrationResult.Success();
        }
    }
}
