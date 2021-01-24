using System;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging.Abstractions;

namespace WatchFunctionsTests
{
    public class WatchFunctionUnitTests
    {
        [Fact]
        public void TestWatchFunctionSuccess()
        {
            // This statement creates a mock HTTP context and an HTTP request. 
            // The request includes a query string that includes the model parameter, which is set to abc.
            var queryStringValue = "abc";
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection
                (
                    new System.Collections.Generic.Dictionary<string, StringValues>()
                    {
                        { "model", queryStringValue }
                    }
                )
            };

            // This statement creates a dummy logger.
            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");

            // These statements invoke the WatchInfo function, passing in the dummy request and logger as parameters.
            var response = WatchPortalFunction.WatchInfo.Run(request, logger);
            response.Wait();

            // Check that the response is an "OK" response
            Assert.IsAssignableFrom<OkObjectResult>(response.Result);

            // Check that the contents of the response are the expected contents
            var result = (OkObjectResult)response.Result;
            dynamic watchinfo = new { Manufacturer = "abc", CaseType = "Solid", Bezel = "Titanium", Dial = "Roman", CaseFinish = "Silver", Jewels = 15 };
            string watchInfo = $"Watch Details: {watchinfo.Manufacturer}, {watchinfo.CaseType}, {watchinfo.Bezel}, {watchinfo.Dial}, {watchinfo.CaseFinish}, {watchinfo.Jewels}";
            Assert.Equal(watchInfo, result.Value);
        }

        [Fact]
        public void TestWatchFunctionFailureNoQueryString()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext());
            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");

            var response = WatchPortalFunction.WatchInfo.Run(request, logger);
            response.Wait();

            // Check that the response is an "Bad" response
            Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);

            // Check that the contents of the response are the expected contents
            var result = (BadRequestObjectResult)response.Result;
            Assert.Equal("Please provide a watch model in the query string", result.Value);
        }

        [Fact]
        public void TestWatchFunctionFailureNoModel()
        {
            var queryStringValue = "abc";
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Query = new QueryCollection
                (
                    new System.Collections.Generic.Dictionary<string, StringValues>()
                    {
                        { "not-model", queryStringValue }
                    }
                )
            };

            var logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");

            var response = WatchPortalFunction.WatchInfo.Run(request, logger);
            response.Wait();

            // Check that the response is an "Bad" response
            Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);

            // Check that the contents of the response are the expected contents
            var result = (BadRequestObjectResult)response.Result;
            Assert.Equal("Please provide a watch model in the query string", result.Value);
        }
    }
}
