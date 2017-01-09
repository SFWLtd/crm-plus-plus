using System;
using System.Linq.Expressions;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public interface ICrmPlusPlusCustomizationClient
    {
        bool CanCreateOneToManyRelationship<TOne, TMany>()
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new();

        void CreateOneToManyRelationship<TOne, TMany>(Expression<Func<TMany, EntityReference<TOne>>> lookupExpr, EntityAttributes.Metadata.AttributeRequiredLevel lookupRequiredLevel, string relationshipPrefix = "new")
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new();

        void CreateEntityWithoutProperties<T>() where T : CrmPlusPlusEntity, new();


        void CreateProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new();

        void CreateAllProperties<T>() where T : CrmPlusPlusEntity, new();

        void Delete<T>() where T : CrmPlusPlusEntity, new();
    }
}
