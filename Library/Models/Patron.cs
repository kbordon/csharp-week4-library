using System;
using System.Collections.Generic;

namespace Library.Models
{
  public class Patron
  {
    private int _id;
    public void SetId(int id) {_id = id;}
    public int GetId() {return _id;}

    private string _name;
    public void SetName(string name) {_name = name;}
    public string GetName() {return _name;}

    public Patron(string name, int id=0)
    {
      SetName(name);
      SetId(id);
    }

    public void Save()
    {
      Query savePatron = new Query("INSERT INTO patrons (name) VALUES(@Name)");
      savePatron.AddParameter("@Name", GetName());
      savePatron.Execute();
      SetId((int)savePatron.GetCommand().LastInsertedId);
    }

    public void Update()
    {
      Query updatePatron = new Query("UPDATE patrons SET name = @Name WHERE patron_id = @Id");
      updatePatron.AddParameter("@Name", GetName());
      updatePatron.AddParameter("@Id", GetId().ToString());
      updatePatron.Execute();
    }

    public static Patron Find(int id)
    {
      Query findPatron = new Query("SELECT * FROM patrons WHERE patron_id = @Id");
      findPatron.AddParameter("@Id", id.ToString());
      var rdr = findPatron.Read();
      string name = "";
      while (rdr.Read())
      {
        name = rdr.GetString(1);
      }
      Patron foundPatron = new Patron(name, id);
      return foundPatron;
    }

    public static List<Patron> GetAll()
    {
      List<Patron> patrons = new List<Patron> {};
      Query allPatrons = new Query("SELECT * FROM patrons");
      var rdr = allPatrons.Read();

      while (rdr.Read())
      {
        string name = rdr.GetString(1);
        int id = rdr.GetInt32(0);
        Patron foundPatron = new Patron(name, id);
        patrons.Add(foundPatron);
      }
      return patrons;
    }

    public void Delete()
    {
      DeleteById(GetId());
    }

    public static void DeleteById(int id)
    {
      Query deletePatron = new Query("DELETE FROM patrons WHERE patron_id = @Id; Delete FROM checkouts WHERE patron_id = @Id");
      deletePatron.AddParameter("@Id", id.ToString());
      deletePatron.Execute();
    }

    public static void ClearAll()
    {
      Query deletePatron = new Query("DELETE FROM patrons; DELETE FROM checkouts; DELETE FROM history");
      deletePatron.Execute();
    }

    public bool AddCheckOut(int bookId, double days = 7)
    {

      Book requested = Book.Find(bookId);
      int freeCopy = requested.GetAvailable();

      if (freeCopy < 1)
      {
        return false;
      }

      Query patronCheckout = new Query("INSERT INTO checkouts VALUES(@PatronId, @CopyId, @CheckoutDate, @DueDate)");
      patronCheckout.AddParameter("@PatronId", GetId().ToString());
      patronCheckout.AddParameter("@CopyId", freeCopy.ToString());

      DateTime currentDate = DateTime.Now;
      patronCheckout.AddParameter("@CheckoutDate", currentDate.ToString("yyyy-MM-dd HH:mm:ss"));
      DateTime dueDate = currentDate.AddDays(days);
      patronCheckout.AddParameter("@DueDate", dueDate.ToString("yyyy-MM-dd HH:mm:ss"));
      patronCheckout.Execute();

      return true;
    }

    public List<Book> GetCheckouts()
    {
      Query checkouts = new Query(@"
        SELECT books.*, copies.copy_id, checkouts.check_out, checkouts.due_date FROM checkouts
          JOIN (copies, books)
          ON copies.copy_id = checkouts.copy_id
        WHERE patron_id = @Id
      ");
      checkouts.AddParameter("@Id", GetId().ToString());
      var rdr = checkouts.Read();
      List<Book> checkedOutBooks = new List<Book> {};
      while (rdr.Read())
      {
        string name = rdr.GetString(1);
        int id = rdr.GetInt32(0);
        int copyId = rdr.GetInt32(2);
        Book checkedOut = new Book(name, id);
        checkedOut.SetCopyId(copyId);

        checkedOutBooks.Add(checkedOut);
      }
      return checkedOutBooks;
    }

    public void CheckIn(int copyId)
    {

      Query getCheckoutDate = new Query(@"
        SELECT checkouts.check_out, books.book_id FROM copies
          JOIN(checkouts, books)
          ON copies.book_id = books.book_id
        WHERE copies.copy_id = @CopyId
      ");
      getCheckoutDate.AddParameter("@CopyId", copyId.ToString());
      string checkOutDate = "";
      int bookId = 0;
      var checkOutDateRdr = getCheckoutDate.Read();
      while (checkOutDateRdr.Read())
      {
        checkOutDate = checkOutDateRdr.GetDateTime(0).ToString("yyyy-MM-dd HH:mm:ss");
        bookId = checkOutDateRdr.GetInt32(1);
      }

      DateTime now = DateTime.Now;
      string nowSql = now.ToString("yyyy-MM-dd HH:mm:ss");

      Query ClearCheckout = new Query(@"
        INSERT INTO history VALUES(@PatronId, @BookId, @CheckOutDate, @CheckInDate);
        DELETE FROM checkouts WHERE copy_id = @CopyId
      ");
      ClearCheckout.AddParameter("@PatronId", GetId().ToString());
      ClearCheckout.AddParameter("@CopyId", copyId.ToString());
      ClearCheckout.AddParameter("@BookId", bookId.ToString());
      ClearCheckout.AddParameter("@CheckOutDate", checkOutDate);
      ClearCheckout.AddParameter("@CheckInDate", nowSql);
      ClearCheckout.Execute();
    }

    public List<Book> GetHistory()
    {
      List<Book> history = new List<Book>{};
      Query getHistory = new Query(@"
        SELECT history.*, books.title FROM history
          JOIN (books)
          ON history.book_id = books.book_id
        WHERE patron_id = @PatronId
      ");
      getHistory.AddParameter("@PatronId", GetId().ToString());
      var rdr = getHistory.Read();
      while (rdr.Read())
      {
        string name = rdr.GetString(4);
        int id = rdr.GetInt32(1);

        Book pastBook = new Book(name, id);
        history.Add(pastBook);
      }
      return history;
    }
  }
}
