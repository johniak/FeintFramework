using System;
using FeintSDK;
using NetTopologySuite.Geometries;

namespace FeintSite.Models
{
    public class TripPointModel : BaseModel
    {
        public DateTime Time { get; set; }
        public Point Location { get; set; }
        public TripModel Trip { get; set; }
    }
}