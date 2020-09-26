using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000012 RID: 18
	internal sealed class VoiceReadyPayload
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00004287 File Offset: 0x00002487
		// (set) Token: 0x06000103 RID: 259 RVA: 0x0000428F File Offset: 0x0000248F
		[JsonProperty("ssrc")]
		public uint SSRC { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000104 RID: 260 RVA: 0x00004298 File Offset: 0x00002498
		// (set) Token: 0x06000105 RID: 261 RVA: 0x000042A0 File Offset: 0x000024A0
		[JsonProperty("ip")]
		public string Address { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000106 RID: 262 RVA: 0x000042A9 File Offset: 0x000024A9
		// (set) Token: 0x06000107 RID: 263 RVA: 0x000042B1 File Offset: 0x000024B1
		[JsonProperty("port")]
		public ushort Port { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000042BA File Offset: 0x000024BA
		// (set) Token: 0x06000109 RID: 265 RVA: 0x000042C2 File Offset: 0x000024C2
		[JsonProperty("modes")]
		public IReadOnlyList<string> Modes { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000042CB File Offset: 0x000024CB
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000042D3 File Offset: 0x000024D3
		[JsonProperty("heartbeat_interval")]
		public int HeartbeatInterval { get; set; }
	}
}
