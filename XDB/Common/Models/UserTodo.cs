using System.Collections.Generic;

namespace XDB.Common.Models
{
    public class UserTodo
    {
        public ulong Id { get; set; }
        public List<string> ListItems { get; set; }
    }
}
