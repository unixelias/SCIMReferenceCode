// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM.WebHostSample.Provider
{
    using Microsoft.SCIM;
    using Microsoft.SCIM.Repository.ScimResources;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class InMemoryGroupProvider : ProviderBase
    {
        //private readonly InMemoryStorage storage;
        private readonly IGroupRepository repository;

        public InMemoryGroupProvider(IGroupRepository repository)
        {
            //this.storage = InMemoryStorage.Instance;
            this.repository = repository;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (repository.CheckIfGroupExistsAsync(group.Identifier, group.DisplayName).Result)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            //Update Metadata
            DateTime created = DateTime.UtcNow;
            group.Metadata.Created = created;
            group.Metadata.LastModified = created;

            string resourceIdentifier = Guid.NewGuid().ToString();
            resource.Identifier = resourceIdentifier;

            repository.CreateAsync(group).Wait();

            return Task.FromResult(resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string identifier = resourceIdentifier.Identifier;

            repository.DeleteGroupByIdAsync(identifier).Wait();

            return Task.CompletedTask;
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
            }

            IEnumerable<Resource> results;
            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();

            var predicate = PredicateBuilder.False<Core2Group>();
            Expression<Func<Core2Group, bool>> predicateAnd;
            predicateAnd = PredicateBuilder.True<Core2Group>();

            if (queryFilter == null)
            {
                results = repository.ListAllAsync().Result.Select((Core2Group user) => user as Resource);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
                {
                    throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                }

                if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
                {
                    throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidParameters);
                }

                if (queryFilter.FilterOperator != ComparisonOperator.Equals)
                {
                    throw new NotSupportedException(string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterOperatorNotSupportedTemplate, queryFilter.FilterOperator));
                }

                if (queryFilter.AttributePath.Equals(AttributeNames.DisplayName))
                {
                    string displayName = queryFilter.ComparisonValue;
                    predicateAnd = predicateAnd.And(p => string.Equals(p.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    throw new NotSupportedException(string.Format(SystemForCrossDomainIdentityManagementServiceResources.ExceptionFilterAttributePathNotSupportedTemplate, queryFilter.AttributePath));
                }
            }

            predicate = predicate.Or(predicateAnd);
            results = repository.ListAllAsync().Result.Where(predicate.Compile());

            return Task.FromResult(results.ToArray());
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group exisitingGroup = repository.GetGroupByIdAsync(group.Identifier).Result;

            if (exisitingGroup == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // Update metadata
            group.Metadata.Created = exisitingGroup.Metadata.Created;
            group.Metadata.LastModified = DateTime.UtcNow;

            repository.UpdateGroupByIdAsync(group.Identifier, group).Wait();

            Resource result = group as Resource;
            return Task.FromResult(result);
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Resource result;
            string identifier = parameters.ResourceIdentifier.Identifier;
            result = repository.GetGroupByIdAsync(identifier).Result as Resource;

            if (result != null)
            {
                return Task.FromResult(result);
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(SystemForCrossDomainIdentityManagementServiceResources.ExceptionInvalidOperation);
            }

            PatchRequest2 patchRequest =
                patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            Core2Group group = repository.GetGroupByIdAsync(patch.ResourceIdentifier.Identifier).Result;

            if (group != null)
            {
                group.Apply(patchRequest);
                // Update metadata
                group.Metadata.LastModified = DateTime.UtcNow;
                repository.UpdateGroupByIdAsync(patch.ResourceIdentifier.Identifier, group).Wait();
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Task.CompletedTask;
        }
    }
}