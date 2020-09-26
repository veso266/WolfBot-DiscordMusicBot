using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000010 RID: 16
	internal sealed class VoiceIdentifyPayload
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00004203 File Offset: 0x00002403
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x0000420B File Offset: 0x0000240B
		[JsonProperty("server_id")]
		public ulong ServerId { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00004214 File Offset: 0x00002414
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x0000421C File Offset: 0x0000241C
		[JsonProperty("user_id", NullValueHandling = 1)]
		public ulong? UserId { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00004225 File Offset: 0x00002425
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000422D File Offset: 0x0000242D
		[JsonProperty("session_id")]
		public string SessionId { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00004236 File Offset: 0x00002436
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000423E File Offset: 0x0000243E
		[JsonProperty("token")]
		public string Token { get; set; }
	}
}
