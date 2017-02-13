using System.Collections.Generic;

namespace XDB.Common.Attributes
{
    public class UserTodo
    {
        public ulong Id { get; set; }
        public List<string> ListItems { get; set; }
    }
}
