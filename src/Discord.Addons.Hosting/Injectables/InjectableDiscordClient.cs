﻿using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting.Injectables
{
    internal class InjectableDiscordSocketClient : DiscordSocketClient
    {
        public InjectableDiscordSocketClient(IOptions<DiscordHostConfiguration> config) : base(config.Value.SocketConfig)
        {
            this.RegisterSocketClientReady();
        }
    }

    internal class InjectableDiscordShardedClient : DiscordShardedClient
    {
        public InjectableDiscordShardedClient(IOptions<DiscordHostConfiguration> config) : base(config.Value.SocketConfig)
        {
            this.RegisterShardedClientReady();
        }
    }
}
