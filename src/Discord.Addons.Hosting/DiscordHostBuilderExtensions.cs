﻿using System;
using System.Linq;
using Discord.Addons.Hosting.Injectables;
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Discord.Addons.Hosting
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with Discord.Net configuration methods.
    /// </summary>
    public static class DiscordHostBuilderExtensions
    {
        /// <summary>
        /// Adds and optionally configures a <see cref="DiscordShardedClient"/> along with the required services.
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
        /// <returns>The generic host builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
        public static IHostBuilder ConfigureDiscordShardedHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null)
        {
            builder.ConfigureDiscordHostInternal<DiscordShardedClient>(config);

            return builder.ConfigureServices((_, collection) =>
            {
                if (collection.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
                {
                    throw new InvalidOperationException("Cannot add more than one Discord Client to host");
                }

                collection.AddSingleton<DiscordShardedClient, InjectableDiscordShardedClient>();
            });
        }

        /// <summary>
        /// Adds and optionally configures a <see cref="DiscordSocketClient"/> along with the required services.
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for the <see cref="DiscordHostConfiguration" /> that will be used to configure the host.</param>
        /// <returns>The generic host builder.</returns>
        /// <exception cref="InvalidOperationException">Thrown if client is already added to the service collection</exception>
        public static IHostBuilder ConfigureDiscordHost(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null)
        {
            builder.ConfigureDiscordHostInternal<DiscordSocketClient>(config);

            return builder.ConfigureServices((_, collection) =>
            {
                if (collection.Any(x => x.ServiceType.BaseType == typeof(BaseSocketClient)))
                {
                    throw new InvalidOperationException("Cannot add more than one Discord Client to host");
                }

                collection.AddSingleton<DiscordSocketClient, InjectableDiscordSocketClient>();
            });
        }

        private static IHostBuilder ConfigureDiscordHostInternal<T>(this IHostBuilder builder, Action<HostBuilderContext, DiscordHostConfiguration>? config = null) where T: BaseSocketClient
        {
            return builder.ConfigureServices((context, collection) =>
            {
                collection.AddOptions<DiscordHostConfiguration>().Validate(x => ValidateToken(x.Token));

                if (config != null)
                {
                    collection.Configure<DiscordHostConfiguration>(x => config(context, x));
                }
                
                collection.AddSingleton(typeof(LogAdapter<>));
                collection.AddHostedService<DiscordHostedService<T>>();
            });

            static bool ValidateToken(string token)
            {
                try
                {
                    TokenUtils.ValidateToken(TokenType.Bot, token);
                    return true;
                }
                catch (Exception e) when (e is ArgumentNullException || e is ArgumentException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds a <see cref="CommandService"/> instance to the host for use with a Discord.Net client. />
        /// </summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that the configuration and service provider can be used.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="config">The delegate for configuring the <see cref="CommandServiceConfig" /> that will be used to initialise the service.</param>
        /// <returns>The (generic) host builder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if config is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="CommandService"/> is already added to the collection</exception>
        public static IHostBuilder UseCommandService(this IHostBuilder builder, Action<HostBuilderContext, CommandServiceConfig> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            builder.ConfigureServices((context, collection) =>
            {
                if (collection.Any(x => x.ServiceType == typeof(CommandService)))
                {
                    throw new InvalidOperationException("Cannot add more than one CommandService to host");
                }

                collection.Configure<CommandServiceConfig>(x => config(context, x));

                collection.AddSingleton(x=> new CommandService(x.GetRequiredService<IOptions<CommandServiceConfig>>().Value));
                collection.AddHostedService<CommandServiceRegistrationHost>();
            });

            return builder;
        }
    }
}
