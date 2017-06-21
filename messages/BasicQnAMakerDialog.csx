using System;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

// For more information about this template visit http://aka.ms/azurebots-csharp-qnamaker
[Serializable]
public class BasicQnAMakerDialog : QnAMakerDialog
{
    // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
    public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute(Utils.GetAppSetting("QnASubscriptionKey"), Utils.GetAppSetting("QnAKnowledgebaseId"))))
    {
    }

    public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
    {
        await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
        context.Wait(MessageReceivedAsync);
    }

    private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            context.Done(message.Text);

            // if ((message.Text != null) && (message.Text.Trim().Length > 0))
            // {
            //     context.Done(message.Text);
            // }
            // else
            // {
            //     --attempts;
            //     if (attempts > 0)
            //     {
            //         await context.PostAsync("I'm sorry, I don't understand your reply. What is your name (e.g. 'Bill', 'Melinda')?");

            //         context.Wait(this.MessageReceivedAsync);
            //     }
            //     else
            //     {
            //         context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
            //     }
            // }
        }
}