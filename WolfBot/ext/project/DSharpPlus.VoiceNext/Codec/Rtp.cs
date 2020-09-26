using System;
using System.Buffers.Binary;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x02000022 RID: 34
	internal sealed class Rtp : IDisposable
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00004C7C File Offset: 0x00002E7C
		public unsafe void EncodeHeader(ushort sequence, uint timestamp, uint ssrc, Span<byte> target)
		{
			if (target.Length < 12)
			{
				throw new ArgumentException("Header buffer is too short.", "target");
			}
			*target[0] = 128;
			*target[1] = 120;
			BinaryPrimitives.WriteUInt16BigEndian(target.Slice(2), sequence);
			BinaryPrimitives.WriteUInt32BigEndian(target.Slice(4), timestamp);
			BinaryPrimitives.WriteUInt32BigEndian(target.Slice(8), ssrc);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00004CE7 File Offset: 0x00002EE7
		public unsafe bool IsRtpHeader(ReadOnlySpan<byte> source)
		{
			return source.Length >= 12 && (*source[0] == 128 || *source[0] == 144) && *source[1] == 120;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00004D28 File Offset: 0x00002F28
		public unsafe void DecodeHeader(ReadOnlySpan<byte> source, out ushort sequence, out uint timestamp, out uint ssrc, out bool hasExtension)
		{
			if (source.Length < 12)
			{
				throw new ArgumentException("Header buffer is too short.", "source");
			}
			if ((*source[0] != 128 && *source[0] != 144) || *source[1] != 120)
			{
				throw new ArgumentException("Invalid RTP header.", "source");
			}
			hasExtension = (*source[0] == 144);
			sequence = BinaryPrimitives.ReadUInt16BigEndian(source.Slice(2));
			timestamp = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(4));
			ssrc = BinaryPrimitives.ReadUInt32BigEndian(source.Slice(8));
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00004DCE File Offset: 0x00002FCE
		public int CalculatePacketSize(int encryptedLength, EncryptionMode encryptionMode)
		{
			switch (encryptionMode)
			{
			case EncryptionMode.XSalsa20_Poly1305_Lite:
				return 12 + encryptedLength + 4;
			case EncryptionMode.XSalsa20_Poly1305_Suffix:
				return 12 + encryptedLength + Interop.SodiumNonceSize;
			case EncryptionMode.XSalsa20_Poly1305:
				return 12 + encryptedLength;
			default:
				throw new ArgumentException("Unsupported encryption mode.", "encryptionMode");
			}
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00004E0C File Offset: 0x0000300C
		public void GetDataFromPacket(ReadOnlySpan<byte> packet, out ReadOnlySpan<byte> data, EncryptionMode encryptionMode)
		{
			switch (encryptionMode)
			{
			case EncryptionMode.XSalsa20_Poly1305_Lite:
				data = packet.Slice(12, packet.Length - 12 - 4);
				return;
			case EncryptionMode.XSalsa20_Poly1305_Suffix:
				data = packet.Slice(12, packet.Length - 12 - Interop.SodiumNonceSize);
				return;
			case EncryptionMode.XSalsa20_Poly1305:
				data = packet.Slice(12);
				return;
			default:
				throw new ArgumentException("Unsupported encryption mode.", "encryptionMode");
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00004E88 File Offset: 0x00003088
		public void Dispose()
		{
		}

		// Token: 0x040000A2 RID: 162
		public const int HeaderSize = 12;

		// Token: 0x040000A3 RID: 163
		private const byte RtpNoExtension = 128;

		// Token: 0x040000A4 RID: 164
		private const byte RtpExtension = 144;

		// Token: 0x040000A5 RID: 165
		private const byte RtpVersion = 120;
	}
}
