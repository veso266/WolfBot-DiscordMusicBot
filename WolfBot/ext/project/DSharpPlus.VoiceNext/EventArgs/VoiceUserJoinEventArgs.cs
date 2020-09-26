using System;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpPlus.VoiceNext.EventArgs
{
	/// <summary>
	/// Arguments for <see cref="E:DSharpPlus.VoiceNext.VoiceNextConnection.UserJoined" />.
	/// </summary>
	// Token: 0x0200000C RID: 12
	public sealed class VoiceUserJoinEventArgs : DiscordEventArgs
	{
		/// <summary>
		/// Gets the user who left.
		/// </summary>
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x000040F3 File Offset: 0x000022F3
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x000040FB File Offset: 0x000022FB
		public DiscordUser User { get; internal set; }

		/// <summary>
		/// Gets the SSRC of the user who joined.
		/// </summary>
		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004104 File Offset: 0x00002304
		// (set) Token: 0x060000DB RID: 219 RVA: 0x0000410C File Offset: 0x0000230C
		public uint SSRC { get; internal set; }

		// Token: 0x060000DC RID: 220 RVA: 0x00004115 File Offset: 0x00002315
		internal VoiceUserJoinEventArgs(DiscordClient discord) : base(discord)
		{
		}
	}
}
