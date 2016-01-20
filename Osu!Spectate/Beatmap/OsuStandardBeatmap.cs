using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace OsuSpectate.Beatmap
{
    public class OsuStandardBeatmap
    {
        

        string OsuFileFormat;
        int BackgroundTexture;
        int BackgroundTextureWidth;
        int BackgroundTextureHeight;
        string BackgroundFilePath;
        string AudioFilePath;
        //GENERAL
        string AudioFilename;       //AudioFilename (String) specifies the location of the audio file relative to the current folder.
        long AudioLeadIn;           //AudioLeadIn (Integer, milliseconds) is the amount of time added before the audio file begins playing. Useful for audio files that begin immediately.
        long PreviewTime;           //PreviewTime (Integer, milliseconds) defines when the audio file should begin playing when selected in the song selection menu.
        bool Countdown;             //Countdown (Boolean) specifies whether or not a countdown occurs before the first hit object appears.
        string SampleSet;           //SampleSet (String) specifies which set of hit sounds will be used throughout the beatmap.
        float StackLeniency;        //StackLeniency (Float) is how often closely placed hit objects will be stacked together.
        int Mode;                   //Mode (Integer) defines the game mode of the beatmap. (0=osu!, 1=Taiko, 2=Catch the Beat, 3=osu!mania)
        bool LetterboxInBreaks;     //LetterboxInBreaks (Boolean) specifies whether the letterbox appears during breaks.
        bool WidescreenStoryboard;  //WidescreenStoryboard (Boolean) specifies whether or not the storyboard should be widescreen.
        //EDITOR
        int[] Bookmarks;            //Bookmarks (Integer List, milliseconds) is a list of comma-separated times of editor bookmarks.
        float DistanceSpacing;      //DistanceSpacing (Float) is a multiplier for the "Distance Snap" feature.
        int BeatDivisor;            //BeatDivisor (Integer) specifies the beat division for placing objects.
        int Gridsize;               //GridSize (Integer) specifies the size of the grid for the "Grid Snap" feature.
        float TimelineZoom;           //TimelineZoom (Integer) specifies the zoom in the editor timeline.
        //METADATA
        string Title;               //Title (String) is the title of the song limited to ASCII characters.
        string TitleUnicode;        //TitleUnicode (String) is the title of the song with unicode support. If not present, Title is used.
        string Artist;              //Artist (String) is the name of the song's artist limited to ASCII characters.
        string ArtistUnicode;       //ArtistUnicode (String) is the name of the song's artist with unicode support. If not present, Artist is used.
        string Creator;             //Creator (String) is the username of the mapper.
        string Version;//difficulty //Version (String) is the name of the beatmap's difficulty.
        string Source;              //Source (String) describes the origin of the song.
        string[] Tags;              //Tags (String List) is a collection of words describing the song. Tags are searchable in both the online listings and in the song selection menu.
        int BeatmapID;              //BeatmapID (Integer) is the ID of the single beatmap.
        int BeatmapSetID;           //BeatmapSetID (Integer) is the ID of the beatmap set.
        //DIFFICULTY
        float HPDrainRate;          //HPDrainRate (Float) specifies the HP Drain difficulty.
        float CircleSize;           //CircleSize (Float) specifies the size of hit object circles.
        float OverallDifficulty;    //OverallDifficulty (Float) specifies the amount of time allowed to click a hit object on time.
        float ApproachRate;         //ApproachRate (Float) specifies the amount of time taken for the approach circle and hit object to appear.
        float SliderMultiplier;     //SliderMultiplier (Float) specifies a multiplier for the slider velocity. Default value is 1.
        float SliderTickRate;       //SliderTickRate (Float) specifies how often slider ticks appear. Default value is 1.
        //EVENTS
        //storyboard stuff i guess...
        //TIMING POINTS
        //Timing Points describe a number of properties regarding offsets, beats per minute and hit sounds. Offset (Integer, milliseconds) defines when the Timing Point takes effect. Milliseconds per Beat (Float) defines the beats per minute of the song. For certain calculations, it is easier to use milliseconds per beat. Meter (Integer) defines the number of beats in a measure. Sample Type (Integer) defines the type of hit sound samples that are used. Sample Set (Integer) defines the set of hit sounds that are used. Volume (Integer) is a value from 0 - 100 that defines the volume of hit sounds. Kiai Mode (Boolean) defines whether or not Kiai Time effects are active. Inherited (Boolean) defines whether or not the Timing Point is an inherited Timing Point.
        List<TimingPoint> TimingPointList;
        List<TimingPoint> NonInheritedTimingPointList;
        //COLORS
        Color[] ComboColors;        //Combo# (Integer List) is a list of three numbers, each from 0 - 255 defining an RGB color.
        //HIT OBJECTS
        private List<OsuStandardHitObject> HitObjectList;
        Dictionary<int, int?> sliderTexturesNomod;
        Dictionary<int, int?> sliderTexturesHardrock;
        Dictionary<int, int?> sliderTexturesEasy;


        public OsuStandardBeatmap(string path)
        {
            TimingPointList = new List<TimingPoint>(0);
            NonInheritedTimingPointList = new List<TimingPoint>(0);
            string BackgroundFileName = null;
            string line;
            string title;
            string value;
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '[')
                    {
                        switch (line)
                        {
                            case "[General]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(':'))
                                        {
                                            title = line.Split(':')[0].Trim();
                                            value = line.Split(':')[1].Trim();
                                            switch (title)
                                            {
                                                case ("AudioFilename"):
                                                    AudioFilename = value;
                                                    break;
                                                case ("AudioLeadIn"):
                                                    AudioLeadIn = int.Parse(value);
                                                    break;
                                                case ("PreviewTime"):
                                                    PreviewTime = int.Parse(value);
                                                    break;
                                                case ("CountDown"):
                                                    if (value == "0")
                                                    {
                                                        Countdown = false;
                                                    }
                                                    else
                                                    {
                                                        Countdown = true;
                                                    }
                                                    break;
                                                case ("SampleSet"):
                                                    SampleSet = value;
                                                    break;
                                                case ("StackLeniency"):
                                                    StackLeniency = float.Parse(value);
                                                    break;
                                                case ("Mode"):
                                                    Mode = int.Parse(value);
                                                    break;
                                                case ("LetterboxInBreaks"):
                                                    if (value == "0")
                                                    {
                                                        LetterboxInBreaks = false;
                                                    }
                                                    else
                                                    {
                                                        LetterboxInBreaks = true;
                                                    }
                                                    break;
                                                case ("WidescreenStoryboard"):
                                                    if (value == "0")
                                                    {
                                                        WidescreenStoryboard = false;
                                                    }
                                                    else
                                                    {
                                                        WidescreenStoryboard = true;
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "[Editor]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(':'))
                                        {
                                            title = line.Split(':')[0].Trim();
                                            value = line.Split(':')[1].Trim();
                                            switch (title)
                                            {
                                                case ("Bookmarks")://TODO
                                                    string[] temp = value.Split(',');
                                                    Bookmarks = new int[temp.Length];
                                                    for (int i = 1; i < temp.Length; i++)
                                                    {
                                                        Bookmarks[i] = int.Parse(temp[i]);
                                                    }
                                                    break;
                                                case ("DistanceSpacing"):
                                                    DistanceSpacing = float.Parse(value);
                                                    break;
                                                case ("BeatDivisor"):
                                                    BeatDivisor = int.Parse(value);
                                                    break;
                                                case ("GridSize"):
                                                    Gridsize = int.Parse(value);
                                                    break;
                                                case ("TimelineZoom"):
                                                    TimelineZoom = float.Parse(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "[Metadata]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(':'))
                                        {
                                            title = line.Split(':')[0].Trim();
                                            value = line.Split(':')[1].Trim();
                                            switch (title)
                                            {
                                                case ("Title"):
                                                    Title = value;
                                                    break;
                                                case ("TitleUnicode"):
                                                    TitleUnicode = value;
                                                    break;
                                                case ("Artist"):
                                                    Artist = value;
                                                    break;
                                                case ("ArtistUnicode"):
                                                    ArtistUnicode = value;
                                                    break;
                                                case ("Creator"):
                                                    Creator = value;
                                                    break;
                                                case ("Version"):
                                                    Version = value;
                                                    break;
                                                case ("Source"):
                                                    Source = value;
                                                    break;
                                                case ("Tags"):
                                                    Tags = value.Split(' ');
                                                    break;
                                                case ("BeatmapID"):
                                                    BeatmapID = int.Parse(value);
                                                    break;
                                                case ("BeatmapSetID"):
                                                    BeatmapSetID = int.Parse(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "[Difficulty]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(':'))
                                        {
                                            title = line.Split(':')[0].Trim();
                                            value = line.Split(':')[1].Trim();
                                            switch (title)
                                            {
                                                case ("HPDrainRate"):
                                                    HPDrainRate = float.Parse(value);
                                                    break;
                                                case ("CircleSize"):
                                                    CircleSize = float.Parse(value);
                                                    break;
                                                case ("OverallDifficulty"):
                                                    OverallDifficulty = float.Parse(value);
                                                    break;
                                                case ("ApproachRate"):
                                                    ApproachRate = float.Parse(value);
                                                    break;
                                                case ("SliderMultiplier"):
                                                    SliderMultiplier = float.Parse(value);
                                                    break;
                                                case ("SliderTickRate"):
                                                    SliderTickRate = float.Parse(value);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "[Events]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Count() > 1)
                                        {
                                            if (line.Trim().Substring(0, 2) != "//" && BackgroundFileName == null)
                                            {
                                                BackgroundFileName = line.Split(',')[2].Split('"')[1];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                break;
                            case "[TimingPoints]":
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        string[] args = line.Split(',');
                                        if(args.Length<8)
                                        {
                                            break;
                                        }
                                        TimingPoint p = new TimingPoint(line);
                                        TimingPointList.Add(p);
                                        if(!p.Inherited)
                                        {
                                            NonInheritedTimingPointList.Add(p);
                                        }
                                    }
                                }
                                break;
                            case "[Colours]":
                                List<Color> TempColorList = new List<Color>();
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(':'))
                                        {
                                            value = line.Split(':')[1].Trim();
                                            string[] Split = value.Split(',');
                                            TempColorList.Add(Color.FromArgb(255, int.Parse(Split[0]), int.Parse(Split[1]), int.Parse(Split[2])));
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                ComboColors = TempColorList.ToArray();
                                break;
                            case "[HitObjects]":
                                HitObjectList = new List<OsuStandardHitObject>();
                                int currentComboIndex = 0;
                                int currentComboNumber = 1;
                                while (file.Peek() != '[')
                                {
                                    if ((line = file.ReadLine()) != null)
                                    {
                                        if (line.Contains(','))
                                        {
                                            string[] split = line.Split(',');
                                            int x = Int32.Parse(split[3]);
                                            if ((x & 4) == 4) 
                                            {
                                                currentComboIndex++;
                                                currentComboNumber = 1;
                                            }
                                            HitObjectList.Add(OsuStandardHitObject.getNewHitobject(split, currentComboIndex, currentComboNumber, this, HitObjectList.Count));
                                            currentComboNumber++;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                break;

                            default:
                                Console.WriteLine("other");
                                break;
                        }
                    }
                }
            }
            string BeatmapDirectory = path;
            string[] temp2 = path.Split('\\');
            BeatmapDirectory = BeatmapDirectory.Substring(0, path.Count() - temp2[temp2.Length - 1].Count());
            AudioFilePath = BeatmapDirectory + AudioFilename;
            BackgroundFilePath = BeatmapDirectory + BackgroundFileName;
            BackgroundTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, BackgroundTexture);
            Bitmap bmp = new Bitmap(BackgroundFilePath);
            BackgroundTextureWidth = bmp.Width;
            BackgroundTextureHeight = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            HitObjectList.RemoveAll(item => item == null);
            TimingPointList.Sort();
            NonInheritedTimingPointList.Sort();
            sliderTexturesNomod = new Dictionary<int, int?>(0);
            sliderTexturesHardrock = new Dictionary<int, int?>(0);
            sliderTexturesEasy = new Dictionary<int, int?>(0);
            for(int i = 0; i < HitObjectList.Count; i++)
            {
                if(HitObjectList.ElementAt(i).getType()=="slider")
                {
                    ((OsuStandardSlider)HitObjectList.ElementAt(i)).updateTiming();
                    sliderTexturesNomod.Add(HitObjectList.ElementAt(i).getId(), null);
                    sliderTexturesHardrock.Add(HitObjectList.ElementAt(i).getId(), null);
                    sliderTexturesEasy.Add(HitObjectList.ElementAt(i).getId(), null);
                }
            }
            Console.WriteLine("finished loading beatmap: " + Title);
        }
        public int GetSliderTexture(OsuStandardSlider slider, ReplayAPI.Mods mods)
        {
            if ((mods & ReplayAPI.Mods.HardRock) == ReplayAPI.Mods.HardRock) 
            {
                if (!sliderTexturesHardrock[slider.getId()].HasValue)
                {
                    sliderTexturesHardrock[slider.getId()] = GenerateSliderTexture(slider, mods);
                }
                return sliderTexturesHardrock[slider.getId()].Value;
            }
            if ((mods & ReplayAPI.Mods.Easy) == ReplayAPI.Mods.Easy)
            {
                if (!sliderTexturesEasy[slider.getId()].HasValue)
                {
                    sliderTexturesEasy[slider.getId()] = GenerateSliderTexture(slider, mods);
                }
                return sliderTexturesEasy[slider.getId()].Value;
            }
            if (!sliderTexturesNomod[slider.getId()].HasValue)
            {
                sliderTexturesNomod[slider.getId()] = GenerateSliderTexture(slider, mods);
            }
            return sliderTexturesNomod[slider.getId()].Value;
        }
        public int GenerateSliderTexture(OsuStandardSlider slider, ReplayAPI.Mods mods)
        {
            
            float scale = 1.5f;
            Bitmap SliderBMP = new Bitmap((int)(((slider.curve.maxX-slider.curve.minX)+2.0f*GetCSRadius(mods)) * scale), (int)(((slider.curve.maxY - slider.curve.minY) + 2.0f * GetCSRadius(mods)) * scale));
            Graphics SliderGFX = Graphics.FromImage(SliderBMP);
            SliderGFX.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            SliderGFX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            SliderGFX.Clear(Color.Transparent);
            float cs = GetCSRadius(mods);

            Pen backPen = new Pen(Color.White, cs * 1.8f * scale);
            backPen.LineJoin = LineJoin.Round;
            backPen.EndCap = LineCap.Round;
            backPen.StartCap = LineCap.Round;

            Pen frontPen = new Pen(Color.Black, cs * 1.6f * scale);
            frontPen.LineJoin = LineJoin.Round;
            frontPen.EndCap = LineCap.Round;
            frontPen.StartCap = LineCap.Round;

            GraphicsPath path = new GraphicsPath();

            
            path.AddLines(slider.curve.getPoints());
            Matrix m = new Matrix();
            m.Scale(scale, scale);
            m.Translate(-(slider.curve.minX - GetCSRadius(mods)), -(slider.curve.minY - GetCSRadius(mods)));
            path.Transform(m);
            SliderGFX.DrawPath(backPen, path);
            SliderGFX.DrawPath(frontPen, path);
            SliderBMP.MakeTransparent(Color.Black);
            BitmapData Data = SliderBMP.LockBits(new Rectangle(0, 0, SliderBMP.Width, SliderBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            SliderBMP.UnlockBits(Data);
            SliderGFX.ReleaseHdc(SliderGFX.GetHdc());
            SliderGFX.Flush();
            SliderGFX.Dispose();
            
            int ID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, ID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            


            SliderBMP.Dispose();
            
            return ID;
        }
        public TimeSpan GetOD300Milliseconds(ReplayAPI.Mods mods)
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - OverallDifficulty * 6.0f)));
        }
        public TimeSpan GetOD100Milliseconds(ReplayAPI.Mods mods)
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - OverallDifficulty * 8.0f)));
        }
        public TimeSpan GetOD50Milliseconds(ReplayAPI.Mods mods)
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - OverallDifficulty * 10.0f)));
        }
        public TimeSpan GetARMilliseconds(ReplayAPI.Mods mods)
        {
            if ((mods & ReplayAPI.Mods.HardRock) == ReplayAPI.Mods.HardRock)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(ApproachRate * 1.4f, 10.0f)) * 120.0f) - (Math.Max((Math.Min(ApproachRate * 1.4f, 10.0f) - 5.0f) * 30.0f, 0.0f)))));
            }
            if ((mods & ReplayAPI.Mods.Easy) == ReplayAPI.Mods.Easy)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(ApproachRate * 0.5f, 10.0f)) * 120.0f) - (Math.Max((ApproachRate*0.5f - 5.0f) * 30.0f, 0.0f)))));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (ApproachRate) * 120.0f) - (Math.Max((ApproachRate - 5.0f) * 30.0f, 0.0f)))));
        }
        public float GetCSRadius(ReplayAPI.Mods mods)
        {
            if ((mods & ReplayAPI.Mods.HardRock) == ReplayAPI.Mods.HardRock)
            {
                return 4.0f * (12.0f - Math.Min(CircleSize * 1.3f, 10.0f));
            }
            if ((mods & ReplayAPI.Mods.Easy) == ReplayAPI.Mods.Easy)
            {
                return 4.0f * (12.0f - CircleSize*0.5f);
            }
            return 4.0f * (12.0f - CircleSize);
        }
        public int GetHitObjectCount()
        {
            return HitObjectList.Count;
        }
        public OsuStandardHitObject GetHitObject(int i, ReplayAPI.Mods mods)
        {
            if ((mods & ReplayAPI.Mods.HardRock) == ReplayAPI.Mods.HardRock)
            {
                return HitObjectList.ElementAt(i).flipY();
            } else
            {
                return HitObjectList.ElementAt(i);
            }
        }
        public TimeSpan GetSliderDuration(TimeSpan time, OsuStandardSlider slider)
        {
            
            int index1=0;
            while(TimingPointList.ElementAt(index1).Offset.CompareTo(time)<=0)
            {
                index1++;
                if (index1 >= TimingPointList.Count)
                {break;}
            }
            TimingPoint inherited = TimingPointList.ElementAt(index1-1);
            int index2 = 0;
            while (NonInheritedTimingPointList.ElementAt(index2).Offset.CompareTo(time) <= 0)
            {
                index2++;
                if(index2>=NonInheritedTimingPointList.Count)
                {break;}
            }
            TimingPoint parent = NonInheritedTimingPointList.ElementAt(index2-1);
            if(!inherited.Inherited)
            {
                return TimeSpan.FromMilliseconds(slider.pixelLength * slider.repeat / 100.0f * parent.BeatLength.TotalMilliseconds / SliderMultiplier);
            } else
            {
                return TimeSpan.FromMilliseconds(slider.pixelLength * slider.repeat / 100.0f * parent.BeatLength.TotalMilliseconds / SliderMultiplier* (-0.01f * inherited.BeatLength.TotalMilliseconds));
            }
        }
        
        #region getters
        public int GetBackgroundTexture() { return BackgroundTexture; }
        public string GetOsuFileFormat() { return OsuFileFormat; }
        public int GetBackgroundTextureHeight() { return BackgroundTextureHeight; }
        public int GetBackgroundTextureWidth() { return BackgroundTextureWidth; }
        public string GetBackgroundFilePath() { return BackgroundFilePath; }
        public string GetAudioFilePath() { return AudioFilePath; }

        //GENERAL
        public string GetAudioFilename() { return AudioFilename; }//AudioFilename (String) specifies the location of the audio file relative to the current folder.
        public long GetAudioLeadIn() { return AudioLeadIn; }//AudioLeadIn (Integer, milliseconds) is the amount of time added before the audio file begins playing. Useful for audio files that begin immediately.
        public long GetPreviewTime() { return PreviewTime; }//PreviewTime (Integer, milliseconds) defines when the audio file should begin playing when selected in the song selection menu.
        public bool GetCountdown() { return Countdown; }//Countdown (Boolean) specifies whether or not a countdown occurs before the first hit object appears.
        public string GetSampleSet() { return SampleSet; }//SampleSet (String) specifies which set of hit sounds will be used throughout the beatmap.
        public float GetStackLeniency() { return StackLeniency; }//StackLeniency (Float) is how often closely placed hit objects will be stacked together.
        public int GetMode() { return Mode; }//Mode (Integer) defines the game mode of the beatmap. (0=osu!, 1=Taiko, 2=Catch the Beat, 3=osu!mania)
        public bool GetLetterboxInBreaks() { return LetterboxInBreaks; }//LetterboxInBreaks (Boolean) specifies whether the letterbox appears during breaks.
        public bool GetWidescreenStoryboard() { return WidescreenStoryboard; }//WidescreenStoryboard (Boolean) specifies whether or not the storyboard should be widescreen.
        //EDITOR
        public int[] GetBookmarks() { return Bookmarks; }//Bookmarks (Integer List, milliseconds) is a list of comma-separated times of editor bookmarks.
        public float GetDistanceSpacing() { return DistanceSpacing; }//DistanceSpacing (Float) is a multiplier for the "Distance Snap" feature.
        public int GetBeatDivisor() { return BeatDivisor; }//BeatDivisor (Integer) specifies the beat division for placing objects.
        public int GetGridsize() { return Gridsize; }//GridSize (Integer) specifies the size of the grid for the "Grid Snap" feature.
        public float GetTimelineZoom() { return TimelineZoom; }  //TimelineZoom (Integer) specifies the zoom in the editor timeline.
        //METADATA
        public string GetTitle() { return Title; }//Title (String) is the title of the song limited to ASCII characters.
        public string GetTitleUnicode() { return TitleUnicode; }//TitleUnicode (String) is the title of the song with unicode support. If not present, Title is used.
        public string GetArtist() { return Artist; }//Artist (String) is the name of the song's artist limited to ASCII characters.
        public string GetArtistUnicode() { return ArtistUnicode; }//ArtistUnicode (String) is the name of the song's artist with unicode support. If not present, Artist is used.
        public string GetCreator() { return Creator; }//Creator (String) is the username of the mapper.
        public string GetVersion() { return Version; }//Version (String) is the name of the beatmap's difficulty.
        public string GetSource() { return Source; }//Source (String) describes the origin of the song.
        public string[] GetTags() { return Tags; }//Tags (String List) is a collection of words describing the song. Tags are searchable in both the online listings and in the song selection menu.
        public int GetBeatmapID() { return BeatmapID; }//BeatmapID (Integer) is the ID of the single beatmap.
        public int GetBeatmapSetID() { return BeatmapSetID; }//BeatmapSetID (Integer) is the ID of the beatmap set.
        //DIFFICULTY
        public float GetHPDrainRate() { return HPDrainRate; }//HPDrainRate (Float) specifies the HP Drain difficulty.
        public float GetCircleSize() { return CircleSize; }//CircleSize (Float) specifies the size of hit object circles.
        public float GetOverallDifficulty() { return OverallDifficulty; }//OverallDifficulty (Float) specifies the amount of time allowed to click a hit object on time.
        public float GetApproachRate() { return ApproachRate; }//ApproachRate (Float) specifies the amount of time taken for the approach circle and hit object to appear.
        public float GetSliderMultiplier() { return SliderMultiplier; }//SliderMultiplier (Float) specifies a multiplier for the slider velocity. Default value is 1.
        public float GetSliderTickRate() { return SliderTickRate; }//SliderTickRate (Float) specifies how often slider ticks appear. Default value is 1.
        //EVENTS
        //storyboard stuff i guess...
        //TIMING POINTS
        //Timing Points describe a number of properties regarding offsets, beats per minute and hit sounds. Offset (Integer, milliseconds) defines when the Timing Point takes effect. Milliseconds per Beat (Float) defines the beats per minute of the song. For certain calculations, it is easier to use milliseconds per beat. Meter (Integer) defines the number of beats in a measure. Sample Type (Integer) defines the type of hit sound samples that are used. Sample Set (Integer) defines the set of hit sounds that are used. Volume (Integer) is a value from 0 - 100 that defines the volume of hit sounds. Kiai Mode (Boolean) defines whether or not Kiai Time effects are active. Inherited (Boolean) defines whether or not the Timing Point is an inherited Timing Point.
        public List<TimingPoint> GetTimingPointList() { return TimingPointList; }
        //COLORS
        public Color[] GetComboColors() { return ComboColors; }        //Combo# (Integer List) is a list of three numbers, each from 0 - 255 defining an RGB color.
        #endregion
    }

    public struct TimingPoint : IComparable<TimingPoint>
    {
        public TimeSpan Offset;
        public TimeSpan BeatLength;
        public int Meter;//beats per measure
        public int SampleType;
        public int SampleSet;
        public int Volume;//0 to 100
        public bool KiaiTime;
        public bool Inherited;

        public TimingPoint(string initialize)
        {
            string[] args = initialize.Split(',');
            Offset = TimeSpan.FromMilliseconds(long.Parse(args[0]));
            BeatLength = TimeSpan.FromMilliseconds(float.Parse(args[1]));
            Meter = int.Parse(args[2]);
            SampleType = int.Parse(args[3]);
            SampleSet = int.Parse(args[4]);
            Volume = int.Parse(args[5]);
            Inherited = int.Parse(args[6]) == 0;
            KiaiTime = int.Parse(args[7]) == 1;
        }
        public TimingPoint(TimeSpan time)
        {
            Offset = time;
            BeatLength = new TimeSpan();
            Meter = 0;
            SampleType = 0;
            SampleSet = 0;
            Volume = 0;
            Inherited = false;
            KiaiTime = false;
        }
        public int CompareTo(TimingPoint other)
        {
            return this.Offset.CompareTo(other.Offset);
        }
    }
}
