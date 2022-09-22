// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM
{
    using System;
#if NET
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route(ServiceConstants.RouteUsers)]
    [Authorize]
    [ApiController]
    public sealed class UsersController : ControllerTemplate<Core2EnterpriseUser>
    {
        public UsersController(IProvider provider, IMonitor monitor)
#else
    public abstract class UsersControllerBase : ControllerTemplate<Core2EnterpriseUser>
    {
        public UsersControllerBase(IProvider provider, IMonitor monitor)
#endif
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }
    }
}
