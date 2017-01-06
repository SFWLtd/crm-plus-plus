using System.Collections;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client
{
    [EntityName("civica_entitywithnameandinfo")]
    [EntityInfo("Entity with name and info", OwnershipType.OrganizationOwned)]
    public class EntityWithProperties : CrmPlusPlusEntity
    {
        [PropertyInfo("Test", AttributeRequiredLevel.None)]
        [String(100, StringFormatName.Email)]
        public string StringPropertyWithoutNameAttribute { get; set; }

        [PropertyName("civica_test")]
        [String(100, StringFormatName.Email)]
        public string StringPropertyWithoutInfoAttribute { get; set; }

        [PropertyName("civica_test")]
        [PropertyInfo("Test", AttributeRequiredLevel.None)]
        public string StringPropertyWithoutStringAttribute { get; set; }

        [PropertyName("civica_test")]
        [PropertyInfo("Test", AttributeRequiredLevel.None)]
        [String(100, StringFormatName.Email)]
        public string StringPropertyWithAllRequiredAttributes { get; set; }

        
        public IEnumerable<JoinedEntity> JoinedEntities { get; set; }
    }
}
