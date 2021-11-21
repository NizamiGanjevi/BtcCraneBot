using System;
using System.Text.RegularExpressions;

namespace BtcCraneBot
{
    class BotWork
    {

        DBCommand dBCommand = new DBCommand();
        public void UserIdenty(int _userId, int _refNumber)
        {                
            var _id = dBCommand.ReadUserId(_userId);
            if (_id == 0)
            {
                if (_refNumber == 0)
                {
                    dBCommand.InsertData(_userId, 0.00000000, 0, 0, 0, 0);
                }
                else
                {
                    var _secondLevelRefNumber = (long)dBCommand.ReadData(_refNumber, "ref_number");
                    dBCommand.InsertData(_userId, 0.00000000, 0, 0, 0, _refNumber);
                    UpdateReferalData(_refNumber, "user_ref_value");
                    if (_secondLevelRefNumber != 0)
                    {
                        UpdateReferalData(_secondLevelRefNumber, "user_refs_value");
                    }
                }                
            }                        
        }        
        public void ClaimBtc(int _userId)
        {            
            var _select = "balance";
            _select = "claim_time";
            var _claimTime = (long)dBCommand.ReadData(_userId, _select);
            var _nextClaimTime = _claimTime + 10;
            var _currentTime = GetUnixTime();
            var _claimBonus = RandomBonus();

            if(_nextClaimTime <= _currentTime)
            {               
                dBCommand.UpdateClaimData(_userId, _claimBonus, GetUnixTime().ToString());
                dBCommand.UpdateTotalPaidValue(_claimBonus);
                BonusReplySwitch(1, _userId, 0, _claimBonus);
                var _referalNumber = (long)dBCommand.ReadData(_userId, "ref_number");
                if (_referalNumber != 0)
                {                    
                    var _referalLevel = 1;
                    ReferalBonus(_referalNumber, _referalLevel, _claimBonus);
                }
            }
            else
            {
                var _waitTime = _nextClaimTime - _currentTime;
                BonusReplySwitch(2, _userId, _waitTime, 0);
            }            
        }
        public object[] GetWalletInfo(Telegram.Bot.Types.Message command)
        {            
            string[] _selectArray = { "balance", "user_ref_value", "user_refs_value" };
            var _userId = command.From.Id;
            object[] _walletData = new object[3];
            int i = 0;
            foreach (string _selectData in _selectArray)
            {
                _walletData[i] = dBCommand.ReadData(_userId, _selectData);
                i++;
            }
            return _walletData;
        }
        public int UserParametr(string _userCommand)
        {
            if (_userCommand.Length > 6)
            {
                try
                {
                    var _userParametr = Convert.ToInt32(Regex.Replace(_userCommand, @"[^\d]+", ""));
                    return _userParametr;
                }
                catch(Exception)
                {                    
                    return 0;
                }                
            }
            else
            {
                return 0;
            }
        }
        public string ReferalLink(int _userId)
        {
            string _referalLink = String.Format("telegram.me/Free_ADV_bot?start=" + _userId.ToString());
            return _referalLink;
        }
        public string[] GetBTCPrice()
        {
            var priceGetter = new ExchangeSharp.ExchangeYobitAPI();
            string[] _price = new string[2];
            _price[0] = priceGetter.GetTicker("btc_rur").Last.ToString("0");
            _price[1] = priceGetter.GetTicker("btc_usd").Last.ToString("0");            

            return _price;
        }
        public string GetTime()
        {
            var _currentTime = DateTime.Now.ToString();
            return _currentTime;
        }
        private void ReferalBonus(long _userId, int _referalLevel, double _claimBonus)
        {
            var _claimTime = "claim_time";
            if (_referalLevel == 1)
            {
                var _bonus = Math.Round(_claimBonus * 0.05, 8);
                dBCommand.UpdateClaimData(_userId, _bonus, _claimTime);
                BonusReplySwitch(3, _userId, 0, _bonus);
                var _referalNumber = (long)dBCommand.ReadData(_userId, "ref_number");
                if (_referalNumber != 0)
                {
                    ReferalBonus(_referalNumber, 2, _claimBonus);
                    dBCommand.UpdateTotalPaidValue(_claimBonus);
                }
            }
            else if (_referalLevel == 2)
            {
                var _bonus = Math.Round(_claimBonus * 0.02, 8);
                dBCommand.UpdateClaimData(_userId, _bonus, _claimTime);
                dBCommand.UpdateTotalPaidValue(_bonus);
                BonusReplySwitch(4, _userId, 0, _bonus);
            }
        }

        private int GetUnixTime()
        {
            var _time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return _time;
        }

        private void BonusReplySwitch(int _replySwitch, long _chatId, long _waitTime, double _claimBonus)
        {
            string _sClaimBonus = _claimBonus.ToString("0.00000000");
            switch (_replySwitch)
            {
                case 1: BonusReplySend(_chatId, "🎉 Вы получили бонус " + _sClaimBonus + " BTC!"); break;                    
                case 2: BonusReplySend(_chatId, "Вам еще ждать " + _waitTime.ToString()); break;
                case 3: BonusReplySend(_chatId, "🎁 Вы получили бонус за реферала! " + _sClaimBonus + " BTC"); break;
                case 4: BonusReplySend(_chatId, "🎁 Вы получили бонус за реферала вашего реферала! " + _sClaimBonus + " BTC"); break;
            }

        }
        private void BonusReplySend(long _chatId, string _replyText)
        {
            BotInterface botInterface = new BotInterface();
            botInterface.ReplyButtons(_chatId, _replyText);
        }
        private void UpdateReferalData(long _referalId, string _refLevel)
        {
            dBCommand.UpdateReferalData(_referalId, _refLevel);
        }
        private double RandomBonus()
        {
            Random random = new Random();
            double _resultBonus;
            int _chance = random.Next(1, 10001);
            if(_chance == 10001)
            {
                _resultBonus = random.Next(10000, 20000);
                _resultBonus = _resultBonus / 100000000;
                return _resultBonus;
            }
            else if(_chance >= 9990 && _chance <= 10000)
            {
                _resultBonus = random.Next(5000, 9999);
                _resultBonus = _resultBonus / 100000000;
                return _resultBonus;
            }
            else if(_chance >= 9850 && _chance < 9900)
            {
                _resultBonus = random.Next(1000, 4999);
                _resultBonus = _resultBonus / 100000000;
                return _resultBonus;
            }
            else if(_chance >= 9750 && _chance < 9850)
            {
                _resultBonus = random.Next(500, 999);
                _resultBonus = _resultBonus / 100000000;
                return _resultBonus;
            }
            else
            {
                _resultBonus = random.Next(100, 499);
                _resultBonus = _resultBonus / 100000000;
                return _resultBonus;
            }
        }
    }
}
