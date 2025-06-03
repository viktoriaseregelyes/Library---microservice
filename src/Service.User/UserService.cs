using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Service.User
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public List<User> GetAllUsers()
        {
            return _users.Find(Builders<User>.Filter.Empty).ToList();
        }

        public User GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, new ObjectId(id));
            return _users.Find(filter).FirstOrDefault();
        }

        public User DeleteUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, new ObjectId(id));
            return _users.FindOneAndDelete(filter);
        }

        public void AddUser(User user)
        {
            _users.InsertOne(user);
        }
    }
}
