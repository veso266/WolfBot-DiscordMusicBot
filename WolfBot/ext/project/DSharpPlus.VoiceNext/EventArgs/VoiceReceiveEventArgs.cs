using System;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpPlus.VoiceNext.EventArgs
{
	/// <summary>
	/// Represents arguments for VoiceReceived events.
	/// </summary>
	// Token: 0x0200000B RID: 11
	public class VoiceReceiveEventArgs : DiscordEventArgs
	{
		/// <summary>
		/// Gets the SSRC of the audio source.
		/// </summary>
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004084 File Offset: 0x00002284
		// (set) Token: 0x060000CC RID: 204 RVA: 0x0000408C File Offset: 0x0000228C
		public uint SSRC { get; internal set; }

		/// <summary>
		/// Gets the user that sent the audio data.
		/// </summary>
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00004095 File Offset: 0x00002295
		// (set) Token: 0x060000CE RID: 206 RVA: 0x0000409D File Offset: 0x0000229D
		public DiscordUser User { get; internal set; }

		/// <summary>
		/// Gets the received voice data, decoded to PCM format.
		/// </summary>
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000CF RID: 207 RVA: 0x000040A6 File Offset: 0x000022A6
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x000040AE File Offset: 0x000022AE
		public ReadOnlyMemory<byte> PcmData { get; internal set; }

		/// <summary>
		/// Gets the received voice data, in Opus format. Note that for packets that were lost and/or compensated for, this will be empty.
		/// </summary>
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000040B7 File Offset: 0x000022B7
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x000040BF File Offset: 0x000022BF
		public ReadOnlyMemory<byte> OpusData { get; internal set; }

		/// <summary>
		/// Gets the format of the received PCM data.
		/// <para>
		/// Important: This isn't always the format set in <see cref="P:DSharpPlus.VoiceNext.VoiceNextConfiguration.AudioFormat" />, and depends on the audio data recieved.
		/// </para>
		/// </summary>
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x000040C8 File Offset: 0x000022C8
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x000040D0 File Offset: 0x000022D0
		public AudioFormat AudioFormat { get; internal set; }

		/// <summary>
		/// Gets the millisecond duration of the PCM audio sample.
		/// </summary>
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x000040D9 File Offset: 0x000022D9
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x000040E1 File Offset: 0x000022E1
		public int AudioDuration { get; internal set; }

		// Token: 0x060000D7 RID: 215 RVA: 0x000040EA File Offset: 0x000022EA
		internal VoiceReceiveEventArgs(DiscordClient client) : base(client)
		{
		}
	}
}
