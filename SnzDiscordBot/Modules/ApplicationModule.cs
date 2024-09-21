using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using SnzDiscordBot.Models.InteractionModels;
using Color = Discord.Color;

namespace SnzDiscordBot.Modules;

public class ApplicationModule : InteractionModuleBase<SocketInteractionContext>
{
    #region Buttons

    private readonly ButtonBuilder _acceptButton = new() {
        CustomId = "accept_button", 
        Label = "Принять", 
        Style = ButtonStyle.Success,
    };
    private readonly ButtonBuilder _cancelButton = new() {
        CustomId = "cancel_button",
        Label = "Отклонить",
        Style = ButtonStyle.Secondary,
    };
    
    private readonly ButtonBuilder _acceptButtonDisabled = new() {
        CustomId = "accept_button", 
        Label = "Принять", 
        Style = ButtonStyle.Success,
    };
    private readonly ButtonBuilder _cancelButtonDisabled = new() {
        CustomId = "cancel_button",
        Label = "Отклонить",
        Style = ButtonStyle.Secondary,
    };

    #endregion
    private readonly IConfiguration _config;
    public ApplicationModule(IConfiguration config)
    {
        _config = config;
    }
    
    [SlashCommand("application", "Отправить заявку на вступление")]
    public async Task ApplicationCommand()
    {
        var channelId = ulong.Parse(_config["Settings:Application_channel_Id"] ?? string.Empty);

        if (Context.Guild.Channels.FirstOrDefault(x => x.Id == channelId) != null)
        {
            await RespondWithModalAsync<ApplicationModel>("application_form");
        }
    }

    [ModalInteraction("application_form")]
    public async Task HandlerApplicationForm(ApplicationModel form)
    {
        if (Regex.IsMatch(form.Nick, @"^[a-zA-Z0-9]+$")) {
              await RespondAsync("Ваш позывной может содержать только латинские символы и цифры!", ephemeral: true);  
        }
        
        if (!byte.TryParse(form.Points, out var point) || point < 1 || point > 10)
        {
            await RespondAsync("Ваша субъективная оценка игры должны быть в диапазоне от 1 по 10!", ephemeral: true);
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
        embed.AddField("Субъективная оценка игры", point.ToString() + "/10");

        ComponentBuilder componentBuilder = new();
        componentBuilder.WithButton(_acceptButton);
        componentBuilder.WithButton(_cancelButton);

        await RespondAsync(embed: embed.Build(), components: componentBuilder.Build());

    }


    [ComponentInteraction("accept_button")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task AcceptButtonHandler()
    {

        #region Изменяем пользователя

        var interaction = (IComponentInteraction)Context.Interaction;

        var message = interaction.Message;

        var embedProper = message.Embeds.First();

        await Context.Guild.DownloadUsersAsync();

        var userId = ulong.Parse(embedProper.Footer!.Value.Text);

        var user = Context.Guild.Users.FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            await RespondAsync("Пользователь не найден на сервере", ephemeral: true);
        }

        await user!.RemoveRoleAsync(ulong.Parse(_config["Settings:Remove_Application_Role_Id"]!));

        await user.AddRoleAsync(ulong.Parse(_config["Settings:Add_Application_Role_Id"]!));

        await user.ModifyAsync(x => x.Nickname = "[SNZ] " + embedProper.Fields[0].Value);


        #endregion

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
        
        componentBuilder.WithButton(_acceptButtonDisabled);
        componentBuilder.WithButton(_cancelButtonDisabled);


        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentBuilder.Build();
        });

        await RespondAsync("Заявка принята", ephemeral: true);
        #endregion

    }


    [ComponentInteraction("cancel_button")]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public async Task CancelButtonHandler()
    {
        await RespondWithModalAsync<CancelModel>("cancel_form");

    }


    [ModalInteraction("cancel_form")]
    public async Task HandlerCancelForm(CancelModel form)
    {

        var interaction = (IModalInteraction)Context.Interaction;

        var message = interaction.Message;

        var embedProper = message.Embeds.First();

        var userId = ulong.Parse(embedProper.Footer!.Value.Text);

        var user = Context.Guild.Users.FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            await RespondAsync("Пользователь не найден на сервере", ephemeral: true);
        }

        #region Изменение Embed

        var embed = new EmbedBuilder()
        {
            Author = new()
            {
                Name = user!.GlobalName,
                IconUrl = user.GetAvatarUrl(),
            },
            Color = Color.Red,
            Title = "Заявка отклонена ❌",
        };
        embed.AddField("Позывной", embedProper.Fields[0].Value);
        embed.AddField("Steam", embedProper.Fields[1].Value);
        embed.AddField("От куда узнал", embedProper.Fields[2].Value);
        embed.AddField("Субъективная оценка игры", embedProper.Fields[3].Value);
        embed.AddField("Причина отклонения", form.Text);

        ComponentBuilder componentBuilder = new();
        componentBuilder.WithButton(_acceptButtonDisabled);
        componentBuilder.WithButton(_cancelButtonDisabled);


        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = componentBuilder.Build();
        });

        await RespondAsync("Заявка отклонена", ephemeral: true);
        #endregion
    }
}