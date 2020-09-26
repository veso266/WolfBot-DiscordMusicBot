using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000014 RID: 20
	internal class VoiceSelectProtocolPayloadData
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000430E File Offset: 0x0000250E
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00004316 File Offset: 0x00002516
		[JsonProperty("address")]
		public string Address { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000431F File Offset: 0x0000251F
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00004327 File Offset: 0x00002527
		[JsonProperty("port")]
		public ushort Port { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00004330 File Offset: 0x00002530
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00004338 File Offset: 0x00002538
		[JsonProperty("mode")]
		public string Mode { get; set; }
	}
}
