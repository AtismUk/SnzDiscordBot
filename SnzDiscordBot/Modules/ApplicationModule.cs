using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using SnzDiscordBot.DataBase.Entities;
using SnzDiscordBot.Models.InteractionModels;
using SnzDiscordBot.Services.Interfaces;
using Group = SnzDiscordBot.DataBase.Entities.Group;

namespace SnzDiscordBot.Modules;

public class ApplicationModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ISettingsService _settingsService;
    private readonly IMemberService _memberService;
    
    public ApplicationModule(ISettingsService settings, IMemberService memberService)
    {
        _settingsService = settings;
        _memberService = memberService;
    }
    
    [SlashCommand("application", "Отправить заявку на вступление")]
    public async Task ApplicationCommand()
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        if (settings == null)
        {
            await RespondAsync("Ошибка базы данных! Не удалось получить настройки.", ephemeral: true);
            return;
        }
        
        if (Context.Channel.Id == settings.ApplicationChannelId)
        {
            await RespondWithModalAsync<ApplicationModel>("application_form");
        }
        else
        {
            await RespondAsync("В этом канале запрещено подавать заявку!", ephemeral: true);
        }
    }

    [ModalInteraction("application_form")]
    public async Task HandlerApplicationForm(ApplicationModel form)     
    {
        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        if (settings == null)
        {
            await RespondAsync("Ошибка базы данных! Не удалось получить настройки.", ephemeral: true);
            return;
        }
        
        if (!Regex.IsMatch(form.Nick, @"^[a-zA-Z0-9]+$")) {
              await RespondAsync("Ваш позывной может содержать только латинские символы и цифры!", ephemeral: true);  
        }
        
        if (!byte.TryParse(form.Points, out var point) || point > 10)
        {
            await RespondAsync("Оценка вашей игры должны быть в диапазоне от 0 до 10!", ephemeral: true);
        }

        if (!long.TryParse(form.SteamId, out var steamId) || form.SteamId.Length != 17)
        {
            await RespondAsync("Введите корректный SteamID64!", ephemeral: true);
        }

        var embed = new EmbedBuilder()
        {
            Author = new EmbedAuthorBuilder
            {
                Name = Context.User.GlobalName,
                IconUrl = Context.User.GetAvatarUrl()
            },
            Color = Color.LightOrange,
            Title = "Заявка на вступление ⌛",
            Footer = new EmbedFooterBuilder
            {
                Text = Context.User.Id.ToString()
            }
        };
        embed.AddField("Позывной", form.Nick);
        embed.AddField("Steam", "https://steamcommunity.com/profiles/" + steamId.ToString());
        embed.AddField("От куда узнал", form.Info);
        embed.AddField("Субъективность оценка игры", point.ToString() + "/10");

        ComponentBuilder componentBuilder = new();
        
        ButtonBuilder acceptButton = new() {
            CustomId = "accept_button", 
            Label = "Принять", 
            Style = ButtonStyle.Success,
        };
        ButtonBuilder cancelButton = new() {
            CustomId = "cancel_button",
            Label = "Отклонить",
            Style = ButtonStyle.Secondary,
        };
        
        componentBuilder.WithButton(acceptButton);
        componentBuilder.WithButton(cancelButton);

        await RespondAsync(embed: embed.Build(), components: componentBuilder.Build());
    }


    [ComponentInteraction("accept_button")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AcceptButtonHandler()
    {
        #region Проверки

        var settings = await _settingsService.GetSettingsAsync(Context.Guild.Id);
        if (settings == null)
        {
            await RespondAsync("Ошибка базы данных! Не удалось получить настройки.", ephemeral: true);
            return;
        }
        
        var interaction = (IComponentInteraction)Context.Interaction;
        if (interaction == null)
        {
            await RespondAsync("Ошибка! Не удалось найти интеракцию.", ephemeral: true);
            return;
        }
        
        var message = interaction.Message;
        if (message == null)
        {
            await RespondAsync("Ошибка! Не удалось найти сообщение заявки.", ephemeral: true);
            return;
        }
        
        var embedProper = message.Embeds.First();
        if (embedProper == null)
        {
            await RespondAsync("Ошибка! Не удалось найти Embed из старого сообщения.", ephemeral: true);
            return;
        }

        if (!ulong.TryParse(embedProper.Footer!.Value.Text, out var userId))
        {
            await RespondAsync("Ошибка! Не удалось преобразовать ID пользователя.", ephemeral: true);
            return;
        }
        
        var user = Context.Guild.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null)
        {
            await RespondAsync("Ошибка! Пользователь не найден на сервере. Заявка будет удалена.", ephemeral: true);
            await interaction.Message.DeleteAsync();
            return;
        }

        #endregion
        
        #region Изменяем пользователя
        
        await user.RemoveRoleAsync(settings.ApplicationRemoveRoleId);
        await user.AddRoleAsync(settings.ApplicationAddRoleId);
        await user.ModifyAsync(x => x.Nickname = "[SNZ] " + embedProper.Fields[0].Value);

        #endregion

        var member = await _memberService.UpdateMemberAsync(Context.Guild.Id, user.Id, embedProper.Fields[0].Value, Rank.Rookie, Group.Unknown, [], Status.Active);
        
        #region Изменение Embed

        var embed = new EmbedBuilder()
        {
            Author = new()
            {
                Name = user.GlobalName,
                IconUrl = user.GetAvatarUrl(),
            },
            Color = Color.Green,
            Title = "Заявка принята ✅",
        };
        embed.AddField("Позывной", embedProper.Fields[0].Value);
        embed.AddField("Steam", embedProper.Fields[1].Value);
        embed.AddField("От куда узнал", embedProper.Fields[2].Value);
        embed.AddField("Субъективная оценка игры", embedProper.Fields[3].Value);

        ComponentBuilder componentBuilder = new();
        
        ButtonBuilder acceptButton = new() {
            CustomId = "accept_button", 
            Label = "Принять", 
            Style = ButtonStyle.Success,
            IsDisabled = true
        };
        ButtonBuilder cancelButton = new() {
            CustomId = "cancel_button",
            Label = "Отклонить",
            Style = ButtonStyle.Secondary,
            IsDisabled = true
        };

        componentBuilder.WithButton(acceptButton);
        componentBuilder.WithButton(cancelButton);

        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentBuilder.Build();
            
        });

        #endregion

        if (member != null)
        {
            await RespondAsync("Заявка принята успешно!", ephemeral: true);
            return;
        }
        
        await RespondAsync("Заявка принята с ошибкой базы данных.", ephemeral: true);
    }


    [ComponentInteraction("cancel_button")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task CancelButtonHandler()
    {
        await RespondWithModalAsync<ApplicationCancelModel>("cancel_form");
    }

    
    [ModalInteraction("cancel_form")]
    public async Task HandlerCancelForm(ApplicationCancelModel form)
    {
        #region Проверки

        var interaction = ;
        if (Context.Interaction )
        {
            await RespondAsync("Ошибка! Не удалось найти интеракцию.", ephemeral: true);
            return;
        }

        var message = interaction.Message;
        if (message == null)
        {
            await RespondAsync("Ошибка! Не удалось найти сообщение заявки.", ephemeral: true);
            return;
        }

        var embedProper = message.Embeds.First();
        if (embedProper == null)
        {
            await RespondAsync("Ошибка! Не удалось найти Embed из старого сообщения.", ephemeral: true);
            return;
        }

        if (!ulong.TryParse(embedProper.Footer!.Value.Text, out var userId))
        {
            await RespondAsync("Ошибка! Не удалось преобразовать ID пользователя.", ephemeral: true);
            return;
        }

        var user = Context.Guild.Users.FirstOrDefault(x => x.Id == userId);
        if (user == null)
        {
            await RespondAsync("Ошибка! Пользователь не найден на сервере. Заявка будет удалена.", ephemeral: true);
            await interaction.Message.DeleteAsync();
            return;
        }

        #endregion
        
        switch (form.Kick.ToLower())
        {
            case "да":
                await user.KickAsync("Заявка отклонена с исключением пользователя.");
                break;
            case "нет":
                break;
            default:
                await RespondAsync("Ошибка! Не удалось определить будет пользователь исключен или нет.", ephemeral: true);
                return;
        }

        #region Изменение Embed

        var embed = new EmbedBuilder()
        {
            Author = new()
            {
                Name = user.GlobalName,
                IconUrl = user.GetAvatarUrl(),
            },
            Color = Color.Red,
            Title = "Заявка отклонена ❌",
        };
        embed.AddField("Позывной", embedProper.Fields[0].Value);
        embed.AddField("Steam", embedProper.Fields[1].Value);
        embed.AddField("От куда узнал", embedProper.Fields[2].Value);
        embed.AddField("Субъективная оценка игры", embedProper.Fields[3].Value);
        embed.AddField("Причина отклонения", form.Reason);
        embed.AddField("Пользователь исключен?", form.Kick);

        ComponentBuilder componentBuilder = new();

        ButtonBuilder acceptButton = new() {
            CustomId = "accept_button", 
            Label = "Принять", 
            Style = ButtonStyle.Success,
            IsDisabled = true
        };
        ButtonBuilder cancelButton = new() {
            CustomId = "cancel_button",
            Label = "Отклонить",
            Style = ButtonStyle.Secondary,
            IsDisabled = true
        };
        
        componentBuilder.WithButton(acceptButton);
        componentBuilder.WithButton(cancelButton);


        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentBuilder.Build();
        });
        
        #endregion
        
        await RespondAsync("Заявка отклонена успешно!", ephemeral: true);
    }
}