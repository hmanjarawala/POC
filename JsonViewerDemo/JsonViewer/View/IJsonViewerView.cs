namespace JsonViewer.View
{
    public interface IJsonViewerView
    {
        void ClearGrid();
        TreeGridNode AddNodeToGrid(string key, object value, string type, string path);
    }
}
