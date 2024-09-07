using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnzDiscordBot.Models.InteractionModels
{
    public class CancelModel : IModal
    {
        public string Title => "Причина отклонения";

        [InputLabel("Причина")]
        [ModalTextInput("text_input", Discord.TextInputStyle.Short, placeholder: "Причина", maxLength: 100)]
        public string Text { get; set; }
    }
}