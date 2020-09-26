using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000019 RID: 25
	internal sealed class VoiceUserJoinPayload
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00004468 File Offset: 0x00002668
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00004470 File Offset: 0x00002670
		[JsonProperty("user_id")]
		public ulong UserId { get; private set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00004479 File Offset: 0x00002679
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00004481 File Offset: 0x00002681
		[JsonProperty("audio_ssrc")]
		public uint SSRC { get; private set; }
	}
}
