using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfoMessage : ReactionMenuMessage
    {
        private const string NEXT_PAGE = "➡";
        private const string PREV_PAGE = "⬅";
        private const string BEGIN = "⏮";
        private const string END = "⏭";

        private List<Embed> _pages;
        private int _pageIndex;
        private IUserMessage _message;

        public WeaponInfoMessage(IUser user, List<Embed> pages, int startIndex) : base(user)
        {
            _pages = pages;
            _pageIndex = startIndex < _pages.Count ? startIndex : 0;
        }

        public async override Task<IUserMessage> CreateMessageAsync(IMessageChannel channel)
        {
            // Send the message starting with the first page.
            _message = await channel.SendMessageAsync($"Weapon Info {GetPageNumberString()}", embed: _pages[_pageIndex]);

            // Add all the reaction options.
            await _message.AddReactionAsync(new Emoji(BEGIN));
            await _message.AddReactionAsync(new Emoji(PREV_PAGE));
            await _message.AddReactionAsync(new Emoji(NEXT_PAGE));
            await _message.AddReactionAsync(new Emoji(END));

            // Return the message.
            return _message;
        }

        public async override Task HandleReaction(SocketReaction reaction)
        {
            switch (reaction.Emote.Name)
            {
                case BEGIN:
                    // Don't do anything if it's already on the first page.
                    if (_pageIndex == 0) break;
                    _pageIndex = 0;
                    await UpdateMessage();
                    break;
                case PREV_PAGE:
                    // Don't do anything if it's already on the first page.
                    if (_pageIndex == 0) break;
                    _pageIndex--;
                    await UpdateMessage();
                    break;
                case NEXT_PAGE:
                    // Don't do anything if it's already on the last page.
                    if (_pageIndex == _pages.Count - 1) break;
                    _pageIndex++;
                    await UpdateMessage();
                    break;
                case END:
                    // Don't do anything if it's already on the last page.
                    if (_pageIndex == _pages.Count - 1) break;
                    _pageIndex = _pages.Count - 1;
                    await UpdateMessage();
                    break;
                default:
                    break;
            }
        }

        public async override Task CloseMenuAsync()
        {
            await _message.DeleteAsync();
        }

        private string GetPageNumberString() => $"Page {_pageIndex + 1} / {_pages.Count}";

        private async Task UpdateMessage()
        {
            await _message.ModifyAsync(m =>
            {
                m.Content = $"Weapon Info {GetPageNumberString()}";
                m.Embed = _pages[_pageIndex];
            });
        }
    }
}
