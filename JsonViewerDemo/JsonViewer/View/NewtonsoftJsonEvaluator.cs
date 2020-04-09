using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace JsonViewer.View
{
    using Classes;
    using Model;
    using Presenter;

    public partial class NewtonsoftJsonEvaluator : Form
    {
        const string APPLICATION_TITLE = "JsonPath Application";

        readonly IJsonPathEvaluatorPresenter _presenter;

        public NewtonsoftJsonEvaluator(IJsonPathEvaluatorPresenter presenter)
        {
            InitializeComponent();

            _presenter = presenter;
        }

        private string validateWindow()
        {
            string retVal = string.Empty;

            if (txtJson.Text.Trim().Length == 0)
                retVal = "Enter Json Value";

            if (txtExpression.Text.Trim().Length == 0)
                retVal = "Enter valid Expression";

            var expressions = txtExpression.Text.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if(expressions.Length==0)
                retVal = "No expression found delimitted by semi colon (;)";

            var expression = expressions.Where(exp => !exp.StartsWith("$"));

            if (expression.Count() > 0)
                retVal = string.Format("Following expressions should starts with $:\r\n{0}", 
                    string.Join(Environment.NewLine, expression));

            return retVal;
        }

        private bool isFirstLine()
        {
            return txtOutput.Text.Trim().Length == 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            txtOutput.Text = string.Empty;
            var result = validateWindow();

            if (!string.IsNullOrEmpty(result))
                MessageBox.Show(result, APPLICATION_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);

            try
            {
                JsonPathModel model = new JsonPathModel
                {
                    SourceJson = string.Join(Environment.NewLine, txtJson.Lines),
                    Expressions = Array.ConvertAll(txtExpression.Text.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries), (s) => new JsonPathExpression { Expression = s })
                };

                var expressions = _presenter.ParseExpression(model, ScriptEvaluatorFactory.ScriptEvaluatorTypes.Json, new JsonValueSystem());

                foreach (var expression in expressions)
                {
                    if (!isFirstLine())
                        txtOutput.Text += Environment.NewLine + Environment.NewLine;

                    txtOutput.Text += string.Format("Output of {0} is: \r\n {1}\r\n", expression.Expression,
                        string.Join(Environment.NewLine + Environment.NewLine, expression.Nodes));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, APPLICATION_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
