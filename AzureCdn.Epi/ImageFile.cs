using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace AzureCdn.Epi
{
    [ContentType(GUID = "0b989137-acd4-4fb4-ae0a-caef85eea512")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,gif,bmp,png")]
    public class ImageFile : ImageData
    {
        public virtual bool Protected { get; set; }
    }
}
