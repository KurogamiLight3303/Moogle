using System.Collections.Generic;

namespace MoogleEngine
{
    public class SearchNode
    {
        public char Character { get; set; }
        public List<long>? Positions { get; set; }
        public List<SearchNode>? Next { get; set; }
    }
}