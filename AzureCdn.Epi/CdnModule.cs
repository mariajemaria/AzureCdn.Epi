using System;
using System.Configuration;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace AzureCdn.Epi
{
    [ModuleDependency(typeof(InitializationModule))]
    public class CdnModule : IInitializableModule
    {
        private static readonly Lazy<string> CdnUrl = new Lazy<string>(() => ConfigurationManager.AppSettings["episerver:CdnExternalMediaUrl"]);
        private static readonly string[] MediaPaths = { "contentassets", RouteCollectionExtensions.SiteAssetStaticSegment, RouteCollectionExtensions.GlobalAssetStaticSegment };


        public void Initialize(InitializationEngine context)
        {
            if (String.IsNullOrEmpty(CdnUrl.Value)) { return; }

            ContentRoute.CreatedVirtualPath += ContentRoute_CreatedVirtualPath;
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        private static void ContentRoute_CreatedVirtualPath(object sender, UrlBuilderEventArgs e)
        {
            if (e.RequestContext.GetContextMode() != ContextMode.Default || !MediaPaths.Any(p => e.UrlBuilder.Path.StartsWith(p))) { return; }

            var contentLink = e.RouteValues[RoutingConstants.NodeKey] as ContentReference;
            IContent content;

            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();

            if (!loader.TryGet(contentLink, out content)) { return; }

            var imageFile = content as ImageFile;

            if (imageFile == null || imageFile.Protected) { return; }

            var blobLocalPath = imageFile.BinaryData.ID.LocalPath;

            var fullCdnUri = string.Format("{0}/{1}",
                CdnUrl.Value.TrimEnd('/'),
                blobLocalPath.TrimStart('/'));

            e.UrlBuilder.Uri = new Uri(fullCdnUri);
        }
    }
}
