using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace WycademyV2.Commands.Entities
{
    public class WeaponInfoMessage : ReactionMenuMessage
    {
        private const string NEXT_PAGE = "➡";
        private const string PREV_PAGE = "⬅";
        private const string BEGIN = "⏮";
        private const string END = "⏭";

        private List<string> _pages;
        private int _pageIndex;
        private IUserMessage _message;
        private int _startIndex;

        public WeaponInfoMessage(IUser user, List<string> pages, int start) : base(user)
        {
            _pages = pages;
            _startIndex = start;
        }

        public async override Task HandleReaction(SocketReaction reaction)
        {
            switch (reaction.Emoji.Name)
            {
                case BEGIN:
                    // Don't do anything if it's already on the first page.
                    if (_pageIndex == 0) break;
                    _pageIndex = 0;
                    await _message.ModifyAsync(m => m.Content = MakeCodeBlock(_pages[0]) );
                    break;

                case PREV_PAGE:
                    // Don't do anything if it's on the first page.
                    if (_pageIndex == 0) break;
                    await _message.ModifyAsync(m => m.Content = MakeCodeBlock(_pages[--_pageIndex]) );
                    break;

                case NEXT_PAGE:
                    // Don't do anything if it's on the last page.
                    if (_pageIndex == _pages.Count - 1) break;
                    await _message.ModifyAsync(m => m.Content = MakeCodeBlock(_pages[++_pageIndex]) );
                    break;

                case END:
                    // Don't do anything if it's already on the last page.
                    if (_pageIndex == _pages.Count - 1) break;
                    _pageIndex = _pages.Count - 1;
                    await _message.ModifyAsync(m => m.Content = MakeCodeBlock(_pages[_pages.Count - 1 ]) );
                    break;
            }
        }

        public async override Task<IUserMessage> CreateMessageAsync(IMessageChannel channel)
        {
            _pageIndex = _startIndex < _pages.Count ? _startIndex : 0;
            // Send the starting page, at the specified index (if said index is within the range of pages).
            var message = await channel.SendMessageAsync( MakeCodeBlock( _pages[_pageIndex] ) );

            await message.AddReactionAsync(BEGIN);
            await message.AddReactionAsync(PREV_PAGE);
            await message.AddReactionAsync(NEXT_PAGE);
            await message.AddReactionAsync(END);

            _message = message;
            return message;
        }

        public async override Task CloseMenuAsync()
        {
            await _message.DeleteAsync();
        }

        private string MakeCodeBlock(string input) => $"```{input}\nPage {_pageIndex + 1} / {_pages.Count}```";
    }
}
