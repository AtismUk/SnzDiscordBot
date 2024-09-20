using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class MentionModel : IModal
{
    [InputLabel("Заголовок")]
    public string Title { get; set; }
    
    [ModalTextInput("Информация")]
    public string Description { get; set; }
    
    [InputLabel("Ссылка на превью")]
    public string ImageUrl { get; set; }
    
    [InputLabel("Ссылка на лого")]
    public string ThumbnailUrl { get; set; }
}