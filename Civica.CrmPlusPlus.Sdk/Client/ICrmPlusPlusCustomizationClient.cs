using System;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.DefaultEntities;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public interface ICrmPlusPlusCustomizationClient
    {
        Solution Solution { get; }
        Publisher Publisher { get; }

        bool CanCreateOneToManyRelationship<TOne, TMany>()
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new();

        void CreateOneToManyRelationship<TOne, TMany>(Expression<Func<TMany, EntityReference<TOne>>> lookupExpr, EntityAttributes.Metadata.AttributeRequiredLevel lookupRequiredLevel, string relationshipPrefix = "new")
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new();

        void CreateEntity<T>() where T : CrmPlusPlusEntity, new();


        void CreateProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new();

        void CreateAllProperties<T>() where T : CrmPlusPlusEntity, new();

        void Delete<T>() where T : CrmPlusPlusEntity, new();
    }
}
