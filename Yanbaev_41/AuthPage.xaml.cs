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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {

        Random random = new Random();
        string captchaCode = "";

        public AuthPage()
        {
            InitializeComponent();
        }



        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Pass.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }


            if (CaptchaInput.Text.Trim().ToUpper() != captchaCode)
            {
                MessageBox.Show("Неверный код CAPTCHA");

                GenerateCaptcha();

                LoginButton.IsEnabled = false;
                LoginButton.Content = "Подождите...";

                await Task.Delay(10000);

                LoginButton.IsEnabled = true;
                LoginButton.Content = "Войти";

                return;
            }


            User user = new Yanbaev_41Entities().User
                .FirstOrDefault(p => p.UserLogin == login &&
                                     p.UserPassword == password);

            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
            }
            else
            {
                MessageBox.Show("Введены неверные данные");

                CaptchaPanel.Visibility = Visibility.Visible;
                GenerateCaptcha();
            }
        }

        private void GenerateCaptcha()
        {
            string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            captchaCode = "";

            for (int i = 0; i < 4; i++)
            {
                captchaCode += symbols[random.Next(symbols.Length)];
            }

            CaptchaOne.Text = captchaCode[0].ToString();
            CaptchaTwo.Text = captchaCode[1].ToString();
            CaptchaThree.Text = captchaCode[2].ToString();
            CaptchaFour.Text = captchaCode[3].ToString();

            CaptchaInput.Text = "";
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Login.Text = "";
            Pass.Text = "";
            CaptchaInput.Text = "";

            CaptchaPanel.Visibility = Visibility.Collapsed;

            CaptchaOne.Text = "";
            CaptchaTwo.Text = "";
            CaptchaThree.Text = "";
            CaptchaFour.Text = "";

            captchaCode = "";
        }


        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage());
        }

    }
}
