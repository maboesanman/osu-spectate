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
                    return new CircularArcCurve(points, length);
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
        float Length;
        PointF Center;
        float StartDegree;
        float NumberOfDegrees; //positive for counter clockwise, negative for clockwise
        float radius;
        public PointF[] approximatePoints;
        public CircularArcCurve(PointF[] points, float l)
        {
            if (points.Length != 3)
            {
                Console.WriteLine("invalid slider curve");
                return;
            }
            Length = l;
            Center = Circumcenter(points[0],points[1],points[2]);
            StartDegree = (float)Math.Atan2(points[0].Y - Center.Y, points[0].X - Center.X);
            float MiddleDegree = (float)Math.Atan2(points[1].Y - Center.Y, points[1].X - Center.X);
            float Temp = StartDegree-MiddleDegree;
            radius = ((new Vector2F(points[0])) - (new Vector2F(Center))).Norm();
            if (Temp < 0)
            {
                Temp = (float)(2 * Math.PI + Temp);
            }
            if (Temp < Math.PI)
            {
                NumberOfDegrees = (float)(l / radius);
            }
            if (Temp >= Math.PI)
            {
                NumberOfDegrees = -(float)(l / radius);
            }
            approximatePoints = new PointF[30];
            for (int i = 0; i < 30; i++)
            {
                approximatePoints[i] = pointOnCurve(1.0f * i / 30.0f);
            }
        }
        public override float getLength()
        {
            return Length;
        }
        public override PointF[] getPoints()
        {
            return approximatePoints;
        }
        public override PointF pointOnCurve(float time)
        {
            Vector2F C = new Vector2F(Center);
            Vector2F Point = new Vector2F((float)Math.Cos(NumberOfDegrees * time + StartDegree), (float)Math.Sin(NumberOfDegrees * time + StartDegree));
            Vector2F P = C + radius * Point;
            return new PointF(P.x, P.y);
        }
        public static PointF Circumcenter(PointF point1, PointF point2, PointF point3)
        {
            Vector2F A = new Vector2F(point1);
            Vector2F B = new Vector2F(point2);
            Vector2F C = new Vector2F(point3);
            float a = (B - C).NormSquared();
            float b = (A - C).NormSquared();
            float c = (A - B).NormSquared();
            Vector2F M = (A * a * (-a + b + c) + B * b * (a - b + c) + C * c * (a + b - c)) / (a * (-a + b + c) + b * (a - b + c) + c * (a + b - c));
            
            return new PointF(M.x, M.y);
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
   class Vector2F
    {
        public float x;
        public float y;
        public Vector2F(float a, float b)
        {
            x = a;
            y = b;
        }
        public Vector2F(PointF p)
        {
            x = p.X;
            y = p.Y;
        }
        public static Vector2F operator *(Vector2F v, float s)
        {
            return new Vector2F(v.x * s, v.y * s);
        }
        public static Vector2F operator /(Vector2F v, float s)
        {
            return new Vector2F(v.x / s, v.y / s);
        }
        public static Vector2F operator *(float s, Vector2F v)
        {
            return new Vector2F(v.x * s, v.y * s);
        }
        public static Vector2F operator +(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2F operator -(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.x - v2.x, v1.y - v2.y);
        }
        public float Dot(Vector2F v)
        {
            return x * v.x + y * v.y;
        }
        public float NormSquared()
        {
            return x * x + y * y;
        }
        public float Norm()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
    }
}
