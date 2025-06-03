using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Service.User
{
    public class User
    {
        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}