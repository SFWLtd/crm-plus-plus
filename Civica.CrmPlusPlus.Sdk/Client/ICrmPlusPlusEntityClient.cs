using System;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.Client.Association;
using Civica.CrmPlusPlus.Sdk.Client.Retrieve;
using Civica.CrmPlusPlus.Sdk.Querying;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public interface ICrmPlusPlusEntityClient
    {
        void Associate(string relationshipName, Associate association);

        void Disassociate(string relationshipName, Associate association);

        Guid Create<T>(T entity) where T : CrmPlusPlusEntity, new();

        void Update<T>(T entity) where T : CrmPlusPlusEntity, new();

        T Retrieve<T>(Retrieval<T> retrieval) where T : CrmPlusPlusEntity, new();

        void Delete<T>(T entity) where T : CrmPlusPlusEntity, new();

        IEnumerable<T> RetrieveMultiple<T>(Query<T> query) where T : CrmPlusPlusEntity, new();
    }
}
