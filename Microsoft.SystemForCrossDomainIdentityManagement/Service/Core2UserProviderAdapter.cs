// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM
{
    public class Core2UserProviderAdapter : ProviderAdapterTemplate<Core2User>
    {
        public Core2UserProviderAdapter(IProvider provider)
            : base(provider)
        {
        }

        public override string SchemaIdentifier
        {
            get
            {
                return SchemaIdentifiers.Core2User;
            }
        }
    }
}
