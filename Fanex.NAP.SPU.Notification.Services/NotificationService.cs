using RestSharp;
using System;
using System.Net;
using System.Text;

namespace Fanex.NAP.SPU.Notification.Services
{
    public class NotificationService
    {
        private MessageModel _messageModel;
        private string _token;
        private string _groupId;        

        private static DateTime LastTime { get; set; }
        private static string LastMessage { get; set; }
        private static int Counter { get; set; }


        public NotificationService Message(MessageModel messageModel)
        {            
            if (messageModel != null)
            {
                _messageModel = messageModel;
            }
            else
            {
                throw new ArgumentException("Parameter cannot be null", nameof(messageModel));
            }

            return this;
        }

        public NotificationService Config(string token = null, string groupId = null)
        {
            _token = token ?? AppSetting.TelegramToken;
            _groupId = groupId ?? AppSetting.TelegramId;

            if (string.IsNullOrWhiteSpace(_token))
            {
                throw new ArgumentException("Token cannot be null", nameof(token));
            }

            if (string.IsNullOrWhiteSpace(_groupId))
            {
                throw new ArgumentException("Group Id cannot be null", nameof(groupId));
            }

            return this;
        }

        public void SendWithDelay(int delayInSecond = 0)
        {            
            if(delayInSecond != 0)
            {
                var second = (DateTime.Now - LastTime).TotalSeconds;

                if(second >= delayInSecond)
                {
                    if (_messageModel.Message != LastMessage)
                    {
                        Counter = 0;
                    }

                    if (Counter == 0 || (_messageModel.Message == LastMessage && Counter < 3))
                    {
                        var result = SendMessage();

                        if (result)
                        {
                            LastTime = DateTime.Now;
                            LastMessage = _messageModel.Message;
                            Counter++;                          
                            
                        }
                    }                   
                }
            }                      
        }

        public void SendWithoutDelay()
        {
            SendMessage();            
        }

        private bool SendMessage()
        {
            var message = new StringBuilder();
                        
            message.Append("\U0001F525 ERROR \U0001F525 in `[`**" + _messageModel.SystemName + "**`]`");
            message.Append("\n\r");
            message.Append("**Environment**: ");            
            message.Append(_messageModel.Environment);
            message.Append("\n\r");            
            message.Append("**Timestamp(GMT%2B7)**: ");
            message.Append(_messageModel.Timestamp);
            message.Append("\n\r");
            message.Append("**Message**: ");
            message.Append(_messageModel.Message);

            if(!string.IsNullOrWhiteSpace(_messageModel.MoreInfo))
            {
                message.Append("\n\r");
                message.Append("**More Info**: ");
                message.Append(_messageModel.MoreInfo);
            }            

            var client = new RestClient($"https://bot.nexdev.net/skynex/api/messages/forwardwithtoken?token={_token}&message={message.ToString()}&conversationId={_groupId}");            
            
            var request = new RestRequest(Method.POST);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.Execute(request).StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
