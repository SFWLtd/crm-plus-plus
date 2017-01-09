using Civica.CrmPlusPlus.Sdk.Querying;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests.Querying
{
    public class QueryTests
    {
        [Fact]
        public void QueryForTestEntity_WithoutProperties_ShouldIncludeCreatedOnAndModifiedON()
        {
            var fetchXml = Query
                .ForEntity<TestEntity>()
                .ToFetchXml()
                .ClearXmlFormatting();

            Assert.Equal("<fetch mapping='logical' distinct='false'><entity name='testentity'><attribute name='createdon'/><attribute name='modifiedon'/></entity></fetch>", fetchXml);
        }

        [Fact]
        public void QueryWithIncludedProperty_FormsFetchXmlCorrectly()
        {
            var fetchXml = Query
                .ForEntity<TestEntity>()
                .Include(e => e.StringTestProperty)
                .ToFetchXml()
                .ClearXmlFormatting();

            Assert.Equal("<fetch mapping='logical' distinct='false'><entity name='testentity'><attribute name='createdon'/><attribute name='modifiedon'/><attribute name='test'/></entity></fetch>", fetchXml);
        }

        [Fact]
        public void QueryWithEmptyFilter_DoesNotIncludeFilterInFetchXml()
        {
            var fetchXml = Query
                .ForEntity<TestEntity>()
                .Filter(FilterType.And, filter => { })
                .ToFetchXml()
                .ClearXmlFormatting();

            var expected =
                @"<fetch mapping='logical' distinct='false'>
                    <entity name='testentity'>
                        <attribute name='createdon'/>
                        <attribute name='modifiedon'/>
                    </entity>
                </fetch>";

            Assert.Equal(expected.ClearXmlFormatting(), fetchXml);
        }

        [Fact]
        public void QueryWithOneFilterCondition_FormsFetchXmlCorrectly()
        {
            var fetchXml = Query
                .ForEntity<TestEntity>()
                .Filter(FilterType.And, filter =>
                {
                    filter.Condition(e => e.StringTestProperty, ConditionOperator.Equal, "Value");
                })
                .ToFetchXml()
                .ClearXmlFormatting();

            var expected = 
                @"<fetch mapping='logical' distinct='false'>
                    <entity name='testentity'>
                        <attribute name='createdon'/>
                        <attribute name='modifiedon'/>
                        <filter type='and'>
                            <condition attribute='test' operator='eq' value='Value'/>
                        </filter>
                    </entity>
                </fetch>";

            Assert.Equal(expected.ClearXmlFormatting(), fetchXml);
        }

        [Fact]
        public void QueryWithMultipleLayeredFilterConditions_FormsFetchXmlCorrectly()
        {
            var fetchXml = Query
                .ForEntity<TestEntity>()
                .Filter(FilterType.Or, filter => 
                {
                    filter.InnerFilter(FilterType.And, innerFilter =>
                    {
                        innerFilter.Condition(e => e.StringTestProperty, ConditionOperator.Equal, "Value1");
                        innerFilter.Condition(e => e.ModifiedOn, ConditionOperator.LessThanOrEqual, "2017-01-01");
                    });
                    filter.InnerFilter(FilterType.And, innerFilter =>
                    {
                        innerFilter.Condition(e => e.StringTestProperty, ConditionOperator.Equal, "Value2");
                        innerFilter.Condition(e => e.CreatedOn, ConditionOperator.LessThanOrEqual, "2016-01-01");
                    });
                })
                .ToFetchXml()
                .ClearXmlFormatting();

            var expected =
                @"<fetch mapping='logical' distinct='false'>
                    <entity name='testentity'>
                        <attribute name='createdon'/>
                        <attribute name='modifiedon'/>
                        <filter type='or'>
                            <filter type='and'>
                                <condition attribute='test' operator='eq' value='Value1'/>
                                <condition attribute='modifiedon' operator='le' value='2017-01-01'/>
                            </filter>
                            <filter type='and'>
                                <condition attribute='test' operator='eq' value='Value2'/>
                                <condition attribute='createdon' operator='le' value='2016-01-01'/>
                            </filter>
                        </filter>
                    </entity>
                </fetch>";

            Assert.Equal(expected.ClearXmlFormatting(), fetchXml);
        }

        [Fact]
        public void QueryWithJoinedEntities_FormsXmlCorrectly()
        {
            var fetchXml = Query.ForEntity<TestEntity>()
                .Join1ToN(e => e.JoinedEntities, e => e.TestEntityId, JoinType.Outer, query =>
                {
                    query.Include(e => e.Number);
                })
                .ToFetchXml()
                .ClearXmlFormatting();

            var expected =
                @"<fetch mapping='logical' distinct='false'>
                    <entity name='testentity'>
                        <attribute name='createdon'/>
                        <attribute name='modifiedon'/>
                        <link-entity name='testjoinedentity' alias='testjoinedentity' from='testentityid' to='testlookupname' link-type='outer'>
                            <attribute name='createdon'/>
                            <attribute name='modifiedon'/>
                            <attribute name='testnumber'/>
                        </link-entity>
                    </entity>
                </fetch>";

            Assert.Equal(expected.ClearXmlFormatting(), fetchXml);
        }

        [Fact]
        public void QueryWithNestedJoinedEntities_DoesNotIncludeAttributesForLinkedEntityDepthMoreThanOne()
        {
            var fetchXml = Query.ForEntity<TestEntity>()
                .Join1ToN(e => e.JoinedEntities, e => e.TestEntityId, JoinType.Outer, query =>
                {
                    query.Include(e => e.Number);
                    query.Join1ToN(e => e.NestedJoinedEntities, e => e.TestNestedLookupName, JoinType.Outer, innerQuery =>
                    {
                        innerQuery.Include(e => e.Number); // This should not be included
                    });
                })
                .ToFetchXml()
                .ClearXmlFormatting();

            var expected =
                @"<fetch mapping='logical' distinct='false'>
                    <entity name='testentity'>
                        <attribute name='createdon'/>
                        <attribute name='modifiedon'/>
                        <link-entity name='testjoinedentity' alias='testjoinedentity' from='testentityid' to='testlookupname' link-type='outer'>
                            <attribute name='createdon'/>
                            <attribute name='modifiedon'/>
                            <attribute name='testnumber'/>
                            <link-entity name='testnestedjoinedentity' alias='testnestedjoinedentity' from='testjoinedentityid' to='testnestedlookupname' link-type='outer'/>
                        </link-entity>
                    </entity>
                </fetch>";

            Assert.Equal(expected.ClearXmlFormatting(), fetchXml);
        }

        [Fact]
        public void QueryIncludingAllAttributes_AfterIncludingSingleAttribute_RemoveSingleAttributeXml()
        {
            var query = Query.ForEntity<TestEntity>()
                .Include(e => e.StringTestProperty)
                .IncludeAllProperties()
                .ToFetchXml()
                .ClearXmlFormatting();

            Assert.DoesNotContain("<attribute name='test'/>", query);
        }

        [Fact]
        public void QueryIncludingSingleAttribute_AfterIncludingAllAttributes_DoesNothing()
        {
            var queryWithoutSingleInclusion = Query.ForEntity<TestEntity>()
                .IncludeAllProperties()
                .ToFetchXml()
                .ClearXmlFormatting();

            var queryWithSingleInclusion = Query.ForEntity<TestEntity>()
                .IncludeAllProperties()
                .Include(e => e.StringTestProperty)
                .ToFetchXml()
                .ClearXmlFormatting();

            Assert.Equal(queryWithoutSingleInclusion, queryWithSingleInclusion);
        }
    }
}
