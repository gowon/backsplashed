namespace Backsplashed.Core.Abstractions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PhotoInfo
    {
        [Key]
        public string Id { get; set; }

        public string Term { get; set; }
        public string Author { get; set; }
        public string LinkUrl { get; set; }
        public string RawUrl { get; set; }
        public string ThumbUrl { get; set; }
    }
}