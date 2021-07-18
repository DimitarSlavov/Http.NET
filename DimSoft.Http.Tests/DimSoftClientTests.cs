using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DimSoft.Http.Tests
{
    [TestClass]
    public class DimSoftClientTests
    {
        [TestMethod]
        public async Task GetAsyncShouldSucceed()
        {
            IEnumerable<string> content = new string[] { "test" };
            var uri = new Uri("http://localhost:1111");

            //Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent(JsonSerializer.Serialize(content)),
               })
               .Verifiable();

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handlerMock.Object));

            //Act
            var client = new DimSoftClient(factoryMock.Object);
            var result = await client.GetAsync<IEnumerable<string>>(uri.AbsoluteUri);

            //Assert
            Assert.AreEqual(content.First(), result.Content.First());
        }
    }
}
