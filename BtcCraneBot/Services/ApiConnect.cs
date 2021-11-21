using System.ComponentModel;
using Telegram.Bot;

namespace BtcCraneBot
{
    class ApiConnect
    {
        public static TelegramBotClient botConnector;

        public async void MessageReader(object sender, DoWorkEventArgs e)
        {
            var _worker = sender as BackgroundWorker;
            var _apiKey = e.Argument as string;

            try
            {
                var _botInterface = new TelegramBotClient(_apiKey);
                await _botInterface.SetWebhookAsync("");
                botConnector = _botInterface;                
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            BotInterface _userInterface = new BotInterface();
            try
            {
                _userInterface.MessageReceiver();
                Form1.botRun = true;
            }
            catch(System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            

        }
    }
}
