using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Post> Posts {get; set;}
        public DbSet<File> Files {get; set;}

        public async Task<IList<Post>> GetThread(ulong ParentPostId)
        {
            return await Posts
                .Include(post => post.File)
                .Where(post => post.ParentId == ParentPostId || post.Id == ParentPostId)
                .ToListAsync();
        }

        public async Task<Dictionary<ulong, List<Post>>> GetThreads()
        {
            // Create rtn dict
            Dictionary<ulong, List<Post>> rtn = new();

            // Get posts ordered by Id
            var posts = await Posts
                .Include(p => p.File)
                .OrderBy(p => (int)p.Id)
                .ToListAsync();

            // Add posts to threads
            foreach (var post in posts)
            {
                // Get thread id
                var threadId = post.ParentId ?? post.Id;

                // If thread doesn't exist, create
                if (!rtn.ContainsKey(threadId))
                {
                    rtn[threadId] = new();
                }

                // Add post to thread
                rtn[threadId].Add(post);
            }

            // Return thread dict
            return rtn;
        }
    }
}