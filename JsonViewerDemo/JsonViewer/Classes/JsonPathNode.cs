using System;
using System.Collections;
using System.Diagnostics;

namespace JsonViewer.Classes
{
    [Serializable]
    public sealed class JsonPathNode
    {
        private readonly object value;
        private readonly string path;

        public JsonPathNode(object value, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("path");

            this.value = value;
            this.path = path;
        }

        public object Value
        {
            get { return value; }
        }

        public string Path
        {
            get { return path; }
        }

        public override string ToString()
        {
            return Path + " = " + Value;
        }

        public static object[] ValuesFrom(ICollection nodes)
        {
            object[] values = new object[nodes != null ? nodes.Count : 0];

            if (values.Length > 0)
            {
                Debug.Assert(nodes != null);

                int i = 0;
                foreach (JsonPathNode node in nodes)
                    values[i++] = node.Value;
            }

            return values;
        }

        public static string[] PathsFrom(ICollection nodes)
        {
            string[] paths = new string[nodes != null ? nodes.Count : 0];

            if (paths.Length > 0)
            {
                Debug.Assert(nodes != null);

                int i = 0;
                foreach (JsonPathNode node in nodes)
                    paths[i++] = node.Path;
            }

            return paths;
        }
    }
}
