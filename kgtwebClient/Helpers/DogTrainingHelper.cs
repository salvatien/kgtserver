using Dogs.ViewModels.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace kgtwebClient.Helpers
{

    public class DogTrainingHelper
    {
        static string url = System.Configuration.ConfigurationManager.AppSettings["ServerBaseUrl"];
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri(url) };

        //private static double earthRadius = 6378.1370D; //in kilometres, D to use double
        //private static double _d2r = (Math.PI / 180D); //D to use double
        private static double oneLongitudeDegreeLengthAtEquatorInKilometers = 111.321;
        private static double oneLatitudeDegreeInKilometers = 110.567;
        public static int CalculateGPSTrackLength(Trkseg track)
        {
            var trackPoints = track.Trkpt;
            double trackLength = 0.0;
            for(int i=0; i<trackPoints.Count -2; i++)
            {
                var lat1 = double.Parse(trackPoints[i].Lat, CultureInfo.InvariantCulture);
                var lon1 = double.Parse(trackPoints[i].Lon, CultureInfo.InvariantCulture);
                var lat2 = double.Parse(trackPoints[i+1].Lat, CultureInfo.InvariantCulture);
                var lon2 = double.Parse(trackPoints[i+1].Lon, CultureInfo.InvariantCulture);
                trackLength += DistanceBetweenCoordinatesInMeters(lat1, lon1, lat2, lon2);
            }
            return (int)trackLength;
        }

        public static TimeSpan CalculateDelayTime(Trkseg dogTrack, Trkseg lostPersonTrack)
        {
            return CalculateGPSTrackStartTime(dogTrack) - CalculateGPSTrackStartTime(lostPersonTrack);
        }

        public static TimeSpan CalculateDuration(Trkseg track)
        {
            var trackPoints = track.Trkpt;
            var startTime =  DateTime.Parse(trackPoints[0].Time);
            var endTime = DateTime.Parse(trackPoints[trackPoints.Count-1].Time);
            return endTime - startTime;
        }

        public static DateTime CalculateGPSTrackStartTime(Trkseg track)
        {
            var trackPoints = track.Trkpt;
            return DateTime.Parse(trackPoints[0].Time);
        }

        ////Haversine is distance between gps coordinates - https://en.wikipedia.org/wiki/Haversine_formula 
        //private static int HaversineInMeters(double lat1, double long1, double lat2, double long2)
        //{
        //    return (int)(1000D * HaversineKilometers(lat1, long1, lat2, long2));
        //}

        ////Haversine is distance between gps coordinates - https://en.wikipedia.org/wiki/Haversine_formula 
        //private static double HaversineKilometers(double lat1, double long1, double lat2, double long2)
        //{
        //    double dlong = (long2 - long1) * _d2r;
        //    double dlat = (lat2 - lat1) * _d2r;
        //    double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * _d2r) * Math.Cos(lat2 * _d2r) * Math.Pow(Math.Sin(dlong / 2D), 2D);
        //    double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
        //    double d = earthRadius * c;

        //    return d;
        //}

        private static double LongitudeDegreeLength(double lat)
        {
            return Math.Cos(lat) * oneLongitudeDegreeLengthAtEquatorInKilometers;
        }
        //no need to use Haversine because the points are always only a few meters away, so round shape of Earth doesnt matter
        private static double DistanceBetweenCoordinatesInMeters(double lat1, double long1, double lat2, double long2)
        {
            var dlong = long2 - long1;
            var dlat = lat2 - lat1;
            var longitudeDegreeLength = LongitudeDegreeLength(lat1);
            var dx = dlong * longitudeDegreeLength;
            var dy = dlat * oneLatitudeDegreeInKilometers;
            return Math.Sqrt(dx * dx + dy * dy) * 1000.0;
        }

        public static List<Trkpt> ConvertTrkptModelListToTrkptList(List<TrkptModel> trkptModels)
        {
            var trkptList = new List<Trkpt>();
            foreach (var trkptModel in trkptModels)
            {
                var trkpt = new Trkpt
                {
                    Lat = trkptModel.Latitude.ToString(CultureInfo.InvariantCulture),
                    Lon = trkptModel.Longitude.ToString(CultureInfo.InvariantCulture),
                    Time = trkptModel.Time.ToString()
                };
                trkptList.Add(trkpt);
            }
            return trkptList;
        }
    }
}