using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;

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
        // Проверим, есть ли у бота право управлять ролями
        var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
        if (!botUser.GuildPermissions.ManageRoles)
        {
            await RespondAsync("У бота недостаточно прав для управления ролями.", ephemeral: true);
            return;
        }

        int addedCount = 0;
        
        // Получаем пользователей асинхронно
        await foreach (var users in Context.Guild.GetUsersAsync())
        {
            foreach (var user in users)
            {
                // Пропускаем пользователей, у которых уже есть роль, которую нужно игнорировать
                if (user.RoleIds.Contains(ignore_role.Id)) continue;

                // Проверяем, может ли бот добавить роль пользователю
                if (add_role.Position <= botUser.Hierarchy && user.Hierarchy < botUser.Hierarchy)
                {
                    try
                    {
                        // Добавляем роль
                        await user.AddRoleAsync(add_role);
                        addedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Обрабатываем исключения, если не удалось добавить роль
                        await RespondAsync($"Не удалось добавить роль пользователю {user.Username}: {ex.Message}", ephemeral: true);
                    }
                }
            }
        }

        // Отвечаем, сколько пользователей получили роль
        await RespondAsync($"Роль {add_role.Name} была добавлена {addedCount} пользователям.", ephemeral: true);
    }
}
