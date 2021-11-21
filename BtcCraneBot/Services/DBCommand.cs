namespace BtcCraneBot
{
    class DBCommand : DBConnect
    {
              
        public static void UserTableCreate()
        {
            
            var sqlCommand = @"CREATE TABLE IF NOT EXISTS [BotUsers] (                  [user_id] INTGER NOT NULL UNIQUE,
                                                                                        [balance] REAL,                                                                                        
                                                                                        [user_ref_value] INTGER,
                                                                                        [user_refs_value] INTGER,
                                                                                        [claim_time] INTGER,
                                                                                        [ref_number] INTGER
                                                                                      );";            
            ExecuteNonQuery(sqlCommand);
        }
        public static void ServiceTableCreate()
        {
            var sqlCommand = @"CREATE TABLE IF NOT EXISTS [Service] ( [total_paid] INTGER); INSERT INTO Service (total_paid) VALUES (0); DELETE FROM Service WHERE total_paid != (SELECT MAX(total_paid) FROM Service);";
            ExecuteNonQuery(sqlCommand);            
        }
        public void InsertData(int id, double balance, int userRef, int userRefs, int unixTime, int refNumber)
        {
            var sqlCommand = string.Format(@"INSERT INTO BotUsers (user_id, balance, user_ref_value, user_refs_value, claim_time, ref_number) VALUES ({0}, {1}, {2}, {3}, {4}, {5})", id, balance, userRef, userRefs, unixTime, refNumber);
            ExecuteNonQuery(sqlCommand);            
        }
        public long ReadUserId(long id)
        {
            var sqlCommand = string.Format(@"SELECT user_id FROM BotUsers WHERE user_id = {0}", id);
            object _id = ExecuteScalar(sqlCommand);
            if (_id != null)
            {
                var userId = (long)_id;
                return userId;
            }
            else
            {
                return 0;
            }            
        }
        public void UpdateClaimData(long _userId, double _claimBonus, string _claimTime)
        {
            var _select = "balance";
            var _currentBalance = (double)ReadData(_userId, _select);
            var _claimBalance = _currentBalance + _claimBonus;
            var _sqlCommand = string.Format(@"UPDATE BotUsers SET balance = {0}, claim_time = {1} WHERE user_id = {2}", _claimBalance, _claimTime, _userId);
            ExecuteNonQuery(_sqlCommand);
        }
        public void UpdateReferalData(long _id, string _refLevel)
        {
            ExecuteNonQuery(string.Format(@"UPDATE BotUsers SET {0} = {0} + 1 WHERE user_id = {1}", _refLevel, _id));
        }

        public object ReadData(long _id, string _select)
        {
            var sqlCommand = string.Format(@"SELECT {0} FROM BotUsers WHERE user_id = {1}", _select, _id);
            object _data = ExecuteScalar(sqlCommand);
            return _data;
        }
        public void UpdateTotalPaidValue(double _changeValue)
        {
            ExecuteNonQuery(string.Format(@"UPDATE Service SET total_paid = total_paid + {0}", _changeValue));
        }
        public object ReadTotalPaidData()
        {
            var sqlCommand = string.Format(@"SELECT total_paid FROM Service");
            object _data = ExecuteScalar(sqlCommand);
            if(_data != null)
            {
                return _data;
            }
            else
            {
                return 0;
            }
        }
        public object TotalUsersCount()
        {
            var sqlCommand = string.Format(@"SELECT COUNT(*) FROM BotUsers");
            var _data = ExecuteScalar(sqlCommand);
            return _data;
        }
    }
}
