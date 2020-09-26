using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x0200001B RID: 27
	internal static class Helpers
	{
		// Token: 0x06000143 RID: 323 RVA: 0x000044AC File Offset: 0x000026AC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void ZeroFill(Span<byte> buff)
		{
			int num = 0;
			int i;
			for (i = 0; i < buff.Length / 4; i++)
			{
				MemoryMarshal.Write<int>(buff, ref num);
			}
			if (buff.Length % 4 == 0)
			{
				return;
			}
			while (i < buff.Length)
			{
				*buff[i] = 0;
				i++;
			}
		}
	}
}
