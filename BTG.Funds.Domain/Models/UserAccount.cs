using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BTG.Funds.Domain.Models
{
    public class UserAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 500000;

        [BsonElement("SubscribedFunds")]
        public List<string> SubscribedFunds { get; set; } = new();
    }
}
