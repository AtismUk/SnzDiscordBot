using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using System.Text;

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
    public async Task AddRoleAllCommand(IRole add_role, string? ignore_roles = "")
    {
        await DeferAsync();
        
        var resultMessage = new StringBuilder();
        var errorBuilder = new StringBuilder();
        
        var ignoreRoles = ignore_roles?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => ulong.TryParse(id, out var roleId) ? roleId : (ulong?)null)
            .Where(id => id.HasValue)
            .Select(id => Context.Guild.GetRole(id!.Value))
            .ToHashSet();

        if (ignoreRoles is { Count: > 0 })
        {
            resultMessage.AppendLine("Роли проигнорированы:");
            foreach (var role in ignoreRoles)
            {
                resultMessage.AppendLine(role.Mention);
            }
        }
        
        
        int addedCount = 0;
        
        await foreach (var users in Context.Guild.GetUsersAsync())
        {
            foreach (var user in users)
            {
                if (user.RoleIds.Contains(add_role.Id))
                    continue;
                
                if (ignoreRoles != null && user.RoleIds.Any(roleId => ignoreRoles.Any(role => role.Id == roleId)))
                    continue;
                
                await user.AddRoleAsync(add_role);
                addedCount++;
            }
        }
        
        resultMessage.AppendLine($"Роль {add_role.Name} была добавлена {addedCount} пользователям.");

        if (errorBuilder.Length > 0)
        {
            resultMessage.AppendLine("Ошибки:");
            resultMessage.Append(errorBuilder.ToString());
        }

        await FollowupAsync(resultMessage.ToString());
    }
}