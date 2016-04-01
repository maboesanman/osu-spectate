using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuSpectate.OsuInterOp
{
    class SongDatabase
    {
        string databaseLocation;

    }
    class BeatmapDatabaseEntry
    {
        string ArtistName;
        string ArtistNameUnicode;
        string SongTitle;
        string SongTitleUnicode;
        string Creator;
        string Difficulty;
        string AudioFileName;
        string MD5Hash;
        string BeatmapFileName;
        byte RankedStatus; //4=ranked, 5=approved, 2=pending/graveyard
        short NumHitCircles;
        short NumSliders;
        short NumSpinners;
        long LastModified; //in windows ticks
        float ApproachRate;
        float CircleSize;
        float HPDrain;
        float OverallDifficulty;
        double SliderVelocity;
        double StarRatingStandard;
        double StarRatingTaiko;
        double StarRatingCTB;
        double StarRatingMania;
        int DrainTime; //in seconds
        int TotalTime; //in milliseconds
        int AudioPreview; //in milliseconds
        //some timing points
        int BeatmapID;
        int BeatmapSetID;
        int ThreadID;
        byte GradeAchievedStandard;
        byte GradeAchievedTaiko;
        byte GradeAchievedCTB;
        byte GradeAchievedMania;
        short LocalBeatmapOffset;
        float StackLeniency;
        byte Mode;//0,1,2,3->standard, taiko, ctb, mania
        string SongSource;
        string SongTags;
        short OnlineOffset;
        string TitleFont;
        bool Unplayed;
        long LastPlayed;
        bool OszV2;
        string DirectoryName;
        long LastChecked;
        bool IgnoreBeatmapSounds;
        bool IgnoreBeatmapSkin;
        bool DisableStoryboard;
        bool DisableVideo;
        bool VidualOverride;
        //???
        int LastModificationTime;
        byte ManiaScrollSpeed;
    }
}
