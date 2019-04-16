using System;

namespace Angelo.Connect.SlideShow.Models
{
    public class KenBurnsEffect : IEffect<Slide>
    {
        public string SlideId { get; set; }
        public Slide Slide { get; set; }

        #region Properties
        public string Id { get; set; }
        public int ScaleFrom { get; set; }
        public int ScaleTo { get; set; }
        public int HorizontalOffsetFrom { get; set; }
        public int HorizontalOffsetTo { get; set; }
        public int VerticalOffsetFrom { get; set; }
        public int VerticalOffsetTo { get; set; }
        public int RotationFrom { get; set; }
        public int RotationTo { get; set; }
        public int Duration { get; set; }
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