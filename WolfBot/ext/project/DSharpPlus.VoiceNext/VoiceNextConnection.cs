using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using DSharpPlus.Net.Udp;
using DSharpPlus.Net.WebSocket;
using DSharpPlus.VoiceNext.Codec;
using DSharpPlus.VoiceNext.Entities;
using DSharpPlus.VoiceNext.EventArgs;
using Newtonsoft.Json.Linq;

namespace DSharpPlus.VoiceNext
{
	/// <summary>
	/// VoiceNext connection to a voice channel.
	/// </summary>
	// Token: 0x02000008 RID: 8
	public sealed class VoiceNextConnection : IDisposable
	{
		/// <summary>
		/// Triggered whenever a user speaks in the connected voice channel.
		/// </summary>
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000021 RID: 33 RVA: 0x00002417 File Offset: 0x00000617
		// (remove) Token: 0x06000022 RID: 34 RVA: 0x00002425 File Offset: 0x00000625
		public event AsyncEventHandler<UserSpeakingEventArgs> UserSpeaking
		{
			add
			{
				this._userSpeaking.Register(value);
			}
			remove
			{
				this._userSpeaking.Unregister(value);
			}
		}

		/// <summary>
		/// Triggered whenever a user joins voice in the connected guild.
		/// </summary>
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000023 RID: 35 RVA: 0x00002433 File Offset: 0x00000633
		// (remove) Token: 0x06000024 RID: 36 RVA: 0x00002441 File Offset: 0x00000641
		public event AsyncEventHandler<VoiceUserJoinEventArgs> UserJoined
		{
			add
			{
				this._userJoined.Register(value);
			}
			remove
			{
				this._userJoined.Unregister(value);
			}
		}

		/// <summary>
		/// Triggered whenever a user leaves voice in the connected guild.
		/// </summary>
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000025 RID: 37 RVA: 0x0000244F File Offset: 0x0000064F
		// (remove) Token: 0x06000026 RID: 38 RVA: 0x0000245D File Offset: 0x0000065D
		public event AsyncEventHandler<VoiceUserLeaveEventArgs> UserLeft
		{
			add
			{
				this._userLeft.Register(value);
			}
			remove
			{
				this._userLeft.Unregister(value);
			}
		}

		/// <summary>
		/// Triggered whenever voice data is received from the connected voice channel.
		/// </summary>
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000027 RID: 39 RVA: 0x0000246B File Offset: 0x0000066B
		// (remove) Token: 0x06000028 RID: 40 RVA: 0x00002479 File Offset: 0x00000679
		public event AsyncEventHandler<VoiceReceiveEventArgs> VoiceReceived
		{
			add
			{
				this._voiceReceived.Register(value);
			}
			remove
			{
				this._voiceReceived.Unregister(value);
			}
		}

