using System;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x02000020 RID: 32
	internal enum OpusControl
	{
		// Token: 0x04000097 RID: 151
		SetBitrate = 4002,
		// Token: 0x04000098 RID: 152
		SetBandwidth = 4008,
		// Token: 0x04000099 RID: 153
		SetInBandFec = 4012,
		// Token: 0x0400009A RID: 154
		SetPacketLossPercent = 4014,
		// Token: 0x0400009B RID: 155
		SetSignal = 4024,
		// Token: 0x0400009C RID: 156
		ResetState = 4028,
		// Token: 0x0400009D RID: 157
		GetLastPacketDuration = 4039
	}
}
