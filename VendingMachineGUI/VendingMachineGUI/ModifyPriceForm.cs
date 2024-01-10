using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VendingMachineGUI
{
    /// <summary>
    /// This <p>class</p> runs the modify price window
    ///
    /// <para>Author - Maria Esteban</para>
    /// <para>Version - 1.0 (12-07-23)</para>
    /// <para>Since - 12-07-23</para>
    /// </summary>
    public partial class ModifyProductForm : Form
    {
        public bool worked; // tells whether or not the operation went successfully
        private double price;
        public ModifyProductForm()
        {
            InitializeComponent();
            worked = false;
            price = 0.0;
        }


        /// <summary>
        /// Adds products to the comboBox
        /// </summary>
        /// <param name="products">the products being added</param>
        public void AddProducts(List<Product> products)
        {
            foreach (Product product in products)
            {
                productList.Items.Add(product.ToString());
            }
        }

        /// <summary>
        /// Change the price of the selected item
        /// </summary>
        /// <param name="price">the new price</param>
        /// <returns>Returns the selected item and its new price</returns>
        public string ChangePrice(out double price)
        {
            price = this.price;
            return productList.SelectedItem.ToString();
            
        }

        /// <summary>
        /// Confirms the action
        /// </summary>
        /// <param name="sender">the client</param>
        /// <param name="e">the action</param>
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (Double.TryParse(priceTextBox.Text, out price))
                worked = true;
            else
                worked = false;
            this.Close();
            
        }

        /// <summary>
        /// Cancels the current action
        /// </summary>
        /// <param name="sender">the client</param>
        /// <param name="e">the action</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            worked = false;
            this.Close();
        }
    }
}
