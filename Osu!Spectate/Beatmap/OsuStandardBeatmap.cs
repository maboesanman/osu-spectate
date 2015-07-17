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



namespace OsuSpectate.Beatmap
{
    public class OsuStandardBeatmap
    {
        ReplayAPI.Mods mods = 0;

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
        SortedList<int, TimingPoint> TimingPointList;
        //COLORS
        Color[] ComboColors;        //Combo# (Integer List) is a list of three numbers, each from 0 - 255 defining an RGB color.
        //HIT OBJECTS
        List<OsuStandardHitObject> HitObjectList;


        public OsuStandardBeatmap(string path)
        {
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
                                //TODO
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
                                            if (split[3] == "5" || split[3] == "6")
                                            {
                                                currentComboIndex++;
                                                currentComboNumber = 1;
                                            }
                                            HitObjectList.Add(OsuStandardHitObject.getNewHitobject(split, currentComboIndex, currentComboNumber, this));
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

            Console.WriteLine("finished loading beatmap: " + Title);
        }
        public TimeSpan GetOD300Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (78.0f - OverallDifficulty * 6.0f)));
        }
        public TimeSpan GetOD100Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (138.0f - OverallDifficulty * 8.0f)));
        }
        public TimeSpan GetOD50Milliseconds()
        {
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * (198.0f - OverallDifficulty * 10.0f)));
        }
        public TimeSpan GetARMilliseconds()
        {
            if ((((int)mods) / 16) % 2 == 1)
            {
                return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (Math.Min(ApproachRate * 1.4f, 10.0f)) * 120.0f) - (Math.Max((Math.Min(ApproachRate * 1.4f, 10.0f) - 5.0f) * 30.0f, 0.0f)))));
            }
            return new TimeSpan((long)(((float)TimeSpan.TicksPerMillisecond) * ((1800.0f - (ApproachRate) * 120.0f) - (Math.Max((ApproachRate - 5.0f) * 30.0f, 0.0f)))));
        }
        public float GetCSRadius()
        {
            if ((((int)mods) / 16) % 2 == 1)
            {
                return 4.0f * (12.0f - Math.Min(CircleSize * 1.3f, 10.0f));
            }
            return 4.0f * (12.0f - CircleSize);
        }
        public void setMods(ReplayAPI.Mods m)
        {
            if (((mods & ReplayAPI.Mods.HardRock) ^ (m & ReplayAPI.Mods.HardRock)) == ReplayAPI.Mods.HardRock)
            {
                for (int i = 0; i < HitObjectList.Count; i++)
                {
                    HitObjectList.ElementAt(i).flipY();
                    Console.WriteLine("flipped");
                }
            }
            mods = m;

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
        public SortedList<int, TimingPoint> GetTimingPointList() { return TimingPointList; }
        //COLORS
        public Color[] GetComboColors() { return ComboColors; }        //Combo# (Integer List) is a list of three numbers, each from 0 - 255 defining an RGB color.
        //HIT OBJECTS
        public List<OsuStandardHitObject> GetHitObjectList() { return HitObjectList; }
        #endregion
    }

    public struct TimingPoint
    {

    }
    public struct InheritedTimingPoint
    {

    }
}
