using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Services
{
    public class ToastTimerService
    {
        private const string START = "▶";
        private const string STOP = "⏹";
        private const string RESTART = "↩";
        private const string CANCEL = "❌";

        private Dictionary<ulong, ToastTimerMessage> _messages;
        private DiscordSocketClient _client;

        public ToastTimerService(DiscordSocketClient client)
        {
            _messages = new Dictionary<ulong, ToastTimerMessage>();
            _client = client;
            _client.ReactionAdded += OnReactionAdded;
        }

        /// <summary>
        /// Sends a toast timer message and tracks any reactions.
        /// </summary>
        /// <param name="context">The context of the command, used for user/channel/etc.</param>
        /// <param name="cache">An optional CommandCacheService to cache the message.</param>
        /// <returns>The sent message.</returns>
        public async Task<IUserMessage> SendToastTimerMessageAsync(CommandContext context, CommandCacheService cache = null)
        {
            var embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { IconUrl = _client.CurrentUser.GetAvatarUrl(), Name = _client.CurrentUser.Username })
                .WithColor(new Color(255, 0, 0))
                .WithTitle("Toast Timer")
                .WithDescription($"A timer to help you toast. Click {START} to start the timer, {STOP} to stop the timer, {RESTART} to restart the timer, and {CANCEL} to close this message. It will automatically delete itself after 20 minutes.")
                .WithFooter(new EmbedFooterBuilder() { Text = "tost" });

            IUserMessage m;
            if (cache != null)
            {
                m = await context.Channel.SendCachedMessageAsync(context.Message.Id, cache, embed: embed);
            }
            else
            {
                m = await context.Channel.SendMessageAsync(string.Empty, embed: embed);
            }

            await m.AddReactionAsync(START);
            await m.AddReactionAsync(STOP);
            await m.AddReactionAsync(RESTART);
            await m.AddReactionAsync(CANCEL);

            await Task.Delay(1000);

            _messages.Add(m.Id, new ToastTimerMessage(context.User, context.Channel, m, AutoDeleteCallback));

            return m;
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var userMessage = await cacheable.GetOrDownloadAsync();
            // If the reaction's user isn't specified assume it's not the right one.
            if (!reaction.User.IsSpecified) return;

            // Make sure the message is in the dictionary.
            ToastTimerMessage toastMessage;
            if (_messages.TryGetValue(cacheable.Id, out toastMessage))
            {
                // Don't trigger anything when the bot is originally adding the reactions.
                if (reaction.UserId == _client.CurrentUser.Id) return;
                // Don't do anything if the wrong user clicks a reaction.
                if (toastMessage.User.Id != reaction.UserId) return;

                switch (reaction.Emoji.Name)
                {
                    case START:
                        await toastMessage.StartTimer();
                        break;
                    case STOP:
                        await toastMessage.StopTimer();
                        break;
                    case RESTART:
                        await toastMessage.ResetTimer();
                        break;
                    case CANCEL:
                        await toastMessage.Cancel();
                        toastMessage.Dispose();
                        _messages.Remove(cacheable.Id);
                        break;
                    default:
                        break;
                }
            }
        }

        private async void AutoDeleteCallback(object state)
        {
            // TimerCallback requires that the method takes an object, so here we attempt to cast it back to a ToastTimerMessage.
            var toastMessage = state as ToastTimerMessage;
            if (toastMessage == null) return;

            try
            {
                await toastMessage.Cancel();
            }
            catch (HttpException)
            {
                // If this exception is thrown, the message was already deleted beforehand by somebody with manage messages.
                // There's nothing we can do, so we just move on.
            }
            finally
            {
                toastMessage.Dispose();
                _messages.Remove(toastMessage.Message.Id);
            }
        }

        private class ToastTimerMessage : IDisposable
        {
            // The user to notify and the channel to send the message in.
            public IUser User { get; private set; }
            public IMessageChannel Channel { get; private set; }
            public IUserMessage Message { get; private set; }

            // A System.Threading.Timer, not a System.Timers.Timer, nor a System.Windows.Forms.Timer...
            private Timer _toastTimer;
            private Timer _autoDeleteTimer;
            // The method to pass the messsage to when the autodelete timer expires.
            private TimerCallback _autoDeleteCallback;

            /// <summary>
            /// Initialize a new message with a specified user and channel.
            /// </summary>
            /// <param name="u">The user to notify.</param>
            /// <param name="c">The channel to send messages in.</param>
            /// <param name="m">The message with the controls.</param>
            /// <param name="autodelete">The method to call to autodelete the message after 20 minutes.</param>
            public ToastTimerMessage(IUser u, IMessageChannel c, IUserMessage m, TimerCallback autodelete)
            {
                User = u;
                Channel = c;
                Message = m;
                _autoDeleteCallback = autodelete;
                // Initialize the nova timer but don't start it.
                _toastTimer = new Timer(ToastTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
                _autoDeleteTimer = new Timer(_autoDeleteCallback, this, 1200000, 0);
            }

            /// <summary>
            /// Dispose of this class' timer.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing && _toastTimer != null && _autoDeleteTimer != null)
                {
                    _toastTimer.Dispose();
                    _toastTimer = null;
                    _autoDeleteTimer.Dispose();
                    _autoDeleteTimer = null;
                }
            }

            /// <summary>
            /// Starts the timer.
            /// </summary>
            public async Task StartTimer()
            {
                await Channel.SendMessageAsync("Timer started. You will be notified in 90 seconds.");
                _toastTimer.Change(90000, 0);
            }

            /// <summary>
            /// Stops the timer.
            /// </summary>
            public async Task StopTimer()
            {
                await Channel.SendMessageAsync("Timer stopped. Note: pressing start will reset it to 0.");
                _toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            /// <summary>
            /// Resets the timer (stops then starts again).
            /// </summary>
            public async Task ResetTimer()
            {
                await Channel.SendMessageAsync("Timer reset to 0. You will be notified in 90 seconds.");
                _toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _toastTimer.Change(90000, 0);
            }

            /// <summary>
            /// Deletes the timer message.
            /// </summary>
            public async Task Cancel()
            {
                await Message.DeleteAsync();
            }

            private async void ToastTimerCallback(object state)
            {
                // Notify the user after 90 seconds.
                await Channel.SendMessageAsync($"{User.Mention}, nova coming in ~10 seconds!");
                // Disable the timer until it's started again.
                _toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }
    }
}
