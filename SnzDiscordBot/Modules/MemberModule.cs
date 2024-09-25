using Discord;
using Discord.Interactions;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class MemberModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMemberService _memberService;
    private readonly IEventService _eventService;
    
    public MemberModule(IMemberService memberService, IEventService eventService)
    {
        _memberService = memberService;
        _eventService = eventService;
    }
    
    [SlashCommand("member-info", "Посмотреть данные участника")]
    public async Task MemberInfoCommand(IUser user)
    {
        var member = await _memberService.GetMemberAsync(Context.Guild.Id, user.Id);

        if (member == null)
        {
            await RespondAsync("Пользователь не найден!", ephemeral: true);
            return;
        }
        
        var lastEvent = await _eventService.GetLastEvent(Context.Guild.Id, member.UserId);
        
        var fields = new List<EmbedFieldBuilder>
        {
            new()
            {
                Name = "Позывной",
                Value = member.Username,
            },
            new()
            {
                Name = "Звание",
                Value = _memberService.GetRankName(member.Rank),
            },
            new()
            {
                Name = "Группа",
                Value = _memberService.GetGroupName(member.Group),
            },
            new()
            {
                Name = "Статус",
                Value = _memberService.GetStatusName(member.Status),
            }
        };

        if (member.Roles.Count > 0)
        {
            var roles = string.Join(", ", member.Roles.Select(role => _memberService.GetRoleName(role)));

            fields.Add(new EmbedFieldBuilder
            {
                Name = "Роли",
                Value = roles
            });
        }

        Console.WriteLine(lastEvent?.DateTime.ToString("dd/MM/yyyy HH:mm")+" asddddddddddddddddddddddddddddddddddddddd");
        if (lastEvent != null)
        {
            fields.Add(new EmbedFieldBuilder
            {
                Name = "Был на играх последний раз",
                Value = lastEvent.DateTime.ToString("yyyy-MM-dd HH:mm")
            });
        }


        var emberBuilder = new EmbedBuilder()
        {
            Title = "Запрос информации об участнике",
            Fields = fields,
            Author = new EmbedAuthorBuilder()
            {
                Name = Context.User.Username,
                IconUrl = Context.User.GetAvatarUrl(),
            },
        };
        
        await RespondAsync(embed: emberBuilder.Build());
    }

    [SlashCommand("update-member", "Обновить данные участника")]
    public async Task UpdateMemberCommand(IUser user, string? username = null, string? rank = null, string? group = null, string? status = null, string? roles = null)
    {
        var parsedRank = string.IsNullOrEmpty(rank) ? (Rank?)null : Enum.Parse<Rank>(rank, true);
        var parsedGroup = string.IsNullOrEmpty(group) ? (Group?)null : Enum.Parse<Group>(group, true);
        var parsedStatus = string.IsNullOrEmpty(status) ? (Status?)null : Enum.Parse<Status>(status, true);
        var parsedRoles = string.IsNullOrEmpty(roles) ? null : roles
            .Split(',')
            .Select(r => Enum.Parse<Role>(r, true))
            .ToList();

        if (await _memberService.AddUpdateMemberAsync(Context.Guild.Id, user.Id, username, parsedRank, parsedGroup, parsedStatus, parsedRoles))
        {
            await RespondAsync("Пользователь обновлен успешно!", ephemeral: true);
        }
        else
        {
            await RespondAsync("Ошибка базы данных!", ephemeral: true);
        }
    }
}