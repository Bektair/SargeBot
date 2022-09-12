using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Intel;
//https://kunuk.wordpress.com/2011/09/20/markerclusterer-with-c-example-and-html-canvas-part-3/

public class AlgorithmClusterBundle
{
    public static string GetId(int idx, int idy) //O(1)
    {
        return idx + ";" + idy;
    }

    public static DateTime Starttime;
    readonly List<XY> _points = new List<XY>(); //Give the ClusterAlgorithm this
    public List<XY> Clusters = new List<XY>();
    public const int MARKERCLUSTERER_SIZE = 20;
    public const bool UseProfiling = false; //debug, output time spend

    //draw
    public const double MinX = 10;
    public const double MinY = 10;
    public const double MaxX = 400;
    public const double MaxY = 300;
    public static readonly Random Rand = new Random();
    static private readonly string NL = Environment.NewLine;
    static readonly CultureInfo Culture_enUS = new CultureInfo("en-US");
    const string S = "G";
    private const string ctx = "ctx"; // javascript canvas
    public const string FolderPath = @"c:\temp\";

    public MarkerClusterer markClusterResult;

    public AlgorithmClusterBundle(List<XY> points)
    {
        Starttime = DateTime.Now;
        _points = points;
        markClusterResult = new MarkerClusterer(_points);
        markClusterResult.GetCluster();

        GenerateJavascriptDrawFile();
    }

    public static string GetRandomColor()
    {
        int r = Rand.Next(10, 250);
        int g = Rand.Next(10, 250);
        int b = Rand.Next(10, 250);
        return string.Format("'rgb({0},{1},{2})'", r, g, b);
    }

    public static string ToStringEN(double d)
    {
        double rounded = Math.Round(d, 3);
        return rounded.ToString(S, Culture_enUS);
    }

    public void GenerateJavascriptDrawFile()
    {
        var sb = new StringBuilder();

        // markers
        var head = "function drawMarkers(ctx) {" + NL;
        var tail = NL + "}" + NL;

        sb.Append(head);

        // if to many points, the canvas can not be drawn or is slow, 
        // use max points and clusters for drawing
        const int max = 10000;

        // markers
        if (_points != null)
        {
            sb.Append(NL);
            for (int i = 0; i < _points.Count; i++)
            {
                var p = _points[i];
                sb.Append(string.Format("drawMark({0}, {1}, {2}, {3});{4}",
                          ToStringEN(p.X), ToStringEN(p.Y), p.Color, ctx, NL));
                if (i > max)
                    break;
            }
        }
        string clusterInfo = "0";
        if (Clusters != null)
        {
            sb.Append(NL);
            for (int i = 0; i < Clusters.Count; i++)
            {
                var c = Clusters[i];
                sb.Append(string.Format(
                    "drawMarkerCluster({0}, {1}, {2}, {3}, {4}, {5});{6}",
                                        ToStringEN(c.X), ToStringEN(c.Y), c.Color,
                                        c.Size, MARKERCLUSTERER_SIZE, ctx, NL));

                if (i > max)
                    break;
            }
            clusterInfo = Clusters.Count + string.Empty;
        }
        sb.Append("ctx.fillStyle = 'rgb(0,0,0)';" + NL);
        sb.Append(string.Format("ctx.fillText('Clusters = ' + {0}, {1}, {2});{3}",
            clusterInfo, ToStringEN(MinX + 10), ToStringEN(MaxY + 20), NL));

        sb.Append(tail);
        CreateFile(sb.ToString());
       // Console.WriteLine(sb.ToString());

    }

    static void CreateFile(string s)
    {
        var path = new FileInfo(FolderPath + "draw.js");
        var isCreated = FileUtil.WriteFile(s, path);
        Console.WriteLine(isCreated + " = File is Created");
    }


    public class MarkerClusterer : BaseClusterAlgorithm
    {



        public MarkerClusterer(List<XY> dataset) : base(dataset)
        {
        }

        public override List<XY> GetCluster()
        {
            var cluster = RunClusterAlgo();
            return cluster;
        }

