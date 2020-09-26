using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000015 RID: 21
	internal sealed class VoiceServerUpdatePayload
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00004349 File Offset: 0x00002549
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00004351 File Offset: 0x00002551
		[JsonProperty("token")]
		public string Token { get; set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600011B RID: 283 RVA: 0x0000435A File Offset: 0x0000255A
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00004362 File Offset: 0x00002562
		[JsonProperty("guild_id")]
		public ulong GuildId { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000436B File Offset: 0x0000256B
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00004373 File Offset: 0x00002573
		[JsonProperty("endpoint")]
		public string Endpoint { get; set; }
	}
}
