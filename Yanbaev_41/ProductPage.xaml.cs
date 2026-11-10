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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        List<Product> allProducts;

        public ProductPage()
        {
            InitializeComponent();
            allProducts = new Yanbaev_41Entities().Product.ToList();
            UpdateProducts();
        }


        private void UpdateProducts()
        {
            var currentProducts = allProducts.ToList();

            // Поиск по наименованию
            currentProducts = currentProducts.Where(p =>
                p.ProductName.ToLower().Contains(
                    TBoxSearch.Text.ToLower())).ToList();

            if (ComboType.SelectedIndex == 1)
            {
                currentProducts = currentProducts
                    .Where(p => p.ProductDiscountAmount < 10).ToList();
            }

            if (ComboType.SelectedIndex == 2)
            {
                currentProducts = currentProducts
                    .Where(p => p.ProductDiscountAmount >= 10 &&
                                p.ProductDiscountAmount < 15).ToList();
            }

            if (ComboType.SelectedIndex == 3)
            {
                currentProducts = currentProducts
                    .Where(p => p.ProductDiscountAmount >= 15).ToList();
            }

            if (RButtonUp.IsChecked == true)
            {
                currentProducts = currentProducts
                    .OrderBy(p => p.ProductCost).ToList();
            }

            if (RButtonDown.IsChecked == true)
            {
                currentProducts = currentProducts
                    .OrderByDescending(p => p.ProductCost).ToList();
            }

            ProductListView.ItemsSource = currentProducts;

            TBlockCount.Text = currentProducts.Count +
                               " из " + allProducts.Count;
        }


        private void TBoxSearch_TextChanged(object sender,
            TextChangedEventArgs e)
        {
            if (allProducts != null)
                UpdateProducts();
        }

        private void Sort_Checked(object sender, RoutedEventArgs e)
        {
            if (allProducts != null)
                UpdateProducts();
        }

        private void ComboType_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (allProducts != null)
                UpdateProducts();
        }


        public ProductPage(User user) : this()
        {
            if (user == null)
                return;

            TBlockAuthLabel.Text = "Вы авторизованы как: ";

            FIOTB.Text = user.UserSurname + " " +
                         user.UserName + " " +
                         user.UserPatronymic;

            switch (user.UserRole)
            {
                case 1:
                    RoleTB.Text = "Клиент";
                    break;

                case 2:
                    RoleTB.Text = "Менеджер";
                    break;

                case 3:
                    RoleTB.Text = "Администратор";
                    break;

                default:
                    RoleTB.Text = "Неизвестная роль";
                    break;
            }
        }


    }

}
