using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class ApplicationModel : IModal
{
    public string Title => "Форма заявки на вступления";

    [InputLabel("Позывной")]
    [ModalTextInput("nick_input", Discord.TextInputStyle.Short, placeholder: "Позывной", maxLength: 70)]
    public string Nick { get; set; }

    [InputLabel("Steam Id")]
    [ModalTextInput("steamid_input", Discord.TextInputStyle.Short, placeholder: "Steam Id", maxLength: 70)]
    public string SteamId { get; set; }
    
    [InputLabel("От куда вы о нас узнали")]
    [ModalTextInput("info_input", Discord.TextInputStyle.Short, placeholder: "Информация", maxLength: 100)]
    public string Info { get; set; }
    
    [InputLabel("Ваша субъективная оценка игры от 1 до 10")]
    [ModalTextInput("points_input", Discord.TextInputStyle.Short, placeholder: "Баллы", maxLength: 2)]
    public string Points { get; set; }
}