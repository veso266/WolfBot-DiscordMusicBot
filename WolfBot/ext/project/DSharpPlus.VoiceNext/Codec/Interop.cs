using System;
using System.Runtime.InteropServices;

namespace DSharpPlus.VoiceNext.Codec
{
	/// <summary>
	/// This is an interop class. It contains wrapper methods for Opus and Sodium.
	/// </summary>
	// Token: 0x0200001C RID: 28
	internal static class Interop
	{
		/// <summary>
		/// Gets the Sodium key size for xsalsa20_poly1305 algorithm.
		/// </summary>
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000144 RID: 324 RVA: 0x000044FB File Offset: 0x000026FB
		public static int SodiumKeySize { get; } = (uint)Interop._SodiumSecretBoxKeySize();

		/// <summary>
		/// Gets the Sodium nonce size for xsalsa20_poly1305 algorithm.
		/// </summary>
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00004502 File Offset: 0x00002702
		public static int SodiumNonceSize { get; } = (uint)Interop._SodiumSecretBoxNonceSize();

		/// <summary>
		/// Gets the Sodium MAC size for xsalsa20_poly1305 algorithm.
		/// </summary>
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00004509 File Offset: 0x00002709
		public static int SodiumMacSize { get; } = (uint)Interop._SodiumSecretBoxMacSize();

		// Token: 0x06000147 RID: 327
		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_xsalsa20poly1305_keybytes")]
		[return: MarshalAs(UnmanagedType.SysUInt)]
		private static extern UIntPtr _SodiumSecretBoxKeySize();

		// Token: 0x06000148 RID: 328
		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_xsalsa20poly1305_noncebytes")]
		[return: MarshalAs(UnmanagedType.SysUInt)]
		private static extern UIntPtr _SodiumSecretBoxNonceSize();

		// Token: 0x06000149 RID: 329
		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_xsalsa20poly1305_macbytes")]
		[return: MarshalAs(UnmanagedType.SysUInt)]
		private static extern UIntPtr _SodiumSecretBoxMacSize();

		// Token: 0x0600014A RID: 330
		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_easy")]
		private unsafe static extern int _SodiumSecretBoxCreate(byte* buffer, byte* message, ulong messageLength, byte* nonce, byte* key);

		// Token: 0x0600014B RID: 331
		[DllImport("libsodium", CallingConvention = CallingConvention.Cdecl, EntryPoint = "crypto_secretbox_open_easy")]
		private unsafe static extern int _SodiumSecretBoxOpen(byte* buffer, byte* encryptedMessage, ulong encryptedLength, byte* nonce, byte* key);

