# Discord.Addons.Hosting

## Summary? 

This is a fork for the experimental Discord library that's a fork of [Discord.NET](https://github.com/discord-net/Discord.Net). 
This exists for the sole purpose of making my life easier so I can continue using this library with the latest versions of the Discord API, 
as it seems the developers have pretty much given up unfortunately.

[Discord.Net-Labs](https://github.com/Discord-Net-Labs/Discord.Net-Labs) hosting with [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host). 
This package provides extensions to a .NET Generic Host (`IHostBuilder`) that will run a Discord.Net socket/sharded client as a controllable `IHostedService`. This simplifies initial bot creation and moves the usual boilerplate to a convenient builder pattern.

## Addendum
I will NOT be providing any documentation for this fork as it is not meant for public use. If you wish to learn how to use this library
please look [here](https://github.com/Hawxy/Discord.Addons.Hosting). I've simply forked the repository and made it public in the name of open-source.

Do not expect any support to come from me as this is simply a straight upgrade and cleanup of the original Discord.Addons.Hosting
to add support for the latest API version. Discord.NET seems to have slowed/stopped development so I needed to upgrade to Discord API v9 (as of late).