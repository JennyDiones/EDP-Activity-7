-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1:3307
-- Generation Time: May 19, 2026 at 04:50 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bookstoreis`
--

-- --------------------------------------------------------

--
-- Table structure for table `authors`
--

CREATE TABLE `authors` (
  `author_id` int(11) NOT NULL,
  `first_name` varchar(100) NOT NULL,
  `last_name` varchar(100) NOT NULL,
  `nationality` varchar(100) DEFAULT NULL,
  `birth_year` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `authors`
--

INSERT INTO `authors` (`author_id`, `first_name`, `last_name`, `nationality`, `birth_year`) VALUES
(1, 'Jose', 'Rizal', 'Filipino', 1861),
(2, 'Nick', 'Joaquin', 'Filipino', 1917),
(3, 'F. Sionil', 'Jose', 'Filipino', 1924),
(4, 'Lualhati', 'Bautista', 'Filipino', 1945),
(5, 'Bob', 'Ong', 'Filipino', 1972),
(6, 'Ricky Lee', 'Garcia', 'Filipino', 1947),
(7, 'Erin', 'Entrada Kelly', 'Filipino-American', 1977),
(8, 'Jessica', 'Hagedorn', 'Filipino', 1949),
(9, 'Stephen', 'King', 'American', 1947),
(10, 'J.K.', 'Rowling', 'British', 1965),
(11, 'Jose', 'Rizal', 'Filipino', 1861),
(12, 'Nick', 'Joaquin', 'Filipino', 1917),
(13, 'F. Sionil', 'Jose', 'Filipino', 1924),
(14, 'Lualhati', 'Bautista', 'Filipino', 1945),
(15, 'Bob', 'Ong', 'Filipino', 1972),
(16, 'Ricky', 'Lee', 'Filipino', 1947),
(17, 'Erin', 'Entrada Kelly', 'Filipino-American', 1977),
(18, 'Jessica', 'Hagedorn', 'Filipino', 1949),
(19, 'Stephen', 'King', 'American', 1947),
(20, 'J.K.', 'Rowling', 'British', 1965);

-- --------------------------------------------------------

--
-- Table structure for table `books`
--

CREATE TABLE `books` (
  `book_id` int(11) NOT NULL,
  `title` varchar(200) DEFAULT NULL,
  `author` varchar(100) DEFAULT NULL,
  `isbn` varchar(50) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `stock_quantity` int(11) DEFAULT 0,
  `category` varchar(50) DEFAULT NULL,
  `genre` varchar(100) DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `books`
--

INSERT INTO `books` (`book_id`, `title`, `author`, `isbn`, `price`, `stock_quantity`, `category`, `genre`) VALUES
(1, 'Noli Me Tangere (Annotated Edition)', 'Jose Rizal', '978-971-27-0001-1', 350.00, 69, 'Fiction', ''),
(2, 'El Filibusterismo (Annotated Edition)', 'Jose Rizal', '978-971-27-0002-2', 350.00, 75, 'Fiction', ''),
(3, 'Dekada 70', 'Lualhati Bautista', '978-971-27-0003-3', 299.00, 59, 'Fiction', ''),
(4, 'Mga Ibong Mandaragit', 'F. Sionil Jose', '978-971-27-0004-4', 320.00, 45, 'Fiction', ''),
(5, 'ABNKKBSNPLAKo?!', 'Bob Ong', '978-971-27-0005-5', 250.00, 99, 'Fiction', ''),
(6, 'Kung Paano Ko Sinira ang Aking Buhay', 'Bob Ong', '978-971-27-0006-6', 250.00, 90, 'Fiction', ''),
(7, 'Si Amapola sa 65 na Kabanata', 'Nick Joaquin', '978-971-27-0007-7', 280.00, 55, 'Fiction', ''),
(8, 'Smaller and Smaller Circles', 'Ricky Lee', '978-971-27-0008-8', 399.00, 39, 'Fiction', ''),
(9, 'Ilustrado', 'Erin Entrada Kelly', '978-971-27-0009-9', 420.00, 34, 'Fiction', ''),
(10, 'Dogeaters', 'Jessica Hagedorn', '978-971-27-0010-0', 380.00, 29, 'Fiction', ''),
(11, 'Philippine History: A New Perspective', 'Jose Rizal', '978-971-27-0011-1', 450.00, 71, 'History', ''),
(12, 'Rizal Without the Overcoat', 'Nick Joaquin', '978-971-27-0012-2', 310.00, 48, 'Non-Fiction', ''),
(13, 'Mga Kuwento ni Lola Basyang', 'Lualhati Bautista', '978-971-27-0013-3', 220.00, 65, 'Children', ''),
(15, 'Ang Paboritong Libro ni Hudas', 'Bob Ong', '978-971-27-0015-5', 260.00, 64, 'Fiction', ''),
(16, 'Pretty Girl', 'Stephen King', '978-0-450-41125-1', 199.00, 25, 'Fiction', ''),
(18, 'Atomic Habits', 'James Clear', '978-0-7352-1129-5', 480.00, 60, 'Self-Help', ''),
(19, 'Alamat ng Pinya', 'Erin Entrada Kelly', '9978-01923-10', 100.00, 20, 'Children', ''),
(20, 'alamat ng saging', 'Lualhati Bautista', '2736482632', 299.00, 20, 'Biography', ''),
(21, 'jenny documentary', 'F. Sionil Jose', '345-6753-980', 1999.00, 109, 'History', ''),
(22, 'Romeo and Juliet', 'William Shakespeare', '7647-07493-09', 2000.00, 50, 'Non-Fiction', ''),
(23, 'alamat ng pagong', 'Jenny Diones', '67874-543-10', 99.00, 33, NULL, ''),
(24, 'Alamat ng Ampalaya', 'Jenny Diones', '7574-3949783', 99.00, 0, NULL, '');

-- --------------------------------------------------------

--
-- Table structure for table `book_authors`
--

CREATE TABLE `book_authors` (
  `book_id` int(11) NOT NULL,
  `author_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE `categories` (
  `category_id` int(11) NOT NULL,
  `category_name` varchar(100) NOT NULL,
  `description` varchar(300) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`category_id`, `category_name`, `description`) VALUES
