namespace Backsplashed.Core.Abstractions
{
    using System;

    public class HistoricalPhotoInfo : PhotoInfo
    {
        public DateTime Timestamp { get; set; }

        public HistoricalPhotoInfo()
        {
        }

        public HistoricalPhotoInfo(PhotoInfo info)
        {
            Id = info.Id;
            Term = info.Term;
            Author = info.Author;
            LinkUrl = info.LinkUrl;
            RawUrl = info.RawUrl;
            ThumbUrl = info.ThumbUrl;
        }
    }
}