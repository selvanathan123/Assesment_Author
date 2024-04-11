using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InspirinngQuotes.Data;
using InspirinngQuotes.Models;

namespace InspirinngQuotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuotesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Quotes
        [HttpGet]
        public async Task<ActionResult<List<Quote>>> GetQuotes()
        {
            var quotes = new List<Quote>();
            try
            {
            quotes = await _context.Quotes.ToListAsync();
            } catch (Exception ex)
            {
                
            }


            return Ok(quotes);
        }

        // GET: api/Quotes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Quote>> GetQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            return quote;
        }

        // POST: api/Quotes
        [HttpPost]
        public async Task<ActionResult<Quote>> AddQuote(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            // Custom response message
            var responseMessage = $"Author details for {quote.Author} added successfully.";

            return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, new { Message = responseMessage, Quote = quote });
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuote(int id, Quote quote)
        {
            if (id != quote.Id)
            {
                return BadRequest();
            }

            _context.Entry(quote).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Get the updated author name from the modified quote
            var updatedQuote = await _context.Quotes.FindAsync(id);
            string updatedAuthorName = updatedQuote.Author;

            // Custom response message
            var responseMessage = $"Author {updatedAuthorName} updated successfully.";

            return Ok(new { Message = responseMessage });
        }

        // DELETE: api/Quotes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);
            if (quote == null)
            {
                return NotFound();
            }

            string authorName = quote.Author; // Capture author name before deletion

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            // Custom response message
            var responseMessage = $"Author {authorName} deleted successfully.";

            return Ok(new { Message = responseMessage });
        }

        private bool QuoteExists(int id)
        {
            return _context.Quotes.Any(e => e.Id == id);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Quote>> SearchQuotes(string author = null, [FromQuery] List<string> tags = null, [FromQuery] List<string> quote = null)
        {
            IQueryable<Quote> query = _context.Quotes;

            // Filter by author (contains search, case-insensitive)
            if (!string.IsNullOrEmpty(author))
            {
                string lowercaseAuthor = author.ToLower();
                query = query.Where(q => q.Author.ToLower().Contains(lowercaseAuthor));
            }

            // Filter by tags (exact match)
            if (tags != null && tags.Any())
            {
                query = query.Where(q => tags.All(t => q.Tags.Contains(t)));
            }

            // Filter by quotes (contains search, case-insensitive)
            if (quote != null && quote.Any())
            {
                // Lowercase all search terms
                List<string> lowercaseQuotes = quote.Select(q => q.ToLower()).ToList();

                // Filter quotes where any quote text contains any of the search terms
                query = query.Where(q => q.QuoteText.Any(qt => lowercaseQuotes.Any(lq => qt.ToLower().Contains(lq))));
            }
             
            var matchedQuotes = query.ToList();
            return Ok(matchedQuotes);
        }






    }
}
