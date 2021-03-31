namespace Ocelot.Cache
{
    using Ocelot.Configuration;
    using Ocelot.DownstreamRouteFinder;
    using Ocelot.Request.Middleware;

    public interface ICacheKeyGenerator
    {
        string GenerateRequestCacheKey(DownstreamRequest downstreamRequest, DownstreamReRoute downstreamRoute);
    }
}
