using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000013 RID: 19
	internal sealed class VoiceSelectProtocolPayload
	{
		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600010D RID: 269 RVA: 0x000042E4 File Offset: 0x000024E4
		// (set) Token: 0x0600010E RID: 270 RVA: 0x000042EC File Offset: 0x000024EC
		[JsonProperty("protocol")]
		public string Protocol { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600010F RID: 271 RVA: 0x000042F5 File Offset: 0x000024F5
		// (set) Token: 0x06000110 RID: 272 RVA: 0x000042FD File Offset: 0x000024FD
		[JsonProperty("data")]
		public VoiceSelectProtocolPayloadData Data { get; set; }
	}
}
