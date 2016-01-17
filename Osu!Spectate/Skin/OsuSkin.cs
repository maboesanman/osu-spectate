using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace OsuSpectate.Skin
{
    public class OsuSkin
    {
        string path;
        bool HD;
        Color[] comboColors;
        #region variables
        //all modes
        //interface
        int menuBackground;
        //score
        int score0;
        int score1;
        int score2;
        int score3;
        int score4;
        int score5;
        int score6;
        int score7;
        int score8;
        int score9;
        int scoreComma;
        int scoreDot;
        int scorePercent;
        int scoreX;
        //countdown
        int ready;
        int go;
        int count1;
        int count2;
        int count3;
        //input overlay
        int inputOverlayBackground;
        int inputOverlayKey;
        //play misc
        int playUnranked;
        int playWarningArrow;
        //ModSelectionIcons
        int selectionModAutoplay;
        int selectionModCinema;
        int selectionModDoubleTime;
        int selectionModEasy;
        int selectionModFlashlight;
        int selectionModHalfTime;
        int selectionModHardRock;
        int selectionModHidden;
        int selectionModNightcore;
        int selectionModNoFail;
        int selectionModPerfect;
        int selectionModRelax;
        int selectionModRelax2;
        int selectionModSpunOut;
        int selectionModSuddenDeath;
        int selectionModTarget;
        int selectionModKey4;
        int selectionModKey5;
        int selectionModKey6;
        int selectionModKey7;
        int selectionModKey8;
        int selectionModFadeIn;
        int selectionModRandom;
        //RankingGrades
        int rankingA;
        int rankingB;
        int rankingC;
        int rankingD;
        int rankingS;
        int rankingSH;
        int rankingX;
        int rankingXH;
        int rankingSmallA;
        int rankingSmallB;
        int rankingSmallC;
        int rankingSmallD;
        int rankingSmallS;
        int rankingSmallSH;
        int rankingSmallX;
        int rankingSmallXH;
        //ranking panel
        int rankingAccuracy;
        int rankingGraph;
        int rankingMaxCombo;
        int rankingPanel;
        int rankingPerfect;
        int rankingReplay;
        int rankingRetry;
        int rankingTitle;

        //standard
        //combo numbers (in the hitcircles)
        int comboDefault0;
        int comboDefault1;
        int comboDefault2;
        int comboDefault3;
        int comboDefault4;
        int comboDefault5;
        int comboDefault6;
        int comboDefault7;
        int comboDefault8;
        int comboDefault9;
        //hitcircle set
        int approachCircle;
        int hitCircle;
        int hitCircleOverlay;
        int reverseArrow;
        //CursorSet
        int cursor;
        int cursorTrail;
        int cursorMiddle;
        //hitbursts
        int[] hit0;
        int[] hit50;
        int[] hit100;
        int[] hit300;
        int[] hit100k;
        int[] hit300g;
        //lighting
        int lighting;
        int particle50;
        int particle100;
        int particle300;
        //follow point
        int[] followPoint;
        //combobursts
        int[] combobursts;
        //section markers
        int sectionPass;
        int sectionFail;
        //sliderset
        int[] sliderBall;
        int sliderFollowCircle;
        int sliderPoint10;
        int sliderPoint30;
        //spinner set
        int spinnerBackground;
        int spinnerCircle;
        int spinnerMetre;
        int spinnerBottom;
        int spinnerMiddle;
        int spinnerMiddle2;
        int spinnerTop;
        int spinnerApproachCircle;
        int spinnerGlow;
        int spinnerSpin;
        int spinnerClear;
        int spinnerOsu;
        int spinnerRPM;
        #endregion
        public OsuSkin(string p, bool hd)
        {
            path = p;
            HD = hd;
            int[] LoadCount = new int[4];
            LoadCount[0] = 0; LoadCount[1] = 0; LoadCount[2] = 0; LoadCount[3] = 0;

            cursor = loadSkinElement(LoadCount, "cursor", "png");

            approachCircle = loadSkinElement(LoadCount, "approachcircle", "png");
            hitCircle = loadSkinElement(LoadCount, "hitcircle", "png");
            hitCircleOverlay = loadSkinElement(LoadCount, "hitcircleoverlay", "png");
            reverseArrow = loadSkinElement(LoadCount, "reversearrow", "png");
            inputOverlayKey = loadSkinElement(LoadCount, "inputoverlay-key", "png");
            inputOverlayBackground = loadSkinElement(LoadCount, "inputoverlay-background", "png");
            comboColors = new Color[6];
            comboColors[0] = Color.FromArgb(145, 229, 103);
            comboColors[1] = Color.FromArgb(255, 213, 128);
            comboColors[2] = Color.FromArgb(242, 121, 97);
            comboColors[3] = Color.FromArgb(255, 140, 179);
            comboColors[4] = Color.FromArgb(187, 103, 229);
            comboColors[5] = Color.FromArgb(140, 236, 255);

            Console.WriteLine(LoadCount[0] + LoadCount[1] + LoadCount[2] + LoadCount[3] + " Textures Loaded,");
            Console.WriteLine(LoadCount[0] + " HD from " + p.Split('\\').Reverse().ElementAt(1));
            Console.WriteLine(LoadCount[1] + " SD from " + p.Split('\\').Reverse().ElementAt(1));
            Console.WriteLine(LoadCount[2] + " HD from default");
            Console.WriteLine(LoadCount[3] + " SD from default");
        }
        private int loadSkinElement(int[] LoadCount, string name, string extension)
        {

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            Bitmap bmp;
            if (HD)
            {
                try
                {
                    bmp = new Bitmap(path + name + @"@2x." + extension);
                    LoadCount[0]++;
                }
                catch (Exception)
                {
                    try
                    {
                        bmp = new Bitmap(path + name + @"." + extension);
                        LoadCount[1]++;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            bmp = new Bitmap(@"Resources\osu!Default Skin\" + name + @"@2x." + extension);
                            LoadCount[2]++;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                bmp = new Bitmap(@"Resources\osu!Default Skin\" + name + @"." + extension);
                                LoadCount[3]++;
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            else
            {

                try
                {
                    bmp = new Bitmap(path + name + @"." + extension);
                }
                catch (System.IO.FileNotFoundException)
                {

                    try
                    {
                        bmp = new Bitmap(@"Resources\osu!Default Skin\" + name + @"." + extension);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        throw;
                    }

                }

            }
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            bmp.Dispose();
            return id;
        }
        #region getters
        //all modes
        //interface

        public int GetMenuBackground() { return menuBackground; } //jpeg
        public Color[] getComboColors() { return comboColors; }
        //score
        public int GetScore0() { return score0; }
        public int GetScore1() { return score1; }
        public int GetScore2() { return score2; }
        public int GetScore3() { return score3; }
        public int GetScore4() { return score4; }
        public int GetScore5() { return score5; }
        public int GetScore6() { return score6; }
        public int GetScore7() { return score7; }
        public int GetScore8() { return score8; }
        public int GetScore9() { return score9; }
        public int GetScoreComma() { return scoreComma; }
        public int GetScoreDot() { return scoreDot; }
        public int GetScorePercent() { return scorePercent; }
        public int GetScoreX() { return scoreX; }
        //countdown
        public int GerReady() { return ready; }
        public int GetGo() { return go; }
        public int GetCount1() { return count1; }
        public int GetCount2() { return count2; }
        public int GetCount3() { return count3; }
        //input overlay
        public int GetInputOverlayBackground() { return inputOverlayBackground; }
        public int GetInputOverlayKey() { return inputOverlayKey; }
        //play misc     
        public int GetPlayUnranked() { return playUnranked; }
        public int GetPlayWarningArrow() { return playWarningArrow; }
        //ModSelectionIcons
        public int GetSelectionModAutoplay() { return selectionModAutoplay; }
        public int GetSelectionModCinema() { return selectionModCinema; }
        public int GetSelectionModDoubleTime() { return selectionModDoubleTime; }
        public int GetSelectionModEasy() { return selectionModEasy; }
        public int GetSelectionModFlashlight() { return selectionModFlashlight; }
        public int GetSelectionModHalfTime() { return selectionModHalfTime; }
        public int GetSelectionModHardRock() { return selectionModHardRock; }
        public int GetSelectionModHidden() { return selectionModHidden; }
        public int GetSelectionModNightcore() { return selectionModNightcore; }
        public int GetSelectionModNoFail() { return selectionModNoFail; }
        public int GetSelectionModPerfect() { return selectionModPerfect; }
        public int GetSelectionModRelax() { return selectionModRelax; }
        public int GetSelectionModRelax2() { return selectionModRelax2; }
        public int GetSelectionModSpunOut() { return selectionModSpunOut; }
        public int GetSelectionModSuddenDeath() { return selectionModSuddenDeath; }
        public int GetSelectionModTarget() { return selectionModTarget; }
        public int GetSelectionModKey4() { return selectionModKey4; }
        public int GetSelectionModKey5() { return selectionModKey5; }
        public int GetSelectionModKey6() { return selectionModKey6; }
        public int GetSelectionModKey7() { return selectionModKey7; }
        public int GetSelectionModKey8() { return selectionModKey8; }
        public int GetSelectionModRandom() { return selectionModFadeIn; }
        //RankingGrades

        public int GetRankingA() { return rankingA; }
        public int GetRankingB() { return rankingB; }
        public int GetRankingC() { return rankingC; }
        public int GetRankingD() { return rankingD; }
        public int GetRankingS() { return rankingS; }
        public int GetRankingSH() { return rankingSH; }
        public int GetRankingX() { return rankingX; }
        public int GetRankingXH() { return rankingXH; }
        public int GetRankingSmallA() { return rankingSmallA; }
        public int GetRankingSmallB() { return rankingSmallB; }
        public int GetRankingSmallC() { return rankingSmallC; }
        public int GetRankingSmallD() { return rankingSmallD; }
        public int GetRankingSmallS() { return rankingSmallS; }
        public int GetRankingSmallSH() { return rankingSmallSH; }
        public int GetRankingSmallX() { return rankingSmallX; }
        public int GetRankingSmallXH() { return rankingSmallXH; }
        //ranking pane() { return nking panel}
        public int GetRankingAccuracy() { return rankingAccuracy; }
        public int GetRankingGraph() { return rankingGraph; }
        public int GetRankingMaxCombo() { return rankingMaxCombo; }
        public int GetRankingPanel() { return rankingPanel; }
        public int GetRankingPerfect() { return rankingPerfect; }
        public int GetRankingReplay() { return rankingReplay; }
        public int GetRankingTitle() { return rankingTitle; }


        //standard
        //combo numbers (in the hitcircles)
        public int GetComboDefault0() { return comboDefault0; }
        public int GetComboDefault1() { return comboDefault1; }
        public int GetComboDefault2() { return comboDefault2; }
        public int GetComboDefault3() { return comboDefault3; }
        public int GetComboDefault4() { return comboDefault4; }
        public int GetComboDefault5() { return comboDefault5; }
        public int GetComboDefault6() { return comboDefault6; }
        public int GetComboDefault7() { return comboDefault7; }
        public int GetComboDefault8() { return comboDefault8; }
        public int GetComboDefault9() { return comboDefault9; }
        //hitcircle se() { return tcircle set}
        public int GetApproachCircle() { return approachCircle; }
        public int GetHitCircle() { return hitCircle; }
        public int GetHitCircleOverlay() { return hitCircleOverlay; }
        public int GetReverseArrow() { return reverseArrow; }
        //CursorSet   () { return rsorSet}
        public int GetCursor() { return cursor; }
        public int GetCursorTrail() { return cursorTrail; }
        public int GetCursorMiddle() { return cursorMiddle; }
        //hitburst() { return tbursts}
        public int[] GetHit0() { return hit0; }
        public int[] GetHit50() { return hit50; }
        public int[] GetHit100() { return hit100; }
        public int[] GetHit300() { return hit300; }
        public int[] GetHit100k() { return hit100k; }
        public int[] GetHit300g() { return hit300g; }
        //lighting
        public int GetLighting() { return lighting; }
        public int GetParticle50() { return particle50; }
        public int GetParticle100() { return particle100; }
        public int GetParticle300() { return particle300; }
        //follow point
        public int[] GetFollowPoint() { return followPoint; }
        //combobursts
        public int[] GetCombobursts() { return combobursts; }
        //section markers
        public int GetSectionPass() { return sectionPass; }
        public int GetSectionFail() { return sectionFail; }
        //sliderset
        public int[] GetSliderBall() { return sliderBall; }
        public int GetSliderFollowCircle() { return sliderFollowCircle; }
        public int GetSliderPoint10() { return sliderPoint10; }
        public int GetSliderPoint30() { return sliderPoint30; }
        //spinner set  
        public int GetSpinnerBackground() { return spinnerBackground; }
        public int GetSpinnerCircle() { return spinnerCircle; }
        public int GetSpinnerMetre() { return spinnerMetre; }
        public int GetSpinnerBottom() { return spinnerBottom; }
        public int GetSpinnerMiddle() { return spinnerMiddle; }
        public int GetSpinnerMiddle2() { return spinnerMiddle2; }
        public int GetSpinnerTop() { return spinnerTop; }
        public int GetSpinnerApproachCircle() { return spinnerApproachCircle; }
        public int GetSpinnerGlow() { return spinnerGlow; }
        public int GetSpinnerSpin() { return spinnerSpin; }
        public int GetSpinnerClear() { return spinnerClear; }
        public int GetSpinnerOsu() { return spinnerOsu; }
        public int GetSpinnerRPM() { return spinnerRPM; }
        #endregion
    }
}
