// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM
{
    using System;

#if NET
    public sealed class RootController : ControllerTemplate<Resource>
    {
        public RootController(IProvider provider, IMonitor monitor)
#else
    public abstract class RootControllerBase : ControllerTemplate<Resource>
    {
        public RootControllerBase(IProvider provider, IMonitor monitor)
#endif
            : base(provider, monitor)
        {
        }

        protected override IProviderAdapter<Resource> AdaptProvider(IProvider provider)
        {
            if (null == provider)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Resource> result = new RootProviderAdapter(provider);
            return result;
        }
    }
}
