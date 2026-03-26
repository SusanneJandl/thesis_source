using System.Collections.ObjectModel;
using System.Windows;
using System.Globalization;
using System.IO;

namespace ChatbotWPF
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ChatHistory { get; set; }
        readonly TranslationManager translation = new(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            var screenHeight = SystemParameters.WorkArea.Height;
            _ = SystemParameters.WorkArea.Width;


            Left = 0;
            Top = screenHeight - Height;
            ChatHistory = [];
            DataContext = this;
            rb_english.Checked += OnSwitch;
            rb_german.Checked += OnSwitch;

            rb_german.IsChecked = true;
            translation.SetCulture(new CultureInfo("de"));
            UpdateStrings();
            this.Loaded += async (_, _) => await RunFromFileIfExists();
        
        }
        private async Task RunFromFileIfExists()
        {
            if (!File.Exists("run.txt"))
                return;

            var lines = File.ReadAllLines("run.txt");

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Format: en;5;question
                var parts = line.Split(';');

                if (parts.Length < 3)
                    continue;

                // Set values WITHOUT UI interaction
                Consts.LANGUAGE = parts[0].ToUpper();
                Consts.ACCURACY = int.TryParse(parts[1], out int acc) ? acc : 3;

                // Optional: update UI visually (not required)
                rb_english.IsChecked = Consts.LANGUAGE == "EN";
                rb_german.IsChecked = Consts.LANGUAGE == "DE";
                Accuracy.Value = Consts.ACCURACY;

                // Run your existing pipeline
                await TestSendMessage(parts[2]);

                // Optional small delay (prevents API overload)
                await Task.Delay(200);
            }
        }
        private void UserInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            placeholder_textblock.Visibility = string.IsNullOrWhiteSpace(UserInput.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Log.Language();
            Log.Accuracy();
            Log.startTotal = DateTime.Now;
            RamTracker.Start();
            _ = SendMessage();
        }


        private async Task SendMessage()
        {
            await Conversation.WithWholeAnswerAsync();
            Log.doneTotal = DateTime.Now;
            Log.TimeLog(Log.startTotal, Log.doneTotal, "TOTAL");
            RamTracker.StopAndLog("RAM USAGE");
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
            if (txt_Accuracy != null) txt_Accuracy.Text = translation.GetResource("txt_Accuracy") ?? "Default Accuracy";
            header_chatbot.Text = translation.GetResource("header_chatbot");
            rb_english.Content = translation.GetResource("rb_english");
            rb_german.Content = translation.GetResource("rb_german");
            if (btn_send != null) btn_send.Content = translation.GetResource("btn_send") ?? "Default Send";
            if (placeholder_textblock != null) placeholder_textblock.Text = translation.GetResource("placeholder_textblock") ?? "Default Placeholder";

        }

        private void Accuracy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Consts.ACCURACY = (int)Accuracy.Value;
        }
        public async Task TestSendMessage(string message)
        {
            UserInput.Text = message;

            Log.Language();
            Log.Accuracy();
            Log.startTotal = DateTime.Now;

            RamTracker.Start();

            await SendMessage();
        }
    }
}