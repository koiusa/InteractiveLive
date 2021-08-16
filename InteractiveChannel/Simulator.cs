using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace InteractiveChannel
{

    public static class Simulator
    {
        public static bool isEnabled { get; set; }
        public class SuperChatEventEx : SuperChatEvent
        {
            public string color { get; set; }
        }
        public class LiveChatMessagesEx : LiveChatMessage
        {
            public string color { get; set; }
        }

        public class LiveStreamEx : LiveStream
        {

        }

        private static List<string> GenerateTextList()
        {
            var textList = new List<string>();
            textList.Add("こんにちわ");
            textList.Add("○○からきました");
            textList.Add("たすかる");
            textList.Add("ほげほげ");
            textList.Add("ふごふご");
            return textList;
        }
        private static List<KeyValuePair<string, string>> GenerateUserList()
        {
            var userlist = new List<KeyValuePair<string, string>>();
            userlist.Add(new KeyValuePair<string, string>("nakanohito", "中の人"));
            userlist.Add(new KeyValuePair<string, string>("yuumeijin", "超有名人"));
            userlist.Add(new KeyValuePair<string, string>("jouren", "常連"));
            return userlist;
        }

        private static List<KeyValuePair<string, ulong>> GenerateAmountMicros()
        {
            var amountMicrosList = new List<KeyValuePair<string, ulong>>();
            amountMicrosList.Add(new KeyValuePair<string, ulong>("円", 100));
            amountMicrosList.Add(new KeyValuePair<string, ulong>("$", 1));
            amountMicrosList.Add(new KeyValuePair<string, ulong>("€", 1));
            return amountMicrosList;
        }

        private static Dictionary<uint, string> GenarateColorList()
        {
            var colorList = new Dictionary<uint, string>();
            colorList.Add(0, "Lightblue");
            colorList.Add(1, "Lightyellow");
            colorList.Add(2, "Lightorange");
            colorList.Add(3, "Lightgreen");
            colorList.Add(4, "cyan");
            colorList.Add(5, "violet");
            colorList.Add(6, "Pink");
            return colorList;
        }

        private static List<KeyValuePair<uint, ulong>> GenarateTierList()
        {
            var colorList = new List<KeyValuePair<uint, ulong>>();
            colorList.Add(new KeyValuePair<uint, ulong>(0, 250));
            colorList.Add(new KeyValuePair<uint, ulong>(1, 500));
            colorList.Add(new KeyValuePair<uint, ulong>(2, 1000));
            colorList.Add(new KeyValuePair<uint, ulong>(3, 2000));
            colorList.Add(new KeyValuePair<uint, ulong>(4, 5000));
            colorList.Add(new KeyValuePair<uint, ulong>(5, 10000));
            colorList.Add(new KeyValuePair<uint, ulong>(6, ulong.MaxValue));
            return colorList;
        }

        private static uint GetTier(ulong amount, List<KeyValuePair<uint, ulong>> tierList)
        {
            var value = tierList.Find(x => amount < x.Value).Key;
            System.Diagnostics.Debug.WriteLine("GetTier " + amount + " to " + value);
            return value;
        }

        private static string GetChatColor(uint tier, Dictionary<uint, string> colorList)
        {
            return colorList[tier];
        }


        private static ulong GenarateAmountMicros(List<KeyValuePair<string, ulong>> amountMicrosList)
        {
            Random r1 = new Random();
            int amountseed = r1.Next(0, amountMicrosList.Count);
            return amountMicrosList[amountseed].Value * (ulong)r1.Next(0, 20) * 10;
        }

        private static Video GenerateLiveCaption()
        {
            var vide = new Video()
            {
                Snippet = new VideoSnippet()
                {
                    PublishedAt = DateTime.Now,
                    Title = "SimulaterLiveStream",
                    Description = "これはデバッグ用にプログラムにハードコードされたデータです",
                }
            };
            return vide;
        }

        private static LiveChatSuperChatDetails GenarateSuperChatDetails(SuperChatEvent superChatEvent)
        {
            if (superChatEvent == null)
            {
                return null;
            }
            return new LiveChatSuperChatDetails()
            {
                AmountMicros = superChatEvent.Snippet.AmountMicros,
                AmountDisplayString = superChatEvent.Snippet.DisplayString,
                UserComment = superChatEvent.Snippet.CommentText,
                Currency = superChatEvent.Snippet.Currency,
                Tier = superChatEvent.Snippet.MessageType,
            };
        }

        private static LiveChatMessagesEx GenerateChat(int id, string text, KeyValuePair<string, string> userlist, SuperChatEventEx superchat, DateTime timestamp)
        {
            string color = "White";
            return new LiveChatMessagesEx()
            {
                Id = id.ToString(),
                Snippet = new LiveChatMessageSnippet()
                {
                    AuthorChannelId = userlist.Key,
                    DisplayMessage = text,
                    Type = superchat != null ? YouTubeApi.LiveChatMessageType.SuperChatEvent : YouTubeApi.LiveChatMessageType.TextMessageEvent,
                    LiveChatId = "",
                    PublishedAt = timestamp,
                    SuperChatDetails = GenarateSuperChatDetails(superchat),
                },
                AuthorDetails = new LiveChatMessageAuthorDetails()
                {
                    ChannelId = userlist.Key,
                    ChannelUrl = "",
                    DisplayName = userlist.Value,
                },
                color = superchat != null ? superchat.color : color,
            };
        }

        private static SuperChatEventEx GenerateSuperChat(string text, KeyValuePair<string, string> user,
            List<KeyValuePair<string, ulong>> amountMicrosList, Dictionary<uint, string> colorList, DateTime timestamp)
        {
            Random r1 = new Random();
            int currencyseed = r1.Next(0, amountMicrosList.Count);
            var amount = GenarateAmountMicros(amountMicrosList);
            var tier = GetTier(amount, GenarateTierList());
            string color = GetChatColor(tier, colorList);
            return new SuperChatEventEx()
            {
                Snippet = new SuperChatEventSnippet()
                {
                    ChannelId = user.Key,
                    CommentText = text,
                    DisplayString = text,
                    AmountMicros = amount,
                    Currency = amountMicrosList[currencyseed].Key,
                    MessageType = tier,
                    CreatedAt = timestamp,
                    IsSuperStickerEvent = false,
                    SupporterDetails = new ChannelProfileDetails()
                    {
                        DisplayName = user.Value,
                    }
                },
                color = color,
            };
        }

        private static SuperChatEventEx GenerateSuperChat(string text, KeyValuePair<string, string> user, DateTime date)
        {
            var amountMicrosList = GenerateAmountMicros();
            var colorList = GenarateColorList();
            return GenerateSuperChat(text, user, amountMicrosList, colorList, date);
        }

        private static YouTubeApi.LiveBroadcastType CurrentState = YouTubeApi.LiveBroadcastType.standby;
        public static Action<YouTubeApi.LiveBroadcastType> onStateChange;

        private static bool _OnLive = false;
        public static bool isOnLive => _OnLive;
        public static Video GetLiveInfo
        {
            get { return liveInfo; }
        }
        private static void StateChange(YouTubeApi.LiveBroadcastType state)
        {
            CurrentState = state;
            onStateChange?.Invoke(state);
        }

        public static ObservableCollection<LiveChatMessagesEx> history;
        public static Video liveInfo;

        public static Action<LiveChatMessage> OnAddChat;
        public static Action<Video> OnModifyCaption;

        public static void SimulateAddChat()
        {
            if (!isEnabled) return;
            System.Diagnostics.Debug.WriteLine("SimulateAddChat");
            var newChat = CreateChat(history.Count);
            history.Add(newChat);
            OnAddChat?.Invoke(newChat);
        }

        public static void SimulateGetCaption()
        {
            if (!isEnabled) return;
            System.Diagnostics.Debug.WriteLine("SimulateGetCaption");
            if (liveInfo == null)
            {
                var newLive = GenerateLiveCaption();
                liveInfo = newLive;
            }
            OnModifyCaption?.Invoke(liveInfo);
        }

        public static void SimulateSetCaption()
        {
            if (!isEnabled) return;
            System.Diagnostics.Debug.WriteLine("SimulateSetCaption");
            OnModifyCaption?.Invoke(liveInfo);
        }

        public static async void StartLive()
        {
            if (!isEnabled) return;
            history = new ObservableCollection<LiveChatMessagesEx>();
            StateChange(YouTubeApi.LiveBroadcastType.live);
            _OnLive = true;
            while (_OnLive)
            {
                var rnd = new Random();
                await Task.Delay(100 * rnd.Next(1, 10));
                SimulateAddChat();
            }
            StateChange(YouTubeApi.LiveBroadcastType.standby);
        }
        public static void EndLive()
        {
            if (!isEnabled) return;
            StateChange(YouTubeApi.LiveBroadcastType.none);
            _OnLive = false;
        }

        public static LiveChatMessagesEx CreateChat(int id)
        {
            if (!isEnabled) return null;
            var textList = GenerateTextList();
            var userlist = GenerateUserList();
            Random r1 = new Random();
            int textseed = r1.Next(0, textList.Count);
            int userseed = r1.Next(0, userlist.Count);
            int sprate = r1.Next(0, 10);
            var timestamp = DateTime.Now;
            var superchat = sprate > 6 ? GenerateSuperChat(textList[textseed], userlist[userseed], timestamp) : null;
            return GenerateChat(id, textList[textseed], userlist[userseed], superchat, timestamp);
        }
    }
}