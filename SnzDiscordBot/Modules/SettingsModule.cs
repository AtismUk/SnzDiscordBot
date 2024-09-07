using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace SnzDiscordBot.Modules;

public class SettingsModule : InteractionModuleBase<SocketInteractionContext>
{
    
    private readonly IConfiguration _config;
    public SettingsModule(IConfiguration config)
    {
        _config = config;
    }

    [SlashCommand("config", "настройка бота")]
    [RequireUserPermission(GuildPermission.BanMembers)]
    public async Task ConfigCommand(IChannel application_channel, IRole new_role, IRole role_member)
    {
        if (Context.User is SocketGuildUser userGuild)
        {
            if (userGuild.Roles.Any(x => x.Permissions.BanMembers))
            {
                _config["Settings:Application_Channel_Id"] = application_channel.Id.ToString();
                _config["Settings:New_Role_Id"] = new_role.Id.ToString();
                _config["Settings:Member_Role_Id"] = role_member.Id.ToString();

                await RespondAsync("Данные успешно сохраннены", ephemeral: true);
            }
        }
    }
}