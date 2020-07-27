using System.Collections.ObjectModel;

namespace Cluster.Model
{
    public class EachPixel
    {
        public int x, y; //координата точки

        public Collection<EachPixel> neighbours = new Collection<EachPixel>(); //Список соседей точки

        public bool IsInCluster = false; //Находится ли точка в кластере
    }
}
