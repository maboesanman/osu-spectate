using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using OsuSpectate.GameplaySource;

using BigMansStuff.PracticeSharp.SoundTouch;

namespace OsuSpectate.Audio
{
    public class AudioPlayer
    {
        private NAudio.Wave.DirectSoundOut output;
        private NAudio.Wave.BlockAlignReductionStream MusicInStream;
        private NAudio.Wave.IWaveProvider MusicStream;
        private NAudio.Wave.WaveStream pcm;
        public AudioPlayer(OsuStandardGameplayInput input, float rate)
        {
            DisposeWave();
            MusicInStream = new NAudio.Wave.BlockAlignReductionStream(NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(input.GetBeatmap().GetAudioFilePath())));
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new TimeStretchWaveProvider(MusicInStream.ToSampleProvider(),rate));
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
                //    MusicStream.Dispose();
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
    public class SineWaveProvider32 : WaveProvider32
    {
        int sample;

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f; // let's not hurt our ears            
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                sample++;
                if (sample >= sampleRate) sample = 0;
            }
            return sampleCount;
        }
    }
    public class TimeStretchWaveProvider : WaveProvider32
    {
        ISampleProvider input;
        float rate;
        SoundTouchSharp st;
        uint inputlength;
        float[] inBuffer;
        int bitPerSample;
        int BUFFER_SIZE;
        public TimeStretchWaveProvider(ISampleProvider i, float r)
        {
            input = i;
            rate = r;
            st = new SoundTouchSharp();
            st.CreateInstance();
            st.SetTempo(1.0f);
            st.SetSampleRate(input.WaveFormat.SampleRate);
            st.SetChannels(input.WaveFormat.Channels);
            st.SetTempoChange(100.0f * (r - 1.0f));
            bitPerSample = input.WaveFormat.BitsPerSample;
            BUFFER_SIZE = bitPerSample * 1024;
            inBuffer = new float[BUFFER_SIZE];
            SetWaveFormat(input.WaveFormat.SampleRate, input.WaveFormat.Channels);
        }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            while(st.GetNumSamples()< sampleCount)
            {
                int x = input.Read(inBuffer, 0, BUFFER_SIZE);

                st.PutSamples(inBuffer, (uint)(x/input.WaveFormat.Channels));
            }
            st.ReceiveSamples(buffer, (uint)(sampleCount /input.WaveFormat.Channels));
            return sampleCount;
        }
    }
}
