using System;
using System.Collections.Generic;
using app.Data;

namespace app.Models
{
    public class Thread
    {
        public Post Parent { get; set; }
        public List<Post> Earliest { get; set; }
        public List<Post> Latest { get; set; }

        public DateTime LastUpdated {get; set;}

        public int PostCount {get; set;}

        public bool ShowElipses {
            get
            {
                return PostCount > AppDbContext.PreviewPerThread * 2;
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