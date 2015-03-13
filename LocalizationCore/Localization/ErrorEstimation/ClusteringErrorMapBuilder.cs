using LocalizationCore.Localization.Map;
using LocalizationCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Localization.ErrorEstimation
{
    public class ClusteringErrorMapBuilder : BaseErrorMapBuilder
    {
        RadioMap Map { get; set; }

        public ClusteringErrorMapBuilder(RadioMap map)
        {
            Map = map;
        }

        public override void BuildAccuracyMap()
        {
            var clusters = ToClusterList(Map);
            bool nothingToMerge = false;    // assumtion

            while (!nothingToMerge)
            {
                nothingToMerge = true;
                Cluster cluster1 = null;
                Cluster cluster2 = null;

                foreach (var cl1 in clusters)
                    foreach (var cl2 in clusters)
                        if (cl1 != cl2 && Cluster.IsNeighbours(cl1, cl2) && Cluster.IsSimilar(cl1, cl2))
                        {
                            cluster1 = cl1;
                            cluster2 = cl2;
                            nothingToMerge = false;
                        }

                if (!nothingToMerge)
                    MergeClusters(clusters, cluster1, cluster2);
            }

            MergeUniLocations(clusters);
            CalculateAccuracy(clusters, LocationMap.Instance.LocationSize);
        }

        private void MergeUniLocations(List<Cluster> clusters)
        {
            bool exist = true;     // suppose
            while (exist)
            {
                exist = false;
                var single = clusters.FirstOrDefault(c => c.LocationCount == 1);
                if (single != null)
                {
                    exist = true;
                    var neighbour = clusters.First(c => c != single && Cluster.IsNeighbours(c, single));
                    MergeClusters(clusters, single, neighbour);
                }
            }
        }

        private void CalculateAccuracy(IEnumerable<Cluster> clusters, double locationSize)
        {
            foreach (var c in clusters)
            {
                var clusterAccuracy = locationSize * Math.Sqrt(c.LocationCount) / 2;
                foreach (var l in c)
                    l.Accuracy = new Coord(clusterAccuracy, clusterAccuracy);
            }
        }

        private static void MergeClusters(List<Cluster> clusters, Cluster cluster1, Cluster cluster2)
        {
            clusters.Remove(cluster1);
            clusters.Remove(cluster2);
            var newCluster = Cluster.Merge(cluster1, cluster2);
            clusters.Add(newCluster);
        }

        private List<Cluster> ToClusterList(RadioMap map)
        {
            return map.Select(f =>
            {
                var res = new Cluster();
                res.Add(f.LocationLink, f);
                return res;
            }).ToList();
        }
    }
}
