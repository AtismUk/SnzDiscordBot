using Discord.Interactions;

namespace SnzDiscordBot.Models.InteractionModels;

public class EventModel : MentionModel
{
    [InputLabel("Время начала")]
    [ModalTextInput("time_input", placeholder: "2024-09-25 12:30")]
    public string StartTime { get; set; } = default!;
}