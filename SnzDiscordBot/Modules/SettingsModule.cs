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

    [SlashCommand("config", "Настройка бота")]
    [RequireUserPermission(GuildPermission.BanMembers)]
    public async Task ConfigCommand(IChannel application_channel, IRole remove_role, IRole add_role)
    {
        if (Context.User is SocketGuildUser userGuild)
        {
            if (userGuild.Roles.Any(x => x.Permissions.BanMembers))
            {
                _config["Settings:Application_Channel_Id"] = application_channel.Id.ToString();
                _config["Settings:Remove_Application_Role_Id"] = remove_role.Id.ToString();
                _config["Settings:Add_Application_Role_Id"] = add_role.Id.ToString();

                await RespondAsync("Данные успешно сохранены!");
            }
        }
    }
}