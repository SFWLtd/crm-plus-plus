using System;
using Civica.CrmPlusPlus.Sdk.Client;
using FakeItEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client
{
    public class CrmPlusPlusCustomizationClientTests
    {
        [Fact]
        public void WhenEntityHasNoEntityNameAttribute_Create_ThrowsInvalidOperationException()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateEntityWithoutProperties<EntityWithoutName>());
        }

        [Fact]
        public void WhenEntityHasNoEntityInfoAttribute_Create_ThrowsInvalidOperationException()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateEntityWithoutProperties<EntityWithoutInfo>());
        }

        [Fact]
        public void WhenEntityHasEntityInfoAndNameAttributes_ShouldCreateEntity()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            client.CreateEntityWithoutProperties<EntityWithProperties>();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutNameAttribute_ShouldThrowInvalidOperationException_AndShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithoutNameAttribute));
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutInfoAttribute_ShouldThrowInvalidOperationException_AndShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithoutInfoAttribute));
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutStringAttribute_ShouldThrowInvalidOperationException_AndShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithoutStringAttribute));
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutAllRequiredAttributes_ShouldCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(organisationService);

            client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithAllRequiredAttributes);
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
