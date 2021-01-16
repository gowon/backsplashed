namespace Backsplashed.Interop.Extensions
{
    using System.Collections.Generic;
    using Core.Abstractions;
    using OASplash.Client.Models;

    public static class PhotoInfoExtensions
    {
        public static CachedPhotoInfo ToPhotoInfo(this Photo photo, string term)
        {
            return new()
            {
                Id = photo.Id,
                Term = term,
                Author = photo.User.Name,
                LinkUrl = photo.Links.Html,
                RawUrl = photo.Urls.Raw,
                ThumbUrl = photo.Urls.Thumb
            };
        }

        public static T PopAt<T>(this IList<T> list, int index)
        {
            var r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
}