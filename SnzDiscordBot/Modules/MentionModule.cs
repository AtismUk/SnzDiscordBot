using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using SnzDiscordBot.Models.InteractionModels;

namespace SnzDiscordBot.Modules;

public class MentionModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfiguration _config;

    #region Buttons
    private readonly ButtonBuilder _yesButton = new()
    {
        CustomId = "yes_button",
        Label = "\u2705 Участвую",
        Style = ButtonStyle.Success,
    };
    private readonly ButtonBuilder _noButton = new()
    {
        CustomId = "no_button",
        Label = "\u274c Не участвую",
        Style = ButtonStyle.Secondary,
    };
    private readonly ButtonBuilder _maybeButton = new()
    {
        CustomId = "maybe_button",
        Label = "\u2753 Не уверен",
        Style = ButtonStyle.Secondary,
    };
    #endregion

    public MentionModule(IConfiguration config)
    {
        _config = config;
    }

    [SlashCommand("mention", "Запустить голосование. Type может быть только \"новость\", \"мероприятие\" или \"расписание\".")]
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
            case "мероприятие":
                await RespondWithModalAsync<MentionModel>("event_form");
                break;
            default:
                await RespondAsync("Тип может быть только \"**новость**\", \"**расписание**\" или \"**мероприятие**\".", ephemeral: true);
                break;
        }
    }

    #region News
    [ModalInteraction("news_form")]
    public async Task HandlerNewsForm(MentionModel form)
    {
        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(_config["Settings:News_Channel_Id"] ?? string.Empty));
        if (channel == null)
        {
            await FollowupAsync("Ошибка. Указанный в конфиге канал не найден.", ephemeral: true);
        }

        var embedBuilder = new EmbedBuilder
        {
            Title = form.Title,
            Description = form.Description,
            ImageUrl = form.ImageUrl,
            ThumbnailUrl = form.ThumbnailUrl
        };

        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}",embed: embedBuilder.Build());
        
        await FollowupAsync("Успешно выполнено.", ephemeral: true);
    }
    
    #endregion

    #region Schedule
    [ModalInteraction("schedule_form")]
    public async Task HandlerScheduleForm(MentionModel form)
    {
        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(_config["Settings:News_Channel_Id"] ?? string.Empty));
        if (channel == null)
        {
            await FollowupAsync("Ошибка. Указанный в конфиге канал не найден.", ephemeral: true);
        }
        
        var embedBuilder = new EmbedBuilder
        {
            Title = form.Title,
            Description = form.Description,
            ImageUrl = form.ImageUrl,
            ThumbnailUrl = form.ThumbnailUrl
        };

        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}", embed: embedBuilder.Build());
        
        await FollowupAsync("Успешно выполнено.", ephemeral: true);
    }
    #endregion
    
    #region Event
    [ModalInteraction("event_form")]
    public async Task HandlerEventForm(MentionModel form)
    {
        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(_config["Settings:News_Channel_Id"] ?? string.Empty));
        if (channel == null)
        {
            await FollowupAsync("Ошибка. Указанный в конфиге канал не найден.", ephemeral: true);
        }
        
        var embedBuilder = new EmbedBuilder()
        {
            Title = form.Title,
            Description = form.Description,
            ImageUrl = form.ImageUrl,
            ThumbnailUrl = form.ThumbnailUrl,
        };
        var componentsBuilder = new ComponentBuilder();
        
        componentsBuilder.WithButton(_yesButton);
        componentsBuilder.WithButton(_noButton);
        componentsBuilder.WithButton(_maybeButton);
        
        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}", embed: embedBuilder.Build(), components: componentsBuilder.Build());
    }
    
    [ComponentInteraction("yes_button")]
    public async Task YesButtonHandler()
    {
        var interaction = (IModalInteraction)Context.Interaction;
        var message = interaction.Message;
        var embedProper = message.Embeds.First();
        var user = interaction.User;

        #region Изменение Embed

        var fieldsOld = embedProper!.Fields.ToList();
        
        // Получаем текущие значения полей "Участвуют", "Не участвуют" и "Не определились"
        var yesList =  new List<string>();
        var noList =  new List<string>();
        var maybeList =  new List<string>();
        
        foreach (var field in fieldsOld)
        {
            switch (field.Name.ToLower())
            {
                case "участвуют":
                    yesList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не участвуют":
                    noList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не определились":
                    maybeList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
            }
        }

        // Удаляем пользователя из других списков
        noList.Remove(user.Username);
        maybeList.Remove(user.Username);

        // Если пользователя нет в списке "Да", добавляем его
        if (!yesList.Contains(user.Username))
            yesList.Add(user.Username);

        // Собираем новые поля
        var fieldYes = new EmbedFieldBuilder()
        {
            Name = "Участвуют",
            Value = string.Join("\n", yesList.Count > 0 ? yesList : ["Пока никто не участвует"]),
            IsInline = true
        };

        var fieldNo = new EmbedFieldBuilder()
        {
            Name = "Не участвуют",
            Value = string.Join("\n", noList.Count > 0 ? noList : ["Никто не отказался"]),
            IsInline = true
        };

        var fieldMaybe = new EmbedFieldBuilder()
        {
            Name = "Не определились",
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не сомневается"]),
            IsInline = true
        };

        var fieldsNew = new List<EmbedFieldBuilder> { fieldYes, fieldNo, fieldMaybe };

        // Создаем новый Embed
        var embed = new EmbedBuilder()
        {
            Title = embedProper.Title,
            Description = embedProper.Description,
            ImageUrl = embedProper.Image?.ToString(),
            ThumbnailUrl = embedProper.Thumbnail?.ToString(),
            Fields = fieldsNew,
        };

        // Строим кнопки
        var componentsBuilder = new ComponentBuilder();
        componentsBuilder.WithButton(_yesButton);
        componentsBuilder.WithButton(_noButton);
        componentsBuilder.WithButton(_maybeButton);

        // Модифицируем сообщение
        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentsBuilder.Build();
        })!;

        await FollowupAsync("Успешно выполнено.", ephemeral: true);

        #endregion
    }

    
    [ComponentInteraction("no_button")]
    public async Task NoButtonHandler()
    {
        var interaction = (IModalInteraction)Context.Interaction;
        var message = interaction.Message;
        var embedProper = message.Embeds.First();
        var user = interaction.User;

        #region Изменение Embed

        var fieldsOld = embedProper!.Fields.ToList();
        
        // Получаем текущие значения полей "Участвуют", "Не участвуют" и "Не определились"
        var yesList =  new List<string>();
        var noList =  new List<string>();
        var maybeList =  new List<string>();
        
        foreach (var field in fieldsOld)
        {
            switch (field.Name.ToLower())
            {
                case "участвуют":
                    yesList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не участвуют":
                    noList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не определились":
                    maybeList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
            }
        }

        // Удаляем пользователя из других списков
        yesList.Remove(user.Username);
        maybeList.Remove(user.Username);

        // Если пользователя нет в списке "Да", добавляем его
        if (!noList.Contains(user.Username))
            noList.Add(user.Username);

        // Собираем новые поля
        var fieldYes = new EmbedFieldBuilder()
        {
            Name = "Участвуют",
            Value = string.Join("\n", yesList.Count > 0 ? yesList : ["Пока никто не участвует"]),
            IsInline = true
        };

        var fieldNo = new EmbedFieldBuilder()
        {
            Name = "Не участвуют",
            Value = string.Join("\n", noList.Count > 0 ? noList : ["Никто не отказался"]),
            IsInline = true
        };

        var fieldMaybe = new EmbedFieldBuilder()
        {
            Name = "Не определились",
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не сомневается"]),
            IsInline = true
        };

        var fieldsNew = new List<EmbedFieldBuilder> { fieldYes, fieldNo, fieldMaybe };

        // Создаем новый Embed
        var embed = new EmbedBuilder()
        {
            Title = embedProper.Title,
            Description = embedProper.Description,
            ImageUrl = embedProper.Image?.ToString(),
            ThumbnailUrl = embedProper.Thumbnail?.ToString(),
            Fields = fieldsNew,
        };

        // Строим кнопки
        var componentsBuilder = new ComponentBuilder();
        componentsBuilder.WithButton(_yesButton);
        componentsBuilder.WithButton(_noButton);
        componentsBuilder.WithButton(_maybeButton);

        // Модифицируем сообщение
        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentsBuilder.Build();
        })!;

        #endregion
    }
    
    [ComponentInteraction("maybe_button")]
    public async Task MaybeButtonHandler()
    {
        var interaction = (IModalInteraction)Context.Interaction;
        var message = interaction.Message;
        var embedProper = message.Embeds.First();
        var user = interaction.User;

        #region Изменение Embed

        var fieldsOld = embedProper!.Fields.ToList();
        
        // Получаем текущие значения полей "Участвуют", "Не участвуют" и "Не определились"
        var yesList =  new List<string>();
        var noList =  new List<string>();
        var maybeList =  new List<string>();
        
        foreach (var field in fieldsOld)
        {
            switch (field.Name.ToLower())
            {
                case "участвуют":
                    yesList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не участвуют":
                    noList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
                case "не определились":
                    maybeList = field.Value.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    continue;
            }
        }

        // Удаляем пользователя из других списков
        noList.Remove(user.Username);
        yesList.Remove(user.Username);

        // Если пользователя нет в списке "Да", добавляем его
        if (!maybeList.Contains(user.Username))
            maybeList.Add(user.Username);

        // Собираем новые поля
        var fieldYes = new EmbedFieldBuilder()
        {
            Name = "Участвуют",
            Value = string.Join("\n", yesList.Count > 0 ? yesList : ["Пока никто не участвует"]),
            IsInline = true
        };

        var fieldNo = new EmbedFieldBuilder()
        {
            Name = "Не участвуют",
            Value = string.Join("\n", noList.Count > 0 ? noList : ["Никто не отказался"]),
            IsInline = true
        };

        var fieldMaybe = new EmbedFieldBuilder()
        {
            Name = "Не определились",
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не сомневается"]),
            IsInline = true
        };

        var fieldsNew = new List<EmbedFieldBuilder> { fieldYes, fieldNo, fieldMaybe };

        // Создаем новый Embed
        var embed = new EmbedBuilder()
        {
            Title = embedProper.Title,
            Description = embedProper.Description,
            ImageUrl = embedProper.Image?.ToString(),
            ThumbnailUrl = embedProper.Thumbnail?.ToString(),
            Fields = fieldsNew,
        };

        // Строим кнопки
        var componentsBuilder = new ComponentBuilder();
        componentsBuilder.WithButton(_yesButton);
        componentsBuilder.WithButton(_noButton);
        componentsBuilder.WithButton(_maybeButton);

        // Модифицируем сообщение
        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentsBuilder.Build();
        })!;

        #endregion
    }
    #endregion

    #region EditMention

    [SlashCommand("edit-mention", "Изменить уже созданное оповещение.")]
    [RequireUserPermission(GuildPermission.MentionEveryone)]
    public async Task EditMentionCommand(string channel_id, string message_id)
    {
        await RespondWithModalAsync<MentionModel>($"edit_form:{channel_id}:{message_id}");
    }
    
    [ModalInteraction("edit_form")]
    public async Task HandlerEditForm(string customId, MentionModel form)
    {
        // Разделяем customId, чтобы получить channel_id и message_id
        var parts = customId.Split(':');
        if (parts.Length < 3)
        {
            await FollowupAsync("Произошла ошибка при получении данных.", ephemeral: true);
            return;
        }

        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(parts[1]));
        if (channel == null)
        {
            await FollowupAsync("Не удалось найти канал.", ephemeral: true);
            return;
        }
        
        var message = channel.GetMessageAsync(ulong.Parse(parts[2])).Result;
        if (message == null)
        {
            await FollowupAsync("Не удалось найти сообщение.", ephemeral: true);
            return;
        }

        // Создаем новый Embed с измененными данными
        var embed = new EmbedBuilder()
        {
            Title = form.Title,
            Description = form.Description,
            ImageUrl = form.ImageUrl,
            ThumbnailUrl = form.ThumbnailUrl,
        };

        // Редактируем сообщение
        await channel.ModifyMessageAsync(message.Id, properties => properties.Embed = embed.Build());

        await FollowupAsync("Сообщение успешно отредактировано.", ephemeral: true);
    }

    #endregion
}