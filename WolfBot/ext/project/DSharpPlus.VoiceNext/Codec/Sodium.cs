using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace DSharpPlus.VoiceNext.Codec
{
	// Token: 0x02000023 RID: 35
	internal sealed class Sodium : IDisposable
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00004E8A File Offset: 0x0000308A
		public static IReadOnlyDictionary<string, EncryptionMode> SupportedModes { get; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00004E91 File Offset: 0x00003091
		public static int NonceSize
		{
			get
			{
				return Interop.SodiumNonceSize;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00004E98 File Offset: 0x00003098
		private RandomNumberGenerator CSPRNG { get; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00004EA0 File Offset: 0x000030A0
		private byte[] Buffer { get; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00004EA8 File Offset: 0x000030A8
		private ReadOnlyMemory<byte> Key { get; }

		// Token: 0x06000181 RID: 385 RVA: 0x00004EB0 File Offset: 0x000030B0
		static Sodium()
		{
			Dictionary<string, EncryptionMode> dictionary = new Dictionary<string, EncryptionMode>();
			dictionary["xsalsa20_poly1305_lite"] = EncryptionMode.XSalsa20_Poly1305_Lite;
			dictionary["xsalsa20_poly1305_suffix"] = EncryptionMode.XSalsa20_Poly1305_Suffix;
			dictionary["xsalsa20_poly1305"] = EncryptionMode.XSalsa20_Poly1305;
			Sodium.SupportedModes = new ReadOnlyDictionary<string, EncryptionMode>(dictionary);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00004EE8 File Offset: 0x000030E8
		public Sodium(ReadOnlyMemory<byte> key)
		{
			if (key.Length != Interop.SodiumKeySize)
			{
				throw new ArgumentException(string.Format("Invalid Sodium key size. Key needs to have a length of {0} bytes.", Interop.SodiumKeySize), "key");
			}
			this.Key = key;
			this.CSPRNG = RandomNumberGenerator.Create();
			this.Buffer = new byte[Interop.SodiumNonceSize];
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00004F4C File Offset: 0x0000314C
		public void GenerateNonce(ReadOnlySpan<byte> rtpHeader, Span<byte> target)
		{
			if (rtpHeader.Length != 12)
			{
				throw new ArgumentException(string.Format("RTP header needs to have a length of exactly {0} bytes.", 12), "rtpHeader");
			}
			if (target.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce buffer size. Target buffer for the nonce needs to have a capacity of {0} bytes.", Interop.SodiumNonceSize), "target");
			}
			rtpHeader.CopyTo(target);
			Helpers.ZeroFill(target.Slice(rtpHeader.Length));
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00004FC8 File Offset: 0x000031C8
		public void GenerateNonce(Span<byte> target)
		{
			if (target.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce buffer size. Target buffer for the nonce needs to have a capacity of {0} bytes.", Interop.SodiumNonceSize), "target");
			}
			this.CSPRNG.GetBytes(this.Buffer);
			MemoryExtensions.AsSpan<byte>(this.Buffer).CopyTo(target);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00005028 File Offset: 0x00003228
		public void GenerateNonce(uint nonce, Span<byte> target)
		{
			if (target.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce buffer size. Target buffer for the nonce needs to have a capacity of {0} bytes.", Interop.SodiumNonceSize), "target");
			}
			BinaryPrimitives.WriteUInt32BigEndian(target, nonce);
			Helpers.ZeroFill(target.Slice(4));
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00005078 File Offset: 0x00003278
		public void AppendNonce(ReadOnlySpan<byte> nonce, Span<byte> target, EncryptionMode encryptionMode)
		{
			switch (encryptionMode)
			{
			case EncryptionMode.XSalsa20_Poly1305_Lite:
				nonce.Slice(0, 4).CopyTo(target.Slice(target.Length - 4));
				return;
			case EncryptionMode.XSalsa20_Poly1305_Suffix:
				nonce.CopyTo(target.Slice(target.Length - 12));
				return;
			case EncryptionMode.XSalsa20_Poly1305:
				return;
			default:
				throw new ArgumentException("Unsupported encryption mode.", "encryptionMode");
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x000050E4 File Offset: 0x000032E4
		public void GetNonce(ReadOnlySpan<byte> source, Span<byte> target, EncryptionMode encryptionMode)
		{
			if (target.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce buffer size. Target buffer for the nonce needs to have a capacity of {0} bytes.", Interop.SodiumNonceSize), "target");
			}
			switch (encryptionMode)
			{
			case EncryptionMode.XSalsa20_Poly1305_Lite:
				source.Slice(source.Length - 4).CopyTo(target);
				return;
			case EncryptionMode.XSalsa20_Poly1305_Suffix:
				source.Slice(source.Length - Interop.SodiumNonceSize).CopyTo(target);
				return;
			case EncryptionMode.XSalsa20_Poly1305:
				source.Slice(0, 12).CopyTo(target);
				return;
			default:
				throw new ArgumentException("Unsupported encryption mode.", "encryptionMode");
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00005190 File Offset: 0x00003390
		public void Encrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> nonce)
		{
			if (nonce.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce size. Nonce needs to have a length of {0} bytes.", Interop.SodiumNonceSize), "nonce");
			}
			if (target.Length != Interop.SodiumMacSize + source.Length)
			{
				throw new ArgumentException(string.Format("Invalid target buffer size. Target buffer needs to have a length that is a sum of input buffer length and Sodium MAC size ({0} bytes).", Interop.SodiumMacSize), "target");
			}
			int num;
			if ((num = Interop.Encrypt(source, target, this.Key.Span, nonce)) != 0)
			{
				throw new CryptographicException(string.Format("Could not encrypt the buffer. Sodium returned code {0}.", num));
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00005230 File Offset: 0x00003430
		public void Decrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> nonce)
		{
			if (nonce.Length != Interop.SodiumNonceSize)
			{
				throw new ArgumentException(string.Format("Invalid nonce size. Nonce needs to have a length of {0} bytes.", Interop.SodiumNonceSize), "nonce");
			}
			if (target.Length != source.Length - Interop.SodiumMacSize)
			{
				throw new ArgumentException(string.Format("Invalid target buffer size. Target buffer needs to have a length that is input buffer decreased by Sodium MAC size ({0} bytes).", Interop.SodiumMacSize), "target");
			}
			int num;
			if ((num = Interop.Decrypt(source, target, this.Key.Span, nonce)) != 0)
			{
				throw new CryptographicException(string.Format("Could not decrypt the buffer. Sodium returned code {0}.", num));
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000052CF File Offset: 0x000034CF
		public void Dispose()
		{
			this.CSPRNG.Dispose();
		}

		// Token: 0x0600018B RID: 395 RVA: 0x000052DC File Offset: 0x000034DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static KeyValuePair<string, EncryptionMode> SelectMode(IEnumerable<string> availableModes)
		{
			foreach (KeyValuePair<string, EncryptionMode> result in Sodium.SupportedModes)
			{
				if (availableModes.Contains(result.Key))
				{
					return result;
				}
			}
			throw new CryptographicException("Could not negotiate Sodium encryption modes, as none of the modes offered by Discord are supported. This is usually an indicator that something went very wrong.");
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00005340 File Offset: 0x00003540
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CalculateTargetSize(ReadOnlySpan<byte> source)
		{
			return source.Length + Interop.SodiumMacSize;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000534F File Offset: 0x0000354F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CalculateSourceSize(ReadOnlySpan<byte> source)
		{
			return source.Length - Interop.SodiumMacSize;
		}
	}
}
