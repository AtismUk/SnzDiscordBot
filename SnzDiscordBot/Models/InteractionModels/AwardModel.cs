using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class AwardModel : IModal
{
    public string Title => "Форма для создания награды";
    
    [InputLabel("Уникальный дескриптор")]
    [ModalTextInput("descriptor_input", placeholder: "meow", maxLength: 50)]
    public string Descriptor { get; set; } = string.Empty;
    
    [InputLabel("Приоритет")]
    [ModalTextInput("priority_input", placeholder: "123", maxLength: 50)]
    public string Priority { get; set; } = string.Empty;
    
    [InputLabel("Название")]
    [ModalTextInput("name_input", placeholder: "Награда", maxLength: 1000)]
    public string Name { get; set; } = string.Empty;
    
    [InputLabel("Описание")]
    [ModalTextInput("description_input", placeholder: "Я описание.", maxLength: 1000)]
    public string Description { get; set; } = string.Empty;
    
    [InputLabel("Ссылка на изображение")]
    [ModalTextInput("image_input", placeholder: "Ссылка на картинку. Поставьте минус если без картинки.", maxLength: 1000)]
    public string ImageUrl { get; set; } = string.Empty;
}