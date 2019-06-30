-- phpMyAdmin SQL Dump
-- version 4.0.4.2
-- http://www.phpmyadmin.net
--
-- Máquina: localhost
-- Data de Criação: 30-Jun-2019 às 22:33
-- Versão do servidor: 5.6.13
-- versão do PHP: 5.4.17

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Base de Dados: `livrariabcc`
--
CREATE DATABASE IF NOT EXISTS `livrariabcc` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
USE `livrariabcc`;

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_Delete`(IN $Id INT(11))
BEGIN
	DELETE FROM Livro WHERE Id = $Id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_Insert`(OUT `$Id` INT(11), IN `$ISBN` VARCHAR(17), IN `$Autor` VARCHAR(100), IN `$Nome` VARCHAR(100), IN `$Preco` DECIMAL(8,2), IN `$DataPublicacao` DATETIME, IN `$ImagemCapa` VARCHAR(1000))
BEGIN
	INSERT INTO Livro
    (`ISBN`, `Autor`, `Nome`, `Preco`, `DataPublicacao`, `ImagemCapa`)
    VALUES
    ($ISBN, $Autor, $Nome, $Preco, $DataPublicacao, $ImagemCapa);
    
    SET $Id = LAST_INSERT_ID();
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_Search`(
	IN `$ISBN` VARCHAR(17), 
	IN `$Autor` VARCHAR(100), 
	IN `$Nome` VARCHAR(100), 
	IN `$Preco` DECIMAL(8,2), 
	IN `$DataPublicacao` DATETIME, 
	IN `$ImagemCapa` VARCHAR(1000)
)
BEGIN
	SELECT * 
    FROM Livro 
    WHERE 
		($ISBN IS NULL OR ISBN = $ISBN)
        AND ($Autor IS NULL OR Autor LIKE CONCAT('%', $Autor, '%'))
        AND ($Nome IS NULL OR Nome LIKE CONCAT('%', $Nome, '%'))
        AND ($Preco IS NULL OR Preco LIKE CONCAT('%', $Preco, '%'))
        AND ($DataPublicacao IS NULL OR DataPublicacao = $DataPublicacao)
        AND ($ImagemCapa IS NULL OR ImagemCapa LIKE CONCAT('%', $ImagemCapa, '%'));
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_SelectAll`()
    NO SQL
SELECT * FROM Livro$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_SelectById`(IN $Id INT(11))
BEGIN
	SELECT * FROM Livro WHERE Id = $Id;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Livro_Update`(INOUT `$Id` INT(11), IN `$ISBN` VARCHAR(17), IN `$Autor` VARCHAR(100), IN `$Nome` VARCHAR(100), IN `$Preco` DECIMAL(8,2), IN `$DataPublicacao` DATETIME, IN `$ImagemCapa` VARCHAR(1000))
BEGIN
	UPDATE Livro
    SET
		ISBN = $ISBN,
        Autor =$Autor,
        Nome = $Nome,
        Preco = $Preco,
        DataPublicacao = $DataPublicacao,
        ImagemCapa = $ImagemCapa
	WHERE
		Id = $Id;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estrutura da tabela `livro`
--

CREATE TABLE IF NOT EXISTS `livro` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ISBN` varchar(17) NOT NULL,
  `Autor` varchar(100) NOT NULL,
  `Nome` varchar(100) NOT NULL,
  `Preco` decimal(8,2) NOT NULL,
  `DataPublicacao` datetime NOT NULL,
  `ImagemCapa` varchar(1000) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=9 ;

--
-- Extraindo dados da tabela `livro`
--

INSERT INTO `livro` (`Id`, `ISBN`, `Autor`, `Nome`, `Preco`, `DataPublicacao`, `ImagemCapa`) VALUES
(5, '978-1-891830-71-6', 'Jeffrey Brown', 'AEIOU or Any Easy Intimacy', '12.00', '2019-01-01 00:00:00', 'aeiou_lg.gif'),
(6, '978-1-60309-025-4', 'Eddie Campbell', ' Alec: The Years Have Pants', '35.00', '2019-06-02 00:00:00', 'alec_cover_sc_lg.jpg'),
(8, '978-1-60309-464-1', 'Campbell Whyte', 'Home Time II: Beyond The Weaving', '29.95', '2020-03-17 00:00:00', 'home_time_ii_cover_lo-res_lg.jpg');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
