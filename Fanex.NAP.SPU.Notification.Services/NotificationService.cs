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
        private string _apiUrl;

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

        /// <summary>
        /// Config telegram api setting.
        /// </summary>
        /// <param name="token">Set token or add appsetting with key telegram-token</param>
        /// <param name="groupId">Set token or add appsetting with key telegram-id</param>
        /// <param name="apiUrl">Set token or add appsetting with key telegram-api-url</param>
        /// <returns></returns>
        public NotificationService Config(string token = null, string groupId = null, string apiUrl = null)
        {
            _token = token ?? AppSetting.TelegramToken;
            _groupId = groupId ?? AppSetting.TelegramId;
            _apiUrl = apiUrl ?? AppSetting.ApiUrl;

            if (string.IsNullOrWhiteSpace(_token))
            {
                throw new ArgumentException("Token cannot be null", nameof(token));
            }

            if (string.IsNullOrWhiteSpace(_groupId))
            {
                throw new ArgumentException("Group Id cannot be null", nameof(groupId));
            }

            if (string.IsNullOrWhiteSpace(_apiUrl))
            {
                throw new ArgumentException("API Url cannot be null", nameof(apiUrl));
            }

            return this;
        }

        /// <summary>
        /// Send message with delay in second
        /// </summary>
        /// <param name="delayInSecond">Delay time in second</param>
        /// <param name="isInfo">The type of message(info or error)</param>
        public void SendWithDelay(int delayInSecond = 0, bool isInfo = false)
        {
            if (delayInSecond != 0)
            {
                var second = (DateTime.Now - LastTime).TotalSeconds;

                if (second >= delayInSecond)
                {
                    if (_messageModel.Message != LastMessage)
                    {
                        Counter = 0;
                    }

                    if (Counter == 0 || (_messageModel.Message == LastMessage && Counter < 3))
                    {
                        var result = isInfo ? SendMessageInfo() : SendMessageError();

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

        /// <summary>
        /// Send message without delay time
        /// </summary>
        /// <param name="isInfo">Type of message(info or error)</param>
        public void SendWithoutDelay(bool isInfo = false)
        {
            if (isInfo)
            {
                SendMessageInfo();
            }
            else
            {
                SendMessageError();
            }
        }

        private bool SendMessageError()
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

            if (!string.IsNullOrWhiteSpace(_messageModel.MoreInfo))
            {
                message.Append("\n\r");
                message.Append("**More Info**: ");
                message.Append(_messageModel.MoreInfo);
            }

            var client = new RestClient($"{_apiUrl}?token={_token}&message={message.ToString()}&conversationId={_groupId}");

            var request = new RestRequest(Method.POST);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.Execute(request).StatusCode == System.Net.HttpStatusCode.OK;
        }

        private bool SendMessageInfo()
        {
            var message = new StringBuilder();

            message.Append("\U0001F525 INFO \U0001F525 in `[`**" + _messageModel.SystemName + "**`]`");
            message.Append("\n\r");
            message.Append("**Environment**: ");
            message.Append(_messageModel.Environment);
            message.Append("\n\r");
            message.Append("**Timestamp(GMT%2B7)**: ");
            message.Append(_messageModel.Timestamp);
            message.Append("\n\r");
            message.Append("**Message**: ");
            message.Append(_messageModel.Message);

            if (!string.IsNullOrWhiteSpace(_messageModel.MoreInfo))
            {
                message.Append("\n\r");
                message.Append("**More Info**: ");
                message.Append(_messageModel.MoreInfo);
            }
            //https://bot.nexdev.net/skynex/api/messages/forwardwithtoken
            var client = new RestClient($"{_apiUrl}?token={_token}&message={message.ToString()}&conversationId={_groupId}");

            var request = new RestRequest(Method.POST);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.Execute(request).StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
