using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace SnzDiscordBot.Modules;

public class AddRoleAll : InteractionModuleBase<SocketInteractionContext>
{
    public AddRoleAll() { }
    
    [SlashCommand("addroleall", "Выдать роль всем")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AddRoleAllCommand(IRole add_role, string? ignore_roles = "")
    {
        // Отправляем ответ сразу, чтобы не улететь в таймаут.
        await DeferAsync();
        
        // Определяем строки.
        var resultMessage = new StringBuilder();
        var errorBuilder = new StringBuilder();

        #region Обрабатываем игнорируемые роли.
        
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
        
        #endregion

        #region Выдаем пользователям роль.

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

        #endregion

        // Добавляем запись об ошибках.
        if (errorBuilder.Length > 0)
        {
            resultMessage.AppendLine("Ошибки:");
            resultMessage.Append(errorBuilder.ToString());
        }

        // Редактируем ответ на отработанный.
        await FollowupAsync(resultMessage.ToString());
    }
}