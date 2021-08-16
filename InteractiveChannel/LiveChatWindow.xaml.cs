using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UnityMemoryMappedFile;

namespace InteractiveChannel
{
    /// <summary>
    /// LiveChatWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LiveChatWindow : Window
    {
        private Binding HistorySortTarget;
        private Binding SuperChatSortTarget;
        private uint timelineCountLimit = 10;
        private double autoScrollRange = 5;

        public LiveChatWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWin = (MainWindow)Owner;
            MainWin.onStartStream = InitListView;
            InitListView();
            Globals.YouTubeApi.onAddChat = OnLiveChatMessage;
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
             
        }

        void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Window NewSize is " + e.NewSize.Height + " to " + e.NewSize.Width);
            double relativeHeight = e.NewSize.Height - 100;
            if (relativeHeight > ChatTypeTab.MinHeight)
            {
                ChatTypeTab.MaxHeight = relativeHeight;
            }
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SendButton_Click");
            var liveChatMessage = new LiveChatMessage
            {
                Snippet = new LiveChatMessageSnippet
                {
                    TextMessageDetails = new LiveChatTextMessageDetails
                    {
                        MessageText = CommentInputText.Text
                    }
                }
            };
            Globals.YouTubeApi.InsertLiveChat(liveChatMessage);
            await Globals.Client.SendCommandAsync(new PipeCommands.SendMessage { Message = CommentInputText.Text });
        }

        private object GetListViewItem(object row, IList<string> path)
        {
            var value = row.GetType().GetProperty(path[0]).GetValue(row);
            if (path.Count() <= 1)
            {
                return value;
            }
            else
            {
                path.RemoveAt(0);
                return GetListViewItem(value, path);
            }
        }

        private void listHeader_Click(object sender, RoutedEventArgs e)
        {
            var header = (GridViewColumnHeader)e.OriginalSource;

            if (header.Column == null)
            {
                return;
            }

            var sortTarget = (Binding)header.Column.DisplayMemberBinding;
            sortListView((ListView)sender, sortTarget);

            HistorySortTarget = sortTarget;
            SuperChatSortTarget = sortTarget;
        }

        private void sortListView(ListView sender, Binding sortTarget)
        {
            if (sortTarget != null)
            {
                var path = sortTarget.Path.Path;
                var listview = sender;
                if (listview.ItemsSource != null)
                {
                    //要素でソート
                    var pre_sort_items = listview.ItemsSource.Cast<LiveChatMessage>();
                    var sorted_items = pre_sort_items.OrderBy(row =>
                    {
                        var pathchain = new List<string>(path.Split("."));
                        return GetListViewItem(row, pathchain);
                    });

                    if (sorted_items.SequenceEqual(pre_sort_items))
                    {
                        sorted_items.Reverse();
                    }

                    listview.ItemsSource = sorted_items;
                }
            }
        }


        #region FOR_DEBUG
        private async void OnLiveChatMessage(LiveChatMessage liveChatMessage)
        {
            var timeline = (ObservableCollection<LiveChatMessage>)TimeLine.ItemsSource;
            var history = (ObservableCollection<LiveChatMessage>)History.ItemsSource;
            var newChat = liveChatMessage;
            timeline.Add(newChat);
            while (timeline.Count > timelineCountLimit)
            {
                timeline.RemoveAt(0);
            }
            if (newChat != null)
            {
                if (newChat.Snippet.Type == YouTubeApi.LiveChatMessageType.SuperChatEvent)
                {
                    var superchat = (ObservableCollection<LiveChatMessage>)SuperChat.ItemsSource;
                    superchat.Add(newChat);
                }
            }
            history.Add(newChat);

            AutoScrollOnButtom(TimeLine, FindChild<ScrollViewer>(TimeLineGrid), newChat);
            AutoScrollOnButtom(History, FindChild<ScrollViewer>(HistoryGrid), newChat);
            AutoScrollOnButtom(SuperChat, FindChild<ScrollViewer>(SuperChatGrid), newChat);

            if (newChat != null)
            {
                //Unityにイベント送信
                System.Diagnostics.Debug.WriteLine("OnLiveChatMessage To Unity");
                await Globals.Client.SendCommandAsync(new PipeCommands.LiveChatMessage {
                    snippet = new PipeCommands.LiveChatMessage.Snippet {
                        authorChannelId = newChat.Snippet.AuthorChannelId,
                        publishedAt = newChat.Snippet.PublishedAt.Value,
                        displayMessage = newChat.Snippet.DisplayMessage },
                    authorDetails = new PipeCommands.LiveChatMessage.AuthorDetails
                    {
                        displayName = newChat.AuthorDetails.DisplayName,
                        channelId = newChat.AuthorDetails.ChannelId,
                        channelUrl = newChat.AuthorDetails.ChannelUrl,
                        profileImageUrl = newChat.AuthorDetails.ProfileImageUrl
                    }
                });
            }
        }
        private void AutoScrollOnButtom(ListView chatView, ScrollViewer scrollViewer, LiveChatMessage newChat)
        {
            if (scrollViewer != null)
            {
                if (scrollViewer.VerticalOffset > scrollViewer.ScrollableHeight - autoScrollRange)
                {
                    chatView.ScrollIntoView(newChat);
                }
            }
        }

        private void InitListView()
        {
            var timeline = new ObservableCollection<LiveChatMessage>();
            var history = new ObservableCollection<LiveChatMessage>();
            var superchats = new ObservableCollection<LiveChatMessage>();
            TimeLine.ItemsSource = timeline;
            History.ItemsSource = history;
            SuperChat.ItemsSource = superchats;
        }

        /// <summary>
        /// 指定した型の最初に見つかったビジュアル要素を返す
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="root">探索対象のビジュアル要素</param>
        /// <returns>見つかった場合はその要素</returns>
        private static T FindChild<T>(DependencyObject root) where T : FrameworkElement
        {
            var childNumber = VisualTreeHelper.GetChildrenCount(root);
            for (var i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                if (child != null && child is T)
                {
                    return child as T;
                }
                else
                {
                    return FindChild<T>(child);
                }
            }
            return null;
        }
        #endregion
    }
}
