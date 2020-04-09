using System;
using JsonViewer.View;
using JsonViewer.Classes;
using System.Collections;

namespace JsonViewer.Presenter
{
    internal class JsonViewerPresenter : IJsonViewerPresenter
    {
        private IJsonViewerView _view;

        public void ParseJsonStringToNodes(string json)
        {
            _view.ClearGrid();

            var dict = JsonConvert.DeserializeObject(json);
            AddNodeToGrid(null, dict, "$");
        }

        public void SetView(IJsonViewerView view)
        {
            _view = view;
        }

        private void AddNodeToGrid(TreeGridNode parent, object nodes, string path)
        {
            IDictionary dict = nodes as IDictionary;

            if (dict != null)
            {
                foreach (var node in dict.Keys)
                {
                    TreeGridNode tnode = CreateNode(parent, node.ToString(), dict[node], path + "." + node.ToString());
                }
            }

            IList list = nodes as IList;

            if (list != null)
            {
                int index = 0;
                foreach (var item in list)
                {
                    TreeGridNode tnode = CreateNode(parent, Convert.ToString(index), item, path + "[" + Convert.ToString(index++) + "]");
                }
            }
        }

        private TreeGridNode SetValueToNode(TreeGridNode parent, string key, object value, string type, string path)
        {
            if (parent != null)
            {
                return parent.Nodes.Add(key, value, type, path);
            }
            else
            {
                return _view.AddNodeToGrid(key, value, type, path);
            }
        }

        private TreeGridNode SetObjectToNode(TreeGridNode parent, string key, IDictionary value, string path)
        {
            TreeGridNode node = SetValueToNode(parent, key, string.Empty, "D", path);

            AddNodeToGrid(node, value, path);
            return node;
        }

        private TreeGridNode SetArrayToNode(TreeGridNode parent, string key, IList value, string path)
        {
            TreeGridNode node = SetValueToNode(parent, key, string.Empty, "A", path);

            int index = 0;
            foreach (var item in value)
            {
                CreateNode(node, Convert.ToString(index), item, path + "[" + Convert.ToString(index++) + "]");
            }
            return node;
        }

        private TreeGridNode CreateNode(TreeGridNode parent, string key, object value, string path)
        {
            if (IsObject(value))
            {
                return SetObjectToNode(parent, key, value as IDictionary, path);
            }
            else if (IsArray(value))
            {
                return SetArrayToNode(parent, key, (IList)value, path);
            }
            else
            {
                return SetValueToNode(parent, key, value, "V", path);
            }
        }

        private bool IsObject(object value)
        {
            return value is IDictionary;
        }

        private bool IsArray(object value)
        {
            return value is IList;
        }
    }
}
