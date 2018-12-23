using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Functions
{
    public class LaplaceFunction : Function
    {
        public LaplaceExpression Expression { get; set; }

        public override Dictionary<EngrNumber, Complex> Points => throw new NotImplementedException();

        public override Complex Calculate(double f)
        {
            /* s = jw */
            return Expression.Evaluate(2 * Math.PI * f * Complex.ImaginaryOne);
        }

        public Complex Calculate(Complex point)
        {
            return Expression.Evaluate(point);
        }
    }


    public abstract class LaplaceExpression
    {
        public abstract Complex Evaluate(Complex point);
    }

    public class LaplaceConstant : LaplaceExpression
    {
        public Complex ConstantValue;

        public override Complex Evaluate(Complex point)
        {
            return ConstantValue;
        }
    }

    public class LaplaceLiteral : LaplaceExpression
    {
        public override Complex Evaluate(Complex point)
        {
            return point;
        }
    }

    public class LaplaceProduct : LaplaceExpression
    {
        public List<LaplaceExpression> Expressions { get; } = new List<LaplaceExpression>();

        public override Complex Evaluate(Complex point)
        {
            Complex output = 1;
            foreach (var exp in Expressions)
                output *= exp.Evaluate(point);

            return output;
        }

        public LaplaceProduct(LaplaceExpression exp1, LaplaceExpression exp2)
        {
            Expressions.Add(exp1);
            Expressions.Add(exp2);
        }

        public LaplaceProduct(LaplaceExpression exp1, LaplaceExpression exp2, LaplaceExpression exp3)
        {
            Expressions.Add(exp1);
            Expressions.Add(exp2);
            Expressions.Add(exp3);
        }

    }

    public class LaplaceSum : LaplaceExpression
    {
        public List<LaplaceExpression> Expressions { get; } = new List<LaplaceExpression>();

        public LaplaceSum(LaplaceExpression exp1, LaplaceExpression exp2)
        {
            Expressions.Add(exp1);
            Expressions.Add(exp2);
        }

        public LaplaceSum(LaplaceExpression exp1, LaplaceExpression exp2, LaplaceExpression exp3)
        {
            Expressions.Add(exp1);
            Expressions.Add(exp2);
            Expressions.Add(exp3);
        }

        public LaplaceSum(LaplaceExpression exp1, LaplaceExpression exp2, LaplaceExpression exp3, LaplaceExpression exp4)
        {
            Expressions.Add(exp1);
            Expressions.Add(exp2);
            Expressions.Add(exp3);
            Expressions.Add(exp4);
        }

        public override Complex Evaluate(Complex point)
        {
            Complex output = 0;
            foreach (var exp in Expressions)
                output += exp.Evaluate(point);

            return output;
        }
    }

    public class LaplacePower : LaplaceExpression
    {
        public LaplaceExpression ebase;
        public LaplaceExpression exp;

        public override Complex Evaluate(Complex point)
        {
            return Complex.Pow(ebase.Evaluate(point), exp.Evaluate(point));
        }
    }

    public class LaplaceQuotient : LaplaceExpression
    {
        public LaplaceExpression num;
        public LaplaceExpression denom;

        public override Complex Evaluate(Complex point)
        {
            return num.Evaluate(point) / denom.Evaluate(point);
        }
    }

}
