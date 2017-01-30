using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Microsoft.Xrm.Sdk;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class EntityExtensions
    {
        internal static CrmPlusPlusEntity ToCrmPlusPlusEntity(this Entity entity, Type crmPlusPlusEntityType, string alias = "")
        {
            var crmPlusPlusEntity = (CrmPlusPlusEntity)Activator.CreateInstance(crmPlusPlusEntityType);
            crmPlusPlusEntity.Id = entity.Id;

            if (!string.IsNullOrEmpty(alias) && !alias.EndsWith("."))
            {
                alias = alias + ".";
            }

            if (alias == string.Empty)
            {
                crmPlusPlusEntity.CreatedOn = entity.Contains("createdon") ? DateTime.Parse(entity["createdon"].ToString()) : DateTime.MinValue;
                crmPlusPlusEntity.ModifiedOn = entity.Contains("modifiedon") ? DateTime.Parse(entity["modifiedon"].ToString()) : DateTime.MinValue;
            }
            else
            {
                crmPlusPlusEntity.Id = entity.Contains(alias + "id") ? (Guid)((AliasedValue)entity[alias + "id"]).Value : crmPlusPlusEntity.Id;

                crmPlusPlusEntity.CreatedOn = entity.Contains(alias + "createdon") ? DateTime.Parse(((AliasedValue)entity[alias + "createdon"]).Value.ToString()) : DateTime.MinValue;
                crmPlusPlusEntity.ModifiedOn = entity.Contains(alias + "modifiedon") ? DateTime.Parse(((AliasedValue)entity[alias + "modifiedon"]).Value.ToString()) : DateTime.MinValue;
            }

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(crmPlusPlusEntityType))
            {
                if (property.Name == "ModifedOn" || property.Name == "CreatedOn")
                {
                    continue;
                }

                var attributes = property.Attributes.AsEnumerable();

                var propertyNameAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyNameAttribute));
                var propertyInfoAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyInfoAttribute));
                var typeInfoAttr = attributes
                    .SingleOrDefault(attr => (attr.GetType() == typeof(BooleanAttribute) && property.PropertyType == typeof(bool))
                        || (attr.GetType() == typeof(DateTimeAttribute) && property.PropertyType == typeof(DateTime))
                        || (attr.GetType() == typeof(DecimalAttribute) && property.PropertyType == typeof(decimal))
                        || (attr.GetType() == typeof(DoubleAttribute) && property.PropertyType == typeof(double))
                        || (attr.GetType() == typeof(IntegerAttribute) && property.PropertyType == typeof(int))
                        || (attr.GetType() == typeof(StringAttribute) && property.PropertyType == typeof(string))
                        || (attr.GetType() == typeof(OptionSetAttribute) && property.PropertyType.IsEnum)
                        || (attr.GetType() == typeof(LookupAttribute) && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)));

                if (propertyNameAttr != null && propertyInfoAttr != null && typeInfoAttr != null)
                {
                    var propertyName = ((PropertyNameAttribute)propertyNameAttr).PropertyName;

                    if (entity.Contains(alias + propertyName))
                    {
                        object value = null;

                        if (alias == string.Empty)
                        {
                            value = entity[propertyName];
                        }
                        else
                        {
                            value = ((AliasedValue)entity[alias + propertyName]).Value;
                        }

                        if (value.GetType() == typeof(EntityReference))
                        {
                            var referenceEntityName = property.PropertyType.GetGenericArguments().Single();
                            var entityReferenceType = typeof(EntityReference<>).MakeGenericType(referenceEntityName);

                            value = Activator.CreateInstance(entityReferenceType, new object[] { ((Microsoft.Xrm.Sdk.EntityReference)value).Id });
                        }
                        else if (value.GetType() == typeof(OptionSetValue))
                        {
                            var integerOptionValue = ((OptionSetValue)value).Value;
                            value = integerOptionValue;
                        }

                        property.SetValue(crmPlusPlusEntity, value);
                    }
                }
            }

            return crmPlusPlusEntity;
        }

        internal static T ToCrmPlusPlusEntity<T>(this Microsoft.Xrm.Sdk.Entity entity, string alias = "") where T : CrmPlusPlusEntity, new()
        {
            return (T)ToCrmPlusPlusEntity(entity, typeof(T), alias);
        }

        internal static IEnumerable<Entity> WhereEntitiesHaveKeyThatContains(this IEnumerable<Entity> entities, string substringOfKey)
        {
            var filteredEntities = new List<Entity>();
            foreach (var entity in entities)
            {
                var keys = entity.Attributes.Keys;

                if (keys.Any(k => k.Contains(substringOfKey)))
                {
                    filteredEntities.Add(entity);
                }
            }

            return filteredEntities;
        }
    }
}
