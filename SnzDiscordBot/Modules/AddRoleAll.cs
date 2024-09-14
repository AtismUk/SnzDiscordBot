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

        var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
        
        // Разбиваем строку на отдельные идентификаторы ролей
        var ignoreRoleIds = ignore_roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(id => ulong.TryParse(id, out var roleId) ? roleId : (ulong?)null)
                                       .Where(id => id.HasValue)
                                       .Select(id => id.Value)
                                       .ToHashSet();

        int addedCount = 0;
        var errorBuilder = new StringBuilder();
        
        await foreach (var users in Context.Guild.GetUsersAsync())
        {
            foreach (var user in users)
            {
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
        var resultMessage = new StringBuilder();
        resultMessage.AppendLine($"Роль {add_role.Name} была добавлена {addedCount} пользователям.");

        if (errorBuilder.Length > 0)
        {
            resultMessage.AppendLine("Ошибки:");
            resultMessage.Append(errorBuilder.ToString());
        }

        // Отправляем окончательный ответ после завершения выполнения команды
        await FollowupAsync(resultMessage.ToString());
    }
}