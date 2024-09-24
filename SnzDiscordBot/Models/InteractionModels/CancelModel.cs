using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class CancelModel : IModal
{
    public string Title => "Причина отклонения";

    [InputLabel("Причина")]
    [ModalTextInput("text_input", placeholder: "Причина", maxLength: 100)]
    public string Text { get; set; } = default!;
}