        // O(k*n)
        List<XY> RunClusterAlgo()
        {
            // put points in buckets     
            int allPointsCount = BaseDataset.Count;
            var firstPoint = BaseDataset[0];
            firstPoint.Color = GetRandomColor();
            var firstId = 0.ToString();
            var firstBucket = new Bucket(firstId) { Centroid = firstPoint };
            BaseBucketsLookup.Add(firstId, firstBucket);

            for (int i = 1; i < allPointsCount; i++)
            {
                var set = new HashSet<string>(); //cluster candidate list
                var p = BaseDataset[i];
                // iterate clusters and collect candidates
                foreach (var bucket in BaseBucketsLookup.Values)
                {
                    var isInCluster = MathTool.BoxWithin(p, bucket.Centroid, MARKERCLUSTERER_SIZE);
                    if (!isInCluster)
                        continue;

                    set.Add(bucket.Id);
                    //use first, short dist will be calc at last step before returning data
                    break;
                }

                // if not within box area, then make new cluster   
                if (set.Count == 0)
                {
                    var pid = i.ToString();
                    p.Color = GetRandomColor();
                    var newbucket = new Bucket(pid) { Centroid = p };
                    BaseBucketsLookup.Add(pid, newbucket);
                }
            }

            //important, align all points to closest cluster point
            BaseUpdatePointsByCentroid();

            return BaseGetClusterResult();
        }
    }


    public abstract class BaseClusterAlgorithm
    {
        public List<XY> BaseDataset; // all points
                                     //id, bucket
        public readonly Dictionary<string, Bucket> BaseBucketsLookup =
            new Dictionary<string, Bucket>();

        public BaseClusterAlgorithm() { }
        public BaseClusterAlgorithm(List<XY> dataset)
        {
            if (dataset == null || dataset.Count == 0)
                throw new ApplicationException(
                    string.Format("dataset is null or empty"));

            BaseDataset = dataset;
        }

        public abstract List<XY> GetCluster();


        //O(k?? random fn can be slow, but is not slow because atm the k is always 1)
        public static XY[] BaseGetRandomCentroids(List<XY> list, int k)
        {
            Profile("BaseGetRandomCentroids beg");
            var set = new HashSet<XY>();
            int i = 0;
            var kcentroids = new XY[k];

            int MAX = list.Count;
            while (MAX >= k)
            {
                int index = Rand.Next(0, MAX - 1);
                var xy = list[index];
                if (set.Contains(xy))
                    continue;

                set.Add(xy);
                kcentroids[i++] = new XY(xy.X, xy.Y) { Color = GetRandomColor() };

                if (i >= k)
                    break;
            }
            return kcentroids;
        }

