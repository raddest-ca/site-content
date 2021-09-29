using System;
using System.Collections.Generic;

namespace app.Models
{
    public class Thread
    {
        public Post Parent { get; set; }
        public List<Post> Earliest { get; set; }
        public List<Post> Latest { get; set; }

        public DateTime LastUpdated {get; set;}

        public bool ShowElipses {
            get
            {
                return Latest.Count > 0;
            }
        }

        public List<Post> Flattened
        {
            get 
            {
                var rtn = new List<Post>();
                rtn.Add(Parent);
                rtn.AddRange(Earliest);
                rtn.AddRange(Latest);
                return rtn;
            }
        }
    }
}