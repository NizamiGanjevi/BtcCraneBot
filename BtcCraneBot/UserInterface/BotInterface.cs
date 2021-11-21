using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace BtcCraneBot
{
    class BotInterface : ApiConnect
    {
        BotWork botWork = new BotWork();
        ReplyToUser replyToUser = new ReplyToUser();

        string WalletInfoText(Telegram.Bot.Types.Message command)
         {
            object[] _dataArray = botWork.GetWalletInfo(command);            
            double balance = (double)_dataArray[0];
            var userRefValue = (long)_dataArray[1];
            var userRefsValue = (long)_dataArray[2];
            string _walletInfo = string.Format("💼 <b>Кошелек</b>" + "\r\n" +
                 "Баланс <b>{0}</b> ВТС" + "\r\n" +
                 "Вы пригласили <b>{1}</b> человек" + "\r\n" +
                 "Ваши рефералы пригласили <b>{2}</b> человек", balance.ToString("0.00000000"), userRefValue.ToString(), userRefsValue.ToString());
            return _walletInfo;
         }
        public async void MessageReceiver()
        {            
            try
            {
                int _offset = 0;
                {
                    while (true)
                    {
                        Telegram.Bot.Types.Update[] _clientMessages = await botConnector.GetUpdatesAsync(_offset);                        

                        foreach (var clientCommand in _clientMessages)
                        {
                            var _commandType = clientCommand.Type;
                            if (_commandType == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                            {
                                var _command = clientCommand.CallbackQuery;
                                if(_command.Data == "Test1")
                                {
                                    replyToUser.TestReply(_command);
                                }
                            }
                            if(_commandType == Telegram.Bot.Types.Enums.UpdateType.Message)
                            {
                                var _command = clientCommand.Message;
                                if (_command.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                                {
                                    ReplyToUser(_command);
                                }
                            }
                            
                            _offset = ++clientCommand.Id;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
               MessageReceiver();
            }
                        
        }
        public async void ReplyButtons(long _chatID, string answer)
        {
            var _replyKeyboard = new ReplyKeyboardMarkup();

            _replyKeyboard.Keyboard = new KeyboardButton[][] {
                   new KeyboardButton[] {
                    new KeyboardButton("💰Получить Биткоин"), new KeyboardButton("💼Кошелёк"),
                }, new KeyboardButton[] {
                    new KeyboardButton("📈Курс"), new KeyboardButton("👨‍💼Партнерам")
                }, new KeyboardButton[] {
                    new KeyboardButton("📊Статистика"), new KeyboardButton("📄О Сервисе")
                } };

            _replyKeyboard.ResizeKeyboard = true;
            await botConnector.SendTextMessageAsync(_chatID, answer, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: _replyKeyboard);
        }
        private void ReplyToUser(Telegram.Bot.Types.Message command)
        {              
            var _userCommand = command.Text;
            var _messageId = command.MessageId;            
            var _userID = command.From.Id;            
            var _parseCommand = _userCommand.Split(' ')[0];            
            if (_parseCommand == "/start")
            {
                var _userParametr = botWork.UserParametr(_userCommand);
                botWork.UserIdenty(_userID, _userParametr);
                ReplyButtons(_userID, "bla bla");
            }
            else
            {
                botWork.UserIdenty(_userID, 0);
                switch (_userCommand)
                {                    
                    case "💰Получить Биткоин": botWork.ClaimBtc(_userID); break;
                    case "💼Кошелёк": InlaneButtons(_userID, WalletInfoText(command)); break;
                    case "📈Курс": ReplyButtons(_userID, BtcPrice()); break;
                    case "👨‍💼Партнерам": ReplyButtons(_userID, botWork.ReferalLink(_userID)); break;
                    case "📊Статистика": replyToUser.ReplySend(_userID, 1); break;
                    case "📄О Сервисе":  break;
                }
            }            
        }
        async void InlaneButtons(long _userId, string answer)
        {
            var inlineCallback1 = "Test1";            
            var _inlineButtons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📤Вывод", inlineCallback1)                   
                }
            });            

            await botConnector.SendTextMessageAsync(_userId, answer, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: _inlineButtons);
        }
        private string BtcPrice()
        {
            var _currentTime = botWork.GetTime();
            var _price = botWork.GetBTCPrice();
            var _priceString = String.Format("Биржевой Курс:" + "\r\n" +
                                                      "<b>₽{0} RUB</b> на {1}" + "\r\n" +
                                                      "<b>${2} USD</b> на {3}", _price[0], _currentTime, _price[1], _currentTime);            
            return _priceString;
        }
    }
}
