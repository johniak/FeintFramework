using System;
using FeintSDK;

namespace FeintSite.Models
{
    public class TripModel : BaseModel
    {
        public String? Name { get; set; }
        public DateTime StartTime { get; set; }
    }
}