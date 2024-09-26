using Discord;
using Discord.Interactions;
using System.Text;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class AddRoleAllModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMemberService _memberService;
    
    public AddRoleAllModule(IMemberService memberService)
    {
        _memberService = memberService;
    }
    
    [SlashCommand("addRoleAll", "Выдать роль всем.")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AddRoleAllCommand(IRole add_role, string? ignore_roles = "", string? update_status = null)
    {
        // Отправляем ответ сразу, чтобы не улететь в таймаут.
        await DeferAsync(ephemeral: true);
        
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
        
        #region Обрабатываем статус

        Status? targetStatus = null;
        
        if (update_status != null)
        {
            if (!Enum.TryParse<Status>(update_status, out var status))
            {
                targetStatus = status;
                await FollowupAsync("Не удалось распознать целевой статус!\nДоступны варианты:\n-Unknown\n-Active\n-Vacation\n-Reserve\n-Left", ephemeral: true);
                return; 
            }
        }

        #endregion
        
        #region Выдаем пользователям роль.

        int addedCount = 0;
        int changedStatusCount = 0;
        
        await foreach (var users in Context.Guild.GetUsersAsync())
        {
            foreach (var user in users)
            {
                if (user.RoleIds.Contains(add_role.Id))
                    continue;
                
                if (ignoreRoles != null && user.RoleIds.Any(roleId => ignoreRoles.Any(role => role.Id == roleId)))
                    continue;
                
                await user.AddRoleAsync(add_role);

                if (await _memberService.UpdateMemberAsync(Context.Guild.Id, user.Id, status: targetStatus) == null)
                {
                    errorBuilder.AppendLine($"Не удалось изменить статус пользователю: {user.Mention}");
                }
                else
                {
                    changedStatusCount++;
                }
                addedCount++;
            }
        }
        
        resultMessage.AppendLine($"Роль {add_role.Name} была добавлена {addedCount} пользователям.");
        if (targetStatus != null)
            resultMessage.AppendLine($"Статус изменен {changedStatusCount} пользователям на {targetStatus.ToString()}.");

        #endregion

        // Добавляем запись об ошибках.
        if (errorBuilder.Length > 0)
        {
            resultMessage.AppendLine("Ошибки:");
            resultMessage.Append(errorBuilder.ToString());
        }

        // Редактируем ответ.
        await FollowupAsync(resultMessage.ToString(), ephemeral: true);
    }
}