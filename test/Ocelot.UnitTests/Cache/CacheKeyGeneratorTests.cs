using Microsoft.AspNetCore.Http;
using Ocelot.Cache;
using Ocelot.Configuration;
using Ocelot.Configuration.Builder;
using Ocelot.Middleware;
using Shouldly;
using System.Net.Http;
using TestStack.BDDfy;
using Xunit;

namespace Ocelot.UnitTests.Cache
{
    public class CacheKeyGeneratorTests
    {
        private readonly ICacheKeyGenerator _cacheKeyGenerator;
        private readonly DownstreamContext _downstreamContext;
        private readonly DownstreamReRoute _downstreamRoute;

        public CacheKeyGeneratorTests()
        {
            _cacheKeyGenerator = new CacheKeyGenerator();
            _cacheKeyGenerator = new CacheKeyGenerator();
            _downstreamContext = new DownstreamContext(new DefaultHttpContext())
            {
                DownstreamRequest = new Ocelot.Request.Middleware.DownstreamRequest(new HttpRequestMessage(HttpMethod.Get, "https://some.url/blah?abcd=123"))
            };
            _downstreamRoute = new DownstreamReRouteBuilder().WithKey("key1").Build();
        }

        //[Fact]
        //public void should_generate_cache_key_from_context()
        //{
        //    this.Given(x => x.GivenCacheKeyFromContext(_downstreamContext, _downstreamRoute))
        //        .BDDfy();
        //}

        //private void GivenCacheKeyFromContext(DownstreamContext context, DownstreamReRoute downstreamRoute)
        //{
        //    string generatedCacheKey = _cacheKeyGenerator.GenerateRequestCacheKey(context, downstreamRoute);
        //    string cachekey = MD5Helper.GenerateMd5("GET-https://some.url/blah?abcd=123");
        //    generatedCacheKey.ShouldBe(cachekey);
        //}
    }
}
