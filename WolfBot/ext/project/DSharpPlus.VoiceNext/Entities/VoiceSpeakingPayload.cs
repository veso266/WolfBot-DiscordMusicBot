using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000017 RID: 23
	internal sealed class VoiceSpeakingPayload
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000125 RID: 293 RVA: 0x000043AE File Offset: 0x000025AE
		// (set) Token: 0x06000126 RID: 294 RVA: 0x000043B6 File Offset: 0x000025B6
		[JsonProperty("speaking")]
		public bool Speaking { get; set; }

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000043BF File Offset: 0x000025BF
		// (set) Token: 0x06000128 RID: 296 RVA: 0x000043C7 File Offset: 0x000025C7
		[JsonProperty("delay", NullValueHandling = 1)]
		public int? Delay { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000043D0 File Offset: 0x000025D0
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000043D8 File Offset: 0x000025D8
		[JsonProperty("ssrc", NullValueHandling = 1)]
		public uint? SSRC { get; set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600012B RID: 299 RVA: 0x000043E1 File Offset: 0x000025E1
		// (set) Token: 0x0600012C RID: 300 RVA: 0x000043E9 File Offset: 0x000025E9
		[JsonProperty("user_id", NullValueHandling = 1)]
		public ulong? UserId { get; set; }
	}
}
