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

namespace Shoes
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();

            var currentProducts = ShoesDE2026Entities.GetContext().Product.ToList();

            ProductListView.ItemsSource = currentProducts;

            //SearchEtcStackPanel.Visibility

            //выбор поставщиков и удаление дубликатов
            List<String> SupplierCBItems = new List<string>(); //динамический массив строк
            SupplierCBItems.Add("Все поставщики");
            var allSuppliers = currentProducts.Select(p => p.ProductSupplier).Distinct().ToList(); //выбор поставщиков из бд без дубликатов
            foreach (string supplier in allSuppliers)
            {
                SupplierCBItems.Add(supplier);
            }
            //SupplierCB.ItemsSource = currentProducts.Select(p => p.ProductSupplier).Distinct().ToList();
            SupplierCB.ItemsSource = SupplierCBItems;

            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var currentProducts = ShoesDE2026Entities.GetContext().Product.ToList();

            /// фильтрация
            /// поиск
            /// сортировка
            /// загрузка списка в листвью


            //фильтрация
            if (SupplierCB.SelectedValue != null)
            {
                switch (SupplierCB.SelectedValue.ToString())
                {
                    case "Все поставщики":
                        break;

                    default:
                        currentProducts = currentProducts.Where(p => p.ProductSupplier == SupplierCB.SelectedValue.ToString()).ToList();
                        break;
                }
            }

            //поиск
            currentProducts = currentProducts.Where(p => p.StringInfoCombined.ToLower().Contains(SearchTB.Text.ToLower())).ToList();

            //сортировка
            if (SortAscRB.IsChecked == true)
                currentProducts = currentProducts.OrderBy(p => p.QuantityInStock).ToList();
            if (SortDescRB.IsChecked == true)
                currentProducts = currentProducts.OrderByDescending(p => p.QuantityInStock).ToList();


            ProductListView.ItemsSource = currentProducts;
        }

        private void SearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SupplierCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortDescRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortAscRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }
    }
}
