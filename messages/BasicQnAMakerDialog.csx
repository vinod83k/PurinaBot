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
        context.Wait(MessageReceived);
    }

    [QnAMakerResponseHandler(50)]
    public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
    {
        await context.PostAsync($"I found an answer that might help...{result.Answer}.");
        context.Wait(MessageReceived);
    }

}