using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>

    public partial class AddEditPage : Page
    {
        private Product currentProduct = new Product();

        private bool isThisEditingMode;
        private string newImagePath = "";
        public AddEditPage(Product selectedProduct)
        {
            InitializeComponent();

            if (selectedProduct != null)
            {
                currentProduct = selectedProduct;
                ArticleTB.Opacity = 0.5;
                ArticleTB.IsEnabled = false;
                isThisEditingMode = true;

                ManufacturerCB.SelectedValue = currentProduct.ProductManufacturer;
                CategoryCB.SelectedValue = currentProduct.ProductCategory;
            }
            else
                isThisEditingMode = false;

            DataContext = currentProduct;

            List<String> allManufacturers = new List<String>();
            foreach(Product prod in ShoesDE2026Entities.GetContext().Product.ToList())
            {
                allManufacturers.Add(prod.ProductManufacturer.ToString());
            }
            ManufacturerCB.ItemsSource = allManufacturers.Distinct();

            List<String> allCategories = new List<String>();
            foreach (Product prod in ShoesDE2026Entities.GetContext().Product.ToList())
            {
                allCategories.Add(prod.ProductCategory.ToString());
            }
            CategoryCB.ItemsSource = allCategories.Distinct();
        }

        /// /////////////////////////////////////////////////////////
        private string CheckArticle(string article)
        {
            string msg = "";

            article = article.Trim();

            if (article.Length != 6)
                msg = "Длина артикула должна составлять 6 символов";
            else
            {
                var prodWSameArticle = ShoesDE2026Entities.GetContext().Product.ToList().Where(p => p.Article == article);
                if (prodWSameArticle.Count() > 0)
                    msg = "Товар с таким артикулом уже существует";
            }

            return msg;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (!isThisEditingMode)
            {
                string articleErrorMsg = "";

                if (string.IsNullOrWhiteSpace(currentProduct.Article))
                    errors.AppendLine("Введите артикул товара");
                else
                {
                    articleErrorMsg = CheckArticle(currentProduct.Article.Trim());
                    if (articleErrorMsg.Length > 0)
                        errors.AppendLine(articleErrorMsg);
                    /*if (currentProduct.Article.Trim().Length != 6)
                        errors.AppendLine("Длина артикула должна составлять 6 символов");
                    else
                    {
                        Product prodWSameArticle = new Product();
                        prodWSameArticle = ShoesDE2026Entities.GetContext().Product.ToList().Find(p => p.Article == currentProduct.Article);
                        if (prodWSameArticle != null)
                            errors.AppendLine("Товар с таким артикулом уже существует");
                    }*/
                }
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductName))
                errors.AppendLine("Введите наименование товара");

            if (string.IsNullOrWhiteSpace(currentProduct.Unit))
                errors.AppendLine("Введите ед. измерения товара");

            /*           
            if (!int.TryParse(BuildingTB.Text.Trim(), out int building))
                errors.AppendLine("Номер здания должен быть целым числом");
            */

            if (!decimal.TryParse(PriceTB.Text.Trim(), out decimal curProdPrice))
                errors.AppendLine("Цена товара должна быть числом");
            else
            {
                if (currentProduct.ProductPrice <= 0)
                    errors.AppendLine("Цена товара должна быть числом больше 0");
                else
                    currentProduct.ProductPrice = curProdPrice;
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductSupplier))
                errors.AppendLine("Введите наименование поставщика");

            if (ManufacturerCB.SelectedValue == null)
                errors.AppendLine("Выберите наименование производителя");

            if (CategoryCB.SelectedValue == null)
                errors.AppendLine("Выберите категорию товара");

            if (!int.TryParse(DiscountTB.Text.Trim(), out int curProdDiscount))
                errors.AppendLine("Скидка должна быть целым числом");
            else
            {
                if (currentProduct.ProductDiscount < 0)
                    errors.AppendLine("Скидка не может быть меньше 0");
                else currentProduct.ProductDiscount = curProdDiscount;
            }

            if (!int.TryParse(QInStockTB.Text.Trim(), out int curProdQuantity))
                errors.AppendLine("Количество должно быть целым числом");
            else
            {
                if (currentProduct.QuantityInStock < 0)
                    errors.AppendLine("Количество не может быть меньше 0");
                else currentProduct.QuantityInStock = curProdQuantity;
            }

            if (string.IsNullOrWhiteSpace(currentProduct.ProductDescription))
                errors.AppendLine("Введите описание товара");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Обработка изображения
            if (newImagePath != null)
            {
                try
                {
                    // Удаляем старое изображение, если оно есть и мы в режиме редактирования
                    if (isThisEditingMode && !string.IsNullOrEmpty(currentProduct.ProductPhoto))
                    {
                        DeleteOldImage(currentProduct.ProductPhoto);
                    }

                    // Сохраняем новое изображение
                    string newFileName = SaveAndResizeImage(newImagePath);
                    currentProduct.ProductPhoto = newFileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке изображения: {ex.Message}");
                    return;
                }
            }


            if (!isThisEditingMode)
                ShoesDE2026Entities.GetContext().Product.Add(currentProduct);

            try
            {
                ShoesDE2026Entities.GetContext().SaveChanges();
                MessageBox.Show("Изменения сохранены");
                Manager.MainFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ChangePhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Проверяем размер изображения
                    BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
                    if (image.PixelWidth > 300 || image.PixelHeight > 200)
                    {
                        MessageBox.Show("Размер изображения должен быть не более 300x200 пикселей. Изображение будет автоматически уменьшено.");
                    }

                    // Временно отображаем выбранное изображение
                    PhotoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    newImagePath = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                }
            }
        }

        private string SaveAndResizeImage(string sourcePath)
        {
            string imagesFolder = GetImagesFolder();

            // Создаем папку, если она не существует
            /*if (!System.IO.Directory.Exists(imagesFolder))
            {
                System.IO.Directory.CreateDirectory(imagesFolder);
            }*/

            // Генерируем уникальное имя файла на основе артикула
            string extension = System.IO.Path.GetExtension(sourcePath).ToLower();
            string fileName = $"{currentProduct.Article}{extension}";
            string fullPath = System.IO.Path.Combine(imagesFolder, fileName);

            // Если файл с таким именем уже существует, добавляем суффикс
            int counter = 1;
            while (System.IO.File.Exists(fullPath))
            {
                fileName = $"{currentProduct.Article}_{counter}{extension}";
                fullPath = System.IO.Path.Combine(imagesFolder, fileName);
                counter++;
            }

            // Загружаем и изменяем размер изображения
            BitmapImage originalImage = new BitmapImage(new Uri(sourcePath));

            // Вычисляем новые размеры с сохранением пропорций
            double scaleX = 300.0 / originalImage.PixelWidth;
            double scaleY = 200.0 / originalImage.PixelHeight;
            double scale = Math.Min(scaleX, scaleY);

            int newWidth = (int)(originalImage.PixelWidth * scale);
            int newHeight = (int)(originalImage.PixelHeight * scale);

            // Создаем TransformedBitmap для изменения размера
            TransformedBitmap transformedBitmap = new TransformedBitmap();
            transformedBitmap.BeginInit();
            transformedBitmap.Source = originalImage;
            transformedBitmap.Transform = new ScaleTransform(scale, scale);
            transformedBitmap.EndInit();

            // Сохраняем изображение в формате JPEG для оптимального размера
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(transformedBitmap));

            using (System.IO.FileStream stream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
            {
                encoder.Save(stream);
            }

            return fileName;
        }

        private void DeleteOldImage(string oldFileName)
        {
            if (!string.IsNullOrEmpty(oldFileName))
            {
                try
                {
                    string imagesFolder = GetImagesFolder();
                    string oldFilePath = System.IO.Path.Combine(imagesFolder, oldFileName);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, но не прерываем выполнение
                    System.Diagnostics.Debug.WriteLine($"Ошибка при удалении старого изображения: {ex.Message}");
                }
            }
        }

        private string GetImagesFolder()
        {
            // Жестко заданный путь к папке с изображениями
            return @"E:\VS Projects\4 курс\Shoes\Shoes\img\";
        }

    }
}
