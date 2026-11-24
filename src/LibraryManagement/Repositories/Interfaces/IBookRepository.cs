using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IBookRepository
{
    void Add(Book book);

    Book? GetById(Guid id);

    Book? GetByISBN(string isbn);

    IEnumerable<Book> GetAll();

    bool ExistsByISBN(string isbn);
}