		/// <summary>
		/// Triggered whenever voice WebSocket throws an exception.
		/// </summary>
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000029 RID: 41 RVA: 0x00002487 File Offset: 0x00000687
		// (remove) Token: 0x0600002A RID: 42 RVA: 0x00002495 File Offset: 0x00000695
		public event AsyncEventHandler<SocketErrorEventArgs> VoiceSocketErrored
		{
			add
			{
				this._voiceSocketError.Register(value);
			}
			remove
			{
				this._voiceSocketError.Unregister(value);
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600002B RID: 43 RVA: 0x000024A4 File Offset: 0x000006A4
		// (remove) Token: 0x0600002C RID: 44 RVA: 0x000024DC File Offset: 0x000006DC
		internal event VoiceDisconnectedEventHandler VoiceDisconnected;

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002511 File Offset: 0x00000711
		private static DateTimeOffset UnixEpoch { get; } = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002518 File Offset: 0x00000718
		private DiscordClient Discord { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002520 File Offset: 0x00000720
		private DiscordGuild Guild { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002528 File Offset: 0x00000728
		private ConcurrentDictionary<uint, AudioSender> TransmittingSSRCs { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002530 File Offset: 0x00000730
		private BaseUdpClient UdpClient { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002538 File Offset: 0x00000738
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002540 File Offset: 0x00000740
		private IWebSocketClient VoiceWs { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002549 File Offset: 0x00000749
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002551 File Offset: 0x00000751
		private Task HeartbeatTask { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000036 RID: 54 RVA: 0x0000255A File Offset: 0x0000075A
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002562 File Offset: 0x00000762
		private int HeartbeatInterval { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000038 RID: 56 RVA: 0x0000256B File Offset: 0x0000076B
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002573 File Offset: 0x00000773
		private DateTimeOffset LastHeartbeat { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003A RID: 58 RVA: 0x0000257C File Offset: 0x0000077C
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002584 File Offset: 0x00000784
		private CancellationTokenSource TokenSource { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000258D File Offset: 0x0000078D
		private CancellationToken Token
		{
			get
			{
				return this.TokenSource.Token;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003D RID: 61 RVA: 0x0000259A File Offset: 0x0000079A
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000025A2 File Offset: 0x000007A2
		internal VoiceServerUpdatePayload ServerData { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003F RID: 63 RVA: 0x000025AB File Offset: 0x000007AB
		// (set) Token: 0x06000040 RID: 64 RVA: 0x000025B3 File Offset: 0x000007B3
		internal VoiceStateUpdatePayload StateData { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000041 RID: 65 RVA: 0x000025BC File Offset: 0x000007BC
		// (set) Token: 0x06000042 RID: 66 RVA: 0x000025C4 File Offset: 0x000007C4
		internal bool Resume { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000025CD File Offset: 0x000007CD
		private VoiceNextConfiguration Configuration { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000025D5 File Offset: 0x000007D5
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000025DD File Offset: 0x000007DD
		private Opus Opus { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000025E6 File Offset: 0x000007E6
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000025EE File Offset: 0x000007EE
		private Sodium Sodium { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000025F7 File Offset: 0x000007F7
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000025FF File Offset: 0x000007FF
		private Rtp Rtp { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002608 File Offset: 0x00000808
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002610 File Offset: 0x00000810
		private EncryptionMode SelectedEncryptionMode { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002619 File Offset: 0x00000819
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002621 File Offset: 0x00000821
		private uint Nonce { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000262A File Offset: 0x0000082A
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00002632 File Offset: 0x00000832
		private ushort Sequence { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000050 RID: 80 RVA: 0x0000263B File Offset: 0x0000083B
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002643 File Offset: 0x00000843
		private uint Timestamp { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000052 RID: 82 RVA: 0x0000264C File Offset: 0x0000084C
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002654 File Offset: 0x00000854
		private uint SSRC { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000054 RID: 84 RVA: 0x0000265D File Offset: 0x0000085D
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002665 File Offset: 0x00000865
		private byte[] Key { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000056 RID: 86 RVA: 0x0000266E File Offset: 0x0000086E
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002676 File Offset: 0x00000876
		private IpEndpoint DiscoveredEndpoint { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000058 RID: 88 RVA: 0x0000267F File Offset: 0x0000087F
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00002687 File Offset: 0x00000887
		internal ConnectionEndpoint WebSocketEndpoint { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002690 File Offset: 0x00000890
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002698 File Offset: 0x00000898
		internal ConnectionEndpoint UdpEndpoint { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000026A1 File Offset: 0x000008A1
		// (set) Token: 0x0600005D RID: 93 RVA: 0x000026A9 File Offset: 0x000008A9
		private TaskCompletionSource<bool> ReadyWait { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000026B2 File Offset: 0x000008B2
		// (set) Token: 0x0600005F RID: 95 RVA: 0x000026BA File Offset: 0x000008BA
		private bool IsInitialized { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000026C3 File Offset: 0x000008C3
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000026CB File Offset: 0x000008CB
		private bool IsDisposed { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000026D4 File Offset: 0x000008D4
		// (set) Token: 0x06000063 RID: 99 RVA: 0x000026DC File Offset: 0x000008DC
		private TaskCompletionSource<bool> PlayingWait { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000026E5 File Offset: 0x000008E5
		private AsyncManualResetEvent PauseEvent { get; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000026ED File Offset: 0x000008ED
		private ConcurrentQueue<VoicePacket> PacketQueue { get; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000026F5 File Offset: 0x000008F5
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000026FD File Offset: 0x000008FD
		private VoiceTransmitStream TransmitStream { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00002706 File Offset: 0x00000906
		private ConcurrentDictionary<ulong, long> KeepaliveTimestamps { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000270E File Offset: 0x0000090E
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002716 File Offset: 0x00000916
		private Task SenderTask { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000271F File Offset: 0x0000091F
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00002727 File Offset: 0x00000927
		private CancellationTokenSource SenderTokenSource { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002730 File Offset: 0x00000930
		private CancellationToken SenderToken
		{
			get
			{
				return this.SenderTokenSource.Token;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000273D File Offset: 0x0000093D
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00002745 File Offset: 0x00000945
		private Task ReceiverTask { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000070 RID: 112 RVA: 0x0000274E File Offset: 0x0000094E
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00002756 File Offset: 0x00000956
		private CancellationTokenSource ReceiverTokenSource { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000072 RID: 114 RVA: 0x0000275F File Offset: 0x0000095F
		private CancellationToken ReceiverToken
		{
			get
			{
				return this.ReceiverTokenSource.Token;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000276C File Offset: 0x0000096C
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00002774 File Offset: 0x00000974
		private Task KeepaliveTask { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000277D File Offset: 0x0000097D
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00002785 File Offset: 0x00000985
		private CancellationTokenSource KeepaliveTokenSource { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000077 RID: 119 RVA: 0x0000278E File Offset: 0x0000098E
		private CancellationToken KeepaliveToken
		{
			get
			{
				return this.KeepaliveTokenSource.Token;
			}
		}

		/// <summary>
		/// Gets the audio format used by the Opus encoder.
		/// </summary>
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000078 RID: 120 RVA: 0x0000279B File Offset: 0x0000099B
		public AudioFormat AudioFormat
		{
			get
			{
				return this.Configuration.AudioFormat;
			}
		}

		/// <summary>
		/// Gets whether this connection is still playing audio.
		/// </summary>
		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000027A8 File Offset: 0x000009A8
		public bool IsPlaying
		{
			get
			{
				return this.PlayingWait != null && !this.PlayingWait.Task.IsCompleted;
			}
		}

		/// <summary>
		/// Gets the websocket round-trip time in ms.
		/// </summary>
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000027C7 File Offset: 0x000009C7
		public int WebSocketPing
		{
			get
			{
				return Volatile.Read(ref this._wsPing);
			}
		}

		/// <summary>
		/// Gets the UDP round-trip time in ms.
		/// </summary>
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000027D4 File Offset: 0x000009D4
		public int UdpPing
		{
			get
			{
				return Volatile.Read(ref this._udpPing);
			}
		}

		/// <summary>
		/// Gets the channel this voice client is connected to.
		/// </summary>
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600007C RID: 124 RVA: 0x000027E1 File Offset: 0x000009E1
		// (set) Token: 0x0600007D RID: 125 RVA: 0x000027E9 File Offset: 0x000009E9
		public DiscordChannel Channel { get; internal set; }

		// Token: 0x0600007E RID: 126 RVA: 0x000027F4 File Offset: 0x000009F4
		internal VoiceNextConnection(DiscordClient client, DiscordGuild guild, DiscordChannel channel, VoiceNextConfiguration config, VoiceServerUpdatePayload server, VoiceStateUpdatePayload state)
		{
			this.Discord = client;
			this.Guild = guild;
			this.Channel = channel;
			this.TransmittingSSRCs = new ConcurrentDictionary<uint, AudioSender>();
			this._userSpeaking = new AsyncEvent<UserSpeakingEventArgs>(new Action<string, Exception>(this.Discord.EventErrorHandler), "VNEXT_USER_SPEAKING");
			this._userJoined = new AsyncEvent<VoiceUserJoinEventArgs>(new Action<string, Exception>(this.Discord.EventErrorHandler), "VNEXT_USER_JOINED");
			this._userLeft = new AsyncEvent<VoiceUserLeaveEventArgs>(new Action<string, Exception>(this.Discord.EventErrorHandler), "VNEXT_USER_LEFT");
			this._voiceReceived = new AsyncEvent<VoiceReceiveEventArgs>(new Action<string, Exception>(this.Discord.EventErrorHandler), "VNEXT_VOICE_RECEIVED");
			this._voiceSocketError = new AsyncEvent<SocketErrorEventArgs>(new Action<string, Exception>(this.Discord.EventErrorHandler), "VNEXT_WS_ERROR");
			this.TokenSource = new CancellationTokenSource();
			this.Configuration = config;
			this.Opus = new Opus(this.AudioFormat);
			this.Rtp = new Rtp();
			this.ServerData = server;
			this.StateData = state;
			string endpoint = this.ServerData.Endpoint;
			int num = endpoint.LastIndexOf(':');
			string hostname = string.Empty;
			int port = 80;
			if (num != -1)
			{
				hostname = endpoint.Substring(0, num);
				port = int.Parse(endpoint.Substring(num + 1));
			}
			else
			{
				hostname = endpoint;
			}
			ConnectionEndpoint webSocketEndpoint = default(ConnectionEndpoint);
			webSocketEndpoint.Hostname = hostname;
			webSocketEndpoint.Port = port;
			this.WebSocketEndpoint = webSocketEndpoint;
			this.ReadyWait = new TaskCompletionSource<bool>();
			this.IsInitialized = false;
			this.IsDisposed = false;
			this.PlayingWait = null;
			this.PacketQueue = new ConcurrentQueue<VoicePacket>();
			this.KeepaliveTimestamps = new ConcurrentDictionary<ulong, long>();
			this.PauseEvent = new AsyncManualResetEvent(true);
			this.UdpClient = this.Discord.Configuration.UdpClientFactory.Invoke();
			this.VoiceWs = this.Discord.Configuration.WebSocketClientFactory.Invoke(this.Discord.Configuration.Proxy);
			this.VoiceWs.Disconnected += new AsyncEventHandler<SocketCloseEventArgs>(this.VoiceWS_SocketClosed);
			this.VoiceWs.MessageReceived += new AsyncEventHandler<SocketMessageEventArgs>(this.VoiceWS_SocketMessage);
			this.VoiceWs.Connected += new AsyncEventHandler(this.VoiceWS_SocketOpened);
			this.VoiceWs.ExceptionThrown += new AsyncEventHandler<SocketErrorEventArgs>(this.VoiceWs_SocketException);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002A50 File Offset: 0x00000C50
		~VoiceNextConnection()
		{
			this.Dispose();
		}

		/// <summary>
		/// Connects to the specified voice channel.
		/// </summary>
		/// <returns>A task representing the connection operation.</returns>
		// Token: 0x06000080 RID: 128 RVA: 0x00002A7C File Offset: 0x00000C7C
		internal Task ConnectAsync()
		{
			UriBuilder uriBuilder = new UriBuilder
			{
				Scheme = "wss",
				Host = this.WebSocketEndpoint.Hostname,
				Query = "encoding=json&v=4"
			};
			return this.VoiceWs.ConnectAsync(uriBuilder.Uri);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002ACA File Offset: 0x00000CCA
		internal Task ReconnectAsync()
		{
			return this.VoiceWs.DisconnectAsync(1000, "");
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002AE4 File Offset: 0x00000CE4
		internal Task StartAsync()
		{
			VoiceNextConnection.<StartAsync>d__201 <StartAsync>d__;
			<StartAsync>d__.<>4__this = this;
			<StartAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartAsync>d__.<>1__state = -1;
			<StartAsync>d__.<>t__builder.Start<VoiceNextConnection.<StartAsync>d__201>(ref <StartAsync>d__);
			return <StartAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002B27 File Offset: 0x00000D27
		internal Task WaitForReadyAsync()
		{
			return this.ReadyWait.Task;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00002B34 File Offset: 0x00000D34
		internal unsafe void PreparePacket(ReadOnlySpan<byte> pcm, ref Memory<byte> target)
		{
			if (this.IsDisposed)
			{
				return;
			}
			AudioFormat audioFormat = this.AudioFormat;
			byte[] array = ArrayPool<byte>.Shared.Rent(this.Rtp.CalculatePacketSize(audioFormat.SampleCountToSampleSize(audioFormat.CalculateMaximumFrameSize()), this.SelectedEncryptionMode));
			Span<byte> target2 = MemoryExtensions.AsSpan<byte>(array);
			this.Rtp.EncodeHeader(this.Sequence, this.Timestamp, this.SSRC, target2);
			Span<byte> span = target2.Slice(12, pcm.Length);
			this.Opus.Encode(pcm, ref span);
			ushort sequence = this.Sequence;
			this.Sequence = sequence + 1;
			this.Timestamp += (uint)audioFormat.CalculateFrameSize(audioFormat.CalculateSampleDuration(pcm.Length));
			int num = Sodium.NonceSize;
			Span<byte> span2 = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
			switch (this.SelectedEncryptionMode)
			{
			case EncryptionMode.XSalsa20_Poly1305_Lite:
			{
				Sodium sodium = this.Sodium;
				uint nonce = this.Nonce;
				this.Nonce = nonce + 1u;
				sodium.GenerateNonce(nonce, span2);
				break;
			}
			case EncryptionMode.XSalsa20_Poly1305_Suffix:
				this.Sodium.GenerateNonce(span2);
				break;
			case EncryptionMode.XSalsa20_Poly1305:
				this.Sodium.GenerateNonce(target2.Slice(0, 12), span2);
				break;
			default:
				ArrayPool<byte>.Shared.Return(array, false);
				throw new Exception("Unsupported encryption mode.");
			}
			num = Sodium.CalculateTargetSize(span);
			Span<byte> target3 = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
			this.Sodium.Encrypt(span, target3, span2);
			target3.CopyTo(target2.Slice(12));
			target2 = target2.Slice(0, this.Rtp.CalculatePacketSize(target3.Length, this.SelectedEncryptionMode));
			this.Sodium.AppendNonce(span2, target2, this.SelectedEncryptionMode);
			target = target.Slice(0, target2.Length);
			target2.CopyTo(target.Span);
			ArrayPool<byte>.Shared.Return(array, false);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00002D39 File Offset: 0x00000F39
		internal void EnqueuePacket(VoicePacket packet)
		{
			this.PacketQueue.Enqueue(packet);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002D48 File Offset: 0x00000F48
		private Task VoiceSenderTask()
		{
			VoiceNextConnection.<VoiceSenderTask>d__205 <VoiceSenderTask>d__;
			<VoiceSenderTask>d__.<>4__this = this;
			<VoiceSenderTask>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<VoiceSenderTask>d__.<>1__state = -1;
			<VoiceSenderTask>d__.<>t__builder.Start<VoiceNextConnection.<VoiceSenderTask>d__205>(ref <VoiceSenderTask>d__);
			return <VoiceSenderTask>d__.<>t__builder.Task;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00002D8C File Offset: 0x00000F8C
		private unsafe bool ProcessPacket(ReadOnlySpan<byte> data, ref Memory<byte> opus, ref Memory<byte> pcm, IList<ReadOnlyMemory<byte>> pcmPackets, out AudioSender voiceSender, out AudioFormat outputFormat)
		{
			voiceSender = null;
			outputFormat = default(AudioFormat);
			if (!this.Rtp.IsRtpHeader(data))
			{
				return false;
			}
			ushort num;
			uint num2;
			uint key;
			bool flag;
			this.Rtp.DecodeHeader(data, out num, out num2, out key, out flag);
			AudioSender audioSender = this.TransmittingSSRCs[key];
			voiceSender = audioSender;
			if (num <= audioSender.LastSequence)
			{
				return false;
			}
			int num3 = (int)((audioSender.LastSequence != 0) ? (num - 1 - audioSender.LastSequence) : 0);
			if (num3 >= 5)
			{
				this.Discord.DebugLogger.LogMessage(2, "VNext RX", "5 or more voice packets were dropped when receiving", DateTime.Now, null);
			}
			int nonceSize = Sodium.NonceSize;
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)nonceSize], nonceSize);
			this.Sodium.GetNonce(data, span, this.SelectedEncryptionMode);
			ReadOnlySpan<byte> source;
			this.Rtp.GetDataFromPacket(data, out source, this.SelectedEncryptionMode);
			int num4 = Sodium.CalculateSourceSize(source);
			opus = opus.Slice(0, num4);
			Span<byte> span2 = opus.Span;
			try
			{
				this.Sodium.Decrypt(source, span2, span);
				if (flag && *span2[0] == 190 && *span2[1] == 222)
				{
					int num5 = (int)(*span2[2]) << 8 | (int)(*span2[3]);
					int i;
					for (i = 4; i < num5 + 4; i++)
					{
						int num6 = (int)((*span2[i] & 15) + 1);
						i += num6;
					}
					while (*span2[i] == 0)
					{
						i++;
					}
					span2 = span2.Slice(i);
				}
				if (num3 == 1)
				{
					int lastPacketSampleCount = this.Opus.GetLastPacketSampleCount(audioSender.Decoder);
					AudioFormat audioFormat = this.AudioFormat;
					byte[] array = new byte[audioFormat.SampleCountToSampleSize(lastPacketSampleCount)];
					Span<byte> span3 = MemoryExtensions.AsSpan<byte>(array);
					this.Opus.Decode(audioSender.Decoder, span2, ref span3, true, out audioFormat);
					pcmPackets.Add(MemoryExtensions.AsMemory<byte>(array, 0, span3.Length));
				}
				else if (num3 > 1)
				{
					int lastPacketSampleCount2 = this.Opus.GetLastPacketSampleCount(audioSender.Decoder);
					for (int j = 0; j < num3; j++)
					{
						AudioFormat audioFormat = this.AudioFormat;
						byte[] array2 = new byte[audioFormat.SampleCountToSampleSize(lastPacketSampleCount2)];
						Span<byte> span4 = MemoryExtensions.AsSpan<byte>(array2);
						this.Opus.ProcessPacketLoss(audioSender.Decoder, lastPacketSampleCount2, ref span4);
						pcmPackets.Add(MemoryExtensions.AsMemory<byte>(array2, 0, span4.Length));
					}
				}
				Span<byte> span5 = pcm.Span;
				this.Opus.Decode(audioSender.Decoder, span2, ref span5, false, out outputFormat);
				pcm = pcm.Slice(0, span5.Length);
			}
			finally
			{
				audioSender.LastSequence = num;
			}
			return true;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00003078 File Offset: 0x00001278
		private Task ProcessVoicePacket(byte[] data)
		{
			VoiceNextConnection.<ProcessVoicePacket>d__207 <ProcessVoicePacket>d__;
			<ProcessVoicePacket>d__.<>4__this = this;
			<ProcessVoicePacket>d__.data = data;
			<ProcessVoicePacket>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessVoicePacket>d__.<>1__state = -1;
			<ProcessVoicePacket>d__.<>t__builder.Start<VoiceNextConnection.<ProcessVoicePacket>d__207>(ref <ProcessVoicePacket>d__);
			return <ProcessVoicePacket>d__.<>t__builder.Task;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000030C4 File Offset: 0x000012C4
		private void ProcessKeepalive(byte[] data)
		{
			try
			{
				ulong num = BinaryPrimitives.ReadUInt64LittleEndian(data);
				long num2;
				if (this.KeepaliveTimestamps.TryRemove(num, out num2))
				{
					int num3 = (int)((double)(Stopwatch.GetTimestamp() - num2) / (double)Stopwatch.Frequency * 1000.0);
					Volatile.Write(ref this._wsPing, num3);
					this.Discord.DebugLogger.LogMessage(8, "VNext UDP", string.Format("Received UDP keepalive {0}, ping {1}ms", num, num3), DateTime.Now, null);
				}
			}
			catch (Exception ex)
			{
				this.Discord.DebugLogger.LogMessage(1, "VNext UDP", "Exception occured when handling keepalive", DateTime.Now, ex);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003180 File Offset: 0x00001380
		private Task UdpReceiverTask()
		{
			VoiceNextConnection.<UdpReceiverTask>d__209 <UdpReceiverTask>d__;
			<UdpReceiverTask>d__.<>4__this = this;
			<UdpReceiverTask>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UdpReceiverTask>d__.<>1__state = -1;
			<UdpReceiverTask>d__.<>t__builder.Start<VoiceNextConnection.<UdpReceiverTask>d__209>(ref <UdpReceiverTask>d__);
			return <UdpReceiverTask>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Sends a speaking status to the connected voice channel.
		/// </summary>
		/// <param name="speaking">Whether the current user is speaking or not.</param>
		/// <returns>A task representing the sending operation.</returns>
		// Token: 0x0600008B RID: 139 RVA: 0x000031C4 File Offset: 0x000013C4
		public Task SendSpeakingAsync(bool speaking = true)
		{
			VoiceNextConnection.<SendSpeakingAsync>d__210 <SendSpeakingAsync>d__;
			<SendSpeakingAsync>d__.<>4__this = this;
			<SendSpeakingAsync>d__.speaking = speaking;
			<SendSpeakingAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendSpeakingAsync>d__.<>1__state = -1;
			<SendSpeakingAsync>d__.<>t__builder.Start<VoiceNextConnection.<SendSpeakingAsync>d__210>(ref <SendSpeakingAsync>d__);
			return <SendSpeakingAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Gets a transmit stream for this connection, optionally specifying a packet size to use with the stream. If a stream is already configured, it will return the existing one.
		/// </summary>
		/// <param name="sampleDuration">Duration, in ms, to use for audio packets.</param>
		/// <returns>Transmit stream.</returns>
		// Token: 0x0600008C RID: 140 RVA: 0x0000320F File Offset: 0x0000140F
		public VoiceTransmitStream GetTransmitStream(int sampleDuration = 20)
		{
			if (!AudioFormat.AllowedSampleDurations.Contains(sampleDuration))
			{
				throw new ArgumentOutOfRangeException("sampleDuration", "Invalid PCM sample duration specified.");
			}
			if (this.TransmitStream == null)
			{
				this.TransmitStream = new VoiceTransmitStream(this, sampleDuration);
			}
			return this.TransmitStream;
		}

		/// <summary>
		/// Asynchronously waits for playback to be finished. Playback is finished when speaking = false is signalled.
		/// </summary>
		/// <returns>A task representing the waiting operation.</returns>
		// Token: 0x0600008D RID: 141 RVA: 0x0000324C File Offset: 0x0000144C
		public Task WaitForPlaybackFinishAsync()
		{
			VoiceNextConnection.<WaitForPlaybackFinishAsync>d__212 <WaitForPlaybackFinishAsync>d__;
			<WaitForPlaybackFinishAsync>d__.<>4__this = this;
			<WaitForPlaybackFinishAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForPlaybackFinishAsync>d__.<>1__state = -1;
			<WaitForPlaybackFinishAsync>d__.<>t__builder.Start<VoiceNextConnection.<WaitForPlaybackFinishAsync>d__212>(ref <WaitForPlaybackFinishAsync>d__);
			return <WaitForPlaybackFinishAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Pauses playback.
		/// </summary>
		// Token: 0x0600008E RID: 142 RVA: 0x0000328F File Offset: 0x0000148F
		public void Pause()
		{
			this.PauseEvent.Reset();
		}

		/// <summary>
		/// Asynchronously resumes playback.
		/// </summary>
		/// <returns></returns>
		// Token: 0x0600008F RID: 143 RVA: 0x0000329C File Offset: 0x0000149C
		public Task ResumeAsync()
		{
			VoiceNextConnection.<ResumeAsync>d__214 <ResumeAsync>d__;
			<ResumeAsync>d__.<>4__this = this;
			<ResumeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResumeAsync>d__.<>1__state = -1;
			<ResumeAsync>d__.<>t__builder.Start<VoiceNextConnection.<ResumeAsync>d__214>(ref <ResumeAsync>d__);
			return <ResumeAsync>d__.<>t__builder.Task;
		}

		/// <summary>
		/// Disconnects and disposes this voice connection.
		/// </summary>
		// Token: 0x06000090 RID: 144 RVA: 0x000032DF File Offset: 0x000014DF
		public void Disconnect()
		{
			this.Dispose();
		}

		/// <summary>
		/// Disconnects and disposes this voice connection.
		/// </summary>
		// Token: 0x06000091 RID: 145 RVA: 0x000032E8 File Offset: 0x000014E8
		public void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			this.IsDisposed = true;
			this.IsInitialized = false;
			this.TokenSource.Cancel();
			this.SenderTokenSource.Cancel();
			if (this.Configuration.EnableIncoming)
			{
				this.ReceiverTokenSource.Cancel();
			}
			try
			{
				this.VoiceWs.DisconnectAsync(1000, "").ConfigureAwait(false).GetAwaiter().GetResult();
				this.UdpClient.Close();
			}
			catch
			{
			}
			Opus opus = this.Opus;
			if (opus != null)
			{
				opus.Dispose();
			}
			this.Opus = null;
			Sodium sodium = this.Sodium;
			if (sodium != null)
			{
				sodium.Dispose();
			}
			this.Sodium = null;
			Rtp rtp = this.Rtp;
			if (rtp != null)
			{
				rtp.Dispose();
			}
			this.Rtp = null;
			VoiceDisconnectedEventHandler voiceDisconnected = this.VoiceDisconnected;
			if (voiceDisconnected == null)
			{
				return;
			}
			voiceDisconnected(this.Guild);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000033E8 File Offset: 0x000015E8
		private Task HeartbeatAsync()
		{
			VoiceNextConnection.<HeartbeatAsync>d__217 <HeartbeatAsync>d__;
			<HeartbeatAsync>d__.<>4__this = this;
			<HeartbeatAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HeartbeatAsync>d__.<>1__state = -1;
			<HeartbeatAsync>d__.<>t__builder.Start<VoiceNextConnection.<HeartbeatAsync>d__217>(ref <HeartbeatAsync>d__);
			return <HeartbeatAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000342C File Offset: 0x0000162C
		private Task KeepaliveAsync()
		{
			VoiceNextConnection.<KeepaliveAsync>d__218 <KeepaliveAsync>d__;
			<KeepaliveAsync>d__.<>4__this = this;
			<KeepaliveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<KeepaliveAsync>d__.<>1__state = -1;
			<KeepaliveAsync>d__.<>t__builder.Start<VoiceNextConnection.<KeepaliveAsync>d__218>(ref <KeepaliveAsync>d__);
			return <KeepaliveAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00003470 File Offset: 0x00001670
		private Task Stage1(VoiceReadyPayload voiceReady)
		{
			VoiceNextConnection.<Stage1>d__219 <Stage1>d__;
			<Stage1>d__.<>4__this = this;
			<Stage1>d__.voiceReady = voiceReady;
			<Stage1>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Stage1>d__.<>1__state = -1;
			<Stage1>d__.<>t__builder.Start<VoiceNextConnection.<Stage1>d__219>(ref <Stage1>d__);
			return <Stage1>d__.<>t__builder.Task;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000034BC File Offset: 0x000016BC
		private Task Stage2(VoiceSessionDescriptionPayload voiceSessionDescription)
		{
			this.SelectedEncryptionMode = Sodium.SupportedModes[voiceSessionDescription.Mode.ToLowerInvariant()];
			this.Discord.DebugLogger.LogMessage(8, "VoiceNext", string.Format("Discord updated encryption mode: {0}", this.SelectedEncryptionMode), DateTime.Now, null);
			this.KeepaliveTokenSource = new CancellationTokenSource();
			this.KeepaliveTask = this.KeepaliveAsync();
			byte[] array = new byte[this.AudioFormat.CalculateSampleSize(20)];
			for (int i = 0; i < 3; i++)
			{
				Memory<byte> memory = MemoryExtensions.AsMemory<byte>(new byte[array.Length]);
				this.PreparePacket(array, ref memory);
				this.EnqueuePacket(new VoicePacket(memory, 20, false));
			}
			this.IsInitialized = true;
			this.ReadyWait.SetResult(true);
			return Task.CompletedTask;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00003598 File Offset: 0x00001798
		private Task HandleDispatch(JObject jo)
		{
			VoiceNextConnection.<HandleDispatch>d__221 <HandleDispatch>d__;
			<HandleDispatch>d__.<>4__this = this;
			<HandleDispatch>d__.jo = jo;
			<HandleDispatch>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleDispatch>d__.<>1__state = -1;
			<HandleDispatch>d__.<>t__builder.Start<VoiceNextConnection.<HandleDispatch>d__221>(ref <HandleDispatch>d__);
			return <HandleDispatch>d__.<>t__builder.Task;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000035E4 File Offset: 0x000017E4
		private Task VoiceWS_SocketClosed(SocketCloseEventArgs e)
		{
			VoiceNextConnection.<VoiceWS_SocketClosed>d__222 <VoiceWS_SocketClosed>d__;
			<VoiceWS_SocketClosed>d__.<>4__this = this;
			<VoiceWS_SocketClosed>d__.e = e;
			<VoiceWS_SocketClosed>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<VoiceWS_SocketClosed>d__.<>1__state = -1;
			<VoiceWS_SocketClosed>d__.<>t__builder.Start<VoiceNextConnection.<VoiceWS_SocketClosed>d__222>(ref <VoiceWS_SocketClosed>d__);
			return <VoiceWS_SocketClosed>d__.<>t__builder.Task;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00003630 File Offset: 0x00001830
		private Task VoiceWS_SocketMessage(SocketMessageEventArgs e)
		{
			SocketTextMessageEventArgs socketTextMessageEventArgs = e as SocketTextMessageEventArgs;
			if (socketTextMessageEventArgs == null)
			{
				this.Discord.DebugLogger.LogMessage(0, "VoiceNext", "Discord Voice Gateway spewed out binary gibberish!", DateTime.Now, null);
				return Task.CompletedTask;
			}
			return this.HandleDispatch(JObject.Parse(socketTextMessageEventArgs.Message));
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000367F File Offset: 0x0000187F
		private Task VoiceWS_SocketOpened()
		{
			return this.StartAsync();
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00003687 File Offset: 0x00001887
		private Task VoiceWs_SocketException(SocketErrorEventArgs e)
		{
			return this._voiceSocketError.InvokeAsync(new SocketErrorEventArgs(this.Discord)
			{
				Exception = e.Exception
			});
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000036AC File Offset: 0x000018AC
		private static uint UnixTimestamp(DateTime dt)
		{
			return (uint)(dt - VoiceNextConnection.UnixEpoch).TotalSeconds;
		}

		// Token: 0x0400000E RID: 14
		private AsyncEvent<UserSpeakingEventArgs> _userSpeaking;

		// Token: 0x0400000F RID: 15
		private AsyncEvent<VoiceUserJoinEventArgs> _userJoined;

		// Token: 0x04000010 RID: 16
		private AsyncEvent<VoiceUserLeaveEventArgs> _userLeft;

		// Token: 0x04000011 RID: 17
		private AsyncEvent<VoiceReceiveEventArgs> _voiceReceived;

		// Token: 0x04000012 RID: 18
		private AsyncEvent<SocketErrorEventArgs> _voiceSocketError;

		// Token: 0x04000036 RID: 54
		private ulong _lastKeepalive;

		// Token: 0x0400003D RID: 61
		private int _wsPing;

		// Token: 0x0400003E RID: 62
		private int _udpPing;
	}
}
