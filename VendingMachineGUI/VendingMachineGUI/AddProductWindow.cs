using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VendingMachineGUI
{
    /// <summary>
    /// This class runs the adding product window
    ///
    /// <para>Author - Maria Esteban</para>
    /// <para>Version - 2.0 (12-07-23)</para>
    /// <para>Since - 11-30-23</para>
    /// </summary>
    public partial class AddProductForm : Form
    {
        public bool done; // tells whether or not the operation went successfully
        public string Code {  get; set; }
        public string Description {  get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public AddProductForm(List<string> codes)
        {
            InitializeComponent();
            foreach (string code in codes)
            {
                codesList.Items.Add(code);
            }
        }

        /// <summary>
        /// Confirms the action
        /// </summary>
        /// <param name="sender">the client</param>
        /// <param name="e">the action</param>
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            Code = codesList.SelectedItem.ToString();
            Description = descriptionTextBox.Text;

            if (!double.TryParse(priceTextBox.Text, out double price))
            {
                done = false;
                this.Close();
            }

            if (!int.TryParse(quantityTextBox.Text, out int quantity)) 
            { 
                done = false; 
                this.Close(); 
            }

            Price = price;
            Quantity = quantity;
            done = true;
            this.Close();
        }

        /// <summary>
        /// Cancel the action
        /// </summary>
        /// <param name="sender">the client</param>
        /// <param name="e">the action</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            done = false;
            this.Close();
        }
    }
}
