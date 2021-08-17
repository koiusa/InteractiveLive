using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InteractiveChannel
{
    public class YouTubeApi
    {
        public enum AuthType
        {
            OAuth = 0,
            ApiKey = 1,
        }
        public enum LiveBroadcastType
        {
            standby = -1,
            live = 0,//（ライブ配信中）
            upcoming,//　（ライブ配信予約）
            none, //　（ライブ配信終了）
        }
        public struct LiveChatMessageType
        {
            public const string ChatEndedEvent = "chatEndedEvent";
            public const string MessageDeletedEvent = "messageDeletedEvent";
            public const string NewSponsorEvent = "newSponsorEvent";
            public const string SponsorOnlyModeEndedEvent = "sponsorOnlyModeEndedEvent";
            public const string SponsorOnlyModeStartedEvent = "sponsorOnlyModeStartedEvent";
            public const string SuperChatEvent = "superChatEvent";
            public const string SuperStickerEvent = "superStickerEvent";
            public const string TextMessageEvent = "textMessageEvent";
            public const string Tombstone = "tombstone";
            public const string UserBannedEvent = "userBannedEvent";
        }

        public class Setting
        {
            public AuthType AuthType { get; set; }

            public string ApiKey { get; set; }

            public string VideoId { get; set; }

            public string ClientSecret { get; set; }

            public string ApplicationName { get; set; }
        }

        public Action<LiveBroadcastType> onStateChange;

        public Action<Video> onSetLiveStream;

        public Action<Video> onGetLiveStream;

        public Action<LiveChatMessage> onAddChat;

        public Action<LiveChatSuperChatDetails> onSuperChat;

        public Action<LiveChatSuperStickerDetails> onSuperSticker;

        public Setting CurrentSetting { get; set; }

        //Api call interval by MilliSeconds
        public int CommentIntervalMillis { get; set; } = 6000;

        public LiveBroadcastType CurrentState = LiveBroadcastType.standby;

        private bool isRunuing { get; set; }

        private YouTubeService youtubeService { get; set; }

        private void StateChange(LiveBroadcastType state)
        {
            CurrentState = state;
            onStateChange?.Invoke(state);
        }

        public YouTubeApi()
        {
            CurrentSetting = new Setting();
        }

        public async void StartLive(string videoId)
        {
            CurrentSetting.VideoId = videoId;
            if (CurrentSetting.AuthType.Equals(CurrentSetting.AuthType))
            {
                youtubeService = await GetYoutubeServiceByOAuth();
            }
            else
            {
                youtubeService = GetYoutubeServiceByApiKey();
            }

            if (youtubeService == null)
            {
                System.Diagnostics.Debug.WriteLine("Could not get YouTubeService!");
            }
            else
            {
                isRunuing = true;
                string liveChatId = GetLiveChatID(CurrentSetting.VideoId);
                if (liveChatId != null)
                {
                    await GetLiveChatMessage(liveChatId, null);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Could not get LiveChatID!");
                }
            }
        }

        public void EndLive()
        {
            StateChange(LiveBroadcastType.none);
            isRunuing = false;
        }

        //OAuth2認証の場合
        private Task<UserCredential> GetUserCredential(string[] scopes)
        {
            using (var stream = new FileStream(CurrentSetting.ClientSecret, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token";
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user", CancellationToken.None, new FileDataStore(credPath, true));
            }
        }

        public async Task<YouTubeService> GetYoutubeServiceByOAuth()
        {
            if (string.IsNullOrEmpty(CurrentSetting.ClientSecret))
            {
                System.Diagnostics.Debug.WriteLine("ClientSecret is Noting!");
                return null;
            }

            var scopes = new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl };
            ICredential credential = await GetUserCredential(scopes);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = CurrentSetting.ApplicationName,
            });

            return youtubeService;
        }

        public YouTubeService GetYoutubeServiceByApiKey()
        {
            if (string.IsNullOrEmpty(CurrentSetting.ApiKey))
            {
                System.Diagnostics.Debug.WriteLine("ApiKey is Noting!");
                return null;
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = CurrentSetting.ApiKey
            });

            return youtubeService;
        }

        public string GetLiveChatID(string videoId)
        {
            if (!string.IsNullOrEmpty(videoId)) { 
                //引数で取得したい情報を指定
                var videosList = youtubeService.Videos.List("snippet,LiveStreamingDetails");
                videosList.Id = videoId;
                //動画情報の取得
                var videoListResponse = videosList.Execute();
                foreach (var responseVideo in videoListResponse.Items)
                {
                    onGetLiveStream?.Invoke(responseVideo);
                    return responseVideo.LiveStreamingDetails.ActiveLiveChatId;
                }
            }
            //動画情報取得できない場合はnullを返す
            System.Diagnostics.Debug.WriteLine("LiveChatID is Null");
            return null;
        }

        private async Task GetLiveChatMessage(string liveChatId, string nextPageToken)
        {
            if (!isRunuing)
            {
                StateChange(LiveBroadcastType.standby);
                System.Diagnostics.Debug.WriteLine("GetLiveChatMessage is Return");
                return;
            }

            if (string.IsNullOrEmpty(nextPageToken))
            {
                StateChange(LiveBroadcastType.live);
            }

            var liveChatRequest = youtubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
            System.Diagnostics.Debug.WriteLine("NextToken:" + nextPageToken + " AccessToken:" + liveChatRequest.AccessToken);
            liveChatRequest.PageToken = nextPageToken;
            var liveChatResponse = await liveChatRequest.ExecuteAsync();
            foreach (var liveChat in liveChatResponse.Items)
            {
                try
                {
                    Console.WriteLine($"{liveChat.Snippet.DisplayMessage},{liveChat.AuthorDetails.DisplayName}");

                    if (liveChat.Snippet.Type.Contains("superChatEvent"))
                    {
                        onSuperChat?.Invoke(liveChat.Snippet.SuperChatDetails);
                    }
                    if (liveChat.Snippet.Type.Contains("superStickerEvent"))
                    {
                        onSuperSticker?.Invoke(liveChat.Snippet.SuperStickerDetails);
                    }
                }
                catch { }

                onAddChat?.Invoke(liveChat);
            }

            await Task.Delay(CommentIntervalMillis > (int)liveChatResponse.PollingIntervalMillis ? CommentIntervalMillis : (int)liveChatResponse.PollingIntervalMillis);

            await GetLiveChatMessage(liveChatId, liveChatResponse.NextPageToken);
        }

        public void SetLiveStream(Video aVideo)
        {
            if (youtubeService != null)
            {
                var videosList = youtubeService.Videos.List("snippet");
                videosList.Id = CurrentSetting.VideoId;
                var videoListResponse = videosList.Execute();
                var newVideo = videoListResponse.Items.First();
                newVideo.Snippet.Title = aVideo.Snippet.Title;
                newVideo.Snippet.Description = aVideo.Snippet.Description;
                newVideo.Snippet.Tags = new List<string>();
                var video = youtubeService.Videos.Update(newVideo, "snippet");
                var videoResponse = video?.Execute();
                onSetLiveStream?.Invoke(videoResponse);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Could not SetLiveStream");
            }
        }

        public void InsertLiveChat(LiveChatMessage aLiveChat)
        {
            if (youtubeService != null)
            {
                if (!string.IsNullOrEmpty(aLiveChat.Snippet.TextMessageDetails.MessageText))
                {
                    aLiveChat.Snippet.LiveChatId = GetLiveChatID(CurrentSetting.VideoId);
                    aLiveChat.Snippet.Type = LiveChatMessageType.TextMessageEvent;
                    var livechat = youtubeService.LiveChatMessages.Insert(aLiveChat, "snippet");
                    var livechatResponse = livechat?.Execute();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TextMessageDetails.MessageText is Null!");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Could not InsertLiveChat");
            }
        }
    }
}