        public List<XY> BaseGetClusterResult()
        {
            Profile("BaseGetClusterResult beg");

            // collect used buckets and return the result
            var clusterPoints = new List<XY>();
            foreach (var item in BaseBucketsLookup)
            {
                var bucket = item.Value;
                if (bucket.IsUsed)
                {
                    bucket.Centroid.Size = bucket.Points.Count;
                    clusterPoints.Add(bucket.Centroid);
                }
            }

            return clusterPoints;
        }
        public static XY BaseGetCentroidFromCluster(List<XY> list) //O(n)
        {
            Profile("BaseGetCentroidFromCluster beg");
            int count = list.Count;
            if (list == null || count == 0)
                return null;

            // color is set for the points and the cluster point here
            var color = GetRandomColor(); //O(1)
            XY centroid = new XY(0, 0) { Size = list.Count };//O(1)
            foreach (XY p in list)
            {
                p.Color = color;
                centroid.X += p.X;
                centroid.Y += p.Y;
            }
            centroid.X /= count;
            centroid.Y /= count;
            var cp = new XY(centroid.X, centroid.Y) { Size = count, Color = color };

            return cp;
        }
        //O(k*n)
        public static void BaseSetCentroidForAllBuckets(IEnumerable<Bucket> buckets)
        {
            Profile("BaseSetCentroidForAllBuckets beg");
            foreach (var item in buckets)
            {
                var bucketPoints = item.Points;
                var cp = BaseGetCentroidFromCluster(bucketPoints);
                item.Centroid = cp;
            }
        }
        public double BaseGetTotalError()//O(k)
        {
            int centroidsUsed = BaseBucketsLookup.Values.Count(b => b.IsUsed);
            double sum = BaseBucketsLookup.Values.
                Where(b => b.IsUsed).Sum(b => b.ErrorLevel);
            return sum / centroidsUsed;
        }
        public string BaseGetMaxError() //O(k)
        {
            double maxError = -double.MaxValue;
            string id = string.Empty;
            foreach (var b in BaseBucketsLookup.Values)
            {
                if (!b.IsUsed || b.ErrorLevel <= maxError)
                    continue;

                maxError = b.ErrorLevel;
                id = b.Id;
            }
            return id;
        }
        public XY BaseGetClosestPoint(XY from, List<XY> list) //O(n)
        {
            double min = double.MaxValue;
            XY closests = null;
            foreach (var p in list)
            {
                var d = MathTool.Distance(from, p);
                if (d >= min)
                    continue;

                // update
                min = d;
                closests = p;
            }
            return closests;
        }
        public XY BaseGetLongestPoint(XY from, List<XY> list) //O(n)
        {
            double max = -double.MaxValue;
            XY longest = null;
            foreach (var p in list)
            {
                var d = MathTool.Distance(from, p);
                if (d <= max)
                    continue;

                // update
                max = d;
                longest = p;
            }
            return longest;
        }
        // assign all points to nearest cluster
        public void BaseUpdatePointsByCentroid()//O(n*k)
        {
            Profile("UpdatePointsByCentroid beg");
            int count = BaseBucketsLookup.Count();

            // clear points in the buckets, they will be re-inserted
            foreach (var bucket in BaseBucketsLookup.Values)
                bucket.Points.Clear();

            foreach (XY p in BaseDataset)
            {
                double minDist = Double.MaxValue;
                string index = string.Empty;
                //for (int i = 0; i < count; i++)
                foreach (var i in BaseBucketsLookup.Keys)
                {
                    var bucket = BaseBucketsLookup[i];
                    if (bucket.IsUsed == false)
                        continue;

                    var centroid = bucket.Centroid;
                    var dist = MathTool.Distance(p, centroid);
                    if (dist < minDist)
                    {
                        // update
                        minDist = dist;
                        index = i;
                    }
                }
                //update color for point to match centroid and re-insert
                var closestBucket = BaseBucketsLookup[index];
                p.Color = closestBucket.Centroid.Color;
                closestBucket.Points.Add(p);
            }
        }

        // update centroid location to nearest point, 
        // e.g. if you want to show cluster point on a real existing point area
        //O(n)
        public void BaseUpdateCentroidToNearestContainingPoint(Bucket bucket)
        {
            Profile("BaseUpdateCentroidToNearestContainingPoint beg");
            if (bucket == null || bucket.Centroid == null ||
                bucket.Points == null || bucket.Points.Count == 0)
                return;

            var closest = BaseGetClosestPoint(bucket.Centroid, bucket.Points);
            bucket.Centroid.X = closest.X;
            bucket.Centroid.Y = closest.Y;
        }
        //O(k*n)
        public void BaseUpdateAllCentroidsToNearestContainingPoint()
        {
            Profile("BaseUpdateAllCentroidsToNearestContainingPoint beg");
            foreach (var bucket in BaseBucketsLookup.Values)
                BaseUpdateCentroidToNearestContainingPoint(bucket);
        }
    }

    public class MathTool
    {
        private const double Exp = 2; // 2=euclid, 1=manhatten
                                      //Minkowski dist        
        public static double Distance(XY a, XY b)
        {
            return Math.Pow(Math.Pow(Math.Abs(a.X - b.X), Exp) +
                Math.Pow(Math.Abs(a.Y - b.Y), Exp), 1.0 / Exp);
        }

