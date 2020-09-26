using System;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// VoiceNext client configuration.
	/// </summary>
	// Token: 0x02000006 RID: 6
	public sealed class VoiceNextConfiguration
	{
		/// <summary>
		/// <para>Sets the audio format for Opus. This will determine the quality of the audio output.</para>
		/// <para>Defaults to <see cref="P:DSharpPlus.VoiceNext.AudioFormat.Default" />.</para>
		/// </summary>
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002380 File Offset: 0x00000580
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002388 File Offset: 0x00000588
		public AudioFormat AudioFormat { internal get; set; } = AudioFormat.Default;

		/// <summary>
		/// <para>Sets whether incoming voice receiver should be enabled.</para>
		/// <para>Defaults to false.</para>
		/// </summary>
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002391 File Offset: 0x00000591
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002399 File Offset: 0x00000599
		public bool EnableIncoming { internal get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="T:DSharpPlus.VoiceNext.VoiceNextConfiguration" />.
		/// </summary>
		// Token: 0x0600001B RID: 27 RVA: 0x000023A2 File Offset: 0x000005A2
		public VoiceNextConfiguration()
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="T:DSharpPlus.VoiceNext.VoiceNextConfiguration" />, copying the properties of another configuration.
		/// </summary>
		/// <param name="other">Configuration the properties of which are to be copied.</param>
		// Token: 0x0600001C RID: 28 RVA: 0x000023B8 File Offset: 0x000005B8
		public VoiceNextConfiguration(VoiceNextConfiguration other)
		{
			this.AudioFormat = new AudioFormat(other.AudioFormat.SampleRate, other.AudioFormat.ChannelCount, other.AudioFormat.VoiceApplication);
			this.EnableIncoming = other.EnableIncoming;
		}
	}
}
