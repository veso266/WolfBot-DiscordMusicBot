using System;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpPlus.VoiceNext.EventArgs
{
	/// <summary>
	/// Arguments for <see cref="E:DSharpPlus.VoiceNext.VoiceNextConnection.UserLeft" />.
	/// </summary>
	// Token: 0x0200000D RID: 13
	public sealed class VoiceUserLeaveEventArgs : DiscordEventArgs
	{
		/// <summary>
		/// Gets the user who left.
		/// </summary>
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000411E File Offset: 0x0000231E
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00004126 File Offset: 0x00002326
		public DiscordUser User { get; internal set; }

		/// <summary>
		/// Gets the SSRC of the user who left.
		/// </summary>
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000DF RID: 223 RVA: 0x0000412F File Offset: 0x0000232F
		// (set) Token: 0x060000E0 RID: 224 RVA: 0x00004137 File Offset: 0x00002337
		public uint SSRC { get; internal set; }

		// Token: 0x060000E1 RID: 225 RVA: 0x00004140 File Offset: 0x00002340
		internal VoiceUserLeaveEventArgs(DiscordClient discord) : base(discord)
		{
		}
	}
}
