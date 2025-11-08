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

            if (Consts.HISTORY)
            {
                int excludeLatest = mainWindow.ChatHistory.Count - 1;

                int start = Math.Max(0, excludeLatest - 4);

                for (int i = start; i < excludeLatest; i++)
                {
                    history.Add(mainWindow.ChatHistory[i]);
                }
            }

            FlaskAPIService answer = new();
            string query = input;
            
            string response = await answer.RetrieveAnswerAsync(query, history);
            
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