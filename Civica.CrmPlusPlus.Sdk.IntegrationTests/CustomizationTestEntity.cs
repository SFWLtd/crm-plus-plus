using System;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests
{
    [EntityName("civica_customizationtest")]
    [EntityInfo("Customization Test Entity", OwnershipType.OrganizationOwned)]
    public class CustomizationTestEntity : CrmPlusPlusEntity
    {
        [PropertyName("civica_isatest")]
        [PropertyInfo("Is a test", AttributeRequiredLevel.Recommended)]
        [Boolean]
        public bool IsATest { get; set; }

        [PropertyName("civica_somedate")]
        [PropertyInfo("Some date", AttributeRequiredLevel.None)]
        [DateTime(DateTimeFormat.DateOnly)]
        public DateTime SomeDate { get; set; }

        [PropertyName("civica_cost")]
        [PropertyInfo("Cost", AttributeRequiredLevel.ApplicationRequired)]
        [Decimal(1000000, 0, 2)]
        public decimal Cost { get; set; }

        [PropertyName("civica_length")]
        [PropertyInfo("Length", AttributeRequiredLevel.None)]
        [Double(1000, 0, 2)]
        public double Length { get; set; }

        [PropertyName("civica_wholenumber")]
        [PropertyInfo("Length", AttributeRequiredLevel.Recommended)]
        [Integer(101010, -101010, IntegerFormat.None)]
        public int WholeNumber { get; set; }

        [PropertyName("civica_email")]
        [PropertyInfo("Just some text", AttributeRequiredLevel.None)]
        [String(255, StringFormatName.Email)]
        public string Email { get; set; }
    }
}
