using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Library.Events
{
    public interface IBorrowingEvents
    {
        int BookID { get; set; }
        DateTime DateTime { get; set; }
        bool BorrowType { get; set; }
        string? UserID { get; set; }
    }
}
