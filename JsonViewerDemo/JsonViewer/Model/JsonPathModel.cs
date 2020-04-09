using JsonViewer.Classes;
using System.Collections.Generic;

namespace JsonViewer.Model
{
    public class JsonPathModel
    {
        public string SourceJson { get; set; }

        public IEnumerable<JsonPathExpression> Expressions { get; set; }
    }
}
