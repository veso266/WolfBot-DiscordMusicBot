using System;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// Represents encoder settings preset for Opus.
	/// </summary>
	// Token: 0x02000005 RID: 5
	public enum VoiceApplication
	{
		/// <summary>
		/// Defines that the encoder must optimize settings for voice data.
		/// </summary>
		// Token: 0x04000009 RID: 9
		Voice = 2048,
		/// <summary>
		/// Defines that the encoder must optimize settings for music data.
		/// </summary>
		// Token: 0x0400000A RID: 10
		Music,
		/// <summary>
		/// Defines that the encoder must optimize settings for low latency applications.
		/// </summary>
		// Token: 0x0400000B RID: 11
		LowLatency = 2051
	}
}
