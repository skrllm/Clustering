using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Cluster.Model
{
    class ClusterModel 
    {
        public BitmapImage OriginalImage { get; set; }  //Оригинал, без изменений
        public BitmapImage Image { get; set; } //Изменяемое изображение
        public int MaxDistance { get; set; }
        public Button ImageButton { get; set; }
        public Button PixelsButton { get; set; }
        public Button ClusteringButton { get; set; }

        public Collection<EachCluster> clustering (List<EachPixel> pixels, int MaxDistance)
        {

            Collection<EachCluster> clusters = new Collection<EachCluster>();


            for (int i = 0; i < pixels.Count; i++) //поиск ближайщих соседей расстоянием "r"
            {
                for (int j = 0; j < pixels.Count; j++)
                {
                   
                    double r = DistanceBetweenPixels(pixels[i].x,pixels[j].x,pixels[i].y,pixels[j].y); //Вычисление расстояния

                    if ((r <= MaxDistance) && (i!=j)) { pixels[i].neighbours.Add(pixels[j]); } //Если расстояние меньше максимального расстояния и это не одна и та же точка
                }
            }


            pixels = pixels.OrderBy(o => o.neighbours.Count).ToList(); //сортировка коллекции пикселей по кол-ву соседей по возрастанию


            for (int i = pixels.Count - 1; i >= 0; i--) //перебор пикселей с конца по убыванию соседей
            {
                if (pixels[i].IsInCluster == false) //если точка отсчета не принадлежит кластеру
                {
                    clusters.Add(new EachCluster());

                    pixels[i].IsInCluster = true;

                    clusters[clusters.Count - 1].neighbours.Add(pixels[i]); //Добавляем в кластер точку отсчета
                    

                    if (pixels[i].neighbours.Count > 0) //если у точки есть соседи
                    {
                        for (int j = 0; j < pixels[i].neighbours.Count; j++) //перебор соседей точки
                        {
                            if (pixels[i].neighbours[j].IsInCluster == false) //если сосоед не принадлежит любому кластеру
                            {
                                clusters[clusters.Count - 1].neighbours.Add(pixels[i].neighbours[j]); //добавляем точку в кластер

                                pixels[i].neighbours[j].IsInCluster = true;
                            }
                        }
                    }
                    else //Если нет соседей, то это одиночный кластер с центром в единственной точке
                    {
                        clusters[clusters.Count - 1].x = pixels[i].x;
                        clusters[clusters.Count - 1].y = pixels[i].y;
                    }
                }
            }
            foreach (EachCluster cluster in clusters) //Поиск центра кластера
            {
                double min = double.MaxValue; //Поиск центра кластера по минимальному суммарному расстоянию до каждого соседа в кластере

                if (cluster.neighbours.Count > 1) //Если кластер не одиночный
                {
                    for (int i = 0; i < cluster.neighbours.Count; i++) //перебор каждого кластера
                    {
                        double r = 0; //Расстояние между точками

                        for (int j = 0; j < cluster.neighbours.Count; j++) //Поиск расстояния между точками
                        {
                            if (i != j) //Если это не одна и та же точка
                            {
                                r += DistanceBetweenPixels(cluster.neighbours[j].x, cluster.neighbours[i].x, cluster.neighbours[j].y, cluster.neighbours[i].y);

                            }
                        }
                        if ((r < min) && (r != 0))
                        {
                            cluster.x = cluster.neighbours[i].x; //Присвоение центра кластера
                            cluster.y = cluster.neighbours[i].y;
                            min = r; //новое миниммальное расстояние
                        }
                    }
                }              
            }

            return clusters;
        }
        public double DistanceBetweenPixels(int x1, int x2, int y1, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

    }
}
