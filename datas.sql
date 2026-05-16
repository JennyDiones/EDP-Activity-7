-- ============================================================
--  Bookstore Information System – Sample Seed Data
--  Run this AFTER running the schema (Diones_Activity2.sql)
-- ============================================================

USE `bookstore`;

-- Publishers
INSERT IGNORE INTO `publishers` (name, country, founded_year) VALUES
('Penguin Books',        'United Kingdom', 1935),
('HarperCollins',        'United States',  1989),
('Simon & Schuster',     'United States',  1924),
('Random House',         'United States',  1927),
('Prentice Hall',        'United States',  1913),
('O\'Reilly Media',      'United States',  1978),
('Pearson Education',    'United Kingdom', 1844),
('McGraw-Hill',          'United States',  1888),
('Wiley',                'United States',  1807),
('Addison-Wesley',       'United States',  1942);

-- Categories
INSERT IGNORE INTO `categories` (name, description) VALUES
('Fiction',           'Fictional narrative works'),
('Non-Fiction',       'Factual or real-world topics'),
('Science Fiction',   'Speculative fiction involving science'),
('Fantasy',           'Imaginative fiction with magical elements'),
('Programming',       'Software development and coding books'),
('Database',          'Database design and management'),
('Networking',        'Computer networks and communications'),
('Biography',         'Life stories of real people'),
('Self-Help',         'Personal development books'),
('History',           'Historical accounts and analysis');

-- Authors
INSERT IGNORE INTO `authors` (first_name, last_name, nationality, birth_year) VALUES
('Robert',   'Martin',     'American',   1952),
('Andrew',   'Hunt',       'American',   1964),
('Martin',   'Fowler',     'British',    1963),
('Thomas',   'Cormen',     'American',   1956),
('Donald',   'Knuth',      'American',   1938),
('Brian',    'Kernighan',  'Canadian',   1942),
('Dennis',   'Ritchie',    'American',   1941),
('Bjarne',   'Stroustrup', 'Danish',     1950),
('James',    'Gosling',    'Canadian',   1955),
('Guido',    'Rossum',     'Dutch',      1956);

-- Books
INSERT IGNORE INTO `books` (isbn, title, publication_year, price, stock_quantity, publisher_id, category_id) VALUES
('9780132350884', 'Clean Code',                         2008, 450.00, 25,  6, 5),
('9780201616224', 'The Pragmatic Programmer',            1999, 520.00, 15,  10, 5),
('9780201633610', 'Design Patterns',                    1994, 680.00, 8,   10, 5),
('9780201485677', 'Refactoring',                        1999, 490.00, 35,  10, 5),
('9780262033848', 'Introduction to Algorithms',         2009, 750.00, 5,   7,  5),
('9780131101630', 'The C Programming Language',         1988, 380.00, 20,  5,  5),
('9780321563842', 'The Clean Coder',                    2011, 420.00, 40,  6,  5),
('9780596517748', 'JavaScript: The Good Parts',         2008, 350.00, 30,  6,  5),
('9780134685991', 'Effective Java',                     2018, 560.00, 12,  8,  5),
('9781491950357', 'Learning Python',                    2013, 620.00, 18,  6,  5);

-- Book ↔ Author links
INSERT IGNORE INTO `book_authors` (book_id, author_id) VALUES
(1,1),(2,2),(3,1),(4,3),(5,4),(6,6),(7,1),(8,2),(9,1),(10,10);
