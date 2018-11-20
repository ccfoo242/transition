using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Common
{
    public struct Point2D
    {
        public double X { get; }
        public double Y { get; }

        public static readonly Point2D Origin = new Point2D(0, 0);
        
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return ((obj is Point2D) && Equals((Point2D)obj));
        }

        public bool Equals(Point2D other)
        {
            return (other.X == this.X) && (other.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Point2D Add(Point2D n1, Point2D n2)
        {
            return new Point2D(n1.X + n2.X, n1.Y + n2.Y);
        }

        public static Point2D Substract(Point2D n1, Point2D n2)
        {
            return new Point2D(n1.X - n2.X, n1.Y - n2.Y);
        }

        public static Point2D Negate(Point2D n)
        {
            return new Point2D(n.X * -1, n.Y * -1);
        }

        public static Point2D operator +(Point2D n1, Point2D n2) { return Add(n1, n2); }
        public static Point2D operator -(Point2D n1, Point2D n2) { return Substract(n1, n2); }

        public static double getDistance(Point2D point1, Point2D point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) +
                             Math.Pow(point1.Y - point2.Y, 2));
        }

        public double getDistance(Point2D otherPoint)
        {
            return getDistance(this, otherPoint);
        }

        public override string ToString()
        {
            string output;
            
            output = "Point" + Environment.NewLine;
            output += "X: " + X.ToString() + Environment.NewLine;
            output += "Y: " + Y.ToString() + Environment.NewLine;

            return output;
        }

    }
}
