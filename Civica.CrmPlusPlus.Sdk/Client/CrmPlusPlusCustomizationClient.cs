using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.DefaultEntities;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Civica.CrmPlusPlus.Sdk.Validation;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public class CrmPlusPlusCustomizationClient : ICrmPlusPlusCustomizationClient
    {
        private readonly IOrganizationService service;
        public Solution Solution { get; }
        private Publisher Publisher { get; }

        internal CrmPlusPlusCustomizationClient(Publisher publisher, Solution solution, IOrganizationService service)
        {
            Solution = solution;
            Publisher = publisher;
            this.service = service;
        }

        public void CreateEntity<T>() where T : CrmPlusPlusEntity, new()
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

            var response = (CreateEntityResponse)service.Execute(createEntityRequest);

            var addReq = new AddSolutionComponentRequest()
            {
                ComponentType = (int)SolutionComponentTypes.Entity,
                ComponentId = response.EntityId,
                SolutionUniqueName = Solution.Name
            };
            
            service.Execute(addReq);
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
            }
            else if (propertyType.IsEnum)
            {
                var attrInfo = attributes.SingleOrDefault(attr => attr.GetType() == typeof(OptionSetAttribute));
                if (attrInfo != null)
                {
                    var options = new OptionMetadataCollection();
                    foreach (var value in Enum.GetValues(propertyType))
                    {
                        options.Add(new OptionMetadata(value.ToString().ToLabel(), (int)value));
                    }

                    createAttributeRequest.Attribute = new PicklistAttributeMetadata
                    {
                        SchemaName = propertyName,
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(info.AttributeRequiredLevel),
                        DisplayName = info.DisplayName.ToLabel(),
                        Description = info.Description.ToLabel(),
                        OptionSet = new OptionSetMetadata(options)
                        {
                            IsGlobal = false,
                            DisplayName = info.DisplayName.ToLabel(),
                            Description = info.Description.ToLabel(),
                            IsCustomOptionSet = true,
                            OptionSetType = OptionSetType.Picklist,
                            Name = propertyName
                        }
                    };

                    service.Execute(createAttributeRequest);
                }
            }
        }

        public bool CanCreateOneToManyRelationship<TOne, TMany>()
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new()
        {
            var canBeReferencedRequest = new CanBeReferencedRequest
            {
                EntityName = EntityNameAttribute.GetFromType<TOne>()
            };

            var canBeReferencingRequest = new CanBeReferencingRequest
            {
                EntityName = EntityNameAttribute.GetFromType<TMany>()
            };

            bool canBeReferenced = ((CanBeReferencedResponse)service.Execute(canBeReferencedRequest)).CanBeReferenced;
            bool canBeReferencing = ((CanBeReferencingResponse)service.Execute(canBeReferencingRequest)).CanBeReferencing;

            return canBeReferenced && canBeReferencing;
        }

        public void CreateOneToManyRelationship<TOne, TMany>(Expression<Func<TMany, EntityReference<TOne>>> lookupExpr, EntityAttributes.Metadata.AttributeRequiredLevel lookupRequiredLevel, string relationshipPrefix = "new")
            where TOne : CrmPlusPlusEntity, new()
            where TMany : CrmPlusPlusEntity, new()
        {
            Guard.This(relationshipPrefix).AgainstNullOrEmpty();

            var oneEntityName = EntityNameAttribute.GetFromType<TOne>();
            var manyEntityName = EntityNameAttribute.GetFromType<TMany>();
            var oneDisplayName = EntityInfoAttribute.GetFromType<TOne>().DisplayName;
            var lookupPropertyName = PropertyNameAttribute.GetFromType(lookupExpr);

            var oneToManyRequest = new CreateOneToManyRequest
            {
                OneToManyRelationship = new OneToManyRelationshipMetadata
                {
                    ReferencedEntity = oneEntityName,
                    ReferencingEntity = manyEntityName,
                    SchemaName = relationshipPrefix.EndsWith("_") ? relationshipPrefix : relationshipPrefix + "_" + oneEntityName + "_" + manyEntityName,
                    AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                    {
                        Behavior = AssociatedMenuBehavior.UseLabel,
                        Group = AssociatedMenuGroup.Details,
                        Label = oneDisplayName.ToLabel(),
                        Order = 10000
                    },
                    CascadeConfiguration = new CascadeConfiguration
                    {
                        Assign = CascadeType.NoCascade,
                        Delete = CascadeType.RemoveLink,
                        Merge = CascadeType.NoCascade,
                        Reparent = CascadeType.NoCascade,
                        Share = CascadeType.NoCascade,
                        Unshare = CascadeType.NoCascade
                    }
                },
                Lookup = new LookupAttributeMetadata
                {
                    SchemaName = lookupPropertyName,
                    DisplayName = (oneDisplayName + " Lookup").ToLabel(),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(lookupRequiredLevel.ToSimilarEnum<AttributeRequiredLevel>()),
                    Description = (oneDisplayName + " Lookup").ToLabel()
                }
            };

            service.Execute(oneToManyRequest);
        }
    }
}
