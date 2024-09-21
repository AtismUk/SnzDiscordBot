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

    [SlashCommand("config-application", "Настройка регистрации")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    public async Task ConfigApplicationCommand(IChannel application_channel, IRole remove_role, IRole add_role)
    {
        if (Context.User is SocketGuildUser userGuild)
        {
            if (userGuild.Roles.Any(x => x.Permissions.BanMembers))
            {
                _config["Settings:Application_Channel_Id"] = application_channel.Id.ToString();
                _config["Settings:Remove_Application_Role_Id"] = remove_role.Id.ToString();
                _config["Settings:Add_Application_Role_Id"] = add_role.Id.ToString();

                await RespondAsync("Данные успешно сохранены!", ephemeral: true);
            }
        }
    }
    
    [SlashCommand("config-mention", "Настройка оповещений")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    public async Task ConfigMentionCommand(IChannel news_channel, IChannel event_channel, IChannel schedule_channel)
    {
        if (Context.User is SocketGuildUser userGuild)
        {
            if (userGuild.Roles.Any(x => x.Permissions.BanMembers))
            {
                _config["Settings:News_Channel_Id"] = news_channel.Id.ToString();
                _config["Settings:Event_Channel_Id"] = event_channel.Id.ToString();
                _config["Settings:Schedule_Channel_Id"] = schedule_channel.Id.ToString();

                await RespondAsync("Данные успешно сохранены!", ephemeral: true);
            }
        }
    }
}