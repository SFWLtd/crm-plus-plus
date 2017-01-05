using System;
using System.Linq.Expressions;

namespace Civica.CrmPlusPlus.Sdk
{
    public interface ICrmPlusPlusCustomizationClient
    {
        void CreateEntityWithoutProperties<T>() where T : CrmPlusPlusEntity, new();


        void CreateProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new();

        void CreateAllProperties<T>() where T : CrmPlusPlusEntity, new();

        void Delete<T>() where T : CrmPlusPlusEntity, new();
    }
}
