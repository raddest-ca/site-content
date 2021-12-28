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

        public async Task<IList<Post>> GetThread(int ParentPostId)
        {
            return await Posts
                .Include(post => post.File)
                .Where(post => post.ParentId == ParentPostId || post.Id == ParentPostId)
                .OrderBy(post => post.Created)
                .ToListAsync();
        }

        public static readonly int ThreadsPerPage = 15;
        public static readonly int PreviewPerThread = 7;

        public async Task<int> GetPageCount()
        {
            var count = await Posts
                .Where(Post.IsThread)
                .CountAsync();
            return (int)Math.Ceiling(count / (double)ThreadsPerPage);
        }

        
        public async Task<List<Thread>> GetThreads(int PaginationIndex)
        {
            return Posts
                .Include(post => post.File)
                .Where(Post.IsThread)
                .Select(parent => new Thread{
                    Parent = parent,
                    Earliest = Posts
                        .Include(post => post.File)
                        .Where(post => post.ParentId == parent.Id)
                        .OrderBy(post => post.Created)
                        .Take(PreviewPerThread)
                        .ToList(),
                    Latest = Posts
                        .Include(post => post.File)
                        .Where(post => post.ParentId == parent.Id)
                        .OrderByDescending(post => post.Created)
                        .Take(PreviewPerThread)
                        .ToList(),
                    LastUpdated = Posts
                        .Where(post => post.ParentId == parent.Id || post.Id == parent.Id)
                        .Max(post => post.Created),
                    PostCount = Posts
                        .Where(post => post.ParentId == parent.Id || post.Id == parent.Id)
                        .Count()
                })
                .OrderByDescending(thread => thread.LastUpdated)
                .Skip(ThreadsPerPage * PaginationIndex)
                .Take(ThreadsPerPage)
                .ToList()
                .AsEnumerable()
                .Select(thread => {
                    thread.Latest.Reverse();
                    thread.Latest.RemoveAll(thread.Earliest.Contains);
                    return thread;
                })
                .ToList();
        }
    }
}