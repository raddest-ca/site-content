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
                .ToListAsync();
        }

        public static readonly int ThreadsPerPage = 3;
        public static readonly int PreviewPerThread = 3;

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
                .Where(Post.IsThread)
                .Select(parent => new Thread{
                    Parent = parent,
                    Earliest = Posts
                        .Where(post => post.ParentId == parent.Id)
                        .OrderBy(post => post.Created)
                        .Take(PreviewPerThread)
                        .ToList(),
                    Latest = Posts
                        .Where(post => post.ParentId == parent.Id)
                        .OrderByDescending(post => post.Created)
                        .Take(PreviewPerThread)
                        .ToList(),
                    LastUpdated = Posts
                        .Where(post => post.ParentId == parent.Id || post.Id == parent.Id)
                        .Max(post => post.Created)
                })
                .OrderByDescending(thread => thread.LastUpdated)
                .Skip(ThreadsPerPage * PaginationIndex)
                .Take(ThreadsPerPage)
                .ToList()
                .AsEnumerable()
                .Select(thread => { thread.Latest.Reverse(); return thread;})
                .ToList();
        }
    }
}