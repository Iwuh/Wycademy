using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Threading;
using Discord.Net;

namespace WycademyV2.Commands.Entities
{
    public class ToastTimerMessage : ReactionMenuMessage
    {
        private const string START = "▶";
        private const string STOP = "⏹";
        private const string RESTART = "↩";

        private readonly string[] FOOTERS = new string[] { "You wont cart! 👀", "Boop \xD83E\xDD81", "powswipe \xD83E\xDD81", "Pawswipe nova \xD83E\xDD81 👊 😱 💥", "dying to flemthrowur LOL",
                                                           "ian upswing 💥 😱 ↖ <:mhHammer:263441499822358528> 😬", "ranged cancer 💥 👺 💥 \xD83E\xDD81", "le ebin bonus blast XDXDXD",
                                                           "DC 🔥🗄🔥 👀", "supre nova 💥 \xD83E\xDD81 💥", "CURVED BOOP \xD83E\xDD81 ⤴ ⤵", "DOUBLE NOVA 💥\xD83E\xDD81💥😱💥\xD83E\xDD81💥               👀 👀 👀",
                                                           "plesioth hipcheck from another dimension 👀", "neri kills u 👁 👄 👁 💢", "no carts but you forget tresur \xD83E\xDD14",
                                                           "spawncamped 💥 💩 💥 \xD83E\xDD81 🖐", "backhop 😱 ↙ \xD83E\xDD81 ↖", "timeout 😒", "benned 😱 🔨 <:GreatJaggi:263441401948274692>",
                                                           "Hmmm, you should probably ask ducks"};

        private Random _randFooter;

        private IUserMessage _message;
        private Timer _timer;
        private bool _running;

        public ToastTimerMessage(IUser user) : base(user)
        {
            _randFooter = new Random();
            _timer = new Timer(OnTimerEnded, null, 90000, Timeout.Infinite);
        }

        public async override Task CloseMenuAsync()
        {
            try
            {
                await _message.DeleteAsync();
            }
            catch (HttpException)
            {
                // If we get here the message was already deleted. There's nothing to do, so just move on.
            }
            finally
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        public async override Task<IUserMessage> CreateMessageAsync(IMessageChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 0, 0))
                .WithTitle("Toast Timer")
                .WithDescription($"A handy timer to help you time your Teostra novas. Click {START} to start the timer, {STOP} to stop it, and {RESTART} to restart from 0.")
                .WithFooter(new EmbedFooterBuilder() { Text = FOOTERS[_randFooter.Next(FOOTERS.Length)] });

            var message = await channel.SendMessageAsync(string.Empty, embed: embed);

            await message.AddReactionAsync(START);
            await message.AddReactionAsync(STOP);
            await message.AddReactionAsync(RESTART);

            _message = message;
            return message;
        }

        public async override Task HandleReaction(SocketReaction reaction)
        {
            switch (reaction.Emoji.Name)
            {
                case START:
                    if (_running) break;
                    _timer.Change(90000, Timeout.Infinite);
                    await _message.Channel.SendMessageAsync("Timer started. You will be notified in 90 seconds.");
                    _running = true;
                    break;
                case STOP:
                    if (!_running) break;
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    await _message.Channel.SendMessageAsync($"Timer stopped. Click {START} to start again from 0.");
                    _running = false;
                    break;
                case RESTART:
                    if (_running) _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Change(90000, Timeout.Infinite);
                    await _message.Channel.SendMessageAsync("Timer reset to 0. You will be notified in 90 seconds.");
                    _running = true;
                    break;
            }
        }

        private async void OnTimerEnded(object state)
        {
            await _message.Channel.SendMessageAsync($"{User.Mention}, nova coming in ~10 seconds! (timer stopped)");
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _running = false;
        }
    }
}
