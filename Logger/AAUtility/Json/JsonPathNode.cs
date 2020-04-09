using System;
using System.Collections;

namespace AAUtility.Json
{
    [Serializable]
    sealed class JsonPathNode
    {
        public object Value { get; private set; }

        public string Path { get; private set; }

        public JsonPathNode(object value, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Trim().Length == 0)
                throw new ArgumentException("path");

            Value = value;
            Path = path;
        }

        public static object[] ValuesFrom(ICollection nodes)
        {
            object[] values = new object[nodes != null ? nodes.Count : 0];

            if (values.Length > 0)
            {
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
                int i = 0;
                foreach (JsonPathNode node in nodes)
                    paths[i++] = node.Path;
            }

            return paths;
        }

        public override string ToString()
        {
            return string.Concat(Path, "=", Value);
        }
    }
}
