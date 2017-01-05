using System;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class EnumExtensions
    {
        internal static TTarget ToSimilarEnum<TTarget>(this Enum sourceValue)
        {
            try
            {
               return (TTarget)Enum.Parse(typeof(TTarget), sourceValue.ToString());
            }
            catch(Exception)
            {
                throw new InvalidOperationException(string.Format("Cannot cast value to type '{0}'", typeof(TTarget).Name));
            }
        }
    }
}
