using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.GameplaySource;

namespace OsuSpectate.Audio
{
    public class AudioPlayer
    {
        private NAudio.Wave.DirectSoundOut output;
        private NAudio.Wave.BlockAlignReductionStream MusicStream;
        private NAudio.Wave.WaveStream pcm;
        public AudioPlayer(OsuStandardGameplayInput input)
        {
            DisposeWave();
            output = null;
            MusicStream = null;
            pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(input.GetBeatmap().GetAudioFilePath()));
            MusicStream = new NAudio.Wave.BlockAlignReductionStream(pcm);
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(MusicStream));
            output.Play();

        }
        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    output.Stop();
                    output.Dispose();
                    output = null;
                }
                if (MusicStream != null)
                {
                    MusicStream.Dispose();
                    MusicStream = null;
                }
            }
        }
        public TimeSpan getCurrentTime()
        {
            return pcm.CurrentTime;
        }
        public void kill()
        {
            DisposeWave();
        }
    }
}
