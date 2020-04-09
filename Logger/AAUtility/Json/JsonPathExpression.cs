using System.Collections.Generic;

namespace AAUtility.Json
{
    class JsonPathExpression
    {
        public string Expression { get; set; }

        public IList<JsonPathNode> Nodes { get; private set; }

        public JsonPathExpression() { Nodes = new List<JsonPathNode>(); }
    }
}
