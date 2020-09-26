using System;
using System.Collections.Generic;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x0200001D RID: 29
	internal sealed class Opus : IDisposable
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000483F File Offset: 0x00002A3F
		public AudioFormat AudioFormat { get; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00004847 File Offset: 0x00002A47
		private IntPtr Encoder { get; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000164 RID: 356 RVA: 0x0000484F File Offset: 0x00002A4F
		private List<OpusDecoder> ManagedDecoders { get; }

		// Token: 0x06000165 RID: 357 RVA: 0x00004858 File Offset: 0x00002A58
		public Opus(AudioFormat audioFormat)
		{
			if (!audioFormat.IsValid())
			{
				throw new ArgumentException("Invalid audio format specified.", "audioFormat");
			}
			this.AudioFormat = audioFormat;
			this.Encoder = Interop.OpusCreateEncoder(this.AudioFormat);
			OpusSignal value = OpusSignal.Auto;
			VoiceApplication voiceApplication = this.AudioFormat.VoiceApplication;
			if (voiceApplication != VoiceApplication.Voice)
			{
				if (voiceApplication == VoiceApplication.Music)
				{
					value = OpusSignal.Music;
				}
			}
			else
			{
				value = OpusSignal.Voice;
			}
			Interop.OpusSetEncoderOption(this.Encoder, OpusControl.SetSignal, (int)value);
			Interop.OpusSetEncoderOption(this.Encoder, OpusControl.SetPacketLossPercent, 15);
			Interop.OpusSetEncoderOption(this.Encoder, OpusControl.SetInBandFec, 1);
			Interop.OpusSetEncoderOption(this.Encoder, OpusControl.SetBitrate, 131072);
			this.ManagedDecoders = new List<OpusDecoder>();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00004924 File Offset: 0x00002B24
		public void Encode(ReadOnlySpan<byte> pcm, ref Span<byte> target)
		{
			if (pcm.Length != target.Length)
			{
				throw new ArgumentException("PCM and Opus buffer lengths need to be equal.", "target");
			}
			int sampleDuration = this.AudioFormat.CalculateSampleDuration(pcm.Length);
			int frameSize = this.AudioFormat.CalculateFrameSize(sampleDuration);
			int num = this.AudioFormat.CalculateSampleSize(sampleDuration);
			if (pcm.Length != num)
			{
				throw new ArgumentException("Invalid PCM sample size.", "target");
			}
			Interop.OpusEncode(this.Encoder, pcm, frameSize, ref target);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000049B0 File Offset: 0x00002BB0
		public void Decode(OpusDecoder decoder, ReadOnlySpan<byte> opus, ref Span<byte> target, bool useFec, out AudioFormat outputFormat)
		{
			int num;
			int num2;
			int num3;
			int frameSize;
			Interop.OpusGetPacketMetrics(opus, this.AudioFormat.SampleRate, out num, out num2, out num3, out frameSize);
			outputFormat = ((this.AudioFormat.ChannelCount != num) ? new AudioFormat(this.AudioFormat.SampleRate, num, this.AudioFormat.VoiceApplication) : this.AudioFormat);
			if (decoder.AudioFormat.ChannelCount != num)
			{
				decoder.Initialize(outputFormat);
			}
			int sampleCount = Interop.OpusDecode(decoder.Decoder, opus, frameSize, target, useFec);
			int num4 = outputFormat.SampleCountToSampleSize(sampleCount);
			target = target.Slice(0, num4);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00004A6F File Offset: 0x00002C6F
		public void ProcessPacketLoss(OpusDecoder decoder, int frameSize, ref Span<byte> target)
		{
			Interop.OpusDecode(decoder.Decoder, frameSize, target);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00004A84 File Offset: 0x00002C84
		public int GetLastPacketSampleCount(OpusDecoder decoder)
		{
			int result;
			Interop.OpusGetLastPacketDuration(decoder.Decoder, out result);
			return result;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00004AA0 File Offset: 0x00002CA0
		public OpusDecoder CreateDecoder()
		{
			List<OpusDecoder> managedDecoders = this.ManagedDecoders;
			OpusDecoder result;
			lock (managedDecoders)
			{
				OpusDecoder opusDecoder = new OpusDecoder(this);
				this.ManagedDecoders.Add(opusDecoder);
				result = opusDecoder;
			}
			return result;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00004AF0 File Offset: 0x00002CF0
		public void DestroyDecoder(OpusDecoder decoder)
		{
			List<OpusDecoder> managedDecoders = this.ManagedDecoders;
			lock (managedDecoders)
			{
				if (this.ManagedDecoders.Contains(decoder))
				{
					this.ManagedDecoders.Remove(decoder);
					decoder.Dispose();
				}
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00004B50 File Offset: 0x00002D50
		public void Dispose()
		{
			Interop.OpusDestroyEncoder(this.Encoder);
			List<OpusDecoder> managedDecoders = this.ManagedDecoders;
			lock (managedDecoders)
			{
				foreach (OpusDecoder opusDecoder in this.ManagedDecoders)
				{
					opusDecoder.Dispose();
				}
			}
		}
	}
}
