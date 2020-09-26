using System;

namespace DSharpPlus.VoiceNext.Codec
{
	/// <summary>
	/// Represents an Opus decoder.
	/// </summary>
	// Token: 0x0200001E RID: 30
	public class OpusDecoder : IDisposable
	{
		/// <summary>
		/// Gets the audio format produced by this decoder.
		/// </summary>
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00004BD4 File Offset: 0x00002DD4
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00004BDC File Offset: 0x00002DDC
		public AudioFormat AudioFormat { get; private set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00004BE5 File Offset: 0x00002DE5
		internal Opus Opus { get; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00004BED File Offset: 0x00002DED
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00004BF5 File Offset: 0x00002DF5
		internal IntPtr Decoder { get; private set; }

		// Token: 0x06000172 RID: 370 RVA: 0x00004BFE File Offset: 0x00002DFE
		internal OpusDecoder(Opus managedOpus)
		{
			this.Opus = managedOpus;
		}

		/// <summary>
		/// Used to lazily initialize the decoder to make sure we're
		/// using the correct output format, this way we don't end up
		/// creating more decoders than we need.
		/// </summary>
		/// <param name="outputFormat"></param>
		// Token: 0x06000173 RID: 371 RVA: 0x00004C0D File Offset: 0x00002E0D
		internal void Initialize(AudioFormat outputFormat)
		{
			if (this.Decoder != IntPtr.Zero)
			{
				Interop.OpusDestroyDecoder(this.Decoder);
			}
			this.AudioFormat = outputFormat;
			this.Decoder = Interop.OpusCreateDecoder(outputFormat);
		}

		/// <summary>
		/// Disposes of this Opus decoder.
		/// </summary>
		// Token: 0x06000174 RID: 372 RVA: 0x00004C3F File Offset: 0x00002E3F
		public void Dispose()
		{
			if (this._isDisposed)
			{
				return;
			}
			this._isDisposed = true;
			if (this.Decoder != IntPtr.Zero)
			{
				Interop.OpusDestroyDecoder(this.Decoder);
			}
		}

		// Token: 0x0400008C RID: 140
		private volatile bool _isDisposed;
	}
}
