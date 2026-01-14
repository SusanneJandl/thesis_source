using System.Windows;

namespace ChatbotWPF
{
    internal class Conversation
    {
        public async Task WithWholeAnswerAsync()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            string input = mainWindow.UserInput.Text;
            if (string.IsNullOrWhiteSpace(input))
                return;

            mainWindow.ChatHistory.Add($"\nYou: {input}\n");
            var history = new List<string>();

            
            FlaskAPIService answer = new();
            string query = input;
            
            string response = await answer.RetrieveAnswerQA(query);
            
            mainWindow.ChatHistory.Add("Bot: ");
            int botIndex = mainWindow.ChatHistory.Count - 1;

            mainWindow.UserInput.Clear();

            
            mainWindow.ChatHistory[botIndex] += response;
            

            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.ChatHistoryListBox.Items.Refresh();
                mainWindow.ChatHistoryListBox.UpdateLayout();
                mainWindow.ChatHistoryListBox.ScrollIntoView(mainWindow.ChatHistory[botIndex]);
            });
        }
    }
}