namespace BtcCraneBot
{
     class ReplyToUser : ApiConnect
    {
        public async  void TestReply(Telegram.Bot.Types.CallbackQuery _callbackQuery)
        {
           await botConnector.SendTextMessageAsync(_callbackQuery.From.Id, "<i>Минимальная сумма вывода 0.002 BTC</i>", Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        public async void ReplySend(long _userId, int answerSwitch)
        {
            DBCommand dbCommand = new DBCommand();
            if(answerSwitch == 1)
            {
                var _totalPaid = (double)dbCommand.ReadTotalPaidData();
                var _totalUsers = (long)dbCommand.TotalUsersCount();
                var _replyString = string.Format("Уже выплаченно: <b>{0}</b> BTC\r\nВсего пользователей: <b>{1}</b>", _totalPaid.ToString("0.00000000"), _totalUsers.ToString());
                await botConnector.SendTextMessageAsync(_userId, _replyString, Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
    }
}
