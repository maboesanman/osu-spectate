using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OsuSpectate;

namespace OsuSpectate.Beatmap
{
    abstract public class Curve
    {
        public PointF[] controlPoints;
        float[] approximateIndices;
        PointF[] approximatePoints;
        float actualEndParameter;
        int repeatCount;
        abstract public PointF rawPointOnCurve(float time);
        public PointF pointOnCurve(float time)//adjusted for speed, time is between 0 and 1, where 0 is the start and 1 is the correct length away
        {
            time = time * getLength();
            time = time % (2.0f * getLength() / repeatCount);
            time = getLength() / repeatCount - Math.Abs(time - getLength() / repeatCount);
            float currentLength = 0.0f;
            PointF p1;
            PointF p2;
            int i = 0;
            while (currentLength < time&&i<approximateIndices.Length-2)
            {
                p1 = rawPointOnCurve(approximateIndices[i]);
                p2 = rawPointOnCurve(approximateIndices[i + 1]);
                currentLength += Computation.Distance(p1, p2);
                i++;
            }
            float i1 = approximateIndices[i];
            float i2 = approximateIndices[i+1];
            float d1 = currentLength - Computation.Distance(rawPointOnCurve(approximateIndices[i+1]), rawPointOnCurve(approximateIndices[i]));
            float d2 = currentLength;
            float d3 = time;
            return rawPointOnCurve((d3 - d1) / (d2 - d1) * (i2 - i1) + i1);
        }
        abstract public float getLength();
        //      abstract public float getStartAngle();
        //      abstract public float getEndAngle();
        public static Curve getCurve(char sliderType, PointF[] points, float length, int repeat)
        {
            /*
            Console.Write(sliderType + " ");
            for(int i=0;i<points.Length;i++)
            {
                Console.Write("(" + points[i].X + ", " + points[i].Y + "), ");
            }
            Console.WriteLine(length);
            */
            Curve c;
            switch (sliderType)
            {
                case ('P')://pass through
                    c = new CircularArcCurve(points, length);
                    break;
                case ('C')://catmull
                    c = new LinearBezierCurve(points, length);
                    break;
                case ('L')://linear
                    c = new LinearBezierCurve(points, length);
                    break;
                case ('B')://bezier
                    c = new LinearBezierCurve(points, length);
                    break;
                default:
                    Console.WriteLine("curve type not recognized");
                    return null;
            }
            c.repeatCount = repeat;
            return c;
        }
        public void ComputePoints()
        {
            List<float> indices = new List<float>(0);
            float currentLength = 0.0f;
            float currentIndex = 0.0f;
            float indexDifference=0.0f;
            PointF p1 = rawPointOnCurve(currentIndex);
            PointF p2;
            indices.Add(currentIndex);
            while(currentLength<getLength())
            {
                indexDifference = 0.1f;
                p2 = rawPointOnCurve(currentIndex + indexDifference);
                while (Computation.Distance(p1,p2)>.33f)
                {
                    indexDifference = indexDifference / 2.0f;
                    p2 = rawPointOnCurve(currentIndex + indexDifference);
                }//now p1 and p2 are less than .33 apart
                currentLength += Computation.Distance(p1, p2);
                currentIndex += indexDifference;
                p1 = rawPointOnCurve(currentIndex);
                indices.Add(currentIndex);
                //Console.WriteLine("found a point "+indexDifference+" "+currentIndex+" "+currentLength);
            }
            float i1 = currentIndex - indexDifference;
            float i2 = currentIndex;
            float d1 = currentLength - Computation.Distance(rawPointOnCurve(indices.ElementAt(indices.Count - 1)), rawPointOnCurve(indices.ElementAt(indices.Count - 2)));
            float d2 = currentLength;
            float d3 = getLength();
            indices.Add((d3 - d1) / (d2 - d1) * (i2 - i1) + i1);

            approximateIndices = indices.ToArray();
            approximatePoints = new PointF[indices.Count];
            for (int i=0;i<approximatePoints.Length;i++)
            {
                approximatePoints[i] = rawPointOnCurve(indices.ElementAt(i));
            }

        }

        public PointF[] getPoints()
        {
            return approximatePoints;
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
            controlPoints = points;
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
                NumberOfDegrees = -(float)(Length / radius);
            }
            if (Temp >= Math.PI)
            {
                NumberOfDegrees = (float)(Length / radius);
            }
            ComputePoints();
            
        }
        public override float getLength()
        {
            return Length;
        }
        
        public override PointF rawPointOnCurve(float time)
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
        
        float length;
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
            ComputePoints();

        }
        override public float getLength()
        {
            return length;
        }
        public override PointF rawPointOnCurve(float time)
        {
            //time = Math.Min(time, 1.0f);//force time to be in [0,1]
            //time = Math.Max(time, 0.0f);
            if(time >= 1.0f)
            {
                return beziers.Last().rawPointOnCurve(time);
            }
            if (time <= 0.0f)
            {
                return beziers.First().rawPointOnCurve(time);
            }
            time = time * beziers.Length;
            
            float p = time % 1.0f;
            
            int i = (int)time;
            i = Math.Min(i, beziers.Count()-1);
            i = Math.Max(0, i);
            return beziers.ElementAt(i).rawPointOnCurve(p);
        }
        
    }

    //copied from opsu!

    public class LinearBezierSegment : Curve
    {
        public PointF[] approximatePoints;
        public LinearBezierSegment(PointF[] points)
        {
            this.controlPoints = points;
        }
        override public float getLength()
        { 
            return 1.0f;
        }
        override public PointF rawPointOnCurve(float t)
        {
            //t = Math.Min(t, 1.0f);//force time to be in [0,1]
            //t = Math.Max(t, 0.0f);

            
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
