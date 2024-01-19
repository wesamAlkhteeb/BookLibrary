
create database BookSystem;
use BookSystem;

create table BooksLibrary(
	id INT IDENTITY PRIMARY KEY NOT NULL,
	title VARCHAR(60) NOT NULL,
	author VARCHAR(30) NOT NULL,
	isbn VARCHAR(14) NOT NULL,
);

create table UserLibrary(
	id INT IDENTITY PRIMARY KEY NOT NULL,
	username VARCHAR(20) NOT NULL,
	password VARCHAR(50) NOT NULL
);


create table BorrowLibrary(
	id INT IDENTITY PRIMARY KEY NOT NULL,
	bookLib_id INT UNIQUE NOT NULL, -- one to one
	FOREIGN KEY (bookLib_id) REFERENCES BooksLibrary(id),
	userLib_id INT NOT NULL, -- one to many
	FOREIGN KEY (userLib_id) REFERENCES UserLibrary(id)
);


INSERT INTO BooksLibrary (title, author, isbn) VALUES
	('Clean Code: A Handbook of Agile Software Craftsmanship', 'Robert C. Martin', '978-0132350884'),
	('Harry Potter and the Sorcerer''s Stone', 'J.K. Rowling', '978-0590353427'),
	('Murder on the Orient Express', 'Agatha Christie', '978-0062073495'),
	('The Da Vinci Code', 'Agatha Christie', '978-0307474278'),
	('To Kill a Mockingbird', 'J.K. Rowling', '978-0061120084'),
	('The Great Gatsby', 'F. Scott Fitzgerald', '978-0743273565'),
	('1984', 'George Orwell', '978-0451524935'),
	('The Catcher in the Rye', 'J.D. Salinger', '978-0316769480'),
	('Pride and Prejudice', 'Jane Austen', '978-0141199078'),
	('The Hobbit', 'J.R.R. Tolkien', '978-0547928227'),
	('Sample Book 50', 'Robert C. Martin', '123-456-7890');

EXEC sp_helpindex 'BooksLibrary';

use BookSystem;
SELECT title FROM BooksLibrary WHERE id=3
select * from BorrowLibrary;
delete from BorrowLibrary;