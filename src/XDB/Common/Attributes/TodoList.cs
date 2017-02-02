using System.Collections.Generic;

namespace XDB.Common.Attributes
{
    public class TodoList
    {
        public ulong Id { get; set; }
        public List<string> ListItems { get; set; }
    }
}
