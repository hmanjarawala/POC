using System.Collections.Generic;

namespace JsonViewer.Classes
{
    public class JsonPathExpression
    {
        public JsonPathExpression()
        {
            Nodes = new List<JsonPathNode>();
        }

        public string Expression { get; set; }

        public IList<JsonPathNode> Nodes { get; private set; }
    }
}