		/// <summary>
		/// Encrypts supplied buffer using xsalsa20_poly1305 algorithm, using supplied key and nonce to perform encryption.
		/// </summary>
		/// <param name="source">Contents to encrypt.</param>
		/// <param name="target">Buffer to encrypt to.</param>
		/// <param name="key">Key to use for encryption.</param>
		/// <param name="nonce">Nonce to use for encryption.</param>
		/// <returns>Encryption status.</returns>
		// Token: 0x0600014C RID: 332 RVA: 0x00004510 File Offset: 0x00002710
		public unsafe static int Encrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce)
		{
			int result;
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* message = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* buffer = pinnableReference2;
					fixed (byte* pinnableReference3 = key.GetPinnableReference())
					{
						byte* key2 = pinnableReference3;
						fixed (byte* pinnableReference4 = nonce.GetPinnableReference())
						{
							byte* nonce2 = pinnableReference4;
							result = Interop._SodiumSecretBoxCreate(buffer, message, (ulong)((long)source.Length), nonce2, key2);
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Decrypts supplied buffer using xsalsa20_poly1305 algorithm, using supplied key and nonce to perform decryption.
		/// </summary>
		/// <param name="source">Buffer to decrypt from.</param>
		/// <param name="target">Decrypted message buffer.</param>
		/// <param name="key">Key to use for decryption.</param>
		/// <param name="nonce">Nonce to use for decryption.</param>
		/// <returns>Decryption status.</returns>
		// Token: 0x0600014D RID: 333 RVA: 0x0000456C File Offset: 0x0000276C
		public unsafe static int Decrypt(ReadOnlySpan<byte> source, Span<byte> target, ReadOnlySpan<byte> key, ReadOnlySpan<byte> nonce)
		{
			int result;
			fixed (byte* pinnableReference = source.GetPinnableReference())
			{
				byte* encryptedMessage = pinnableReference;
				fixed (byte* pinnableReference2 = target.GetPinnableReference())
				{
					byte* buffer = pinnableReference2;
					fixed (byte* pinnableReference3 = key.GetPinnableReference())
					{
						byte* key2 = pinnableReference3;
						fixed (byte* pinnableReference4 = nonce.GetPinnableReference())
						{
							byte* nonce2 = pinnableReference4;
							result = Interop._SodiumSecretBoxOpen(buffer, encryptedMessage, (ulong)((long)source.Length), nonce2, key2);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600014E RID: 334
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_create")]
		private static extern IntPtr _OpusCreateEncoder(int sampleRate, int channels, int application, out OpusError error);

		// Token: 0x0600014F RID: 335
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_destroy")]
		public static extern void OpusDestroyEncoder(IntPtr encoder);

		// Token: 0x06000150 RID: 336
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encode")]
		private unsafe static extern int _OpusEncode(IntPtr encoder, byte* pcmData, int frameSize, byte* data, int maxDataBytes);

		// Token: 0x06000151 RID: 337
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_encoder_ctl")]
		private static extern OpusError _OpusEncoderControl(IntPtr encoder, OpusControl request, int value);

		// Token: 0x06000152 RID: 338
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decoder_create")]
		private static extern IntPtr _OpusCreateDecoder(int sampleRate, int channels, out OpusError error);

		// Token: 0x06000153 RID: 339
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decoder_destroy")]
		public static extern void OpusDestroyDecoder(IntPtr decoder);

		// Token: 0x06000154 RID: 340
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decode")]
		private unsafe static extern int _OpusDecode(IntPtr decoder, byte* opusData, int opusDataLength, byte* data, int frameSize, int decodeFec);

		// Token: 0x06000155 RID: 341
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_packet_get_nb_channels")]
		private unsafe static extern int _OpusGetPacketChanelCount(byte* opusData);

		// Token: 0x06000156 RID: 342
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_packet_get_nb_frames")]
		private unsafe static extern int _OpusGetPacketFrameCount(byte* opusData, int length);

		// Token: 0x06000157 RID: 343
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_packet_get_samples_per_frame")]
		private unsafe static extern int _OpusGetPacketSamplePerFrameCount(byte* opusData, int samplingRate);

		// Token: 0x06000158 RID: 344
		[DllImport("libopus", CallingConvention = CallingConvention.Cdecl, EntryPoint = "opus_decoder_ctl")]
		private static extern int _OpusDecoderControl(IntPtr decoder, OpusControl request, out int value);

		// Token: 0x06000159 RID: 345 RVA: 0x000045C8 File Offset: 0x000027C8
		public static IntPtr OpusCreateEncoder(AudioFormat audioFormat)
		{
			OpusError opusError;
			IntPtr result = Interop._OpusCreateEncoder(audioFormat.SampleRate, audioFormat.ChannelCount, (int)audioFormat.VoiceApplication, out opusError);
			if (opusError != OpusError.Ok)
			{
				throw new Exception(string.Format("Could not instantiate Opus encoder: {0} ({1}).", opusError, (int)opusError));
			}
			return result;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00004610 File Offset: 0x00002810
		public static void OpusSetEncoderOption(IntPtr encoder, OpusControl option, int value)
		{
			OpusError opusError;
			if ((opusError = Interop._OpusEncoderControl(encoder, option, value)) != OpusError.Ok)
			{
				throw new Exception(string.Format("Could not set Opus encoder option: {0} ({1}).", opusError, (int)opusError));
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00004648 File Offset: 0x00002848
		public unsafe static void OpusEncode(IntPtr encoder, ReadOnlySpan<byte> pcm, int frameSize, ref Span<byte> opus)
		{
			int num;
			fixed (byte* pinnableReference = pcm.GetPinnableReference())
			{
				byte* pcmData = pinnableReference;
				fixed (byte* pinnableReference2 = opus.GetPinnableReference())
				{
					byte* data = pinnableReference2;
					num = Interop._OpusEncode(encoder, pcmData, frameSize, data, opus.Length);
				}
			}
			if (num < 0)
			{
				OpusError opusError = (OpusError)num;
				throw new Exception(string.Format("Could not encode PCM data to Opus: {0} ({1}).", opusError, (int)opusError));
			}
			opus = opus.Slice(0, num);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000046B8 File Offset: 0x000028B8
		public static IntPtr OpusCreateDecoder(AudioFormat audioFormat)
		{
			OpusError opusError;
			IntPtr result = Interop._OpusCreateDecoder(audioFormat.SampleRate, audioFormat.ChannelCount, out opusError);
			if (opusError != OpusError.Ok)
			{
				throw new Exception(string.Format("Could not instantiate Opus decoder: {0} ({1}).", opusError, (int)opusError));
			}
			return result;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000046FC File Offset: 0x000028FC
		public unsafe static int OpusDecode(IntPtr decoder, ReadOnlySpan<byte> opus, int frameSize, Span<byte> pcm, bool useFec)
		{
			int num;
			fixed (byte* pinnableReference = opus.GetPinnableReference())
			{
				byte* opusData = pinnableReference;
				fixed (byte* pinnableReference2 = pcm.GetPinnableReference())
				{
					byte* data = pinnableReference2;
					num = Interop._OpusDecode(decoder, opusData, opus.Length, data, frameSize, useFec ? 1 : 0);
				}
			}
			if (num < 0)
			{
				OpusError opusError = (OpusError)num;
				throw new Exception(string.Format("Could not decode PCM data from Opus: {0} ({1}).", opusError, (int)opusError));
			}
			return num;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000476C File Offset: 0x0000296C
		public unsafe static int OpusDecode(IntPtr decoder, int frameSize, Span<byte> pcm)
		{
			int num;
			fixed (byte* pinnableReference = pcm.GetPinnableReference())
			{
				byte* data = pinnableReference;
				num = Interop._OpusDecode(decoder, null, 0, data, frameSize, 1);
			}
			if (num < 0)
			{
				OpusError opusError = (OpusError)num;
				throw new Exception(string.Format("Could not decode PCM data from Opus: {0} ({1}).", opusError, (int)opusError));
			}
			return num;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x000047BC File Offset: 0x000029BC
		public unsafe static void OpusGetPacketMetrics(ReadOnlySpan<byte> opus, int samplingRate, out int channels, out int frames, out int samplesPerFrame, out int frameSize)
		{
			fixed (byte* pinnableReference = opus.GetPinnableReference())
			{
				byte* opusData = pinnableReference;
				frames = Interop._OpusGetPacketFrameCount(opusData, opus.Length);
				samplesPerFrame = Interop._OpusGetPacketSamplePerFrameCount(opusData, samplingRate);
				channels = Interop._OpusGetPacketChanelCount(opusData);
			}
			frameSize = frames * samplesPerFrame;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00004801 File Offset: 0x00002A01
		public static void OpusGetLastPacketDuration(IntPtr decoder, out int sampleCount)
		{
			Interop._OpusDecoderControl(decoder, OpusControl.GetLastPacketDuration, out sampleCount);
		}

		// Token: 0x04000081 RID: 129
		private const string SodiumLibraryName = "libsodium";

		// Token: 0x04000085 RID: 133
		private const string OpusLibraryName = "libopus";
	}
}
