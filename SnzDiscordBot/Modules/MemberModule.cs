using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class MemberModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMemberService _memberService;
    
    public MemberModule(IMemberService memberService)
    {
        _memberService = memberService;
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

        if (member.Roles.Length > 0)
        {
            var roles = string.Join(", ", member.Roles.Select(role => _memberService.GetRoleName(role)));

            fields.Add(new EmbedFieldBuilder
            {
                Name = "Роли",
                Value = roles
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
}