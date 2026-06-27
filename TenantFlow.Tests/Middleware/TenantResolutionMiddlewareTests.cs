using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TenantFlow.Api.Middleware;
using TenantFlow.Domain.Entities;
using TenantFlow.Tests.Fakes;
using Task = System.Threading.Tasks.Task;

namespace TenantFlow.Tests.Middleware
{
    public class TenantResolutionMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WithValidTenantIdClaim_CallsSetTenantId()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim("TenantId", tenantId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext();
            context.User = principal;

            var fakeSetter = new FakeTenantContextSetter();
            RequestDelegate next = (ctx) => Task.CompletedTask;
            var middleware = new TenantResolutionMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, fakeSetter);

            // Assert
            Assert.True(fakeSetter.WasCalled);
            Assert.Equal(tenantId, fakeSetter.CalledWithValue);
        }
        [Fact]
        public async Task InvokeAsync_WithNoTenantIdClaim_DoesNotCallSetTenantId()
        {
            // Arrange
            var identity = new ClaimsIdentity { };
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext();
            context.User = principal;

            var fakeSetter = new FakeTenantContextSetter();
            RequestDelegate next = (ctx) => Task.CompletedTask;
            var middleware = new TenantResolutionMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, fakeSetter);

            // Assert
            Assert.False(fakeSetter.WasCalled);

        }

        [Fact]
        public async Task InvokeAsync_WithInvalidTenantIdClaim_DoesNotCallSetTenantId()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("TenantId", "banana") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext();
            context.User = principal;

            var fakeSetter = new FakeTenantContextSetter();
            RequestDelegate next = (ctx) => Task.CompletedTask;
            var middleware = new TenantResolutionMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, fakeSetter);

            // Assert
            Assert.False(fakeSetter.WasCalled);
        }
    }
}
