using System;
using System.Collections.Generic;
using System.Linq;
using JsonViewer.Classes;
using JsonViewer.Model;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace JsonViewer.Presenter
{
    class JsonPathEvaluatorPresenter : IJsonPathEvaluatorPresenter
    {
        public IEnumerable<JsonPathExpression> ParseExpression(JsonPathModel model, ScriptEvaluatorFactory.ScriptEvaluatorTypes evaluatorType = ScriptEvaluatorFactory.ScriptEvaluatorTypes.Basic, IJsonPathValueSystem valueSystem = null)
        {
            IList<JsonPathExpression> result = new List<JsonPathExpression>(model.Expressions.Count());
            var json = JsonConvert.DeserializeObject(model.SourceJson);

            var context = new JsonPathContext
            {
                ScriptEvaluator = new JsonPathScriptEvaluator(ScriptEvaluatorFactory.Create(evaluatorType).EvaluateScript),
                ValueSystem = valueSystem
            };

            foreach(JsonPathExpression expression in model.Expressions)
            {
                var nodes = context.SelectNodes(json, expression.Expression);
                Array.Copy(nodes, expression.Nodes.ToArray(), nodes.Length);
                result.Add(expression);
            }

            return result.AsEnumerable();
        }
    }
}
