using DataAccessLayer.DatabaseContext;
using DataAccessLayer.Models.Books;
using System.Data;

namespace DataAccessLayer.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DbContext dbContext;

        public BookRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string?> BorrowBooks(BookOrderModel bookOrder, int id)
        {
            return await dbContext.TransactionQuery(async (st) =>
            {
                foreach(int bookId in bookOrder.BooksId)
                {
                    DataSet ds = await dbContext.DoQueryWithData(
                        "SELECT * FROM BorrowLibrary WHERE bookLib_id=@id",
                        (sa) =>
                        {
                            sa.SelectCommand.Parameters.AddWithValue("@id", bookId); 
                        });
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataSet dsName = await dbContext.DoQueryWithData(
                        "SELECT title as tt FROM BooksLibrary WHERE id=@id",
                        (sa) =>
                        {
                            sa.SelectCommand.Parameters.AddWithValue("@id", bookId);
                        });
                        st.Rollback();
                        return $"The '{dsName.Tables[0].Rows[0]["tt"]!}' is borrowed.";
                    }
                    int rowAffected = await dbContext.DoQueryWithoutData(
                        "INSERT INTO BorrowLibrary VALUES (@book_id,@user_id);", 
                        (sc) =>
                        {
                            sc.Parameters.AddWithValue("@book_id", bookId);
                            sc.Parameters.AddWithValue("@user_id", id);
                        });
                    if (rowAffected == 0)
                    {
                        st.Rollback();
                        throw new Exception("Server error.");
                    }
                }
                st.Commit();
                return null;
            });
        }

        public async Task<Dictionary<string, object>> GetMyBorrowedBooks(int page, int id)
        {
            int skip = (page - 1) * Constant.PageSize;
            DataSet ds = await dbContext.DoQueryWithData(
                "SELECT bk.id AS book_id , bk.title AS book_title FROM BooksLibrary AS bk LEFT JOIN BorrowLibrary AS br ON bk.id = br.bookLib_id WHERE br.userLib_id = @user_id ORDER BY bk.id OFFSET @skip ROWS fetch next @take rows only",
                (sa) =>
                {
                    sa.SelectCommand.Parameters.AddWithValue("@user_id", id);
                    sa.SelectCommand.Parameters.AddWithValue("@skip", skip);
                    sa.SelectCommand.Parameters.AddWithValue("@take", Constant.PageSize);
                });
            List<Dictionary<string, object>> books = new List<Dictionary<string, object>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                books.Add(new Dictionary<string, object>
                {
                    {"id",dr["book_id"] },
                    {"title",dr["book_title"] }
                });
            }
            return new Dictionary<string, object>
            {
                {"data",books }
            };
        }

        public async Task<Dictionary<string, object>> ReturnBooks(BookOrderModel bookOrder, int id)
        {
            int count = 0;
            foreach(int bookId in bookOrder.BooksId)
            {
                int rowAffected =
                    await dbContext.DoQueryWithoutData("DELETE FROM BorrowLibrary where bookLib_id=@book_id and userLib_id=@user_id",
                        (sc) =>
                        {
                            sc.Parameters.AddWithValue("@book_id", bookId);
                            sc.Parameters.AddWithValue("@user_id", id);
                        });
                count += rowAffected;
            }
            return new Dictionary<string, object>
            {
                { "message", count == bookOrder.BooksId.Count?"All Book is returned.":$"{count} books from {bookOrder.BooksId.Count} is returned."}
            };
        }

        public async Task<Dictionary<string, object>> SearchByAuthor(string author, int page)
        {
            int skip = (page - 1)*Constant.PageSize;
            DataSet ds = await dbContext.DoQueryWithData(
                "SELECT bk.id as book_id , bk.title as book_title , br.id as borrow_id , bk.author as book_author FROM BooksLibrary AS bk LEFT JOIN BorrowLibrary AS br ON bk.id = br.bookLib_id WHERE author=@author order by bk.id OFFSET @skip ROWS fetch next @take rows only;",
                (da) =>
                {
                    da.SelectCommand.Parameters.AddWithValue("@author", author);
                    da.SelectCommand.Parameters.AddWithValue("@skip", skip);
                    da.SelectCommand.Parameters.AddWithValue("@take", Constant.PageSize);
                });
            List<Dictionary<string, object>> books = new List<Dictionary<string, object>>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                books.Add(GetDataFromDataRow(dr));
            }

            return new Dictionary<string, object>
            {
                {"data",books }
            };
        }

        public async Task<Dictionary<string, object>> SearchByISBN(string isbn)
        {
            DataSet ds = await dbContext.DoQueryWithData(
                "SELECT bk.id as book_id , bk.title as book_title , br.id as borrow_id , bk.author as book_author FROM BooksLibrary AS bk LEFT JOIN BorrowLibrary AS br ON bk.id = br.bookLib_id WHERE bk.isbn=@isbn;",
                (da) =>
                {
                    da.SelectCommand.Parameters.AddWithValue("@isbn", isbn);
                });
            Dictionary<string, object>? book = new Dictionary<string, object>();
            if (ds.Tables[0].Rows.Count > 0)
                book = GetDataFromDataRow(ds.Tables[0].Rows[0]);
            return new Dictionary<string, object>
            {
                {"data",book}
            };
        }

        public async Task<Dictionary<string, object>> SearchByTitle(string title)
        {
            
            DataSet ds = await dbContext.DoQueryWithData(
                "SELECT bk.id as book_id , bk.title as book_title , br.id as borrow_id , bk.author as book_author FROM BooksLibrary AS bk LEFT JOIN BorrowLibrary AS br ON bk.id = br.bookLib_id WHERE bk.title=@title;",
                (da) =>
                {
                    da.SelectCommand.Parameters.AddWithValue("@title", title);
                });
            Dictionary<string, object>? book = new Dictionary<string, object>();
            if (ds.Tables[0].Rows.Count > 0)
                book = GetDataFromDataRow(ds.Tables[0].Rows[0]);
            return new Dictionary<string, object>
            {
                {"data",book}
            };
        }

        private Dictionary<string,object> GetDataFromDataRow(DataRow dr)
        {
            return new Dictionary<string, object>
                {
                    {"id", dr["book_id"] },
                    {"title", dr["book_title"] },
                    {"author", dr["book_author"] },
                    {"available", dr["borrow_id"] == DBNull.Value}

                };
        }
        

    }
}
