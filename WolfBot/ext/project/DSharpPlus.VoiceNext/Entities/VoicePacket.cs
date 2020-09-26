using System;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x02000011 RID: 17
	internal struct VoicePacket
	{
		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000FD RID: 253 RVA: 0x0000424F File Offset: 0x0000244F
		public ReadOnlyMemory<byte> Bytes { get; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00004257 File Offset: 0x00002457
		public int MillisecondDuration { get; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000425F File Offset: 0x0000245F
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00004267 File Offset: 0x00002467
		public bool IsSilence { get; set; }

		// Token: 0x06000101 RID: 257 RVA: 0x00004270 File Offset: 0x00002470
		public VoicePacket(ReadOnlyMemory<byte> bytes, int msDuration, bool isSilence = false)
		{
			this.Bytes = bytes;
			this.MillisecondDuration = msDuration;
			this.IsSilence = isSilence;
		}
	}
}
