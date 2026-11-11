using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yanbaev_41
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<CartItem> cartItems;
        User currentUser;

        public OrderWindow(List<CartItem> items, User user)
        {
            InitializeComponent();
            currentUser = user;
            if (user != null)
            {
                TBlockClient.Text = "Клиент: " +
                    user.UserSurname + " " +
                    user.UserName + " " +
                    user.UserPatronymic;

                TBlockClient.Visibility = Visibility.Visible;
            }

            cartItems = items;

            OrderListView.ItemsSource = cartItems;
            UpdateTotals();


            var pickupPoints = new Yanbaev_41Entities()
                .PickupPoint.ToList();

            ComboPickupPoint.ItemsSource = pickupPoints.Select(p => new
            {
                p.PickupID,
                Address = p.PickupIndex + ", " +
                          p.PickupCities + ", " +
                          p.PickupStreet + ", д. " +
                          p.PickupHouseNum
            }).ToList();

            if (pickupPoints.Count > 0)
                ComboPickupPoint.SelectedIndex = 0;

        }


        private void RemoveQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            CartItem item = button.DataContext as CartItem;

            if (item == null)
                return;

            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                cartItems.Remove(item);
            }

            OrderListView.Items.Refresh();
            UpdateTotals();
        }


        private void AddQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            CartItem item = button.DataContext as CartItem;

            if (item == null)
                return;

            item.Quantity++;

            OrderListView.Items.Refresh();
            UpdateTotals();
        }


        private void UpdateTotals()
        {
            decimal total = cartItems.Sum(p =>
                p.Product.ProductCost * p.Quantity);

            decimal discount = cartItems.Sum(p =>
                p.Product.ProductCost * p.Quantity *
                p.Product.ProductDiscountAmount / 100m);

            decimal finalTotal = total - discount;

            TBlockTotal.Text = "Сумма без скидки: " +
                               total.ToString("N2") + " руб.";

            TBlockDiscount.Text = "Скидка: " +
                                  discount.ToString("N2") + " руб.";

            TBlockFinal.Text = "Итого: " +
                               finalTotal.ToString("N2") + " руб.";
        }


        private void BtnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            if (ComboPickupPoint.SelectedValue == null)
            {
                MessageBox.Show("Выберите пункт выдачи");
                return;
            }

            bool inStock = cartItems.All(p =>
                p.Product.ProductQuantityInStock > 3 &&
                p.Product.ProductQuantityInStock >= p.Quantity);
            int deliveryDays = inStock ? 3 : 6;

            Order order = new Order
            {
                OrderDate = DateTime.Today,
                OrderDeliveryDate = DateTime.Today.AddDays(deliveryDays),
                OrderPickupPoint = (int)ComboPickupPoint.SelectedValue,
                OrderClient = currentUser == null
                    ? (int?)null : currentUser.UserID,
                OrderCode = new Random().Next(100, 1000),
                OrderStatus = "Новый"
            };

            foreach (CartItem item in cartItems)
            {
                order.OrderProduct.Add(new OrderProduct
                {
                    ProductArticleNumber = item.Product.ProductArticleNumber,
                    OrderQuantity = item.Quantity
                });
            }

            try
            {
                using (var db = new Yanbaev_41Entities())
                {
                    db.Order.Add(order);
                    db.SaveChanges();
                }


                MessageBox.Show(
                    "Заказ №" + order.OrderID +
                    " успешно оформлен!\n" +
                    "Код получения: " + order.OrderCode +
                    "\nСрок доставки: " + deliveryDays +
                    (deliveryDays == 3 ? " дня" : " дней") +
                    "\nДата доставки: " +
                    order.OrderDeliveryDate.ToString("dd.MM.yyyy"));

                cartItems.Clear();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " +
                                ex.GetBaseException().Message);
            }
        }

    }
}
