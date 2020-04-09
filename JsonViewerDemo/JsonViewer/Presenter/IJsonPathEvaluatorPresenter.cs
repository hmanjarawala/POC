using JsonViewer.Classes;
using JsonViewer.Model;
using System.Collections.Generic;

namespace JsonViewer.Presenter
{
    public interface IJsonPathEvaluatorPresenter
    {
        IEnumerable<JsonPathExpression> ParseExpression(JsonPathModel model, ScriptEvaluatorFactory.ScriptEvaluatorTypes evaluatorType = ScriptEvaluatorFactory.ScriptEvaluatorTypes.Basic, IJsonPathValueSystem valueSystem = null);
    }
}
