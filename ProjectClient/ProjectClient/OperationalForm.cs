using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShoppingClient
{
    public partial class OperationalForm : Form
    {
        private ServerConnection _serverConnection;
        private string _userName;
        private Dictionary<string, int> _products;

        public OperationalForm(ServerConnection serverConnection, string userName)
        {
            InitializeComponent();
            _serverConnection = serverConnection;
            _userName = userName;
            LoadProducts();
        }

        private void LoadProducts()
        {
            string productData = _serverConnection.GetProducts();
            if (productData.StartsWith("PRODUCTS"))
            {
                _products = new Dictionary<string, int>();
                var productEntries = productData.Substring(9).Split('|');
                string initialProductList = string.Empty;

                foreach (var entry in productEntries)
                {
                    var parts = entry.Split(',');
                    _products[parts[0]] = int.Parse(parts[1]);
                    cmbProducts.Items.Add(parts[0]);
                    initialProductList += $"{parts[0]} {parts[1]}, ";
                }

                cmbProducts.SelectedIndex = 0;
                initialProductList = initialProductList.TrimEnd(',', ' ');
                AppendTextToPanel(initialProductList);
            }
            else
            {
                MessageBox.Show("Failed to retrieve product list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            string selectedProduct = cmbProducts.SelectedItem.ToString();
            string purchaseResult = _serverConnection.PurchaseProduct(selectedProduct);

            if (purchaseResult == "PURCHASE_SUCCESS")
            {
                lblStatus.Text = $"Purchased {selectedProduct}";
                AppendTextToPanel($"{selectedProduct} purchased by {_userName}");
            }
            else if (purchaseResult == "NOT_AVAILABLE")
            {
                MessageBox.Show($"{selectedProduct} is no longer available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Failed to purchase product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendTextToPanel(string text)
        {
            if (flowLayoutPanel1.Controls.Count == 0)
            {
                // Add a single RichTextBox initially
                RichTextBox richTextBox = new RichTextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.None,
                    Width = flowLayoutPanel1.Width - 25, // Adjust width to match the panel
                    Height = flowLayoutPanel1.Height - 10 // Match height of the panel
                };
                flowLayoutPanel1.Controls.Add(richTextBox);
            }

            // Assuming only one RichTextBox exists in the panel
            RichTextBox richTextBoxControl = flowLayoutPanel1.Controls[0] as RichTextBox;
            richTextBoxControl.AppendText($"{text}\n");
        }

        private void OperationalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serverConnection.Disconnect();
        }

   
    }
}
