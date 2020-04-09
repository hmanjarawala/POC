using JsonViewer.Presenter;
using JsonViewer.View;
using System;
using System.Windows.Forms;

namespace JsonViewer
{
    public partial class Master : Form
    {
        public Master()
        {
            InitializeComponent();
        }

        private void ShowBasicJsonEvaluatorForm(object sender, EventArgs e)
        {
            IJsonPathEvaluatorPresenter presentor = new JsonPathEvaluatorPresenter();
            Form childForm = new BasicJsonEvaluator(presentor);
            childForm.MdiParent = this;
            childForm.Text = "Basic JsonPath Evaluator";
            childForm.WindowState = FormWindowState.Normal;
            childForm.Show();
        }

        private void ShowNewtonsoftJsonEvaluatorForm(object sender, EventArgs e)
        {
            IJsonPathEvaluatorPresenter presentor = new JsonPathEvaluatorPresenter();
            Form childForm = new NewtonsoftJsonEvaluator(presentor);
            childForm.MdiParent = this;
            childForm.Text = "Newtonsoft JsonPath Evaluator";
            childForm.WindowState = FormWindowState.Normal;
            childForm.Show();
        }

        private void ShowJsonViewerForm(object sender, EventArgs e)
        {
            IJsonViewerPresenter presenter = new JsonViewerPresenter();
            Form childForm = new View.JsonViewer(presenter);
            childForm.MdiParent = this;
            childForm.Text = "Json Viewer";
            childForm.WindowState = FormWindowState.Normal;
            childForm.Show();
        }
    }
}
