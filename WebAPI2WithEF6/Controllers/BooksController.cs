﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI2WithEF6.Models;

namespace WebAPI2WithEF6.Controllers
{
    public class BooksController : ApiController
    {
        private BookServiceContext db = new BookServiceContext();

        List<BookDTO> _bookDTOs = new List<BookDTO>();
        public List<BookDTO> bookDTOs() { return _bookDTOs; }

        public BooksController() { }
        public BooksController(List<BookDTO> items) { _bookDTOs=items; }


        public BookDetailDTO BookDetailDTOs { get; set; }
        // GET api/Books
        public IQueryable<BookDTO> GetBooks()
        {
            //return db.Books;
            var books = from b in db.Books
                        orderby b.Title
                        select new BookDTO()
                        {
                            Id=b.Id,
                            Title=b.Title,
                            AuthorName=b.Author.Name
                        };
            return books;
        }

        // GET api/Books/5
        //[ResponseType(typeof(Book))]
        [ResponseType(typeof(BookDetailDTO))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            //Book book = await db.Books.FindAsync(id);
            //if (book == null)
            //{
            //    return NotFound();
            //}

            //return Ok(book);
            var book = await db.Books.Include(b => b.Author).Select(b =>
                new BookDetailDTO() { 
                    Id=b.Id,
                    Title=b.Title,
                    Year=b.Year,
                    Price=b.Price,
                    AuthorName=b.Author.Name,
                    Genre=b.Genre
                }).SingleOrDefaultAsync(b=>b.Id==id);

            if (book == null)
                return NotFound();
            return Ok(book);
        }

        // PUT api/Books/5
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = book.Id }, book);
            //New code:
            // Load author name
            db.Entry(book).Reference(x=>x.Author).Load();
            var dto = new BookDTO() { 
                Id=book.Id,
                Title = book.Title,
                AuthorName=book.Author.Name
            };

            return CreatedAtRoute("DefaultApi", new { id=book.Id}, dto);
        }

        // DELETE api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}