(1, 'Fiction', 'Novels and short stories'),
(2, 'Non-Fiction', 'Factual and educational books'),
(3, 'Poetry', 'Filipino and international poetry'),
(4, 'History', 'Philippine and world history'),
(5, 'Science', 'Science and technology books'),
(6, 'Biography', 'Life stories of notable people'),
(7, 'Children', 'Books for young readers'),
(8, 'Self-Help', 'Personal development books'),
(9, 'Fiction', 'Novels and short stories'),
(10, 'Non-Fiction', 'Factual and educational books'),
(11, 'Poetry', 'Filipino and international poetry'),
(12, 'History', 'Philippine and world history'),
(13, 'Science', 'Science and technology books'),
(14, 'Biography', 'Life stories of notable people'),
(15, 'Children', 'Books for young readers'),
(16, 'Self-Help', 'Personal development books');

-- --------------------------------------------------------

--
-- Table structure for table `po_items`
--

CREATE TABLE `po_items` (
  `po_item_id` int(11) NOT NULL,
  `po_id` int(11) NOT NULL,
  `book_id` int(11) NOT NULL,
  `quantity` int(11) NOT NULL,
  `unit_cost` decimal(10,2) NOT NULL,
  `subtotal` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `po_items`
--

INSERT INTO `po_items` (`po_item_id`, `po_id`, `book_id`, `quantity`, `unit_cost`, `subtotal`) VALUES
(1, 1, 1, 10, 350.00, 3500.00),
(2, 1, 9, 3, 420.00, 1260.00),
(3, 2, 10, 1, 380.00, 380.00),
(4, 3, 1, 10, 350.00, 3500.00),
(5, 4, 12, 2, 310.00, 620.00);

-- --------------------------------------------------------

--
-- Table structure for table `publishers`
--

CREATE TABLE `publishers` (
  `publisher_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `country` varchar(100) DEFAULT NULL,
  `founded_year` year(4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `publishers`
--

INSERT INTO `publishers` (`publisher_id`, `name`, `country`, `founded_year`) VALUES
(1, 'Anvil Publishing', 'Philippines', '1990'),
(2, 'Adarna House', 'Philippines', '1979'),
(3, 'Vibal Publishing', 'Philippines', '1997'),
(4, 'Rex Book Store', 'Philippines', '1945'),
(5, 'Bookmark Inc.', 'Philippines', '1982'),
(6, 'Penguin Books', 'United Kingdom', '1935'),
(7, 'HarperCollins', 'United States', '1989'),
(8, 'Anvil Publishing', 'Philippines', '1990'),
(9, 'Adarna House', 'Philippines', '1979'),
(10, 'Vibal Publishing', 'Philippines', '1997'),
(11, 'Rex Book Store', 'Philippines', '1945'),
(12, 'Bookmark Inc.', 'Philippines', '1982'),
(13, 'Penguin Books', 'United Kingdom', '1935'),
(14, 'HarperCollins', 'United States', '1989');

-- --------------------------------------------------------

--
-- Table structure for table `purchase_orders`
--

CREATE TABLE `purchase_orders` (
  `po_id` int(11) NOT NULL,
  `supplier_id` int(11) DEFAULT NULL,
  `user_id` int(11) NOT NULL,
  `order_date` datetime DEFAULT current_timestamp(),
  `status` enum('Pending','Received','Cancelled') DEFAULT 'Pending',
  `total_cost` decimal(10,2) DEFAULT 0.00
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `purchase_orders`
--

INSERT INTO `purchase_orders` (`po_id`, `supplier_id`, `user_id`, `order_date`, `status`, `total_cost`) VALUES
(1, NULL, 1, '2026-05-16 21:21:59', 'Received', 4760.00),
(2, NULL, 1, '2026-05-16 21:23:45', 'Cancelled', 380.00),
(3, NULL, 1, '2026-05-16 21:23:55', '', 3500.00),
(4, NULL, 1, '2026-05-16 21:24:08', '', 620.00);

-- --------------------------------------------------------

--
-- Table structure for table `sales`
--

CREATE TABLE `sales` (
  `sale_id` int(11) NOT NULL,
  `sale_date` datetime DEFAULT current_timestamp(),
  `user_id` int(11) DEFAULT NULL,
  `total_amount` decimal(10,2) DEFAULT NULL,
  `payment_method` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sales`
--

INSERT INTO `sales` (`sale_id`, `sale_date`, `user_id`, `total_amount`, `payment_method`) VALUES
(1, '2026-05-15 23:04:46', 1, 100.00, 'Cash'),
(2, '2026-05-15 23:04:52', 1, 100.00, 'Cash'),
(3, '2026-05-15 23:05:10', 1, 1050.00, 'Cash'),
(4, '2026-05-15 23:09:38', 1, 100.00, 'Cash'),
(5, '2026-05-15 23:52:32', 1, 250.00, 'Cash'),
(6, '2026-05-16 15:54:55', 1, 350.00, NULL),
(7, '2026-05-16 15:55:14', 1, 350.00, NULL),
(8, '2026-05-16 15:56:10', 1, 350.00, NULL),
(9, '2026-05-16 15:59:08', 1, 299.00, NULL),
(10, '2026-05-16 16:06:14', 1, 999.00, NULL),
(11, '2026-05-16 16:12:19', 1, 550.00, NULL),
(12, '2026-05-16 16:24:54', 1, 399.00, NULL),
(13, '2026-05-16 16:26:05', 1, 420.00, NULL),
(14, '2026-05-16 16:32:49', 1, 480.00, NULL),
(15, '2026-05-16 16:36:28', 1, 380.00, NULL),
(16, '2026-05-16 16:39:51', 1, 250.00, NULL),
(17, '2026-05-16 16:42:37', 1, 250.00, NULL),
(18, '2026-05-16 16:45:20', 1, 1400.00, NULL),
(19, '2026-05-16 16:57:53', 1, 5850.00, NULL),
(20, '2026-05-16 18:02:42', 1, 2860.00, NULL),
(21, '2026-05-16 20:59:44', 1, 1750.00, NULL),
(22, '2026-05-16 21:21:59', 1, 4760.00, NULL),
(23, '2026-05-16 21:23:45', 1, 380.00, NULL),
(24, '2026-05-16 21:23:55', 1, 3500.00, NULL),
(25, '2026-05-16 21:24:08', 1, 620.00, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `sale_items`
--

CREATE TABLE `sale_items` (
  `item_id` int(11) NOT NULL,
  `sale_id` int(11) DEFAULT NULL,
  `book_id` int(11) DEFAULT NULL,
  `quantity` int(11) DEFAULT NULL,
  `unit_price` decimal(10,2) NOT NULL,
  `subtotal` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sale_items`
--

INSERT INTO `sale_items` (`item_id`, `sale_id`, `book_id`, `quantity`, `unit_price`, `subtotal`) VALUES
(1, NULL, NULL, NULL, 0.00, NULL),
(2, 8, 1, 1, 350.00, 350.00),
(3, 9, 3, 1, 299.00, 299.00),
(4, 10, 21, 1, 999.00, 999.00),
(5, 11, 17, 1, 550.00, 550.00),
(6, 12, 8, 1, 399.00, 399.00),
(7, 13, 9, 1, 420.00, 420.00),
(8, 14, 18, 1, 480.00, 480.00),
(9, 15, 10, 1, 380.00, 380.00),
(10, 16, 6, 1, 250.00, 250.00),
(11, 17, 5, 1, 250.00, 250.00),
(12, 18, 7, 1, 280.00, 280.00),
(13, 18, 7, 4, 280.00, 1120.00),
(14, 19, 11, 13, 450.00, 5850.00),
(15, 20, 15, 1, 260.00, 260.00),
(16, 20, 15, 10, 260.00, 2600.00),
(17, 21, 1, 5, 350.00, 1750.00),
(18, 22, 1, 10, 350.00, 3500.00),
(19, 22, 9, 3, 420.00, 1260.00),
(20, 23, 10, 1, 380.00, 380.00),
(21, 24, 1, 10, 350.00, 3500.00),
(22, 25, 12, 2, 310.00, 620.00);

-- --------------------------------------------------------

--
-- Table structure for table `stock_receiving`
--

CREATE TABLE `stock_receiving` (
  `receive_id` int(11) NOT NULL,
  `receive_date` datetime DEFAULT current_timestamp(),
  `user_id` int(11) DEFAULT NULL,
  `supplier` varchar(100) DEFAULT NULL,
  `total_amount` decimal(10,2) DEFAULT NULL,
  `notes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `suppliers`
--

CREATE TABLE `suppliers` (
  `supplier_id` int(11) NOT NULL,
  `name` varchar(150) NOT NULL,
  `contact` varchar(100) DEFAULT NULL,
  `address` varchar(255) DEFAULT NULL,
  `created_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `suppliers`
--

INSERT INTO `suppliers` (`supplier_id`, `name`, `contact`, `address`, `created_at`) VALUES
(1, 'National Book Store Distributors', '(02) 8888-1234', 'Manila, Philippines', '2026-05-11 14:17:34'),
(2, 'Anvil Publishing', '(02) 7700-5678', 'Mandaluyong, Philippines', '2026-05-11 14:17:34'),
(3, 'Rex Book Store Supply', '(032) 253-9900', 'Cebu City, Philippines', '2026-05-11 14:17:34');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `user_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `full_name` varchar(100) NOT NULL,
  `email` varchar(100) NOT NULL,
  `role` enum('Admin','Staff') NOT NULL DEFAULT 'Staff',
  `is_active` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`user_id`, `username`, `password`, `full_name`, `email`, `role`, `is_active`, `created_at`) VALUES
(1, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'System Administrator', 'admin@bookstore.com', 'Admin', 1, '2026-05-10 11:35:41'),
(2, 'jen', '99187e1fd47e621e04bd1cbbd060b779f8c06c0b8680dd2536711f2e95896dc9', 'Jenny A.  Diones', 'jennydiones@gmail.com', 'Staff', 1, '2026-05-10 15:13:30'),
(3, 'juan', 'ed08c290d7e22f7bb324b15cbadce35b0b348564fd2d5f95752388d86d71bcca', 'Juan Dela Cruz', 'juan@gmail.com', '', 1, '2026-05-11 03:06:35'),
(4, 'nathalla', '03c155b1e5986110959fa812d1338baf7b6cd304e74c8b05ead451b36f003d8f', 'Nathalla Diones', 'athalla@gmail.com', 'Staff', 1, '2026-05-11 03:07:34'),
(5, 'justine', 'cce4bcea63748e8cc0231e0bd1bd37347c061dbcb5e4284f844e5689fc97a646', 'justine bieber', 'bieber@yahoo.com', '', 0, '2026-05-11 05:43:27');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `authors`
--
ALTER TABLE `authors`
  ADD PRIMARY KEY (`author_id`);

--
-- Indexes for table `books`
--
ALTER TABLE `books`
  ADD PRIMARY KEY (`book_id`);

--
-- Indexes for table `book_authors`
--
ALTER TABLE `book_authors`
  ADD PRIMARY KEY (`book_id`,`author_id`),
  ADD KEY `author_id` (`author_id`);

--
-- Indexes for table `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`category_id`);

--
-- Indexes for table `po_items`
--
ALTER TABLE `po_items`
  ADD PRIMARY KEY (`po_item_id`),
  ADD KEY `po_id` (`po_id`),
  ADD KEY `book_id` (`book_id`);

--
-- Indexes for table `publishers`
--
ALTER TABLE `publishers`
  ADD PRIMARY KEY (`publisher_id`);

--
-- Indexes for table `purchase_orders`
--
ALTER TABLE `purchase_orders`
  ADD PRIMARY KEY (`po_id`),
  ADD KEY `supplier_id` (`supplier_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `sales`
--
ALTER TABLE `sales`
  ADD PRIMARY KEY (`sale_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Indexes for table `sale_items`
--
ALTER TABLE `sale_items`
  ADD PRIMARY KEY (`item_id`),
  ADD KEY `sale_id` (`sale_id`);

--
-- Indexes for table `stock_receiving`
--
ALTER TABLE `stock_receiving`
  ADD PRIMARY KEY (`receive_id`);

--
-- Indexes for table `suppliers`
--
ALTER TABLE `suppliers`
  ADD PRIMARY KEY (`supplier_id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `authors`
--
ALTER TABLE `authors`
  MODIFY `author_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `books`
--
ALTER TABLE `books`
  MODIFY `book_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `categories`
--
ALTER TABLE `categories`
  MODIFY `category_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `po_items`
--
ALTER TABLE `po_items`
  MODIFY `po_item_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `publishers`
--
ALTER TABLE `publishers`
  MODIFY `publisher_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `purchase_orders`
--
ALTER TABLE `purchase_orders`
  MODIFY `po_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `sales`
--
ALTER TABLE `sales`
  MODIFY `sale_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT for table `sale_items`
--
ALTER TABLE `sale_items`
  MODIFY `item_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT for table `stock_receiving`
--
ALTER TABLE `stock_receiving`
  MODIFY `receive_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `suppliers`
--
ALTER TABLE `suppliers`
  MODIFY `supplier_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `book_authors`
--
ALTER TABLE `book_authors`
  ADD CONSTRAINT `book_authors_ibfk_1` FOREIGN KEY (`book_id`) REFERENCES `books` (`book_id`),
  ADD CONSTRAINT `book_authors_ibfk_2` FOREIGN KEY (`author_id`) REFERENCES `authors` (`author_id`);

--
-- Constraints for table `po_items`
--
ALTER TABLE `po_items`
  ADD CONSTRAINT `po_items_ibfk_1` FOREIGN KEY (`po_id`) REFERENCES `purchase_orders` (`po_id`) ON DELETE CASCADE,
  ADD CONSTRAINT `po_items_ibfk_2` FOREIGN KEY (`book_id`) REFERENCES `books` (`book_id`);

--
-- Constraints for table `purchase_orders`
--
ALTER TABLE `purchase_orders`
  ADD CONSTRAINT `purchase_orders_ibfk_1` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`),
  ADD CONSTRAINT `purchase_orders_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`);

--
-- Constraints for table `sales`
--
ALTER TABLE `sales`
  ADD CONSTRAINT `sales_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`);

--
-- Constraints for table `sale_items`
--
ALTER TABLE `sale_items`
  ADD CONSTRAINT `sale_items_ibfk_1` FOREIGN KEY (`sale_id`) REFERENCES `sales` (`sale_id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
