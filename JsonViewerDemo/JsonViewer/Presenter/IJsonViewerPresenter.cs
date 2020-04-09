using JsonViewer.View;

namespace JsonViewer.Presenter
{
    public interface IJsonViewerPresenter
    {
        void SetView(IJsonViewerView view);

        void ParseJsonStringToNodes(string json);
    }
}
