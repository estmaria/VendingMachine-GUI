using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace VendingMachineGUI
{
    /// <summary>
    /// This class runs the vending machine Graphical User Interface
    ///
    /// <para>Author - Maria Esteban</para>
    /// <para>Version - 2.3 (12-08-23)</para>
    /// <para>Since - 11-30-23</para>
    /// </summary>
    public partial class VendingMachineGUI : Form

    {
        private static readonly string STORAGEFILE = "Products.txt";
        private static readonly string BANKFILE = "Bank.txt";
        static readonly string CHANGEBOXFILE = "ChangeBox.txt";
        private static readonly string RESTOCKFILE = "ProductsRestock.txt";
        private static readonly string CHANGERESTOCKFILE = "ChangeBoxRestock.txt";
        private static readonly string PASSWORDFILE = "MachinePassword.txt";
        private static List<PictureBox> pictures = new List<PictureBox>(); // pictures of each products
        private Boolean changePassword = false; // tells whether or not the password can be changed
        private static readonly Coin[] COINS =
        {
            Coin.NICKEL, Coin.DIME, Coin.QUARTER, Coin.DOLLAR
        };

        private VendingMachine machine = new VendingMachine();
        internal List<String> codes = new List<String>(); // avalaible codes

        public VendingMachineGUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Runs the opening operations of the vending machine
        /// </summary>
        /// <param name="sender"> the client of the vending machine</param>
        /// <param name="e"></param>
        private void OpeningOperations(object sender, EventArgs e)
        {
            codes.Add("A1"); codes.Add("A2"); codes.Add("A3"); codes.Add("A4"); codes.Add("A5"); codes.Add("A6"); codes.Add("A7"); codes.Add("A8");
            codes.Add("B1"); codes.Add("B2"); codes.Add("B3"); codes.Add("B4"); codes.Add("B5"); codes.Add("B6"); codes.Add("B7");
            codes.Add("C1"); codes.Add("C2"); codes.Add("C3"); codes.Add("C4"); codes.Add("C5"); codes.Add("C6"); codes.Add("C7");
            LoadMachine();
            LoadBank();
            LoadChangeBox();
            LoadPassword();
            UpdateProducts();
        }

        /// <summary>
        /// Runs the closing operations of the vending machine
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e"></param>
        private void ClosingOperations(object sender, FormClosingEventArgs e)
        {
            OffLoadMachine();
            SaveBank();
            SaveChangeBox();
            SavePassword();
        }

        /// <summary>
        /// Loads the vending machine reading from a file
        /// </summary>
        private void LoadMachine(bool restock = false)
        {
            pictures.Add(A1); pictures.Add(A2); pictures.Add(A3); pictures.Add(A4); pictures.Add(A5); pictures.Add(A6); pictures.Add(A7); pictures.Add(A8);
            pictures.Add(B1); pictures.Add(B2); pictures.Add(B3); pictures.Add(B4); pictures.Add(B5); pictures.Add(B6); pictures.Add(B7);
            pictures.Add(C1); pictures.Add(C2); pictures.Add(C3); pictures.Add(C4); pictures.Add(C5); pictures.Add(C6); pictures.Add(C7);

            FileStream inFile;

            if (restock)
                inFile = new FileStream(RESTOCKFILE, FileMode.Open, FileAccess.Read);
            else
                inFile = new FileStream(STORAGEFILE, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);

            string line = reader.ReadLine();

            if (restock)
                machine.products.Clear();

            while (line != null)
            {
                string[] lines = line.Split(',');

                machine.AddProduct(new Product(lines[0], lines[1], Double.Parse(lines[2]), int.Parse(lines[3])));

                line = reader.ReadLine();
            }
            reader.Close();
            inFile.Close();


            foreach (PictureBox picture in pictures)
            {
                picture.Visible = false;
            }
            if (!restock)
                maintenanceControlsPanel.Visible = false;
            UpdateProducts();
        }

        /// <summary>
        /// Saves every products in the vending machine in the products.txt file
        /// </summary>
        private void OffLoadMachine()
        {
            try
            {
                FileStream inFile = new FileStream(STORAGEFILE, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(inFile);

                List<Product> products = machine.GetProductTypes();

                foreach (Product aProduct in products)
                {
                    writer.WriteLine($"{aProduct.Code},{aProduct.Description},{aProduct.Price},{aProduct.Quantity}");
                }

                writer.Close();
                inFile.Close();
            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{STORAGEFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while writing on the file {STORAGEFILE}";
            }
        }

        /// <summary>
        /// Loads the storage coinbox of the vending machine with the coins in the Bank.txt file
        /// </summary>
        private void LoadBank()
        {
            try
            {
                FileStream inFile = new FileStream(BANKFILE, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader reader = new StreamReader(inFile);

                string line = reader.ReadLine();

                while (line != null)
                {
                    string[] lines = line.Split(',');
                    machine.AddCoin(new Coin(Double.Parse(lines[0]), lines[1]), true);

                    line = reader.ReadLine();
                }

                reader.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{BANKFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while reading the file {BANKFILE}";
            }
        }

        /// <summary>
        /// Save all the coins the storage coinbox in the Bank.txt file
        /// </summary>
        private void SaveBank()
        {
            try
            {
                FileStream inFile = new FileStream(BANKFILE, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(inFile);

                foreach (Coin coin in machine.coins)
                {
                    writer.WriteLine($"{coin.Value},{coin.Name}");
                }
                writer.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{BANKFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while writing on the file {BANKFILE}";
            }

        }

        /// <summary>
        /// Loads the change coinbox with the coins in the ChangeBox.txt file
        /// </summary>
        /// <param name="restock">whether or not we are restocking</param>
        private void LoadChangeBox(bool restock = false)
        {
            try
            {
                string fileName;

                if (restock)
                    fileName = CHANGERESTOCKFILE;
                else
                    fileName = CHANGEBOXFILE;

                FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(inFile);

                if (restock)
                    machine.changeBox.RemoveAllCoins();
                string line = reader.ReadLine();

                while (line != null)
                {
                    
                    string[] lines = line.Split(',');
                    machine.changeBox.AddCoin(new Coin(Double.Parse(lines[0]), lines[1]));

                    line = reader.ReadLine();
                }
                reader.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{CHANGEBOXFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while reading the file {CHANGEBOXFILE}";
            }
        }

        /// <summary>
        /// Saves all the coins in the ChangeBox coinbox in the ChangeBox.txt file
        /// </summary>
        private void SaveChangeBox()
        {
            try
            {
                FileStream inFile = new FileStream(CHANGEBOXFILE, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(inFile);

                foreach (Coin coin in machine.changeBox)
                {
                    writer.WriteLine($"{coin.Value},{coin.Name}");
                }

                writer.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{CHANGEBOXFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while writing on the file {CHANGEBOXFILE}";
            }

        }

        /// <summary>
        /// Loads the password to access the maintenance mode
        /// </summary>
        private void LoadPassword()
        {
            try
            {
                FileStream inFile = new FileStream(PASSWORDFILE, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(inFile);

                machine.Password = reader.ReadLine();
                
                reader.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{PASSWORDFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while reading the file {PASSWORDFILE}";
            }
        }

        /// <summary>
        /// Saves the password to access the maintenance mode
        /// </summary>
        private void SavePassword()
        {
            try
            {
                FileStream inFile = new FileStream(PASSWORDFILE, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(inFile);
                
                writer.WriteLine(machine.Password);

                writer.Close();
                inFile.Close();

            }
            catch (FileNotFoundException)
            {
                MainDisplay.Text = $"{PASSWORDFILE} not found";
            }
            catch (Exception e)
            {
                MainDisplay.Text = $"{e.Message} occured while writing on the file {PASSWORDFILE}";
            }
        }

        /// <summary>
        /// Display the current state of each product in the vending machine
        /// </summary>
        private void UpdateProducts()
        {
            string productString = "";


            foreach (PictureBox picture in pictures)
            {
                picture.Visible = false;
            }

            foreach (Product p in machine.products)
            {
                foreach (PictureBox picture in pictures)
                {
                    if (p.Code == picture.Name && p.Quantity > 0)
                        picture.Visible = true;
                    else if (p.Quantity <= 0)
                        picture.Visible = false;
                }

                if (codes.Contains(p.Code))
                {
                    codes.Remove(p.Code);
                }
                    
            }

            ShowProducts.Text = productString;
            // reloads the form
            this.Hide();
            this.Show();
        }

        /// <summary>
        /// Updates the amount of money displayed
        /// </summary>
        private void UpdateMoney()
        {
            DisplayMoney.Text = "Current: " + machine.currentCoins.GetValue().ToString("C");
        }


        /// <summary>
        /// When user presses enter and there is not money in the currrent CoinBox is going to 
        /// display the price of the product selected.When there is money, if there is enough
        /// money to purchase purchase; if there is not enough money display error message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterButton_Click(object sender, EventArgs e)
        {
            Product chosenProduct = null;
            string input = MainDisplay.Text;
            foreach (Product p in machine.GetProductTypes())
            {
                if (input == p.Code)
                    chosenProduct = p;
            }
            if (chosenProduct == null)
                MainDisplay.Text = "No product correspons to that code";


            else if (machine.currentCoins.GetValue() == 0)
            {
                if (chosenProduct.Quantity <= 0)
                    MainDisplay.Text = "Product run out";
                else
                    MainDisplay.Text = chosenProduct.ToString();
            }

            else
            {
                BuyProduct(chosenProduct, out double change);
            }

        }

        /// <summary>
        /// Purchases a product a returns a change
        /// </summary>
        /// <param name="p">the product being purchased</param>
        /// <param name="change">the change from the transaction</param>
        private void BuyProduct(Product p, out double change)
        {
            change = 0;
            if (machine.currentCoins.GetValue() < p.Price)
            {
                MainDisplay.Text = "Not enough money. The price is " + p.Price.ToString("C");
            }
            else
            {
                p.Quantity--;
                if (p.Quantity <= 0)
                {
                    machine.products.Remove(p);
                }


                change = machine.GetChange(machine.currentCoins.GetValue() - p.Price).GetValue();

                machine.coins.AddCoins(machine.currentCoins);
                machine.currentCoins.RemoveAllCoins();
                MainDisplay.Text = "Purchased: " + p.Description + " @ " + p.Price.ToString("C");
                DisplayMoney.Text = "Change: " + change.ToString("C");
            }
            UpdateProducts();
        }

        /// <summary>
        /// Adds A to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ButtonA_Click(object sender, EventArgs e)
        {
            MainDisplay.Text = "";
            MainDisplay.Text = "A";
        }

        /// <summary>
        /// Adds B to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ButtonB_Click(object sender, EventArgs e)
        {
            MainDisplay.Text = "";
            MainDisplay.Text = "B";
        }

        /// <summary>
        /// Adds C to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ButtonC_Click(object sender, EventArgs e)
        {
            MainDisplay.Text = "";
            MainDisplay.Text = "C";
        }

        /// <summary>
        /// Adds 1 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6) // makes sure that the screen doesn't clear
            {
                MainDisplay.Text += "1";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "1";
            }
            
        }

        /// <summary>
        /// Adds 2 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button2_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "2";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "2";
            }
        }

        /// <summary>
        /// Adds 3 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button3_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "3";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "3";
            }
        }

        /// <summary>
        /// Adds 4 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button4_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "4";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "4";
            }
        }

        /// <summary>
        /// Adds 5 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button5_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "5";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "5";
            }
        }

        /// <summary>
        /// Adds 6 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button6_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "6";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "6";
            }
        }

        /// <summary>
        /// Adds 7 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button7_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "7";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "7";
            }
        }

        /// <summary>
        /// Adds 8 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button8_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "8";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "8";
            }
        }

        /// <summary>
        /// Adds 9 to the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Button9_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == "A" || MainDisplay.Text == "B" || MainDisplay.Text == "C" || MainDisplay.Text.Length <= 6)
            {
                MainDisplay.Text += "9";
            }
            else
            {
                MainDisplay.Text = "";
                MainDisplay.Text += "9";
            }
        }

        /// <summary>
        /// Clears the main display label
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            MainDisplay.Text = "";
        }


        /// <summary>
        /// Adds a nickel to the current coinBox
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void AddNickel_Click(object sender, EventArgs e)
        {
            machine.AddCoin(COINS[0]);
            UpdateMoney();
        }

        /// <summary>
        /// Adds a dime to the current coinBox
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void AddDime_Click(object sender, EventArgs e)
        {
            machine.AddCoin(COINS[1]);
            UpdateMoney();
        }

        /// <summary>
        /// Adds a quarter to the current coinBox
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void AddQuarter_Click(object sender, EventArgs e)
        {
            machine.AddCoin(COINS[2]);
            UpdateMoney();
        }

        /// <summary>
        /// Adds a dollar to the current coinBox
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void AddDollar_Click(object sender, EventArgs e)
        {
            machine.AddCoin(COINS[3]);
            UpdateMoney();
        }

        /// <summary>
        /// Cancels the currenct transaction
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
          machine.currentCoins.RemoveAllCoins();
            UpdateMoney();
        }

        /// <summary>
        /// Switchs from vending mode to maintenance mode
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void MaintenanceMode_Click(object sender, EventArgs e)
        {
            if (MainDisplay.Text == machine.Password)
            {
                machine.maintenanceMode = true;
                MainDisplay.Text = "Correct password. You are in maintenance mode";

                maintenanceControlsPanel.Visible = true;

            }
            else
                MainDisplay.Text = "Incorrect password";

        }

        /// <summary>
        /// Adds a new product to the vending machine
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void AddProducts_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                AddProductForm form = new AddProductForm(codes);
                form.ShowDialog(); // opens the form and makes the code stop until the form is closed

                if (form.done)
                {
                    machine.AddProduct(new Product(form.Code, form.Description, form.Price, form.Quantity));
                    MainDisplay.Text = "Product added successfully";
                    UpdateProducts();
                }
                else
                    MainDisplay.Text = "Product was not added";
                
            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

        /// <summary>
        /// Modifies the price of a given item
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ModifyPrices_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                ModifyProductForm form = new ModifyProductForm();
                form.AddProducts(machine.products);
                form.ShowDialog(); // opens the form and makes the code stop until the form is closed
                if (form.worked)
                {
                    string[] list = form.ChangePrice(out double price).Split(' '); 

                    foreach (Product product in machine.GetProductTypes())
                    {
                        if (product.Description == list[2]) // list[2] is the description of the product
                        {
                            product.Price = price;
                        }
                    }
                    MainDisplay.Text = "Price changed successfully";
                }
                else
                {
                    MainDisplay.Text = "Price was not changed";
                } 

            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

        /// <summary>
        /// Empties the bank of the vending machine
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void EmptyCoinbox_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                MainDisplay.Text = "Coin box emptied. Money earned: "+ machine.coins.GetValue().ToString("C");
                machine.coins.RemoveAllCoins();
            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

        /// <summary>
        /// Restocks the vending machine
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Restock_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                LoadMachine(true);
                LoadChangeBox(true);
                MainDisplay.Text = "Machine has been restocked";
            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

        /// <summary>
        /// Changes the password to access the maintenance mode
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void ChangePassword_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                if (!changePassword)
                {
                    MainDisplay.Text = "Enter new password and press again to confirm: ";
                    changePassword = true;
                }
                else
                {
                    string newPassword = MainDisplay.Text;
                    if (newPassword == machine.Password)
                    {
                        MainDisplay.Text = "New password must be different from the current one";
                    }
                    else if (newPassword.Length > 6)
                    {
                        MainDisplay.Text = "Password cannot be greater than 6 characters";
                    }
                    else
                    {
                        machine.Password = newPassword;
                        MainDisplay.Text = "Password successfully changed";
                    }
                    changePassword = false;
                }
            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

        /// <summary>
        /// Quits the maintenance mode and brings the client back to vending mode
        /// </summary>
        /// <param name="sender">the client of the vending machine</param>
        /// <param name="e">the action</param>
        private void Quit_Click(object sender, EventArgs e)
        {
            if (machine.maintenanceMode)
            {
                machine.maintenanceMode = false;
                MainDisplay.Text = "You exit the maintenance mode";

                maintenanceControlsPanel.Visible = false;
                changePassword = false;
            }
            else
                MainDisplay.Text = "You are not in maintenance mode";
        }

    }
}
