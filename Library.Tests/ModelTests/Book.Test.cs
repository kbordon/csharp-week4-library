using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.Models;
namespace Library.Tests
{
  [TestClass]
  public class BookTests : IDisposable
  {

    public BookTests()
    {
      DB.DatabaseTest();
    }

    public void Dispose()
    {
      Author.ClearAll();
      Book.ClearAll();
    }

    [TestMethod]
    public void GetAll_ReturnsDatabaseEmptyAtFirst_0()
    {
      int result = Book.GetAll().Count;

      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Save_SaveBookToDatabase_1()
    {
      Book book1 = new Book("Goosebumps");
      book1.Save();

      Assert.AreEqual(1, Book.GetAll().Count);
    }

    [TestMethod]
    public void Find_FindsBookInDatabase_True()
    {
      Book newBook = new Book("Mr. Fantastic Fox");
      newBook.Save();

      Book foundBook = Book.Find(newBook.GetId());
      Assert.AreEqual(newBook.GetId(), foundBook.GetId());
    }

    [TestMethod]
    public void Update_UpdatesBookTitleInDatabase_False()
    {
      Book newBook = new Book("The Golden Compass");
      newBook.Save();

      newBook.SetTitle("The Not-So-Golden Compass");
      newBook.Update();

      Book foundBook = Book.Find(newBook.GetId());
      Assert.AreNotEqual(foundBook.GetTitle(), "The Golden Compass");
    }

    [TestMethod]
    public void Delete_DeleteBookInDatabase_0()
    {
      Book newBook = new Book("The Mist");
      newBook.Save();
      newBook.Delete();

      int result = Book.GetAll().Count;
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void AddAuthor_JoinsAuthorToBook_2()
    {
      Book newBook = new Book("Good Omens");
      newBook.Save();

      Author author1 = new Author("Neil Gaiman");
      author1.Save();
      Author author2 = new Author("Terry Prachett");
      author2.Save();
      newBook.AddAuthor(author1);
      newBook.AddAuthor(author2);

      Assert.AreEqual(2, newBook.GetAllAuthors().Count);

    }
  }
}
