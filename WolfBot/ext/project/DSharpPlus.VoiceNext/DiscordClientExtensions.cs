using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DSharpPlus.VoiceNext
{
	// Token: 0x02000003 RID: 3
	public static class DiscordClientExtensions
	{
		/// <summary>
		/// Creates a new VoiceNext client with default settings.
		/// </summary>
		/// <param name="client">Discord client to create VoiceNext instance for.</param>
		/// <returns>VoiceNext client instance.</returns>
		// Token: 0x06000011 RID: 17 RVA: 0x00002258 File Offset: 0x00000458
		public static VoiceNextExtension UseVoiceNext(this DiscordClient client)
		{
			return client.UseVoiceNext(new VoiceNextConfiguration());
		}

		/// <summary>
		/// Creates a new VoiceNext client with specified settings.
		/// </summary>
		/// <param name="client">Discord client to create VoiceNext instance for.</param>
		/// <param name="config">Configuration for the VoiceNext client.</param>
		/// <returns>VoiceNext client instance.</returns>
		// Token: 0x06000012 RID: 18 RVA: 0x00002268 File Offset: 0x00000468
		public static VoiceNextExtension UseVoiceNext(this DiscordClient client, VoiceNextConfiguration config)
		{
			if (client.GetExtension<VoiceNextExtension>() != null)
			{
				throw new InvalidOperationException("VoiceNext is already enabled for that client.");
			}
			VoiceNextExtension voiceNextExtension = new VoiceNextExtension(config);
			client.AddExtension(voiceNextExtension);
			return voiceNextExtension;
		}

		/// <summary>
		/// Creates new VoiceNext clients on all shards in a given sharded client.
		/// </summary>
		/// <param name="client">Discord sharded client to create VoiceNext instances for.</param>
		/// <param name="config">Configuration for the VoiceNext clients.</param>
		/// <returns>A dictionary of created VoiceNext clients.</returns>
		// Token: 0x06000013 RID: 19 RVA: 0x00002298 File Offset: 0x00000498
		public static Task<IReadOnlyDictionary<int, VoiceNextExtension>> UseVoiceNextAsync(this DiscordShardedClient client, VoiceNextConfiguration config)
		{
			DiscordClientExtensions.<UseVoiceNextAsync>d__2 <UseVoiceNextAsync>d__;
			<UseVoiceNextAsync>d__.client = client;
			<UseVoiceNextAsync>d__.config = config;
			<UseVoiceNextAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IReadOnlyDictionary<int, VoiceNextExtension>>.Create();
			<UseVoiceNextAsync>d__.<>1__state = -1;
			<UseVoiceNextAsync>d__.<>t__builder.Start<DiscordClientExtensions.<UseVoiceNextAsync>d__2>(ref <UseVoiceNextAsync>d__);
			return <UseVoiceNextAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Gets the active instance of VoiceNext client for the DiscordClient.
		/// </summary>
		/// <param name="client">Discord client to get VoiceNext instance for.</param>
		/// <returns>VoiceNext client instance.</returns>
		// Token: 0x06000014 RID: 20 RVA: 0x000022E3 File Offset: 0x000004E3
		public static VoiceNextExtension GetVoiceNext(this DiscordClient client)
		{
			return client.GetExtension<VoiceNextExtension>();
		}

		/// <summary>
		/// Connects to this voice channel using VoiceNext.
		/// </summary>
		/// <param name="channel">Channel to connect to.</param>
		/// <returns>If successful, the VoiceNext connection.</returns>
		// Token: 0x06000015 RID: 21 RVA: 0x000022EC File Offset: 0x000004EC
		public static Task<VoiceNextConnection> ConnectAsync(this DiscordChannel channel)
		{
			if (channel == null)
			{
				throw new NullReferenceException();
			}
			if (channel.Guild == null)
			{
				throw new InvalidOperationException("VoiceNext can only be used with guild channels.");
			}
			if (channel.Type != 2)
			{
				throw new InvalidOperationException("You can only connect to voice channels.");
			}
			DiscordClient discordClient = channel.Discord as DiscordClient;
			if (discordClient == null || discordClient == null)
			{
				throw new NullReferenceException();
			}
			VoiceNextExtension voiceNext = discordClient.GetVoiceNext();
			if (voiceNext == null)
			{
				throw new InvalidOperationException("VoiceNext is not initialized for this Discord client.");
			}
			if (voiceNext.GetConnection(channel.Guild) != null)
			{
				throw new InvalidOperationException("VoiceNext is already connected in this guild.");
			}
			return voiceNext.ConnectAsync(channel);
		}
	}
}
