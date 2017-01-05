using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client.Association;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client.Association
{
    public class AssociateTests
    {
        [Fact]
        public void WhenAssociatingAnEntity_ItsIdShouldBePartOfTheAssociation()
        {
            var testEntityToBeAssociated = new TestEntityToBeAssociated();
            var testAssociatedEntity = new TestAssociatedEntity();

            var association =
                Associate.ThisEntity(testEntityToBeAssociated)
                    .With(testAssociatedEntity);

            Assert.Equal(testEntityToBeAssociated.Id, association.EntityId);
        }

        [Fact]
        public void WhenAssociatingAnEntity_ItsEntityNameShouldBePartOfTheAssociation()
        {
            var testEntityToBeAssociated = new TestEntityToBeAssociated();
            var testAssociatedEntity = new TestAssociatedEntity();

            var association =
                Associate.ThisEntity(testEntityToBeAssociated)
                    .With(testAssociatedEntity);

            Assert.Equal("test_entitytobeassociated", association.EntityName);
        }

        [Fact]
        public void WhenAssociatingAnEntity_ItsRelatedEntityIdShouldBePartOfTheAssociation()
        {
            var testEntityToBeAssociated = new TestEntityToBeAssociated();
            var testAssociatedEntity = new TestAssociatedEntity();

            var association =
                Associate.ThisEntity(testEntityToBeAssociated)
                    .With(testAssociatedEntity);

            Assert.Equal(testAssociatedEntity.Id, association.RelatedEntities.Single().Key);
        }

        [Fact]
        public void WhenAssociatingAnEntity_ItsRelatedEntityNameShouldBePartOfTheAssociation()
        {
            var testEntityToBeAssociated = new TestEntityToBeAssociated();
            var testAssociatedEntity = new TestAssociatedEntity();

            var association =
                Associate.ThisEntity(testEntityToBeAssociated)
                    .With(testAssociatedEntity);

            Assert.Equal("test_associatedentity", association.RelatedEntities.Single().Value);
        }
    }
}
