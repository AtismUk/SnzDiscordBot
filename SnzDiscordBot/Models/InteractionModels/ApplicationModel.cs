using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class ApplicationModel : IModal
{
    public string Title => "Форма заявки на вступления";
    
    [InputLabel("Позывной")]
    [ModalTextInput("nick_input", placeholder: "Позывной", maxLength: 70)]
    public string Nick { get; set; } = string.Empty;
    
    [InputLabel("Steam Id")]
    [ModalTextInput("steamid_input", placeholder: "Steam Id", maxLength: 17)]
    public string SteamId { get; set; } = string.Empty;
    
    [InputLabel("От куда вы о нас узнали")]
    [ModalTextInput("info_input", placeholder: "Информация", maxLength: 100)]
    public string Info { get; set; } = string.Empty;
    
    [InputLabel("Как вы оцениваете свои навыки игры? (от 1 до 10)")]
    [ModalTextInput("points_input", placeholder: "Баллы", maxLength: 2)]
    public string Points { get; set; } = string.Empty;
}

public class ApplicationCancelModel : IModal
{
    public string Title => "Форма отклонения заявки";

    [InputLabel("Причина")]
    [ModalTextInput("text_input", placeholder: "Причина", maxLength: 100)]
    public string Reason { get; set; } = string.Empty;
    
    [InputLabel("Исключить пользователя? (Да/Нет)")]
    [ModalTextInput("text_input", placeholder: "Да/Нет", maxLength: 3, initValue: "Нет")]
    public string Kick { get; set; } = string.Empty;
}