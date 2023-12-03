using Azure.Core;
using Delivery_API.Services;
using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Delivery_API.Middleware;
namespace Delivery_API.Middleware
{
    public class TokenMiddleware : IMiddleware
    {
        private readonly IUserService _userService;

        public TokenMiddleware(IUserService userService)
        {
            _userService = userService;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (HasAuthorizeAttribute(context))
            {
                var token = string.Empty;

                if (context.Request.Headers.TryGetValue("authorization", out var authHeaderValue))
                {
                    token = authHeaderValue.FirstOrDefault()?.Split(" ").Last() ?? string.Empty;
                }
                //var token = context.Request.Headers["authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!await _userService.IsActiveToken(token))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return;
                }
            }

            await next(context);
        }
        private bool HasAuthorizeAttribute(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
            return authorizeAttribute != null;
        }
    }
}