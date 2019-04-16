using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace Angelo.Connect.SlideShow.Models
{
    public class Parallax : IEffect<Slide>
    {
        public string SlideId { get; set; }
        public Slide Slide { get; set; }

        #region Properties
        public string Id { get; set; }
        public ParallaxEvent ParallaxEvent { get; set; }
        public ParallaxOrigin Origin { get; set; }
        public int AnimationSpeed { get; set; }
        public int Level { get; set; }
        public bool IsMobileDisabled { get; set; }

        public int? LevelDepth1 { get; set; }
        public int? LevelDepth2 { get; set; }
        public int? LevelDepth3 { get; set; }
        public int? LevelDepth4 { get; set; }
        public int? LevelDepth5 { get; set; }
        public int? LevelDepth6 { get; set; }
        public int? LevelDepth7 { get; set; }
        public int? LevelDepth8 { get; set; }
        public int? LevelDepth9 { get; set; }
        public int? LevelDepth10 { get; set; }
        #endregion // Properties
        #region IEffect implementation
        public bool IsEnabled { get; set; }
        public void Invoke(Slide target)
        {
            throw new NotImplementedException();
        }
        #endregion // IEffect implementation
    }
}