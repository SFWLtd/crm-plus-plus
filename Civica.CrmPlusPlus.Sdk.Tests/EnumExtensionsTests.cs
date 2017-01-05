using System;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void CanCastValueWithSameName()
        {
            var value = TestEnum1.Value1;

            var result = value.ToSimilarEnum<TestEnum2>();

            Assert.Equal(TestEnum2.Value1, result);
        }

        [Fact]
        public void WhenTargetValueDoesNotExist_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => TestEnum1.Value3.ToSimilarEnum<TestEnum2>());
        }
    }

    public enum TestEnum1
    {
        Value1,
        Value2,
        Value3
    }

    public enum TestEnum2
    {
        Value1,
        Value2,
        Value3AndSomething
    }
}
