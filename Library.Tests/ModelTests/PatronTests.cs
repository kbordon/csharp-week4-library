using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.Models;
namespace Library.Tests
{
  [TestClass]
  public class PatronTests : IDisposable
  {

    public PatronTests()
    {
      DB.DatabaseTest();
    }

    public void Dispose()
    {
      Author.ClearAll();
      Patron.ClearAll();
      Book.ClearAll();
    }

    [TestMethod]
    public void GetAll_ReturnsDatabaseEmptyAtFirst_0()
    {
      int result = Patron.GetAll().Count;

      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Save_SavePatronToDatabase_1()
    {
      Patron patron1 = new Patron("James Keith");
      patron1.Save();

      Assert.AreEqual(1, Patron.GetAll().Count);
    }

    [TestMethod]
    public void Find_FindsBookInDatabase_True()
    {
      Book newBook = new Book("Marcel Stone");
      newBook.Save();

      Book foundBook = Book.Find(newBook.GetId());
      Assert.AreEqual(newBook.GetId(), foundBook.GetId());
    }

    [TestMethod]
    public void Update_UpdatesPatronNameInDatabase_False()
    {
      Patron newPatron = new Patron("Austin Rock");
      newPatron.Save();

      newPatron.SetName("Austin Stone");
      newPatron.Update();

      Patron foundPatron = Patron.Find(newPatron.GetId());
      Assert.AreNotEqual(foundPatron.GetName(), "Austin Rock");
    }

    [TestMethod]
    public void Delete_DeletePatronInDatabase_0()
    {
      Patron newPatron = new Patron("Lord Rahl");
      newPatron.Save();
      newPatron.Delete();

      int result = Patron.GetAll().Count;
      Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Checkout_CheckedOutBooks_1()
    {
      Patron newPatron = new Patron("Veronica Faelon");
      newPatron.Save();

      Book book1 = new Book("Goosebumps");
      book1.Save();
      book1.addCopy(2);

      bool canCheckout = newPatron.AddCheckOut(book1.GetId());
      if (canCheckout)
      {
        Console.WriteLine("Available Book");
      }
      else
      {
        Console.WriteLine("No Available book");
      }

      List<Book> checkouts = newPatron.GetCheckouts();
      Assert.AreEqual(1, checkouts.Count);
    }

    [TestMethod]
    public void CheckIn_CheckInBooks_0()
    {
      Patron newPatron = new Patron("Veronica Faelon");
      newPatron.Save();

      Book book1 = new Book("Goosebumps");
      book1.Save();
      book1.addCopy(2);

      bool canCheckout = newPatron.AddCheckOut(book1.GetId());
      List<Book> preCheckOuts = newPatron.GetCheckouts();

      newPatron.CheckIn(preCheckOuts[0].GetCopyId());

      List<Book> PostCheckOuts = newPatron.GetCheckouts();

      Assert.AreEqual(0, PostCheckOuts.Count);
    }
  }
}
