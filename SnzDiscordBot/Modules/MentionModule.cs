using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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
            await RespondAsync("Канал не найден!", ephemeral: true);
            return;
        }

        var embedBuilder = new EmbedBuilder()
        {
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

        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}",embed: embedBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }
    
    #endregion

    #region Schedule
    [ModalInteraction("schedule_form")]
    public async Task HandlerScheduleForm(MentionModel form)
    {
        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(_config["Settings:Schedule_Channel_Id"] ?? string.Empty));
        if (channel == null)
        {
            await RespondAsync("Канал не найден!", ephemeral: true);
            return;
        }
        
        var embedBuilder = new EmbedBuilder()
        {
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

        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}", embed: embedBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }
    #endregion
    
    #region Event
    [ModalInteraction("event_form")]
    public async Task HandlerEventForm(MentionModel form)
    {
        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(_config["Settings:Event_Channel_Id"] ?? string.Empty));
        if (channel == null)
        {
            await RespondAsync("Канал не найден!", ephemeral: true);
            return;
        }
        
        var embedBuilder = new EmbedBuilder()
        {
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
        
        var componentsBuilder = new ComponentBuilder();
        
        componentsBuilder.WithButton(_yesButton);
        componentsBuilder.WithButton(_noButton);
        componentsBuilder.WithButton(_maybeButton);
        
        await channel!.SendMessageAsync($"{Context.Guild.EveryoneRole.Mention}", embed: embedBuilder.Build(), components: componentsBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }
    
    [ComponentInteraction("yes_button")]
    public async Task YesButtonHandler()
    {
        var interaction = (SocketMessageComponent?)Context.Interaction;
        if (interaction == null)
        {
            await RespondAsync("Интеракция не найдена!", ephemeral: true);
            return;
        }

        var message = interaction.Message;
        if (message == null)
        {
            await RespondAsync("Сообщение не найдено!", ephemeral: true);
            return;
        }

        var embedProper = message.Embeds.First();
        if (embedProper == null)
        {
            await RespondAsync("Старый Embed не найден!", ephemeral: true);
            return;
        }

        var user = interaction.User;
        if (user == null)
        {
            await RespondAsync("Пользователь не найден!", ephemeral: true);
            return;
        }

        #region Изменение Embed

        var fieldsOld = embedProper.Fields.ToList();

        // Получаем текущие значения полей "Участвуют", "Не участвуют" и "Не определились"
        var yesList = new List<string>();
        var noList = new List<string>();
        var maybeList = new List<string>();

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
        noList.Remove(user.Mention);
        maybeList.Remove(user.Mention);

        // Если пользователя нет в списке "Да", добавляем его
        if (!yesList.Contains(user.Mention))
        {
            yesList.Add(user.Mention);
            yesList.Remove("Пока никто не участвует");
        }

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
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не определился"]),
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

        await RespondAsync("Успешно!", ephemeral: true);
    }

    
    [ComponentInteraction("no_button")]
    public async Task NoButtonHandler()
    {
        var interaction = (SocketMessageComponent?)Context.Interaction;
        if (interaction == null)
        {
            await RespondAsync("Интеракция не найдена!", ephemeral: true);
            return;
        }

        var message = interaction.Message;
        if (message == null)
        {
            await RespondAsync("Сообщение не найдено!", ephemeral: true);
            return;
        }

        var embedProper = message.Embeds.First();
        if (embedProper == null)
        {
            await RespondAsync("Старый Embed не найден!", ephemeral: true);
            return;
        }

        var user = interaction.User;
        if (user == null)
        {
            await RespondAsync("Пользователь не найден!", ephemeral: true);
            return;
        }

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
        yesList.Remove(user.Mention);
        maybeList.Remove(user.Mention);

        // Если пользователя нет в списке "Да", добавляем его
        if (!noList.Contains(user.Mention))
        {
            noList.Add(user.Mention);
            noList.Remove("Никто не отказался");
        }

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
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не определился"]),
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
        
        await RespondAsync("Успешно!", ephemeral: true);
    }
    
    [ComponentInteraction("maybe_button")]
    public async Task MaybeButtonHandler()
    {
        var interaction = (SocketMessageComponent?)Context.Interaction;
        if (interaction == null)
        {
            await RespondAsync("Интеракция не найдена!", ephemeral: true);
            return;
        }

        var message = interaction.Message;
        if (message == null)
        {
            await RespondAsync("Сообщение не найдено!", ephemeral: true);
            return;
        }

        var embedProper = message.Embeds.First();
        if (embedProper == null)
        {
            await RespondAsync("Старый Embed не найден!", ephemeral: true);
            return;
        }

        var user = interaction.User;
        if (user == null)
        {
            await RespondAsync("Пользователь не найден!", ephemeral: true);
            return;
        }

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
        noList.Remove(user.Mention);
        yesList.Remove(user.Mention);

        // Если пользователя нет в списке "Да", добавляем его
        if (!maybeList.Contains(user.Mention))
        {
            maybeList.Add(user.Mention);
            maybeList.Remove("Никто не определился");
        }

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
            Value = string.Join("\n", maybeList.Count > 0 ? maybeList : ["Никто не определился"]),
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
        
        await RespondAsync("Успешно!", ephemeral: true);
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
            return;

        var channel = (IMessageChannel?)Context.Guild.GetChannel(ulong.Parse(parts[1]));
        if (channel == null)
            return;
        
        var message = channel.GetMessageAsync(ulong.Parse(parts[2])).Result;
        if (message == null)
            return;

        // Создаем новый Embed с измененными данными
        var embedBuilder = new EmbedBuilder()
        {
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

        // Редактируем сообщение
        await channel.ModifyMessageAsync(message.Id, properties => properties.Embed = embedBuilder.Build());
        await RespondAsync("Выполнено!", ephemeral: true);
    }

    #endregion
}