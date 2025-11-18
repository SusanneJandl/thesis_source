using System.Collections.ObjectModel;
using System.Windows;
using System.Globalization;

namespace ChatbotWPF
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ChatHistory { get; set; }
        TranslationManager translation = new(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            var screenHeight = SystemParameters.WorkArea.Height; // Excludes taskbar
            var screenWidth = SystemParameters.WorkArea.Width;

            // Set the position for bottom-left corner
            this.Left = 0; // Left edge of the screen
            this.Top = screenHeight - this.Height; // Bottom edge, accounting for window height
            ChatHistory = new ObservableCollection<string>();
            DataContext = this;
            translation.SetCulture(new CultureInfo("de"));
            UpdateStrings();
            Log.History();
            Log.Language();
        }
        private void UserInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            placeholder_textblock.Visibility = string.IsNullOrWhiteSpace(UserInput.Text) ? Visibility.Visible : Visibility.Hidden;
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Log.startTotal = DateTime.Now;
            RamTracker.Start();
            _ = SendMessage();
        }


        private async Task SendMessage()
        {
            Conversation conversation = new();
            await conversation.WithWholeAnswerAsync();
            Log.doneTotal = DateTime.Now;
            Log.TimeLog(Log.startTotal, Log.doneTotal, "TOTAL TIME");
            RamTracker.StopAndLog("C# RAM USAGE");
        }

        private void OnSwitch(object sender, RoutedEventArgs e)
        {
            if (rb_english.IsChecked == true)
            {
                Consts.LANGUAGE = "EN";
                translation.SetCulture(new CultureInfo("en"));
            }
            else if (rb_german.IsChecked == true)
            { 
                Consts.LANGUAGE = "DE";
                translation.SetCulture(new CultureInfo("de"));
            }
            UpdateStrings();
        }

        private void UpdateStrings()
        {
            txt_language.Text = translation.GetResource("txt_language");
            header_chatbot.Text = translation.GetResource("header_chatbot");
            rb_english.Content = translation.GetResource("rb_english");
            rb_german.Content = translation.GetResource("rb_german");
            if (btn_send != null) btn_send.Content = translation.GetResource("btn_send") ?? "Default Send";
            if (placeholder_textblock != null) placeholder_textblock.Text = translation.GetResource("placeholder_textblock") ?? "Default Placeholder";
            
        }
    }
}
 