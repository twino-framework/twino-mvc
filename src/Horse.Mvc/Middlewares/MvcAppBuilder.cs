﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Horse.Mvc.Routing;
using Horse.Protocols.Http;

namespace Horse.Mvc.Middlewares
{
    /// <summary>
    /// MVC Application builder object
    /// </summary>
    internal class MvcAppBuilder : IMvcAppBuilder
    {
        /// <summary>
        /// Descriptors for middlewares that are called for each request
        /// </summary>
        internal List<MiddlewareDescriptor> Descriptors { get; } = new List<MiddlewareDescriptor>();

        public HorseMvc Mvc { get; }

        public MvcAppBuilder(HorseMvc mvc)
        {
            Mvc = mvc;
        }

        /// <summary>
        /// Gets MSDI service provider for horse mvc
        /// </summary>
        public IServiceProvider GetProvider()
        {
            return Mvc.ServiceProvider;
        }

        #region Middleware

        /// <summary>
        /// Uses singleton middleware objects.
        /// Same object is used for all requests
        /// </summary>
        public void UseMiddleware(IMiddleware middleware)
        {
            MiddlewareDescriptor desc = new MiddlewareDescriptor
                                        {
                                            Instance = middleware,
                                            MiddlewareType = middleware.GetType(),
                                            ConstructorParameters = null
                                        };
            Descriptors.Add(desc);
        }

        /// <summary>
        /// Uses middleware, creates new instance for per request.
        /// </summary>
        public void UseMiddleware<TMiddleware>() where TMiddleware : IMiddleware
        {
            ConstructorInfo ctor = typeof(TMiddleware).GetConstructors().FirstOrDefault();
            if (ctor == null)
                throw new ArgumentException(typeof(TMiddleware) + " has no acceptable constructor");

            MiddlewareDescriptor desc = new MiddlewareDescriptor
                                        {
                                            Instance = null,
                                            MiddlewareType = typeof(TMiddleware),
                                            ConstructorParameters = ctor.GetParameters().Select(x => x.ParameterType).ToArray()
                                        };
            Descriptors.Add(desc);
        }

        #endregion

        #region Use Files

        /// <summary>
        /// Uses files in physicalFolder under urlPath.
        /// Subfolders are included.
        /// </summary>
        public void UseFiles(string urlPath, string physicalPath)
        {
            Mvc.FileRoutes.Add(new FileRoute(urlPath, new[] {physicalPath}));
        }

        /// <summary>
        /// Uses files in multiple physical folders under urlPath.
        /// Files are searched with index order. If file couldn't found, it's searched in next physical path.
        /// Subfolders are included.
        /// </summary>
        public void UseFiles(string urlPath, string[] physicalPaths)
        {
            Mvc.FileRoutes.Add(new FileRoute(urlPath, physicalPaths));
        }

        /// <summary>
        /// Uses files in physicalFolder under urlPath.
        /// Validation passes If validation function returns 200 OK response. Otherwise status response result written as response. 
        /// Subfolders are included.
        /// </summary>
        public void UseFiles(string urlPath, string physicalPath, Func<HttpRequest, HttpStatusCode> validation)
        {
            Mvc.FileRoutes.Add(new FileRoute(urlPath, new[] {physicalPath}, validation));
        }

        /// <summary>
        /// Uses files in multiple physical folders under urlPath.
        /// Files are searched with index order. If file couldn't found, it's searched in next physical path.
        /// Validation passes If validation function returns 200 OK response. Otherwise status response result written as response. 
        /// Subfolders are included.
        /// </summary>
        public void UseFiles(string urlPath, string[] physicalPaths, Func<HttpRequest, HttpStatusCode> validation)
        {
            Mvc.FileRoutes.Add(new FileRoute(urlPath, physicalPaths, validation));
        }

        #endregion

        #region Action Routes

        /// <summary>
        /// Uses action route
        /// </summary>
        public void UseActionRoute(string urlPath, Func<HttpRequest, Task<IActionResult>> action)
        {
            Mvc.ActionRoutes.Add(new ActionRoute(urlPath, action));
        }

        /// <summary>
        /// Uses action route
        /// </summary>
        public void UseActionRoute(string urlPath, Func<HttpRequest, IActionResult> action)
        {
            Mvc.ActionRoutes.Add(new ActionRoute(urlPath, action));
        }

        #endregion
    }
}