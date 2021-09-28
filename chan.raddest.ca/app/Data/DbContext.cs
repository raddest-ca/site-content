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

        public DbSet<Post> Posts { get; set; }
        public DbSet<File> Files { get; set; }

        public async Task<IList<Post>> GetThread(ulong ParentPostId)
        {
            return await Posts
                .Include(post => post.File)
                .Where(post => post.ParentId == ParentPostId || post.Id == ParentPostId)
                .ToListAsync();
        }

        public static readonly uint ThreadsPerPage = 10;
        public static readonly uint PreviewPerThread = 10;

        public async Task<uint> GetPageCount()
        {
            var count = await Posts
                .Where(p => p.ParentId == null)
                .CountAsync();
            return (uint)Math.Ceiling(count / (double)ThreadsPerPage);
        }
        
        public async Task<Dictionary<ulong, List<Post>>> GetThreads(uint PaginationIndex)
        {
            // Create rtn dict
            Dictionary<ulong, List<Post>> rtn = new();

            // Query posts ordered by Id
            var threadParents = await Posts
                .Where(p => p.ParentId == null)
                .OrderBy(p => (int)p.Id)
                .Include(p => p.File)
                // .TakeLast((int)ThreadsPerPage)
                .ToListAsync();

            // Create threads in rtn dict
            foreach (var post in threadParents)
            {
                rtn[post.Id] = new();
                rtn[post.Id].Add(post);
            }

            // Create list of parent ids
            var parentIds = threadParents.Select(p => p.Id).ToList();

            // Query posts for each thread
            var threadChildren = await Posts
                .Where(p => p.ParentId != null)
                .Where(p => parentIds.Contains((ulong)p.ParentId))
                .OrderBy(p => (int)p.Id)
                .Include(p => p.File)
                // .TakeLast((int)PreviewPerThread)
                .ToListAsync();

            // Add posts to threads
            foreach (var post in threadChildren)
            {
                rtn[(ulong)post.ParentId].Append(post);
            }

            // Return thread dict
            return rtn;
        }
    }
}