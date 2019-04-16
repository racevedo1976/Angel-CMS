using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Models
{
    // NOTE: Not implementing IContent (yet) because the media that it displays are all content.
    public class Slide : IDocument
    {
        // EF is trying to re-add the widget, so I'm not bothering with it...it was just for foreign keys anyway
        // Is not extranous, since Docs are realted to Folders by FolderItem (many to many)
        // ALWAYS START WITH A FOLDER, SO THIS ISN"T NECESSARY (GetWidgetByFolder, GetSlidesByFolder)
        public string WidgetId { get; set; }
        //public SlideShowWidget Widget { get; set; }

        public string DocumentId { get; set; }
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        public int Delay { get; set; }
        public SlideState State { get; set; }
        public DateTime? VisibleFrom { get; set; }
        public DateTime? VisibleTo { get; set; }
        public string ThumbnailUrl { get; set; }

        #region Background
        public string ImageUrl { get; set; }    // TODO: A collection instead (for parallax)?
        public bool UseVideoBackground { get; set; }
        public string VideoUrl { get; set; }
        public string VideoSource { get; set; }
        public bool EnableVideoSound { get; set; }
        public string Color { get; set; }

        public string ImageSourceSize { get; set; }
        public int Position { get; set; }
        public Tiling Tiling { get; set; }
        public Fit BackgroundFit { get; set; }
        public Parallax Parallax { get; set; }
        public KenBurnsEffect KenBurnsEffect { get; set; }
        #endregion // Background
        #region Animation
        public Transition Transition { get; set; }
        public int SlotBoxAmount { get; set; }
        public int SlotRotation { get; set; }
        public Direction Direction { get; set; }
        public int Duration { get; set; }
        #endregion // Animation
        #region Link
        public bool IsLinkEnabled { get; set; }
        public string SlideLinkUrl { get; set; }
        public LinkTarget LinkTarget { get; set; }
        #endregion // Link

        public ICollection<SlideLayer> Layers { get; set; }
        // Parallax etc.
    }
}
