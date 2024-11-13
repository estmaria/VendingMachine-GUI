# VendingMachine-GUI
This is GUI simulation of a vending machine built using Object Oriented Programming. When the program is starts it reads from two text files to set up its inventory (Products.txt) and the change box (ChangeBox.txt). It has two modes in which you can do different things:

# Vending Mode
- Look up the price or quantity left of a product: enter the code of the product and press "Enter".
- Insert money: click on any of the money buttons (nickle, dime, quarter, dollar) and it would added. Your courrent amount would be displayed.
- Buy products: first enter the money, then enter the code of the product you want to buy and press "Enter".
- Quit: if you press cancel all the money will be given back.
- Enter maintenance mode: introduce the correct password and press "Maintenance". The password is set by reading a text file.
# Maintenance mode
- Add products: press "Add products", select one of the avaliable codes, choose a description, a price and a quatity and press "Confirm".
- Modify prices: press "Modify prices",  select the product you want to change the price from, choose the new price, and press "Confirm".
- Empty coin box: press "Empty coinbox and all the monney that has been entered to buy products will be empty.
- Restock: when you press "Restock" all products will be restocked as well as the change box. The program reads two text files where is specified the quantity of each product and each coin that you want to be restocked. 
- Change Password: press the "Change password", enter the new password, and press the button again to confirm. This will change the text file where the password is stored.
- Quit: press "Quit" and you will return to the Vending Mode.
