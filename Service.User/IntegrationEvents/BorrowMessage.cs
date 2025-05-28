using Library.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Service.User.IntegrationEvents
{
    public class BorrowMessage : IBorrowingEvents
    {
        private int _bookID;
        private DateTime _dateTime;
        private bool _borrowType;

        private string _userID;

        public int BookID { get => _bookID; set => this._bookID = value; }
        public DateTime DateTime { get => _dateTime; set => this._dateTime = value; }
        public bool BorrowType { get => _borrowType; set => this._borrowType = value; }
        public string UserID { get => _userID; set => this._userID = value; }

        public BorrowMessage() { }
    }
}
