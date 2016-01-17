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
        public PointF[] controlPoints;
        float[] approximateIntervals;
        PointF[] approximatePoints;
        float actualEndParameter;
        abstract public PointF rawPointOnCurve(float time);
        public PointF pointOnCurve(float time)//adjusted for speed, time is between 0 and 1, where 0 is the start and 1 is the correct length away
        {
            time = Math.Min(time, 1.0f);//force time to be in [0,1]
            time = Math.Max(time, 0.0f);
            float t1 = approximateIntervals[(int)Math.Min(Math.Floor(time * getLength()), approximateIntervals.Length - 1)];
            float t2 = approximateIntervals[(int)Math.Min(Math.Ceiling(time * getLength()), approximateIntervals.Length - 1)];
            if(t1==t2)
            {
                return rawPointOnCurve(t1);
            }
            return rawPointOnCurve(t1+(time-t1)/(t2-t1));
        }
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
        public void ComputePoints()
        {
            /*
            List<float> intervals = new List<float>(0);
            float length = getLength();
            float approximateLength = 0;
            float i1;
            float i2;
            for (int i = 0; i < (int)Math.Ceiling(length); i++)
            {
                i1 = 1.0f * i / (int)Math.Ceiling(length);
                i2 = 1.0f * (i + 1) / (int)Math.Ceiling(length);
                PointF p1 = rawPointOnCurve(i1);
                PointF p2 = rawPointOnCurve(i2);
                intervals.Add(i1);
                approximateLength += (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
                if(approximateLength>length)
                {
                    Vector2F v1 = new Vector2F(p1);
                    Vector2F v2 = new Vector2F(p2);
                    float t1 = approximateLength - (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
                    float t2 = approximateLength;
                    float t3 = length;
                    actualEndParameter = (t3 - t1) + i * 1.0f;
                    
                    intervals.Add(actualEndParameter);
                }
            }
            float[] adjustedIntervals = new float[(int)Math.Floor(getLength())];//problem here... figure out exactly the correct size of the array.
            
            approximateLength = 0;
            int j = 0;
            i1 = intervals.ElementAt(0);
            i2 = intervals.ElementAt(1);
            for (int i = 0; i < adjustedIntervals.Length; i++)//possibly here
            {
                
                PointF p1 = rawPointOnCurve(i1);
                PointF p2 = rawPointOnCurve(i2);
                while (approximateLength<i)
                {
                    i1 = intervals.ElementAt(j);
                    i2 = intervals.ElementAt(j+1);//possibly here
                    p1 = rawPointOnCurve(i1);
                    p2 = rawPointOnCurve(i2);
                    approximateLength += (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
                    j++;
                }
                float t1 = approximateLength - (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
                float t2 = approximateLength;
                float t3 = i*1.0f;
                adjustedIntervals[i] = ((t3 - t1) * i2 + (t2 - t3) * i1) / (t2 - t1);
            }
            */
            approximateIntervals = new float[(int)getLength()*10];
            
            approximatePoints = new PointF[approximateIntervals.Length];
            for (int i = 0; i < approximatePoints.Length; i++)
            {
                
                approximateIntervals[i] = (i * 1.0f / approximatePoints.Length);
            }
            for (int i=0;i<approximatePoints.Length;i++)
            {
                approximatePoints[i] = rawPointOnCurve(approximateIntervals[i]);
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
            time = Math.Min(time, 1.0f);//force time to be in [0,1]
            time = Math.Max(time, 0.0f);
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
            time = Math.Min(time, 1.0f);//force time to be in [0,1]
            time = Math.Max(time, 0.0f);
            if(time == 1.0f)
            {
                return beziers.Last().rawPointOnCurve(1.0f);
            }
            time = time * beziers.Length;
            float p = time % 1.0f;
            int i = (int)time;
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
            t = Math.Min(t, 1.0f);//force time to be in [0,1]
            t = Math.Max(t, 0.0f);

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
