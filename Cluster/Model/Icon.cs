using System;
using System.Drawing;
using System.IO;
using System.Security.Policy;
using System.Windows.Media.Imaging;

namespace Cluster.Model
{
    class Icon
    {
        private readonly BitmapImage iconImage;

        private readonly Converter converter;
        public Icon()
        {
            iconImage = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+@"\Icons\CubIcon32.bmp")); //т.к это статичное изображение вызываем из модели

            converter = new Converter();
        }

        public System.Drawing.Icon CustomIcon(string number) //Добавление номера в иконку
        {

            var BitmapIconImage = converter.BitmapImage2Bitmap(iconImage);  //Для работы нужно перевести в bitmap

            using (Graphics g = Graphics.FromImage(BitmapIconImage))
            {
                g.DrawString(number, new Font("Tahoma", 8), Brushes.White,5,5); 
            }

            return System.Drawing.Icon.FromHandle(BitmapIconImage.GetHicon()); //конвертация из bitmap в icon
        }
        
    }
}
