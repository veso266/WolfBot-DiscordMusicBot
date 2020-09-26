using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// Defines the format of PCM data consumed or produced by Opus.
	/// </summary>
	// Token: 0x02000002 RID: 2
	public struct AudioFormat
	{
		/// <summary>
		/// Gets the collection of sampling rates (in Hz) the Opus encoder can use.
		/// </summary>
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static IReadOnlyCollection<int> AllowedSampleRates { get; } = new ReadOnlyCollection<int>(new int[]
		{
			8000,
			12000,
			16000,
			24000,
			48000
		});

		/// <summary>
		/// Gets the collection of channel counts the Opus encoder can use.
		/// </summary>
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		public static IReadOnlyCollection<int> AllowedChannelCounts { get; } = new ReadOnlyCollection<int>(new int[]
		{
			1,
			2
		});

		/// <summary>
		/// Gets the collection of sample durations (in ms) the Opus encoder can use.
		/// </summary>
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205E File Offset: 0x0000025E
		public static IReadOnlyCollection<int> AllowedSampleDurations { get; } = new ReadOnlyCollection<int>(new int[]
		{
			5,
			10,
			20,
			40,
			60
		});

		/// <summary>
		/// Gets the default audio format. This is a formt configured for 48kHz sampling rate, 2 channels, with music quality preset.
		/// </summary>
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002065 File Offset: 0x00000265
		public static AudioFormat Default { get; } = new AudioFormat(48000, 2, VoiceApplication.Music);

		/// <summary>
		/// Gets the audio sampling rate in Hz.
		/// </summary>
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206C File Offset: 0x0000026C
		public int SampleRate { get; }

		/// <summary>
		/// Gets the audio channel count.
		/// </summary>
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002074 File Offset: 0x00000274
		public int ChannelCount { get; }

		/// <summary>
		/// Gets the voice application, which dictates the quality preset.
		/// </summary>
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000007 RID: 7 RVA: 0x0000207C File Offset: 0x0000027C
		public VoiceApplication VoiceApplication { get; }

		/// <summary>
		/// Creates a new audio format for use with Opus encoder.
		/// </summary>
		/// <param name="sampleRate">Audio sampling rate in Hz.</param>
		/// <param name="channelCount">Number of audio channels in the data.</param>
		/// <param name="voiceApplication">Encoder preset to use.</param>
		// Token: 0x06000008 RID: 8 RVA: 0x00002084 File Offset: 0x00000284
		public AudioFormat(int sampleRate = 48000, int channelCount = 2, VoiceApplication voiceApplication = VoiceApplication.Music)
		{
			if (!AudioFormat.AllowedSampleRates.Contains(sampleRate))
			{
				throw new ArgumentOutOfRangeException("sampleRate", "Invalid sample rate specified.");
			}
			if (!AudioFormat.AllowedChannelCounts.Contains(channelCount))
			{
				throw new ArgumentOutOfRangeException("channelCount", "Invalid channel count specified.");
			}
			if (voiceApplication != VoiceApplication.Music && voiceApplication != VoiceApplication.Voice && voiceApplication != VoiceApplication.LowLatency)
			{
				throw new ArgumentOutOfRangeException("voiceApplication", "Invalid voice application specified.");
			}
			this.SampleRate = sampleRate;
			this.ChannelCount = channelCount;
			this.VoiceApplication = voiceApplication;
		}

		/// <summary>
		/// Calculates a sample size in bytes.
		/// </summary>
		/// <param name="sampleDuration">Millsecond duration of a sample.</param>
		/// <returns>Calculated sample size in bytes.</returns>
		// Token: 0x06000009 RID: 9 RVA: 0x00002108 File Offset: 0x00000308
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CalculateSampleSize(int sampleDuration)
		{
			if (!AudioFormat.AllowedSampleDurations.Contains(sampleDuration))
			{
				throw new ArgumentOutOfRangeException("sampleDuration", "Invalid sample duration specified.");
			}
			return sampleDuration * this.ChannelCount * (this.SampleRate / 1000) * 2;
		}

		/// <summary>
		/// Gets the maximum buffer size for decoding. This method should be called when decoding Opus data to PCM, to ensure sufficient buffer size.
		/// </summary>
		/// <returns>Buffer size required to decode data.</returns>
		// Token: 0x0600000A RID: 10 RVA: 0x0000213E File Offset: 0x0000033E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetMaximumBufferSize()
		{
			return this.CalculateMaximumFrameSize();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002146 File Offset: 0x00000346
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int CalculateSampleDuration(int sampleSize)
		{
			return sampleSize / (this.SampleRate / 1000) / this.ChannelCount / 2;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000215F File Offset: 0x0000035F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int CalculateFrameSize(int sampleDuration)
		{
			return sampleDuration * (this.SampleRate / 1000);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000216F File Offset: 0x0000036F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int CalculateMaximumFrameSize()
		{
			return 120 * (this.SampleRate / 1000);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002180 File Offset: 0x00000380
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int SampleCountToSampleSize(int sampleCount)
		{
			return sampleCount * this.ChannelCount * 2;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000218C File Offset: 0x0000038C
		internal bool IsValid()
		{
			return AudioFormat.AllowedSampleRates.Contains(this.SampleRate) && AudioFormat.AllowedChannelCounts.Contains(this.ChannelCount) && (this.VoiceApplication == VoiceApplication.Music || this.VoiceApplication == VoiceApplication.Voice || this.VoiceApplication == VoiceApplication.LowLatency);
		}
	}
}
