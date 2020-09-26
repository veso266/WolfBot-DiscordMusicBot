using System;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x0200001F RID: 31
	[Flags]
	internal enum OpusError
	{
		// Token: 0x0400008E RID: 142
		Ok = 0,
		// Token: 0x0400008F RID: 143
		BadArgument = -1,
		// Token: 0x04000090 RID: 144
		BufferTooSmall = -2,
		// Token: 0x04000091 RID: 145
		InternalError = -3,
		// Token: 0x04000092 RID: 146
		InvalidPacket = -4,
		// Token: 0x04000093 RID: 147
		Unimplemented = -5,
		// Token: 0x04000094 RID: 148
		InvalidState = -6,
		// Token: 0x04000095 RID: 149
		AllocationFailure = -7
	}
}
