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
        // Отправляем временный ответ, чтобы избежать таймаута
        await DeferAsync(ephemeral: true);
        var resultMessage = new StringBuilder();
        var errorBuilder = new StringBuilder();
        var ignoredBuilder = new StringBuilder();

        var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
        
        // Разбиваем строку на отдельные идентификаторы ролей
        var ignoreRoleIds = ignore_roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(id => ulong.TryParse(id, out var roleId) ? roleId : (ulong?)null)
                                       .Where(id => id.HasValue)
                                       .Select(id => id.Value)
                                       .ToHashSet();

        foreach (var roleId in ignoreRoleIds)
        {
            var role = Context.Guild.GetRole(roleId);
            ignoredBuilder.AppendLine(role.Name);
        }
        
        
        int addedCount = 0;
        
        await foreach (var users in Context.Guild.GetUsersAsync())
        {
            foreach (var user in users)
            {
                if (user.RoleIds.Contains(add_role.Id))
                {
                    continue;
                }
                
                // Проверяем, есть ли у пользователя любая из ролей, которые нужно игнорировать
                if (user.RoleIds.Any(roleId => ignoreRoleIds.Contains(roleId)))
                {
                    continue;
                }

                try
                {
                    await user.AddRoleAsync(add_role);
                    addedCount++;
                }
                catch (Exception ex)
                {
                    // Собираем ошибки в StringBuilder
                    errorBuilder.AppendLine($"Не удалось добавить роль пользователю {user.Username}: {ex.Message}");
                }
            }
        }

        // Формируем окончательное сообщение
        
        resultMessage.AppendLine($"Роль {add_role.Name} была добавлена {addedCount} пользователям.");

        if (errorBuilder.Length > 0)
        {
            resultMessage.AppendLine("Ошибки:");
            resultMessage.Append(errorBuilder.ToString());
        }
        
        if (ignoredBuilder.Length > 0)
        {
            resultMessage.AppendLine("Роли проигнорированы:");
            resultMessage.Append(ignoredBuilder.ToString());
        }

        // Отправляем окончательный ответ после завершения выполнения команды
        await FollowupAsync(resultMessage.ToString());
    }
}