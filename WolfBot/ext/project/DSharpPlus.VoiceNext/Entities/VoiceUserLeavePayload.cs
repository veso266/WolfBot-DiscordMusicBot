using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x0200001A RID: 26
	internal sealed class VoiceUserLeavePayload
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00004492 File Offset: 0x00002692
		// (set) Token: 0x06000141 RID: 321 RVA: 0x0000449A File Offset: 0x0000269A
		[JsonProperty("user_id")]
		public ulong UserId { get; set; }
	}
}
