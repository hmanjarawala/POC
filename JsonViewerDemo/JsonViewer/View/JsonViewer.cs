using JsonViewer.Presenter;
using System;
using System.Windows.Forms;

namespace JsonViewer.View
{
    public partial class JsonViewer : Form, IJsonViewerView
    {
        private readonly IJsonViewerPresenter _presenter;

        public JsonViewer(IJsonViewerPresenter presenter)
        {
            InitializeComponent();
            _presenter = presenter;
            _presenter.SetView(this);
        }

        private void jsonTreeView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            TreeGridNode currentNode = jsonTreeView.Rows[e.RowIndex] as TreeGridNode;

            txtOutput.Text = Convert.ToString(currentNode.Cells[3].Value);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string input = string.Join(Environment.NewLine, txtJson.Lines);

            try
            {
                _presenter.ParseJsonStringToNodes(input);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "JsonPath Extended", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearGrid()
        {
            jsonTreeView.Nodes.Clear();
        }

        public TreeGridNode AddNodeToGrid(string key, object value, string type, string path)
        {
            return jsonTreeView.Nodes.Add(key, value, type, path);
        }
    }
}
