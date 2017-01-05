﻿using System;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Microsoft.Xrm.Sdk;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class CrmPlusPlusEntityExtensions
    {
        internal static Entity ToCrmEntity<T>(this T crmPlusPlusEntity) where T : CrmPlusPlusEntity, new()
        {
            var entity = new Entity(EntityNameAttribute.GetFromType<T>(), crmPlusPlusEntity.Id);

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                var attributes = property.Attributes.AsEnumerable();

                var propertyNameAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyNameAttribute));
                var propertyInfoAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyInfoAttribute));
                var typeInfoAttr = attributes
                    .SingleOrDefault(attr => (attr.GetType() == typeof(BooleanAttribute) && property.PropertyType == typeof(bool))
                        || (attr.GetType() == typeof(DateTimeAttribute) && property.PropertyType == typeof(DateTime))
                        || (attr.GetType() == typeof(DecimalAttribute) && property.PropertyType == typeof(decimal))
                        || (attr.GetType() == typeof(DoubleAttribute) && property.PropertyType == typeof(double))
                        || (attr.GetType() == typeof(IntegerAttribute) && property.PropertyType == typeof(int))
                        || (attr.GetType() == typeof(StringAttribute) && property.PropertyType == typeof(string)));

                if (propertyNameAttr != null && propertyInfoAttr != null && typeInfoAttr != null)
                {
                    var propertyName = ((PropertyNameAttribute)propertyNameAttr).PropertyName;
                    var value = property.GetValue(crmPlusPlusEntity);

                    if (value != null)
                    {
                        entity[propertyName] = value;
                    }
                }
            }

            return entity;
        }
    }
}
