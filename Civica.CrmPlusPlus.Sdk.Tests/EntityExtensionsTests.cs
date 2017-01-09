using System;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests
{
    public class EntityExtensionsTests
    {
        [Fact]
        public void CorrectlyMapsPropertiesFromCrmEntity()
        {
            var lookupId = Guid.NewGuid();

            var myString = "dhsjadhkjsl";
            var myInt = 32;
            var myDateTime = DateTime.Now;
            var myDecimal = 143.12M;
            var myBool = false;
            var myDouble = 103.23;
            var myLookup = new EntityReference<CrmPlusPlusEntityExtensionsEntity>(lookupId);

            var crmEntity = new Entity("civica_entityexample", Guid.NewGuid());
            crmEntity["createdon"] = DateTime.Now; // Required by default
            crmEntity["modifiedon"] = DateTime.Now; // Required by default
            crmEntity["civica_string"] = myString;
            crmEntity["civica_int"] = myInt;
            crmEntity["civica_datetime"] = myDateTime;
            crmEntity["civica_decimal"] = myDecimal;
            crmEntity["civica_bool"] = myBool;
            crmEntity["civica_double"] = myDouble;
            crmEntity["civica_lookup"] = new EntityReference("civica_entityexample", lookupId);
            crmEntity["civica_optionset"] = new OptionSetValue(3);

            var entity = crmEntity.ToCrmPlusPlusEntity<EntityExtensionsEntity>();

            Assert.Equal(myString, entity.MyString);
            Assert.Equal(myInt, entity.MyInt);
            Assert.Equal(myDateTime, entity.MyDateTime);
            Assert.Equal(myDecimal, entity.MyDecimal);
            Assert.Equal(myBool, entity.MyBool);
            Assert.Equal(myDouble, entity.MyDouble);

            Assert.Equal(myLookup.Id, entity.MyLookup.Id);
            Assert.Equal(typeof(EntityExtensionsEntity), entity.MyLookup.GetType().GetGenericArguments().Single());

            Assert.Equal(MyOptionSet.Three, entity.MyOptionSet);
        }
    }

    [EntityName("civica_entityexample")]
    [EntityInfo("Entity Example", OwnershipType.OrganizationOwned)]
    public class EntityExtensionsEntity : CrmPlusPlusEntity
    {

        [PropertyName("civica_string")]
        [PropertyInfo("String", AttributeRequiredLevel.None)]
        [String(100, StringFormatName.Email)]
        public string MyString { get; set; }

        [PropertyName("civica_int")]
        [PropertyInfo("Integer", AttributeRequiredLevel.None)]
        [Integer(100, 0, IntegerFormat.None)]
        public int MyInt { get; set; }

        [PropertyName("civica_datetime")]
        [PropertyInfo("Date Time", AttributeRequiredLevel.None)]
        [DateTime(DateTimeFormat.DateAndTime)]
        public DateTime MyDateTime { get; set; }

        [PropertyName("civica_bool")]
        [PropertyInfo("Bool", AttributeRequiredLevel.None)]
        [Boolean]
        public bool MyBool { get; set; }

        [PropertyName("civica_double")]
        [PropertyInfo("Double", AttributeRequiredLevel.None)]
        [Double(1000, 0, 2)]
        public double MyDouble { get; set; }

        [PropertyName("civica_decimal")]
        [PropertyInfo("Decimal", AttributeRequiredLevel.None)]
        [Decimal(1000, 0, 2)]
        public decimal MyDecimal { get; set; }

        [PropertyName("civica_lookup")]
        [PropertyInfo("Lookup", AttributeRequiredLevel.None)]
        [Lookup]
        public EntityReference<EntityExtensionsEntity> MyLookup { get; set; }

        [PropertyName("civica_optionset")]
        [PropertyInfo("Option set", AttributeRequiredLevel.None)]
        [OptionSet]
        public MyOptionSet MyOptionSet { get; set; }
    }

    public enum MyOptionSet
    {
        One=1,
        Two=2,
        Three=3
    }
}
