using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public class CrmPlusPlusCustomizationClient
    {
        private readonly IOrganizationService service;

        internal CrmPlusPlusCustomizationClient(IOrganizationService service)
        {
            this.service = service;
        }

        public void CreateEntityWithoutProperties<T>() where T : CrmPlusPlusEntity, new()
        {
            var entityName = EntityNameAttribute.GetFromType<T>();
            var entityInfo = EntityInfoAttribute.GetFromType<T>();

            var createEntityRequest = new CreateEntityRequest
            {
                Entity = new EntityMetadata
                {
                    SchemaName = entityName,
                    DisplayName = entityInfo.DisplayName.ToLabel(),
                    DisplayCollectionName = entityInfo.PluralDisplayName.ToLabel(),
                    Description = entityInfo.Description.ToLabel(),
                    OwnershipType = entityInfo.OwnershipType,
                    IsActivity = false
                },
                PrimaryAttribute = new StringAttributeMetadata
                {
                    SchemaName = entityName + "primary",
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = 100,
                    FormatName = Microsoft.Xrm.Sdk.Metadata.StringFormatName.Text,
                    DisplayName = "Primary attribute".ToLabel(),
                    Description = string.Format("The primary attribute for the {0} entity", entityInfo.DisplayName).ToLabel()
                }
            };

            service.Execute(createEntityRequest);
        }

        public void CreateProperty<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new()
        {
            var entityName = EntityNameAttribute.GetFromType<T>();
            var propertyName = PropertyNameAttribute.GetFromType(propertyExpr);
            var propertyInfo = PropertyInfoAttribute.GetFromType(propertyExpr);

            var attributes = ((MemberExpression)propertyExpr.Body).Member.GetCustomAttributes(true);

            CreateProperty(entityName, typeof(TProperty), attributes, propertyName, propertyInfo);
        }

        public void CreateAllProperties<T>() where T : CrmPlusPlusEntity, new()
        {
            var entityName = EntityNameAttribute.GetFromType<T>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                var attributes = new List<object>();
                foreach (var attribute in property.Attributes)
                {
                    attributes.Add(attribute);
                }

                var propertyNameAttribute = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyNameAttribute));
                var propertyInfoAttribute = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyInfoAttribute));

                if (propertyNameAttribute != null && propertyInfoAttribute != null)
                {
                    var name = (PropertyNameAttribute)propertyNameAttribute;
                    var info = (PropertyInfoAttribute)propertyInfoAttribute;

                    CreateProperty(entityName, property.PropertyType, attributes, name.PropertyName, info);
                }
            }
        }

        public void Delete<T>() where T : CrmPlusPlusEntity, new()
        {
            var entityName = EntityNameAttribute.GetFromType<T>();

            var deleteEntityRequest = new DeleteEntityRequest
            {
                LogicalName = entityName
            };

            service.Execute(deleteEntityRequest);
        }

        private void CreateProperty(string entityName, Type propertyType, IEnumerable<object> attributes, string propertyName, PropertyInfoAttribute info)
        {
            var createAttributeRequest = new CreateAttributeRequest()
            {
                EntityName = entityName
            };

            if (propertyType == typeof(string))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(StringAttribute));
                if (attrInfo != null)
                {
                    var stringInfo = (StringAttribute)attrInfo;

                    createAttributeRequest.Attribute = new StringAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        MaxLength = stringInfo.MaxLength,
                        FormatName = stringInfo.StringFormat
                    };

                    service.Execute(createAttributeRequest);
                    return;
                }
            }
            else if (propertyType == typeof(bool))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(BooleanAttribute));
                if (attrInfo != null)
                {
                    var booleanAttr = (BooleanAttribute)attrInfo;
                    createAttributeRequest.Attribute = new BooleanAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        OptionSet = new BooleanOptionSetMetadata(
                            new OptionMetadata("True".ToLabel(), 1),
                            new OptionMetadata("False".ToLabel(), 0)),
                    };
                }

                service.Execute(createAttributeRequest);
                return;
            }
            else if (propertyType == typeof(DateTime))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(DateTimeAttribute));
                if (attrInfo != null)
                {
                    var dateTimeInfo = (DateTimeAttribute)attrInfo;

                    createAttributeRequest.Attribute = new DateTimeAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        Format = dateTimeInfo.Format,
                        ImeMode = dateTimeInfo.ImeMode
                    };
                }

                service.Execute(createAttributeRequest);
                return;
            }
            else if (propertyType == typeof(decimal))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(DecimalAttribute));
                if (attrInfo != null)
                {
                    var decimalInfo = (DecimalAttribute)attrInfo;

                    createAttributeRequest.Attribute = new DecimalAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        MaxValue = decimalInfo.MaxValue,
                        MinValue = decimalInfo.MinValue,
                        Precision = decimalInfo.Precision
                    };
                }

                service.Execute(createAttributeRequest);
                return;
            }
            else if (propertyType == typeof(double))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(DoubleAttribute));
                if (attrInfo != null)
                {
                    var decimalInfo = (DoubleAttribute)attrInfo;

                    createAttributeRequest.Attribute = new DoubleAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        MaxValue = decimalInfo.MaxValue,
                        MinValue = decimalInfo.MinValue,
                        Precision = decimalInfo.Precision
                    };
                }

                service.Execute(createAttributeRequest);
                return;
            }
            else if (propertyType == typeof(int))
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(IntegerAttribute));
                if (attrInfo != null)
                {
                    var integerInfo = (IntegerAttribute)attrInfo;

                    createAttributeRequest.Attribute = new IntegerAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        MaxValue = integerInfo.MaxValue,
                        MinValue = integerInfo.MinValue,
                        Format = integerInfo.Format
                    };
                }

                service.Execute(createAttributeRequest);
                return;
            }

            throw new InvalidOperationException("Attempted to create property but type information attribute was missing");
        }
    }
}
