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

        public override Complex Calculate(double point)
        {
            return Expression.Evaluate(point);
        }

        public Complex Calculate(Complex point)
        {
            return Expression.Evaluate(point);
        }
    }

    public class StandardTransferFunction : LaplaceFunction
    {
        public EngrNumber Ao;
        public EngrNumber Fp;
        public EngrNumber Fz;
        public EngrNumber Qp;
        public EngrNumber Qz;
        public bool invert;
        public bool reverse;

        public EngrNumber Wp { get => 2 * Math.PI * Fp; }
        public EngrNumber Wz { get => 2 * Math.PI * Fz; }

        public static LaplaceExpression LP1
            (EngrNumber Ao, EngrNumber Fp, EngrNumber Qp, EngrNumber Fz, EngrNumber Qz) =>
                new LaplaceQuotient()
                {
                    num = new LaplaceConstant() { ConstantValue = Ao.ValueDouble },
                    denom = new LaplaceSum(new LaplaceConstant() { ConstantValue = 1 },
                    new LaplaceQuotient()
                    {
                        num = new LaplaceLiteral(),
                        denom = new LaplaceConstant()
                            { ConstantValue = 2 * Math.PI * Fp.ValueDouble }
                    })
                };


        public static LaplaceExpression HP1
            (EngrNumber Ao, EngrNumber Fp, EngrNumber Qp, EngrNumber Fz, EngrNumber Qz) =>
            new LaplaceProduct(new LaplaceConstant() { ConstantValue = Ao.ValueDouble },
                new LaplaceQuotient()
                {
                    num = new LaplaceQuotient()
                    {
                        num = new LaplaceLiteral(),
                        denom = new LaplaceConstant() { ConstantValue = 2 * Math.PI * Fp.ValueDouble }
                    },
                    denom = new LaplaceSum(new LaplaceConstant() { ConstantValue = 1 },
                        new LaplaceQuotient()
                        {
                            num = new LaplaceLiteral(),
                            denom = new LaplaceConstant() { ConstantValue = 2 * Math.PI * Fp.ValueDouble }
                        })
                });
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
