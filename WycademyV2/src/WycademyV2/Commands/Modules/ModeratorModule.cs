using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;

namespace WycademyV2.Commands.Modules
{
    [Summary("Moderator Commands")]
    public class ModeratorModule : ModuleBase
    {
        [Command("clean")]
        [Alias("clear")]
        [Summary("Downloads the last 100 messages in the channel and deletes any from the bot.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermissionOrOwner(GuildPermission.ManageMessages)]
        [RequireUnlocked]
        public async Task CleanBotMessages()
        {
            // Get the last 100 messages from the channel.
            var messages = await Context.Channel.GetMessagesAsync().Flatten();

            // Select the messages that are by the bot.
            var messagesToDelete = messages.Where(m => m.Author.Id == Context.Client.CurrentUser.Id);
            int deletedMessageCount = messagesToDelete.Count();

            // Delete each one.
            foreach (IMessage message in messagesToDelete)
            {
                await message.DeleteAsync();
                // Delay for 200ms after each deletion to avoid hitting ratelimits.
                await Task.Delay(200);
            }

            // Respond with how many messages were cleaned.
            IUserMessage response = await ReplyAsync($"{deletedMessageCount} messages deleted.");
            await Task.Delay(5000);
            // Wait 5 seconds and then delete the response.
            await response.DeleteAsync();
        }
    }
}
