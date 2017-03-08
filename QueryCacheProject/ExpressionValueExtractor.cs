using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QueryCacheProject
{
    internal class ExpressionValueExtractor
    {
        public object[] GetArgumentValues<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is ConstantExpression)
            {
                return ExtractConstantExpression(expression.Body);
            }
            if (expression.Body is MethodCallExpression)
            {
                return ExtractMethodCallExpression(expression);
            }
            throw new NotImplementedException();
        }

        private MemberExpression ResolveMemberExpression(Expression expression)
        {
            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                // if casting is involved, Expression is not x => x.FieldName but x => Convert(x.Fieldname)
                return (MemberExpression)((UnaryExpression)expression).Operand;
            }
            else
            {
                throw new NotSupportedException(expression.ToString());
            }
        }

        private object[] ExtractMethodCallExpression<T>(Expression<Func<T>> expression)
        {
            var body = (MethodCallExpression)expression.Body;
            var values = new List<object>(body.Arguments.Count);

            foreach (var argument in body.Arguments)
            {
                object value;
                if (argument is ConstantExpression)
                {
                    value = ExtractConstantExpression(argument);
                }
                else
                {
                    var exp = ResolveMemberExpression(argument);

                    value = GetValue(exp);
                }

                values.Add(value);
            }

            return values.ToArray();
        }

        private object[] ExtractConstantExpression(Expression exp)
        {
            var e = (ConstantExpression)exp;
            return new[] { e.Value };
        }


        private object GetValue(MemberExpression exp)
        {
            // expression is ConstantExpression or FieldExpression
            if (exp.Expression is ConstantExpression)
            {
                return (((ConstantExpression)exp.Expression).Value)
                    .GetType()
                    .GetField(exp.Member.Name)
                    .GetValue(((ConstantExpression)exp.Expression).Value);
            }
            else if (exp.Expression is MemberExpression)
            {
                return GetValue((MemberExpression)exp.Expression);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
