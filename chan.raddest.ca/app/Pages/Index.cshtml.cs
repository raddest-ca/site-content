using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<Post> Posts {get; set;}

        public void OnGet()
        {
            Posts = _context.Posts.ToList();
        }

        public class PostVM
        {
            [Required]
            public string Content { get; set; }
            public string Name {get; set;}
            public string Hash {get; set;}
        }

        [BindProperty]
        public PostVM Submission { get; set; }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine($"Got post with content {Submission.Content}");
            if (!ModelState.IsValid) 
            {
                OnGet();
                return Page();
            }
            var post = new Post()
            {
                AuthorName = Submission.Name,
                AuthorHash = Post.ComputeHash(Submission.Hash),
                AuthorIp = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                Created = DateTime.Now,
                Content = Submission.Content
            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
