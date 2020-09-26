using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// Represents VoiceNext extension, which acts as Discord voice client.
	/// </summary>
	// Token: 0x02000009 RID: 9
	public sealed class VoiceNextExtension : BaseExtension
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00003763 File Offset: 0x00001963
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x0000376B File Offset: 0x0000196B
		private VoiceNextConfiguration Configuration { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00003774 File Offset: 0x00001974
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x0000377C File Offset: 0x0000197C
		private ConcurrentDictionary<ulong, VoiceNextConnection> ActiveConnections { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00003785 File Offset: 0x00001985
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x0000378D File Offset: 0x0000198D
		private ConcurrentDictionary<ulong, TaskCompletionSource<VoiceStateUpdateEventArgs>> VoiceStateUpdates { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003796 File Offset: 0x00001996
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x0000379E File Offset: 0x0000199E
		private ConcurrentDictionary<ulong, TaskCompletionSource<VoiceServerUpdateEventArgs>> VoiceServerUpdates { get; set; }

		/// <summary>
		/// Gets whether this connection has incoming voice enabled.
		/// </summary>
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000037A7 File Offset: 0x000019A7
		public bool IsIncomingEnabled { get; }

		// Token: 0x060000A8 RID: 168 RVA: 0x000037B0 File Offset: 0x000019B0
		internal VoiceNextExtension(VoiceNextConfiguration config)
		{
			this.Configuration = new VoiceNextConfiguration(config);
			this.IsIncomingEnabled = config.EnableIncoming;
			this.ActiveConnections = new ConcurrentDictionary<ulong, VoiceNextConnection>();
			this.VoiceStateUpdates = new ConcurrentDictionary<ulong, TaskCompletionSource<VoiceStateUpdateEventArgs>>();
			this.VoiceServerUpdates = new ConcurrentDictionary<ulong, TaskCompletionSource<VoiceServerUpdateEventArgs>>();
		}

		/// <summary>
		/// DO NOT USE THIS MANUALLY.
		/// </summary>
		/// <param name="client">DO NOT USE THIS MANUALLY.</param>
		/// <exception cref="T:System.InvalidOperationException" />
		// Token: 0x060000A9 RID: 169 RVA: 0x000037FC File Offset: 0x000019FC
		protected internal override void Setup(DiscordClient client)
		{
			if (base.Client != null)
			{
				throw new InvalidOperationException("What did I tell you?");
			}
			base.Client = client;
			base.Client.VoiceStateUpdated += new AsyncEventHandler<VoiceStateUpdateEventArgs>(this.Client_VoiceStateUpdate);
			base.Client.VoiceServerUpdated += new AsyncEventHandler<VoiceServerUpdateEventArgs>(this.Client_VoiceServerUpdate);
		}

		/// <summary>
		/// Create a VoiceNext connection for the specified channel.
		/// </summary>
		/// <param name="channel">Channel to connect to.</param>
		/// <returns>VoiceNext connection for this channel.</returns>
		// Token: 0x060000AA RID: 170 RVA: 0x00003854 File Offset: 0x00001A54
		public Task<VoiceNextConnection> ConnectAsync(DiscordChannel channel)
		{
			VoiceNextExtension.<ConnectAsync>d__21 <ConnectAsync>d__;
			<ConnectAsync>d__.<>4__this = this;
			<ConnectAsync>d__.channel = channel;
			<ConnectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<VoiceNextConnection>.Create();
			<ConnectAsync>d__.<>1__state = -1;
			<ConnectAsync>d__.<>t__builder.Start<VoiceNextExtension.<ConnectAsync>d__21>(ref <ConnectAsync>d__);
			return <ConnectAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Gets a VoiceNext connection for specified guild.
		/// </summary>
		/// <param name="guild">Guild to get VoiceNext connection for.</param>
		/// <returns>VoiceNext connection for the specified guild.</returns>
		// Token: 0x060000AB RID: 171 RVA: 0x0000389F File Offset: 0x00001A9F
		public VoiceNextConnection GetConnection(DiscordGuild guild)
		{
			if (this.ActiveConnections.ContainsKey(guild.Id))
			{
				return this.ActiveConnections[guild.Id];
			}
			return null;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000038C8 File Offset: 0x00001AC8
		private Task Vnc_VoiceDisconnected(DiscordGuild guild)
		{
			VoiceNextExtension.<Vnc_VoiceDisconnected>d__23 <Vnc_VoiceDisconnected>d__;
			<Vnc_VoiceDisconnected>d__.<>4__this = this;
			<Vnc_VoiceDisconnected>d__.guild = guild;
			<Vnc_VoiceDisconnected>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Vnc_VoiceDisconnected>d__.<>1__state = -1;
			<Vnc_VoiceDisconnected>d__.<>t__builder.Start<VoiceNextExtension.<Vnc_VoiceDisconnected>d__23>(ref <Vnc_VoiceDisconnected>d__);
			return <Vnc_VoiceDisconnected>d__.<>t__builder.Task;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003914 File Offset: 0x00001B14
		private Task Client_VoiceStateUpdate(VoiceStateUpdateEventArgs e)
		{
			DiscordGuild guild = e.Guild;
			if (guild == null)
			{
				return Task.CompletedTask;
			}
			if (e.User == null)
			{
				return Task.CompletedTask;
			}
			if (e.User.Id == base.Client.CurrentUser.Id)
			{
				VoiceNextConnection voiceNextConnection;
				if (e.After.Channel == null && this.ActiveConnections.TryRemove(guild.Id, out voiceNextConnection))
				{
					voiceNextConnection.Disconnect();
				}
				VoiceNextConnection voiceNextConnection2;
				if (this.ActiveConnections.TryGetValue(e.Guild.Id, out voiceNextConnection2))
				{
					voiceNextConnection2.Channel = e.Channel;
				}
				TaskCompletionSource<VoiceStateUpdateEventArgs> taskCompletionSource;
				if (!string.IsNullOrWhiteSpace(e.SessionId) && e.Channel != null && this.VoiceStateUpdates.TryRemove(guild.Id, out taskCompletionSource))
				{
					taskCompletionSource.SetResult(e);
				}
			}
			return Task.CompletedTask;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000039FC File Offset: 0x00001BFC
		private Task Client_VoiceServerUpdate(VoiceServerUpdateEventArgs e)
		{
			VoiceNextExtension.<Client_VoiceServerUpdate>d__25 <Client_VoiceServerUpdate>d__;
			<Client_VoiceServerUpdate>d__.<>4__this = this;
			<Client_VoiceServerUpdate>d__.e = e;
			<Client_VoiceServerUpdate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Client_VoiceServerUpdate>d__.<>1__state = -1;
			<Client_VoiceServerUpdate>d__.<>t__builder.Start<VoiceNextExtension.<Client_VoiceServerUpdate>d__25>(ref <Client_VoiceServerUpdate>d__);
			return <Client_VoiceServerUpdate>d__.<>t__builder.Task;
		}
	}
}
