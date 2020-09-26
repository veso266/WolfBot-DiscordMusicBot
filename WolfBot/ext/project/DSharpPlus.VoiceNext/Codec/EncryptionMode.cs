using System;

namespace DSharpPlus.VoiceNext.Codec
{
	/// <summary>
	/// Specifies an encryption mode to use with Sodium.
	/// </summary>
	// Token: 0x02000024 RID: 36
	public enum EncryptionMode
	{
		/// <summary>
		/// The nonce is an incrementing uint32 value. It is encoded as big endian value at the beginning of the nonce buffer. The 4 bytes are also appended at the end of the packet.
		/// </summary>
		// Token: 0x040000AB RID: 171
		XSalsa20_Poly1305_Lite,
		/// <summary>
		/// The nonce consists of random bytes. It is appended at the end of a packet.
		/// </summary>
		// Token: 0x040000AC RID: 172
		XSalsa20_Poly1305_Suffix,
		/// <summary>
		/// The nonce consists of the RTP header. Nothing is appended to the packet.
		/// </summary>
		// Token: 0x040000AD RID: 173
		XSalsa20_Poly1305
	}
}
