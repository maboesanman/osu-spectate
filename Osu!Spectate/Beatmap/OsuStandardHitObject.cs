using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Drawing;
using System.Drawing.Imaging;

namespace OsuSpectate.Beatmap
{
    public abstract class OsuStandardHitObject
    {
        public abstract OsuStandardBeatmap getBeatmap();
        public abstract string getType();
        public abstract TimeSpan getStart();
        public abstract TimeSpan getEnd();
        public abstract void flipY();
        public static OsuStandardHitObject getNewHitobject(string[] split, int comboIndex, int comboNumber, OsuStandardBeatmap b)
        {
            int x = Int32.Parse(split[3]);
            int y=-1;
            if ((x & 1) == 1) { y = 0; }
            if ((x & 2) == 2) { y = 1; }
            if ((x & 8) == 8) { y = 2; }
            switch (y)
            {
                case 0:
                    return new OsuStandardHitCircle(split, comboIndex, comboNumber, b);
                case 1:
                    return new OsuStandardSlider(split, comboIndex, comboNumber, b);
                case 2:
                    return new OsuStandardSpinner(split, comboIndex, comboNumber, b);
                default:
                    Console.WriteLine("wut " + split[3]);
                    return null;

            }
        }
    }
    public class OsuStandardHitCircle : OsuStandardHitObject
    {//TODO
        public OsuStandardBeatmap beatmap;
        public int x;
        public int y;
        public TimeSpan time; //milliseconds
        public bool newCombo;
        public bool hitsoundWhistle;
        public bool hitsoundFinish;
        public bool hitsoundClap;
        public int[] addition;
        public int comboNumber;
        public int comboIndex;
        public OsuStandardHitCircle(string[] s, int comboI, int comboN, OsuStandardBeatmap b)
        {
            comboNumber = comboN;
            comboIndex = comboI;
            beatmap = b;
            x = int.Parse(s[0]);
            y = int.Parse(s[1]);
            time = new TimeSpan(TimeSpan.TicksPerMillisecond * long.Parse(s[2]));
            newCombo = (s[3] == "5");


        }
        public override string getType() { return "hitcircle"; }
        public override TimeSpan getStart() { return time; }
        public override TimeSpan getEnd() { return time; }
        public override OsuStandardBeatmap getBeatmap() { return beatmap; }
        public override void flipY()
        {
            y = 384 - y;
        }
    }
    public class OsuStandardSlider : OsuStandardHitObject
    {//TODO
        public OsuStandardBeatmap beatmap;
        public int x;
        public int y;
        public float[] xList;
        public float[] yList;
        public PointF[] points;
        public TimeSpan startTime;

        public bool newCombo;
        public bool hitsoundWhistle;
        public bool hitsoundFinish;
        public bool hitsoundClap;

        public float velocity;
        public Curve curve;
        public char sliderType;
        public int repeat;
        public float pixelLength;
        public byte[] edgeHitSound;
        public byte[][] edgeAddition;

        public int comboNumber;
        public int comboIndex;
        public OsuStandardSlider(string[] tokens, int comboI, int comboN, OsuStandardBeatmap b)
        {
            beatmap = b;
            comboNumber = comboN;
            comboIndex = comboI;

            x = int.Parse(tokens[0]);
            y = int.Parse(tokens[1]);
            startTime = new TimeSpan(TimeSpan.TicksPerMillisecond * long.Parse(tokens[2]));
            newCombo = (tokens[3] == "5");


            String[] sliderTokens = tokens[5].Split(new char[2] { '\\', '|' });
            this.sliderType = sliderTokens[0].ElementAt(0);
            this.xList = new float[sliderTokens.Length];
            this.yList = new float[sliderTokens.Length];
            points = new PointF[sliderTokens.Length];
            for (int j = 1; j < sliderTokens.Length; j++)
            {
                String[] sliderXY = sliderTokens[j].Split(':');
                this.xList[j] = float.Parse(sliderXY[0]);
                this.yList[j] = float.Parse(sliderXY[1]);
                this.points[j] = new PointF(this.xList[j], this.yList[j]);
            }
            xList[0] = x;
            yList[0] = y;
            points[0] = new PointF(x, y);
            this.repeat = int.Parse(tokens[6]);
            this.pixelLength = float.Parse(tokens[7]);
            if (tokens.Length > 8)
            {
                String[] edgeHitSoundTokens = tokens[8].Split(new char[2] { '\\', '|' });
                this.edgeHitSound = new byte[edgeHitSoundTokens.Length];
                for (int j = 0; j < edgeHitSoundTokens.Length; j++)
                    edgeHitSound[j] = byte.Parse(edgeHitSoundTokens[j]);
            }
            if (tokens.Length > 9)
            {
                String[] edgeAdditionTokens = tokens[9].Split(new char[2] { '\\', '|' });
                this.edgeAddition = new byte[edgeAdditionTokens.Length][];
                for (int j = 0; j < edgeAdditionTokens.Length; j++)
                {
                    String[] tedgeAddition = edgeAdditionTokens[j].Split(':');
                    edgeAddition[j] = new byte[2];
                    edgeAddition[j][0] = byte.Parse(tedgeAddition[0]);
                    edgeAddition[j][1] = byte.Parse(tedgeAddition[1]);
                }
            }
            curve = Curve.getCurve(sliderType, points, pixelLength);
        }
        public override string getType() { return "slider"; }
        public override TimeSpan getStart() { return startTime; }
        public override TimeSpan getEnd() { return startTime; }//TODO
        public override OsuStandardBeatmap getBeatmap() { return beatmap; }
        public override void flipY()
        {
            for (int i = 0; i < yList.Length; i++)
            {
                points[i].Y = 348.0f - points[i].Y;
                yList[i] = 384 - yList[i];
            }
            y = 384 - y;
            curve = Curve.getCurve(sliderType, points, pixelLength);
        }
    }
    public class OsuStandardSpinner : OsuStandardHitObject
    {//TODO
        public OsuStandardBeatmap beatmap;
        public TimeSpan startTime;
        public TimeSpan endTime;
        public bool hitsoundWhistle;
        public bool hitsoundFinish;
        public bool hitsoundClap;
        public int[] addition;
        public int comboNumber;
        public int comboIndex;
        public OsuStandardSpinner(string[] s, int comboI, int comboN, OsuStandardBeatmap b)
        {
            comboNumber = comboN;
            comboIndex = comboI;

            beatmap = b;
            startTime = new TimeSpan(TimeSpan.TicksPerMillisecond * long.Parse(s[2]));
            endTime = new TimeSpan(TimeSpan.TicksPerMillisecond * long.Parse(s[5]));
        }
        public override string getType() { return "spinner"; }
        public override TimeSpan getStart() { return startTime; }
        public override TimeSpan getEnd() { return endTime; }
        public override OsuStandardBeatmap getBeatmap() { return beatmap; }
        public override void flipY() { }
    }
}
