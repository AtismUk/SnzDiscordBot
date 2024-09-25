using Discord;
using Discord.Interactions;
using SnzDiscordBot.Models.InteractionModels;
using SnzDiscordBot.Services.Interfaces;

namespace SnzDiscordBot.Modules;

public class MentionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ISettingsService _settingsService;
    
    public MentionModule(ISettingsService settings)
    {
        _settingsService = settings;
    }

    [SlashCommand("mention", "Создать оповещение. Тип может быть только \"новость\" или \"расписание\".")]
    [RequireUserPermission(GuildPermission.MentionEveryone)]
    public async Task MentionCommand(string type)
    {
        switch (type.ToLower())
        {
            case "новость":
                await RespondWithModalAsync<MentionModel>("news_form");
                break;
            case "расписание":
                await RespondWithModalAsync<MentionModel>("schedule_form");
                break;
            default:
                await RespondAsync("Тип может быть только \"**новость**\" или \"**расписание**\".", ephemeral: true);
                break;
        }
    }

    #region News
    [ModalInteraction("news_form")]
    public async Task HandlerNewsForm(MentionModel form)
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        var channel = (IMessageChannel?)Context.Guild.GetChannel(settings.NewsChannelId);
        if (channel == null)
        {
            await RespondAsync("Канал не найден!", ephemeral: true);
            return;
        }

        var embedBuilder = new EmbedBuilder()
        {
            Author = new EmbedAuthorBuilder()
            {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Context.User.Username,
            },
            Title = form.UserTitle,
            Description = form.Description,
        };
        if (form.ThumbnailUrl.StartsWith("http"))
        {
            embedBuilder.WithThumbnailUrl(form.ThumbnailUrl);
        }
        if (form.ImageUrl.StartsWith("http"))
        {
            embedBuilder.WithImageUrl(form.ImageUrl);
        }

        await channel.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}",embed: embedBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }
    
    #endregion

    #region Schedule
    [ModalInteraction("schedule_form")]
    public async Task HandlerScheduleForm(MentionModel form)
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        var channel = (IMessageChannel?)Context.Guild.GetChannel(settings.ScheduleChannelId);
        if (channel == null)
        {
            await RespondAsync("Канал не найден!", ephemeral: true);
            return;
        }
        
        var embedBuilder = new EmbedBuilder()
        {
            Author = new EmbedAuthorBuilder()
            {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Context.User.Username,
            },
            Title = form.UserTitle,
            Description = form.Description,
        };
        if (form.ThumbnailUrl.StartsWith("http"))
        {
            embedBuilder.WithThumbnailUrl(form.ThumbnailUrl);
        }
        if (form.ImageUrl.StartsWith("http"))
        {
            embedBuilder.WithImageUrl(form.ImageUrl);
        }

        await channel.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}", embed: embedBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }
    #endregion
}