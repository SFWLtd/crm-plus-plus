using System;
using System.Linq.Expressions;

namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal static class GuardExpressionExtensions
    {
        internal static GuardThis<Expression> AgainstNonMemberExpression(this GuardThis<Expression> guard)
        {
            if (!(guard.Obj is MemberExpression))
            {
                throw new ArgumentException("Expression expected to be of type 'MemberExpression' but it was not");
            }

            return guard;
        }
    }
}
