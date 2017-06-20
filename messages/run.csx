#r "Newtonsoft.Json"
#load "BasicQnAMakerDialog.csx"

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    // Initialize the azure bot
    using (BotService.Initialize())
    {
        // Deserialize the incoming activity
        string jsonContent = await req.Content.ReadAsStringAsync();
        var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);

        // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
        // if request is authenticated
        if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { activity }, CancellationToken.None))
        {
            return BotAuthenticator.GenerateUnauthorizedResponse(req);
        }

        if (activity != null)
        {
            // one of these will have an interface and process it
            switch (activity.GetActivityType())
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, () => new BasicQnAMakerDialog());
                    break;
                case ActivityTypes.ConversationUpdate:
                    var client = new ConnectorClient(new Uri(activity.ServiceUrl));
                    IConversationUpdateActivity update = activity;
                    //if (update.MembersAdded.Any())
                    //{
                    //    var reply = activity.CreateReply();
                    //    var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                    //    foreach (var newMember in newMembers)
                    //    {
                    //        reply.Text = "Welcome User";
                    //        if (!string.IsNullOrEmpty(newMember.Name))
                    //        {
                    //            reply.Text += $" {newMember.Name}";
                    //        }
                    //        reply.Text += "!";
                    //        await client.Conversations.ReplyToActivityAsync(reply);
                    //    }

                        
                    //}

                    var reply1 = activity.CreateReply("I have colors in mind, but need your help to choose the best one.");
                    reply1.Type = ActivityTypes.Message;
                    reply1.TextFormat = TextFormatTypes.Plain;

                    reply1.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                            {
                                new CardAction(){ Title = "Blue", Type=ActionTypes.ImBack, Value="Blue" },
                                new CardAction(){ Title = "Red", Type=ActionTypes.ImBack, Value="Red" },
                                new CardAction(){ Title = "Green", Type=ActionTypes.ImBack, Value="Green" }
                            }
                    };

                    await client.Conversations.ReplyToActivityAsync(reply1);
                    break;
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.Typing:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.Ping:
                default:
                    log.Error($"Unknown activity type ignored: {activity.GetActivityType()}");
                    break;
            }
        }
        return req.CreateResponse(HttpStatusCode.Accepted);
    }
}