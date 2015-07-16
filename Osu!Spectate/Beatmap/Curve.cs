using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace OsuSpectate.Beatmap
{
    abstract public class Curve
    {
        abstract public PointF pointOnCurve(float time);
        abstract public PointF[] getPoints();
        abstract public float getLength();
        //      abstract public float getStartAngle();
        //      abstract public float getEndAngle();
        public static Curve getCurve(char sliderType, PointF[] points, float length)
        {
            switch (sliderType)
            {
                case ('P')://pass through
                    return new LinearBezierCurve(points, length);
                case ('C')://catmull
                    return new LinearBezierCurve(points, length);
                case ('L')://linear
                    return new LinearBezierCurve(points, length);
                case ('B')://bezier
                    return new LinearBezierCurve(points, length);
                default:
                    Console.WriteLine("curve type not recognized");
                    return null;
            }
        }
    }

    public class CircularArcCurve : Curve
    {
        float length;
        PointF center;
        float startDegree;
        float numberOfDegrees; //positive for counter clockwise, negative for clockwise
        public CircularArcCurve(PointF[] points, float l)
        {
            if (points.Length != 3)
            {
                Console.WriteLine("invalid slider curve");
                return;
            }
            length = l;
            center = new PointF();

        }
        public override float getLength()
        {
            throw new NotImplementedException();
        }
        public override PointF[] getPoints()
        {
            throw new NotImplementedException();
        }
        public override PointF pointOnCurve(float time)
        {
            throw new NotImplementedException();
        }
        public static PointF circumcenter(PointF p, PointF q, PointF r)
        {
            PointF a;
            PointF b;
            PointF c;
            if (Math.Abs(q.Y - r.Y) <= Math.Abs(p.Y - r.Y) && Math.Abs(q.Y - r.Y) <= Math.Abs(p.Y - q.Y))
            {
                a = p;
                b = q;
                c = r;
            }
            else if (Math.Abs(p.Y - r.Y) <= Math.Abs(p.Y - q.Y) && Math.Abs(p.Y - r.Y) <= Math.Abs(q.Y - r.Y))
            {
                a = q;
                b = p;
                c = r;
            }
            else
            {
                a = r;
                b = p;
                c = q;
            }
            float x = (c.Y - b.Y + (a.X * a.X - b.X * b.X) / (b.Y - a.Y) + (c.X * c.X - a.X * a.X) / (c.Y - b.Y)) / (2.0f * ((a.X - b.X) / (b.Y - a.Y) + (c.X - a.X) / (c.Y - b.Y)));
            float y = ((a.X - b.X) / (b.Y - a.Y)) * (x - (a.X + b.X) / (2.0f)) + (a.Y + b.Y) / 2.0f;
            return new PointF(x, y);
        }
    }
    /*
    public class CatmullCurve : Curve
    {
        public CatmullCurve(PointF[] points, float length)
        {

        }
    }
    */
    public class LinearBezierCurve : Curve
    {
        LinearBezierSegment[] beziers;
        PointF[] controlPoints;
        float length;
        public PointF[] approximatePoints;
        public LinearBezierCurve(PointF[] points, float l)
        {
            controlPoints = points;
            length = l;
            List<LinearBezierSegment> bezierList = new List<LinearBezierSegment>();
            int startIndex = 0;
            for (int i = 1; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].Equals(controlPoints[i - 1]))
                {
                    PointF[] temp = new PointF[i - startIndex];
                    for (int j = startIndex; j < i; j++)
                    {
                        temp[j - startIndex] = controlPoints[j];
                    }
                    bezierList.Add(new LinearBezierSegment(temp));
                    startIndex = i;
                }
            }
            PointF[] last = new PointF[controlPoints.Length - startIndex];
            for (int j = startIndex; j < controlPoints.Length; j++)
            {
                last[j - startIndex] = controlPoints[j];
            }
            bezierList.Add(new LinearBezierSegment(last));
            beziers = bezierList.ToArray();
            approximatePoints = new PointF[30];
            for (int i = 0; i < 30; i++)
            {
                approximatePoints[i] = pointOnCurve(1.0f * i / 30.0f);
            }
        }
        override public float getLength()
        {
            return length;
        }
        public override PointF pointOnCurve(float time)
        {
            float unmodifiedLength = 0.0f;
            for (int i = 0; i < beziers.Length; i++)
            {
                unmodifiedLength += beziers[i].getLength();
            }
            float totalDistance = 0.0f;
            for (int i = 0; i < beziers.Length; i++)
            {
                if (totalDistance + beziers[i].getLength() > time * length)
                {
                    return beziers[i].pointOnCurve((time * length - unmodifiedLength) / beziers[i].getLength());
                }
                totalDistance += beziers[i].getLength();
            }
            return beziers.First().pointOnCurve(0.0f);
        }
        public override PointF[] getPoints()
        {
            return approximatePoints;
        }
    }

    //copied from opsu!

    public class LinearBezierSegment : Curve
    {
        public PointF[] approximatePoints;
        private PointF[] controlPoints;
        private float length;
        public LinearBezierSegment(PointF[] points)
        {
            this.controlPoints = points;

            // approximate by finding the length of all points
            // (which should be the max possible length of the curve)
            float approxlength = 0;
            for (int i = 0; i < points.Length - 1; i++)
                approxlength += (float)Math.Sqrt((points[i].X - points[i + 1].X) * (points[i].X - points[i + 1].X) + (points[i].Y - points[i + 1].Y) * (points[i].Y - points[i + 1].Y));
            approximatePoints = new PointF[30];
            for (int i = 0; i < 30; i++)
            {
                approximatePoints[i] = pointOnCurve(1.0f * i / 30.0f);
            }
            length = 0.0f;
            for (int i = 0; i < approximatePoints.Length - 1; i++)
            {
                length += (float)Math.Sqrt((approximatePoints[i].X - approximatePoints[i + 1].X) * (approximatePoints[i].X - approximatePoints[i + 1].X) + (approximatePoints[i].Y - approximatePoints[i + 1].Y) * (approximatePoints[i].Y - approximatePoints[i + 1].Y));
            }
        }
        override public float getLength()
        {


            //    Console.WriteLine(length);
            return length;
        }
        public override PointF[] getPoints()
        {
            return approximatePoints;
        }



        override public PointF pointOnCurve(float t)
        {
            PointF c = new PointF();
            int n = controlPoints.Length - 1;
            for (int i = 0; i <= n; i++)
            {
                double b = bernstein(i, n, t);
                c.X += (float)(controlPoints[i].X * b);
                c.Y += (float)(controlPoints[i].Y * b);
            }

            return c;
        }

        /**
         * Calculates the binomial coefficient.
         * http://en.wikipedia.org/wiki/Binomial_coefficient#Binomial_coefficient_in_programming_languages
         */
        public static long binomialCoefficient(int N, int K)
        {
            // This function gets the total number of unique combinations based upon N and K.
            // N is the total number of items.
            // K is the size of the group.
            // Total number of unique combinations = N! / ( K! (N - K)! ).
            // This function is less efficient, but is more likely to not overflow when N and K are large.
            // Taken from:  http://blog.plover.com/math/choose.html
            //
            long r = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        /**
         * Calculates the Bernstein polynomial.
         * @param i the index
         * @param n the degree of the polynomial (i.e. number of points)
         * @param t the t value [0, 1]
         */
        private static double bernstein(int i, int n, float t)
        {
            return binomialCoefficient(n, i) * Math.Pow(t, i) * Math.Pow(1 - t, n - i);
        }
    }
}
