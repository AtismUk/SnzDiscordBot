using Discord;
using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class MentionModel : IModal
{
    public string Title => "Укажите данные для оповещения"; 
    
    [InputLabel("Заголовок")]
    [ModalTextInput("title_input", minLength: 5, maxLength: 100, placeholder: "Ваш заголовок.")]
    public string UserTitle { get; set; } = default!;

    [InputLabel("Информация")]
    [ModalTextInput("description_input", minLength: 20, style: TextInputStyle.Paragraph)]
    public string Description { get; set; } = default!;
    
    [InputLabel("Ссылка на превью")]
    [ModalTextInput("preview_input", placeholder: "Ссылка на превью. Поставьте минус если без превью.")]
    public string ImageUrl { get; set; } = default!;
    
    [InputLabel("Ссылка на лого")]
    [ModalTextInput("logo_input", placeholder: "Ссылка на лого. Поставьте минус если без лого.")]
    public string ThumbnailUrl { get; set; } = default!;
}