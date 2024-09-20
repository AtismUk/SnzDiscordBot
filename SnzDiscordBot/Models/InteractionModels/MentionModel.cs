using Discord;
using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class MentionModel : IModal
{
    public string Title => "Укажите данные для оповещения"; 
    
    [InputLabel("Заголовок")]
    [ModalTextInput("title_input")]
    public string UserTitle { get; set; } = default!;

    [InputLabel("Информация")]
    [ModalTextInput("description_input", style: TextInputStyle.Paragraph)]
    public string Description { get; set; } = default!;
    
    [InputLabel("Ссылка на превью")]
    [ModalTextInput("preview_input")]
    public string ImageUrl { get; set; } = default!;
    
    [InputLabel("Ссылка на лого")]
    [ModalTextInput("logo_input")]
    public string ThumbnailUrl { get; set; } = default!;
}