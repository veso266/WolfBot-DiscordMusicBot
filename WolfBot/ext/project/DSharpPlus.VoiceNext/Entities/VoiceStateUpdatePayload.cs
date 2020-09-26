using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000018 RID: 24
	internal sealed class VoiceStateUpdatePayload
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600012E RID: 302 RVA: 0x000043FA File Offset: 0x000025FA
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00004402 File Offset: 0x00002602
		[JsonProperty("guild_id")]
		public ulong GuildId { get; set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000130 RID: 304 RVA: 0x0000440B File Offset: 0x0000260B
		// (set) Token: 0x06000131 RID: 305 RVA: 0x00004413 File Offset: 0x00002613
		[JsonProperty("channel_id")]
		public ulong? ChannelId { get; set; }

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000441C File Offset: 0x0000261C
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00004424 File Offset: 0x00002624
		[JsonProperty("user_id", NullValueHandling = 1)]
		public ulong? UserId { get; set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000134 RID: 308 RVA: 0x0000442D File Offset: 0x0000262D
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00004435 File Offset: 0x00002635
		[JsonProperty("session_id", NullValueHandling = 1)]
		public string SessionId { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000443E File Offset: 0x0000263E
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00004446 File Offset: 0x00002646
		[JsonProperty("self_deaf")]
		public bool Deafened { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000138 RID: 312 RVA: 0x0000444F File Offset: 0x0000264F
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00004457 File Offset: 0x00002657
		[JsonProperty("self_mute")]
		public bool Muted { get; set; }
	}
}
