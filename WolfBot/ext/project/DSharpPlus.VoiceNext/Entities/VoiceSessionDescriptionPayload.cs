using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000016 RID: 22
	internal sealed class VoiceSessionDescriptionPayload
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00004384 File Offset: 0x00002584
		// (set) Token: 0x06000121 RID: 289 RVA: 0x0000438C File Offset: 0x0000258C
		[JsonProperty("secret_key")]
		public byte[] SecretKey { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00004395 File Offset: 0x00002595
		// (set) Token: 0x06000123 RID: 291 RVA: 0x0000439D File Offset: 0x0000259D
		[JsonProperty("mode")]
		public string Mode { get; set; }
	}
}
