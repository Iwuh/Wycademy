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
        private const string CLOSE = "❌";

        private List<string> _pages;
        private int _pageIndex;
        private IUserMessage _message;

        public WeaponInfoMessage(IUser user, List<string> pages) : base(user)
        {
            _pages = pages;
        }

        public async override Task HandleReaction(SocketReaction reaction)
        {
            switch (reaction.Emoji.Name)
            {
                case BEGIN:
                    // Don't do anything if it's already on the first page.
                    if (_pageIndex == 0) break;
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
                    await _message.ModifyAsync(m => m.Content = MakeCodeBlock(_pages[_pages.Count - 1 ]) );
                    break;

                case CLOSE:
                    await _message.DeleteAsync();
                    break;
            }
        }

        public async override Task<IUserMessage> CreateMessageAsync(IMessageChannel channel)
        {
            var message = await channel.SendMessageAsync(MakeCodeBlock(_pages[0]));
            _pageIndex = 0;

            await message.AddReactionAsync(BEGIN);
            await message.AddReactionAsync(PREV_PAGE);
            await message.AddReactionAsync(NEXT_PAGE);
            await message.AddReactionAsync(END);
            await message.AddReactionAsync(CLOSE);

            _message = message;
            return message;
        }

        private string MakeCodeBlock(string input) => $"```{input}```";
    }
}
