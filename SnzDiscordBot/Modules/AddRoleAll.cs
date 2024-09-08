using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using SnzDiscordBot.Models.InteractionModels;
using Color = Discord.Color;

namespace SnzDiscordBot.Modules;

public class AddRoleAll : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfiguration _config;
    
    public AddRoleAll(IConfiguration config)
    {
        _config = config;
    }
    
    [SlashCommand("addroleall", "Выдать роль всем")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AddRoleAllCommand(IRole add_role, IRole ignore_role)
    {
        await RespondAsync("Test", ephemeral: true);
    }
}