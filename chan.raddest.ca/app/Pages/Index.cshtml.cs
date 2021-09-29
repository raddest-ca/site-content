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
using System.Collections.Specialized;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _context;
        private readonly BlobContainerClient _blob;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext context, BlobContainerClient blobClient)
        {
            _logger = logger;
            _context = context;
            _blob = blobClient;
        }

        public List<Thread> Threads { get; set; } = new();

        public async Task OnGetAsync()
        {
            Threads = await _context.GetThreads(PaginationIndex);
            Submission = Submission.FromRequest(Request);
            PageCount = await _context.GetPageCount();
        }

        [BindProperty]
        public Submission Submission { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PaginationIndex { get; set; }
        public int? PageCount {get; set;}

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            await Submission.Commit(
                _context,
                _blob,
                Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                null
            );

            Response.Cookies.Append("LastName", Submission.Name);
            Response.Cookies.Append("LastHash", Submission.Hash);
            return RedirectToPage("./Index");
        }
    }
}