        public static double Min(double a, double b)
        {
            return a <= b ? a : b;
        }
        public static double Max(double a, double b)
        {
            return a >= b ? a : b;
        }

        public static bool DistWithin(XY a, XY b, double d)
        {
            var dist = Distance(a, b);
            return dist < d;
        }

        public static bool BoxWithin(XY a, XY b, double boxsize)
        {
            var d = boxsize / 2;
            var withinX = a.X - d <= b.X && a.X + d >= b.X;
            var withinY = a.Y - d <= b.Y && a.Y + d >= b.Y;
            return withinX && withinY;
        }
    }

    public static void Profile(string s)//O(1)
    {
        if (!UseProfiling)
            return;
        var timespend = DateTime.Now.Subtract(Starttime).TotalSeconds;
        Console.WriteLine(timespend + " sec. " + s);
    }

    public static class FileUtil
    {
        public static bool WriteFile(string data, FileInfo fileInfo)
        {
            bool isSuccess = false;
            try
            {
                using (StreamWriter streamWriter =
                    File.CreateText(fileInfo.FullName))
                {
                    streamWriter.Write(data);
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + "\nPress a key ... ");
                Console.ReadKey();
            }
            return isSuccess;
        }
    }

    [Serializable()]
    public class XY : IComparable, ISerializable
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Color { get; set; }
        public int Size { get; set; }

        //public string XToString(){return X.ToString(S, Culture_enUS);}
        //public string YToString(){return Y.ToString(S, Culture_enUS);}

        public XY() { }
        public XY(double x, double y)
        {
            X = x;
            Y = y;
            Color = "'rgb(0,0,0)'";//default            
        }
        public XY(XY p) //clone
        {
            this.X = p.X;
            this.Y = p.Y;
            this.Color = p.Color;
            this.Size = p.Size;
        }

        public int CompareTo(object o) // if used in sorted list
        {
            if (this.Equals(o))
                return 0;

            var other = (XY)o;
            if (this.X > other.X)
                return -1;
            if (this.X < other.X)
                return 1;

            return 0;
        }

        // used by k-means random distinct selection of cluster point
        public override int GetHashCode()
        {
            var x = X * 10000; //make the decimals be important
            var y = Y * 10000;
            var r = x * 17 + y * 37;
            return (int)r;
        }
        private const int ROUND = 6;
        public override bool Equals(Object o)
        {
            if (o == null)
                return false;
            var other = o as XY;
            if (other == null)
                return false;

            // rounding could be skipped
            // depends on granularity of wanted decimal precision
            // note, 2 points with same x,y is regarded as being equal
            var x = Math.Round(this.X, ROUND) == Math.Round(other.X, ROUND);
            var y = Math.Round(this.Y, ROUND) == Math.Round(other.Y, ROUND);
            return x && y;
        }


        public XY(SerializationInfo info, StreamingContext ctxt)
        {
            this.X = (double)info.GetValue("X", typeof(double));
            this.Y = (double)info.GetValue("Y", typeof(double));
            this.Color = (string)info.GetValue("Color", typeof(string));
            this.Size = (int)info.GetValue("Size", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("Color", this.Color);
            info.AddValue("Size", this.Size);
        }
    }

    public class Bucket
    {
        public string Id { get; private set; }
        public List<XY> Points { get; private set; }
        public XY Centroid { get; set; }
        public int Idx { get; private set; }
        public int Idy { get; private set; }
        public double ErrorLevel { get; set; } // clusterpoint and points avg dist
        private bool _IsUsed;
        public bool IsUsed
        {
            get { return _IsUsed && Centroid != null; }
            set { _IsUsed = value; }
        }
        public Bucket(string id)
        {
            IsUsed = true;
            Centroid = null;
            Points = new List<XY>();
            Id = id;
        }
        public Bucket(int idx, int idy)
        {
            IsUsed = true;
            Centroid = null;
            Points = new List<XY>();
            Idx = idx;
            Idy = idy;
            Id = GetId(idx, idy);
        }
    }

}