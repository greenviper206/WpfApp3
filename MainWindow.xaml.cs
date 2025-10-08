using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace _2025_WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        string buy_type = "外帶";
        string result_message = "";

        public MainWindow()
        {
            InitializeComponent();

            //讀取飲料品項drinkitem.csv
            InputDrinkItem(drinks);

            //動態產生飲料選單
            DisplayDrinkMenu(drinks);
        }

        private void DisplayDrinkMenu(Dictionary<string, int> drinks)
        {
            DrinkMenu_StackPanel.Height = drinks.Count * 50;
            foreach (var drink in drinks)
            {
                var sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(2),
                    Background = Brushes.LightBlue,
                    Height = 35
                };

                var cb = new CheckBox
                {
                    Content = drink.Key,
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkBlue,
                    Width = 150,
                    Margin = new Thickness(2),
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                var lb_price = new Label
                {
                    Content = $"{drink.Value} 元",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkRed,
                    Width = 30,
                    Margin = new Thickness(2),
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                var sl = new Slider
                {
                    Value = 0,
                    Minimum = 0,
                    Maximum = 20,
                    Margin = new Thickness(2),
                    Width = 150,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsSnapToTickEnabled = true
                };

                var lb_amount = new Label
                {
                    Content = "0",
                    FontFamily = new FontFamily("微軟正黑體"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkGreen,
                    Width = 30,
                    Margin = new Thickness(2),
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                Binding myBinding = new Binding("Value");
                myBinding.Source = sl;
                lb_amount.SetBinding(Label.ContentProperty, myBinding);

                sp.Children.Add(cb);
                sp.Children.Add(lb_price);
                sp.Children.Add(sl);
                sp.Children.Add(lb_amount);

                DrinkMenu_StackPanel.Children.Add(sp);
            }
        }

        private void InputDrinkItem(Dictionary<string, int> drinks)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "選擇飲料品項檔案";
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(fileName);

                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = int.Parse(tokens[1]);
                    drinks.Add(drinkName, price);
                }
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            orders.Clear();

            for (int i = 0; i < DrinkMenu_StackPanel.Children.Count; i++)
            {
                var sp = DrinkMenu_StackPanel.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var drinkName = cb.Content.ToString();
                var sl = sp.Children[2] as Slider;
                var amount = (int)sl.Value;

                if (cb.IsChecked == true && amount > 0) orders.Add(drinkName, amount);
            }

            int total = 0;
            int index = 0;
            result_message = $"購買方式：{buy_type}\n";
            result_message += "飲料訂單如下：\n";
            foreach (var item in orders)
            {
                string drinkitem = item.Key;
                int amount = item.Value;
                int price = drinks[drinkitem];

                if (amount > 0)
                {
                    index++;
                    int subtotal = price * amount;
                    result_message += $"{index}：{drinkitem} {price}元 x {amount}杯 = {subtotal}元\n";
                    total += subtotal;
                }
            }
            result_message += $"\n總計：{total}元";

            int sellPrice = total;
            string discount_message = "無折扣";
            if (total >= 500)
            {
                sellPrice = (int)(total * 0.8);
                discount_message = "八折";
            }
            else if (total >= 300)
            {
                sellPrice = (int)(total * 0.85);
                discount_message = "八五折";
            }
            else if (total >= 200)
            {
                sellPrice = (int)(total * 0.9);
                discount_message = "九折";
            }
            else discount_message = "未達到折扣條件";
            result_message += $"\n折扣：{discount_message}，售價{sellPrice}元\n";

            Result_TextBlock.Text = result_message;

            //將訂單內容存成一個文字檔
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "儲存訂單內容";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllText(fileName, result_message);
            }
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton targetRadioButton = sender as RadioButton;
            buy_type = targetRadioButton.Content.ToString();
        }
    }
}