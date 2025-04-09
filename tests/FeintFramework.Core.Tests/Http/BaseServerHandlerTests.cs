using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FeintFramework.Core.Http;
using FeintFramework.Core.Middleware;
using System;

namespace FeintFramework.Core.Tests.Http
{
    [TestClass]
    public class BaseServerHandlerTests
    {
        private class TestServerHandler : BaseServerHandler
        {
            public TestServerHandler(RequestHandler routerHandler) : base(routerHandler) { }

            public override object? HandleRequest(object request)
            {
                return handler(request);
            }
        }

        [TestMethod]
        public void TestHandlerChainExecution()
        {
            // Arrange
            var mockRouterHandler = new Mock<RequestHandler>();
            mockRouterHandler.Setup(r => r(It.IsAny<object>())).Returns("Router Response");

            var testHandler = new TestServerHandler(mockRouterHandler.Object);

            // Act
            var response = testHandler.HandleRequest("Test Request");

            // Assert
            Assert.AreEqual("Router Response", response);
            mockRouterHandler.Verify(r => r(It.IsAny<object>()), Times.Once);
        }
    }
}