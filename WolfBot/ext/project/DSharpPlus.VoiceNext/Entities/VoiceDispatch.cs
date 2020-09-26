using System;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x0200000F RID: 15
	internal sealed class VoiceDispatch
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000EB RID: 235 RVA: 0x000041B7 File Offset: 0x000023B7
		// (set) Token: 0x060000EC RID: 236 RVA: 0x000041BF File Offset: 0x000023BF
		[JsonProperty("op")]
		public int OpCode { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000ED RID: 237 RVA: 0x000041C8 File Offset: 0x000023C8
		// (set) Token: 0x060000EE RID: 238 RVA: 0x000041D0 File Offset: 0x000023D0
		[JsonProperty("d")]
		public object Payload { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000EF RID: 239 RVA: 0x000041D9 File Offset: 0x000023D9
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x000041E1 File Offset: 0x000023E1
		[JsonProperty("s", NullValueHandling = 1)]
		public int? Sequence { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x000041EA File Offset: 0x000023EA
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x000041F2 File Offset: 0x000023F2
		[JsonProperty("t", NullValueHandling = 1)]
		public string EventName { get; set; }
	}
}
