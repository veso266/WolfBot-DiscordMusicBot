using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext.Codec;
using DSharpPlus.VoiceNext.Entities;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// Stream used to transmit audio data via <see cref="T:DSharpPlus.VoiceNext.VoiceNextConnection" />.
	/// </summary>
	// Token: 0x0200000A RID: 10
	public sealed class VoiceTransmitStream : Stream
	{
		/// <summary>
		/// Gets whether this stream can be read from. Always false.
		/// </summary>
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00003A47 File Offset: 0x00001C47
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets whether this stream can be seeked. Always false.
		/// </summary>
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00003A4A File Offset: 0x00001C4A
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets whether this stream can be written to. Always false.
		/// </summary>
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00003A4D File Offset: 0x00001C4D
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the length of the data in this stream. Always throws <see cref="T:System.InvalidOperationException" />.
		/// </summary>
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00003A50 File Offset: 0x00001C50
		public override long Length
		{
			get
			{
				throw new InvalidOperationException(string.Format("{0} cannot have a length.", base.GetType()));
			}
		}

		/// <summary>
		/// Gets the position in this stream. Always throws <see cref="T:System.InvalidOperationException" />.
		/// </summary>
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00003A67 File Offset: 0x00001C67
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00003A7E File Offset: 0x00001C7E
		public override long Position
		{
			get
			{
				throw new InvalidOperationException(string.Format("Cannot get position of {0}.", base.GetType()));
			}
			set
			{
				throw new InvalidOperationException(string.Format("Cannot seek {0}.", base.GetType()));
			}
		}

		/// <summary>
		/// Gets the PCM sample duration for this stream.
		/// </summary>
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00003A95 File Offset: 0x00001C95
		public int SampleDuration
		{
			get
			{
				return this.PcmBufferDuration;
			}
		}

		/// <summary>
		/// Gets or sets the volume modifier for this stream. Changing this will alter the volume of the output. 1.0 is 100%.
		/// </summary>
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00003A9D File Offset: 0x00001C9D
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00003AA5 File Offset: 0x00001CA5
		public double VolumeModifier
		{
			get
			{
				return this._volume;
			}
			set
			{
				if (value < 0.0 || value > 2.5)
				{
					throw new ArgumentOutOfRangeException("value", "Volume needs to be between 0% and 250%.");
				}
				this._volume = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x00003AD6 File Offset: 0x00001CD6
		private VoiceNextConnection Connection { get; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003ADE File Offset: 0x00001CDE
		private int PcmBufferDuration { get; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00003AE6 File Offset: 0x00001CE6
		private byte[] PcmBuffer { get; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003AEE File Offset: 0x00001CEE
		private Memory<byte> PcmMemory { get; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003AF6 File Offset: 0x00001CF6
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00003AFE File Offset: 0x00001CFE
		private int PcmBufferLength { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00003B07 File Offset: 0x00001D07
		private List<IVoiceFilter> Filters { get; }

		// Token: 0x060000BF RID: 191 RVA: 0x00003B10 File Offset: 0x00001D10
		internal VoiceTransmitStream(VoiceNextConnection vnc, int pcmBufferDuration)
		{
			this.Connection = vnc;
			this.PcmBufferDuration = pcmBufferDuration;
			this.PcmBuffer = new byte[vnc.AudioFormat.CalculateSampleSize(pcmBufferDuration)];
			this.PcmMemory = MemoryExtensions.AsMemory<byte>(this.PcmBuffer);
			this.PcmBufferLength = 0;
			this.Filters = new List<IVoiceFilter>();
		}

		/// <summary>
		/// Reads from the stream. Throws <see cref="T:System.InvalidOperationException" />.
		/// </summary>
		/// <param name="buffer">Buffer to read to.</param>
		/// <param name="offset">Offset to read to.</param>
		/// <param name="count">Number of bytes to read.</param>
		/// <returns>Number of bytes read.</returns>
		// Token: 0x060000C0 RID: 192 RVA: 0x00003B7D File Offset: 0x00001D7D
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException(string.Format("Cannot read from {0}.", base.GetType()));
		}

		/// <summary>
		/// Seeks the stream. Throws <see cref="T:System.InvalidOperationException" />.
		/// </summary>
		/// <param name="offset">Offset to seek to.</param>
		/// <param name="origin">Origin of seeking.</param>
		/// <returns>New position in the stream.</returns>
		// Token: 0x060000C1 RID: 193 RVA: 0x00003B94 File Offset: 0x00001D94
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException(string.Format("Cannot seek {0}.", base.GetType()));
		}

		/// <summary>
		/// Sets length of this stream. Throws <see cref="T:System.InvalidOperationException" />.
		/// </summary>
		/// <param name="value">Length to set.</param>
		// Token: 0x060000C2 RID: 194 RVA: 0x00003BAB File Offset: 0x00001DAB
		public override void SetLength(long value)
		{
			throw new InvalidOperationException(string.Format("Cannot set length of {0}.", base.GetType()));
		}

		/// <summary>
		/// Writes PCM data to the stream. The data is prepared for transmission, and enqueued.
		/// </summary>
		/// <param name="buffer">PCM data buffer to send.</param>
		/// <param name="offset">Start of the data in the buffer.</param>
		/// <param name="count">Number of bytes from the buffer.</param>
		// Token: 0x060000C3 RID: 195 RVA: 0x00003BC2 File Offset: 0x00001DC2
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.Write(new ReadOnlySpan<byte>(buffer, offset, count));
		}

		/// <summary>
		/// Writes PCM data to the stream. The data is prepared for transmission, and enqueued.
		/// </summary>
		/// <param name="buffer">PCM data buffer to send.</param>
		// Token: 0x060000C4 RID: 196 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public unsafe void Write(ReadOnlySpan<byte> buffer)
		{
			byte[] pcmBuffer = this.PcmBuffer;
			lock (pcmBuffer)
			{
				int i = buffer.Length;
				ReadOnlySpan<byte> readOnlySpan = buffer;
				Span<byte> span = this.PcmMemory.Span;
				while (i > 0)
				{
					int num = Math.Min(span.Length - this.PcmBufferLength, i);
					Span<byte> span2 = span.Slice(this.PcmBufferLength);
					readOnlySpan.Slice(0, num).CopyTo(span2);
					this.PcmBufferLength += num;
					i -= num;
					readOnlySpan = readOnlySpan.Slice(num);
					if (this.PcmBufferLength == this.PcmBuffer.Length)
					{
						Span<short> pcmData = MemoryMarshal.Cast<byte, short>(span);
						List<IVoiceFilter> filters = this.Filters;
						lock (filters)
						{
							if (this.Filters.Any<IVoiceFilter>())
							{
								foreach (IVoiceFilter voiceFilter in this.Filters)
								{
									voiceFilter.Transform(pcmData, this.Connection.AudioFormat, this.SampleDuration);
								}
							}
						}
						if (this.VolumeModifier != 1.0)
						{
							for (int j = 0; j < pcmData.Length; j++)
							{
								*pcmData[j] = (short)((double)(*pcmData[j]) * this.VolumeModifier);
							}
						}
						this.PcmBufferLength = 0;
						Memory<byte> memory = MemoryExtensions.AsMemory<byte>(new byte[span.Length]);
						this.Connection.PreparePacket(span, ref memory);
						this.Connection.EnqueuePacket(new VoicePacket(memory, this.PcmBufferDuration, false));
					}
				}
			}
		}

		/// <summary>
		/// Flushes the rest of the PCM data in this buffer to VoiceNext packet queue.
		/// </summary>
		// Token: 0x060000C5 RID: 197 RVA: 0x00003DEC File Offset: 0x00001FEC
		public unsafe override void Flush()
		{
			Span<byte> span = this.PcmMemory.Span;
			Helpers.ZeroFill(span.Slice(this.PcmBufferLength));
			Span<short> pcmData = MemoryMarshal.Cast<byte, short>(span);
			List<IVoiceFilter> filters = this.Filters;
			lock (filters)
			{
				if (this.Filters.Any<IVoiceFilter>())
				{
					foreach (IVoiceFilter voiceFilter in this.Filters)
					{
						voiceFilter.Transform(pcmData, this.Connection.AudioFormat, this.SampleDuration);
					}
				}
			}
			if (this.VolumeModifier != 1.0)
			{
				for (int i = 0; i < pcmData.Length; i++)
				{
					*pcmData[i] = (short)((double)(*pcmData[i]) * this.VolumeModifier);
				}
			}
			Memory<byte> memory = MemoryExtensions.AsMemory<byte>(new byte[span.Length]);
			this.Connection.PreparePacket(span, ref memory);
			this.Connection.EnqueuePacket(new VoicePacket(memory, this.PcmBufferDuration, false));
		}

		/// <summary>
		/// Pauses playback.
		/// </summary>
		// Token: 0x060000C6 RID: 198 RVA: 0x00003F38 File Offset: 0x00002138
		public void Pause()
		{
			this.Connection.Pause();
		}

		/// <summary>
		/// Resumes playback.
		/// </summary>
		/// <returns></returns>
		// Token: 0x060000C7 RID: 199 RVA: 0x00003F48 File Offset: 0x00002148
		public Task ResumeAsync()
		{
			VoiceTransmitStream.<ResumeAsync>d__44 <ResumeAsync>d__;
			<ResumeAsync>d__.<>4__this = this;
			<ResumeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResumeAsync>d__.<>1__state = -1;
			<ResumeAsync>d__.<>t__builder.Start<VoiceTransmitStream.<ResumeAsync>d__44>(ref <ResumeAsync>d__);
			return <ResumeAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Gets the collection of installed PCM filters, in order of their execution.
		/// </summary>
		/// <returns>Installed PCM filters, in order of execution.</returns>
		// Token: 0x060000C8 RID: 200 RVA: 0x00003F8B File Offset: 0x0000218B
		public IEnumerable<IVoiceFilter> GetInstalledFilters()
		{
			foreach (IVoiceFilter voiceFilter in this.Filters)
			{
				yield return voiceFilter;
			}
			List<IVoiceFilter>.Enumerator enumerator = default(List<IVoiceFilter>.Enumerator);
			yield break;
			yield break;
		}

		/// <summary>
		/// Installs a new PCM filter, with specified execution order.
		/// </summary>
		/// <param name="filter">Filter to install.</param>
		/// <param name="order">Order of the new filter. This determines where the filter will be inserted in the filter stream.</param>
		// Token: 0x060000C9 RID: 201 RVA: 0x00003F9C File Offset: 0x0000219C
		public void InstallFilter(IVoiceFilter filter, int order = 2147483647)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException("order", "Filter order must be greater than or equal to 0.");
			}
			List<IVoiceFilter> filters = this.Filters;
			lock (filters)
			{
				List<IVoiceFilter> filters2 = this.Filters;
				if (order >= filters2.Count)
				{
					filters2.Add(filter);
				}
				else
				{
					filters2.Insert(order, filter);
				}
			}
		}

		/// <summary>
		/// Uninstalls an installed PCM filter.
		/// </summary>
		/// <param name="filter">Filter to uninstall.</param>
		/// <returns>Whether the filter was uninstalled.</returns>
		// Token: 0x060000CA RID: 202 RVA: 0x0000401C File Offset: 0x0000221C
		public bool UninstallFilter(IVoiceFilter filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			List<IVoiceFilter> filters = this.Filters;
			bool result;
			lock (filters)
			{
				List<IVoiceFilter> filters2 = this.Filters;
				if (!filters2.Contains(filter))
				{
					result = false;
				}
				else
				{
					result = filters2.Remove(filter);
				}
			}
			return result;
		}

		// Token: 0x04000045 RID: 69
		private double _volume = 1.0;
	}
}
