using System.Collections.ObjectModel;

namespace Cluster.Model
{
    public class EachCluster
    {
        public int x, y; //центр кластера

        public Collection<EachPixel> neighbours = new Collection<EachPixel>(); //Список точек в кластере
    }
}
