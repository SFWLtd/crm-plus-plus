using System;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.DefaultEntities;
using FakeItEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client
{
    public class CrmPlusPlusCustomizationClientTests
    {
        [Fact]
        public void WhenEntityHasNoEntityNameAttribute_Create_ThrowsInvalidOperationException()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateEntity<EntityWithoutName>());
        }

        [Fact]
        public void WhenEntityHasNoEntityInfoAttribute_Create_ThrowsInvalidOperationException()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateEntity<EntityWithoutInfo>());
        }

        [Fact]
        public void WhenEntityHasEntityInfoAndNameAttributes_ShouldCreateEntity()
        {
            var organisationService = A.Fake<IOrganizationService>();

            A.CallTo(() => organisationService.Execute(A<CreateEntityRequest>._)).Returns(new CreateEntityResponse());

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            client.CreateEntity<EntityWithProperties>();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutNameAttribute_ShouldThrowInvalidOperationException_AndShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithoutNameAttribute));
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutInfoAttribute_ShouldThrowInvalidOperationException_AndShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            Assert.Throws<InvalidOperationException>(() => client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithoutInfoAttribute));
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutStringAttribute_ShouldNotAttemptToCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public void WhenEntityHasStringPropertyWithoutAllRequiredAttributes_ShouldCreateProperty()
        {
            var organisationService = A.Fake<IOrganizationService>();

            var client = new CrmPlusPlusCustomizationClient(A.Fake<Publisher>(), A.Fake<Solution>(), organisationService);

            client.CreateProperty<EntityWithProperties, string>(e => e.StringPropertyWithAllRequiredAttributes);
            A.CallTo(() => organisationService.Execute(A<OrganizationRequest>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
