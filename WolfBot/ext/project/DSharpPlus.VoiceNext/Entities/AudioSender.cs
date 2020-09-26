using System;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext.Codec;

namespace DSharpPlus.VoiceNext.Entities
{
	// Token: 0x0200000E RID: 14
	internal class AudioSender : IDisposable
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00004149 File Offset: 0x00002349
		public uint SSRC { get; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00004151 File Offset: 0x00002351
		public ulong Id
		{
			get
			{
				DiscordUser user = this.User;
				if (user == null)
				{
					return 0UL;
				}
				return user.Id;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00004165 File Offset: 0x00002365
		public OpusDecoder Decoder { get; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000416D File Offset: 0x0000236D
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00004175 File Offset: 0x00002375
		public DiscordUser User { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000417E File Offset: 0x0000237E
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00004186 File Offset: 0x00002386
		public ushort LastSequence { get; set; }

		// Token: 0x060000E9 RID: 233 RVA: 0x0000418F File Offset: 0x0000238F
		public AudioSender(uint ssrc, OpusDecoder decoder)
		{
			this.SSRC = ssrc;
			this.Decoder = decoder;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000041A5 File Offset: 0x000023A5
		public void Dispose()
		{
			OpusDecoder decoder = this.Decoder;
			if (decoder == null)
			{
				return;
			}
			decoder.Dispose();
		}
	}
}
