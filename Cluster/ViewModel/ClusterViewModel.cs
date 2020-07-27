using System.Drawing;
using System;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using Cluster.Model;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Cluster.ViewModel
{
    public class ClusterViewModel : ViewModel
    {
        private readonly ClusterModel data = new ClusterModel();

        OpenFileDialog PixelsPath;

        readonly Converter converter = new Converter();

        public int MaxDistance  //работа с MaxDistance
        {
            get { return data.MaxDistance; }
            set
            {
                if (data.MaxDistance != value)
                {
                    data.MaxDistance = value;
                    OnPropertyChange();
                }
            }
        }
        public BitmapImage Image  //работа с Image
        {
            get { return data.Image; }

        }
        public string ImageButtonText  //биндинг текстов кнопок
        {
            get { return Convert.ToString(data.ImageButton.Content); }
        }
        public string PixelsButtonText  
        {
            get { return Convert.ToString(data.PixelsButton.Content); }
        }
        public bool ImageButtonIsEnable  //Биндинг состояния кнопок
        {
            get { return data.ImageButton.IsEnabled; }
        }
        public bool PixelsButtonIsEnable  
        {
            get { return data.PixelsButton.IsEnabled; }
        }
        public bool ClusteringButtonIsEnable
        {
            get { return data.ClusteringButton.IsEnabled; }
        }

        public ICommand ClusteringCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }
        public ICommand UploadPixelsCommand { get; set; }

        public ClusterViewModel()
        {
            ClusteringCommand = new Command(ClusteringMethod, canExecuteMethod);
            UploadImageCommand = new Command(UploadImageMethod, canExecuteMethod);
            UploadPixelsCommand = new Command(UploadPixelsMethod, canExecuteMethod);

            data.PixelsButton = new Button();
            data.ImageButton = new Button();
            data.ClusteringButton = new Button();

            data.PixelsButton.IsEnabled = false; //Кнопки изначально выключены и после загрузки файлов включаются
            data.ClusteringButton.IsEnabled = false;

            data.MaxDistance = 50; //изначальная степень "Близости" в пикселях 
          
            data.ImageButton.Content = "Выбрать изображение";
            data.PixelsButton.Content = "Выбрать список точек";

        }
        private void UploadImageMethod(object parameter) //Загрузка изображения
        {
            var ImagePath = new OpenFileDialog
            {
                Filter = "Image Files(*.jpg; *.jpeg;)|*.jpg; *.jpeg"
            };
            ImagePath.ShowDialog();

            try
            {
                data.OriginalImage = new BitmapImage(new Uri(ImagePath.FileName));
                data.Image = data.OriginalImage;
                data.ImageButton.Content = Path.GetFileName(ImagePath.FileName);
                data.PixelsButton.IsEnabled = true;
                OnPropertyChange();
            }
            catch { }
        }
        private void UploadPixelsMethod(object parameter) //Загрузка Координат точек
        {
            PixelsPath = new OpenFileDialog
            {
                Filter = "Image Files(*.txt;)|*.txt"
            };
            PixelsPath.ShowDialog();

           if (PixelsPath.FileName != "")
           {
                data.PixelsButton.Content = Path.GetFileName(PixelsPath.FileName);
                data.ClusteringButton.IsEnabled = true;
                OnPropertyChange();
           }        

        }
        private void ClusteringMethod(object parameter)
        {
            data.Image = data.OriginalImage;

            List<EachPixel> pixels = new List<EachPixel>(); //Коллекция пикселей

            string PatternMatches = @"\d+";    // Создаем шаблон для координат точек

            Regex rg = new Regex(PatternMatches);

            foreach (var line in File.ReadLines(PixelsPath.FileName)) //
            {
                MatchCollection matchedline = rg.Matches(line);    // Получаем все совпадения слов и символов
                pixels.Add(new EachPixel());
                pixels[pixels.Count - 1].x = Convert.ToInt32(matchedline[0].Value);
                pixels[pixels.Count - 1].y = Convert.ToInt32(matchedline[1].Value);
            }       


            Collection<EachCluster> clusters = data.clustering(pixels, data.MaxDistance); //Кластеризация точек

            Bitmap newImage = converter.BitmapImage2Bitmap(data.Image); //Конвертация Изображения в Bitmap для работы

            Random rnd = new Random();


            foreach (EachCluster cluster in clusters) //Обработка каждого кластера
            {
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)); //рандомный цвет каждого кластера

                for (int i = 0; i < cluster.neighbours.Count; i++) //Каждый пиксель в кластере покрасить в один цвет 
                {

                    newImage.SetPixel(cluster.neighbours[i].x, cluster.neighbours[i].y, randomColor);

                }

                Model.Icon model = new Model.Icon();

                using (Graphics grf = Graphics.FromImage(newImage)) //Рисование Иконки для кластера
                {
                    //Смещение иконки на 16 (Размер Иконки 32) для оцентровки 
                    grf.DrawIcon(model.CustomIcon(Convert.ToString(cluster.neighbours.Count)), cluster.x - 16, cluster.y - 16); 

                }

                data.Image = converter.Bitmap2BitmapImage(newImage);

                OnPropertyChange();
            }

        }

    }
}

    

