using System;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests
{
    public class CrmPlusPlusEntityExtensionsTests
    {
        [Fact]
        public void CorrectlyMapsPropertiesToCrmEntity()
        {
            var lookupId = Guid.NewGuid();

            var myString = "dhsjadhkjsl";
            var myInt = 32;
            var myDateTime = DateTime.Now;
            var myDecimal = 143.12M;
            var myBool = false;
            var myDouble = 103.23;
            var myLookup = new EntityReference<CrmPlusPlusEntityExtensionsEntity>(lookupId);

            var entity = new CrmPlusPlusEntityExtensionsEntity
            {
                MyString = myString,
                MyInt = myInt,
                MyDateTime = myDateTime,
                MyDecimal = myDecimal,
                MyBool = myBool,
                MyDouble = myDouble,
                MyLookup = myLookup
            };

            var crmEntity = entity.ToCrmEntity();

            Assert.Equal(myString, crmEntity["civica_string"]);
            Assert.Equal(myInt, crmEntity["civica_int"]);
            Assert.Equal(myDateTime, crmEntity["civica_datetime"]);
            Assert.Equal(myDecimal, crmEntity["civica_decimal"]);
            Assert.Equal(myBool, crmEntity["civica_bool"]);
            Assert.Equal(myDouble, crmEntity["civica_double"]);

            Assert.Equal(myLookup.Id, ((Microsoft.Xrm.Sdk.EntityReference)crmEntity["civica_lookup"]).Id);
            Assert.Equal(EntityNameAttribute.GetFromType<CrmPlusPlusEntityExtensionsEntity>(), ((Microsoft.Xrm.Sdk.EntityReference)crmEntity["civica_lookup"]).LogicalName);
        }
    }

    [EntityName("civica_entityexample")]
    [EntityInfo("Entity Example", OwnershipType.OrganizationOwned)]
    public class CrmPlusPlusEntityExtensionsEntity : CrmPlusPlusEntity
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
        public EntityReference<CrmPlusPlusEntityExtensionsEntity> MyLookup { get; set; }
    }
}
