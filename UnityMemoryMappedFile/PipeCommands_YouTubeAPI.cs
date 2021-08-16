using System;

namespace UnityMemoryMappedFile
{
    public partial class PipeCommands
    {
        #region Types

        #endregion

        #region Resources
        public class LiveChatMessage
        {
            public class FanFundingEventDetails
            {
                public ulong amountMicros { get; set; }
                public string currency { get; set; }
                public string amountDisplayString { get; set; }
                public string userComment { get; set; }
            }

            public class TextMessageDetails
            {
                public string messageText { get; set; }
            }

            public class MessageDeletedDetails
            {
                public string deletedMessageId { get; set; }
            }

            public class BannedUserDetails
            {
                public string channelId { get; set; }
                public string channelUrl { get; set; }
                public string displayName { get; set; }
                public string profileImageUrl { get; set; }
            }

            public class UserBannedDetails
            {
                public BannedUserDetails bannedUserDetails { get; set; }
                public string banType { get; set; }
                public string banDurationSeconds { get; set; }
            }

            public class SuperChatDetails
            {
                public ulong amountMicros { get; set; }
                public string currency { get; set; }
                public string amountDisplayString { get; set; }
                public string userComment { get; set; }
                public uint tier { get; set; }
            }

            public class SuperStickerMetadata
            {
                public string stickerId { get; set; }
                public string altText { get; set; }
                public string language { get; set; }
            }

            public class SuperStickerDetails
            {
                public SuperStickerMetadata superStickerMetadata { get; set; }
                public ulong amountMicros { get; set; }
                public string currency { get; set; }
                public string amountDisplayString { get; set; }
                public uint tier { get; set; }
            }

            public class Snippet
            {
                public string type { get; set; }
                public string liveChatId { get; set; }
                public string authorChannelId { get; set; }
                public DateTime publishedAt { get; set; }
                public bool hasDisplayContent { get; set; }
                public string displayMessage { get; set; }
                public FanFundingEventDetails fanFundingEventDetails { get; set; }
                public TextMessageDetails textMessageDetails { get; set; }
                public MessageDeletedDetails messageDeletedDetails { get; set; }
                public UserBannedDetails userBannedDetails { get; set; }
                public SuperChatDetails superChatDetails { get; set; }
                public SuperStickerDetails superStickerDetails { get; set; }
            }

            public class AuthorDetails
            {
                public string channelId { get; set; }
                public string channelUrl { get; set; }
                public string displayName { get; set; }
                public string profileImageUrl { get; set; }
                public bool isVerified { get; set; }
                public bool isChatOwner { get; set; }
                public bool isChatSponsor { get; set; }
                public bool isChatModerator { get; set; }
            }

            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Snippet snippet { get; set; }
            public AuthorDetails authorDetails { get; set; }
        }

        //SuperChatEvent
        public class SuperChatEvent
        {
            public class SupporterDetails
            {
                public string channelId { get; set; }
                public string channelUrl { get; set; }
                public string displayName { get; set; }
                public string profileImageUrl { get; set; }
            }

            public class SuperStickerMetadata
            {
                public string stickerId { get; set; }
                public string altText { get; set; }
                public string language { get; set; }
            }

            public class Snippet
            {
                public string channelId { get; set; }
                public SupporterDetails supporterDetails { get; set; }
                public string commentText { get; set; }
                public DateTime createdAt { get; set; }
                public ulong amountMicros { get; set; }
                public string currency { get; set; }
                public string displayString { get; set; }
                public uint messageType { get; set; }
                public bool isSuperStickerEvent { get; set; }
                public SuperStickerMetadata superStickerMetadata { get; set; }
            }
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Snippet snippet { get; set; }
        }
        #endregion
    }
}
