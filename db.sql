-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: entrega2
-- ------------------------------------------------------
-- Server version	8.0.39

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `ekintzak`
--

DROP TABLE IF EXISTS `ekintzak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ekintzak` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ErabiltzaileId` int DEFAULT NULL,
  `EkintzaMota` enum('SORTU','ALDATU','EZABATU','BERRESKURATU') NOT NULL,
  `DataEkintza` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `ErabiltzaileId` (`ErabiltzaileId`),
  CONSTRAINT `ekintzak_ibfk_1` FOREIGN KEY (`ErabiltzaileId`) REFERENCES `erabiltzaileak` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ekintzak`
--

LOCK TABLES `ekintzak` WRITE;
/*!40000 ALTER TABLE `ekintzak` DISABLE KEYS */;
INSERT INTO `ekintzak` VALUES (1,4,'ALDATU','2025-10-12 16:27:40'),(2,4,'EZABATU','2025-10-12 16:34:41'),(3,4,'SORTU','2025-10-12 16:35:14'),(4,4,'BERRESKURATU','2025-10-12 16:56:18'),(5,4,'EZABATU','2025-10-12 16:56:26'),(6,4,'BERRESKURATU','2025-10-12 16:56:48'),(7,4,'EZABATU','2025-10-12 16:59:50'),(8,4,'SORTU','2025-10-12 17:00:22'),(9,4,'ALDATU','2025-10-12 17:00:41'),(10,4,'ALDATU','2025-10-12 17:01:14'),(11,4,'ALDATU','2025-10-12 17:01:22'),(12,4,'ALDATU','2025-10-12 17:10:41'),(13,4,'ALDATU','2025-10-12 17:10:52'),(14,4,'ALDATU','2025-10-12 17:11:13'),(15,4,'ALDATU','2025-10-12 17:11:35'),(16,4,'EZABATU','2025-10-12 17:11:49'),(17,4,'BERRESKURATU','2025-10-12 17:11:57'),(18,4,'EZABATU','2025-10-12 17:12:03'),(19,4,'EZABATU','2025-10-12 17:12:08'),(20,4,'ALDATU','2025-10-12 17:26:11'),(21,4,'ALDATU','2025-11-10 19:12:24'),(22,4,'ALDATU','2025-11-10 19:12:40'),(23,4,'ALDATU','2025-11-11 14:12:08'),(24,4,'EZABATU','2025-11-11 14:14:50'),(25,4,'EZABATU','2025-11-11 14:18:06');
/*!40000 ALTER TABLE `ekintzak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `erabiltzaileak`
--

DROP TABLE IF EXISTS `erabiltzaileak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `erabiltzaileak` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `LangileId` int NOT NULL,
  `ErabiltzaileIzena` varchar(50) NOT NULL,
  `Pasahitza` varchar(255) NOT NULL,
  `Ezabatuta` tinyint(1) DEFAULT '0',
  `DataSortu` datetime DEFAULT CURRENT_TIMESTAMP,
  `DataEzabatu` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `ErabiltzaileIzena` (`ErabiltzaileIzena`),
  KEY `LangileId` (`LangileId`),
  CONSTRAINT `erabiltzaileak_ibfk_1` FOREIGN KEY (`LangileId`) REFERENCES `langileak` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `erabiltzaileak`
--

LOCK TABLES `erabiltzaileak` WRITE;
/*!40000 ALTER TABLE `erabiltzaileak` DISABLE KEYS */;
INSERT INTO `erabiltzaileak` VALUES (4,4,'testuser4','testpass4',0,'2025-10-11 14:42:09',NULL);
/*!40000 ALTER TABLE `erabiltzaileak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `langileak`
--

DROP TABLE IF EXISTS `langileak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `langileak` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Izena` varchar(100) NOT NULL,
  `Abizena` varchar(100) NOT NULL,
  `Arduraduna` tinyint(1) DEFAULT '0',
  `Ezabatuta` tinyint(1) DEFAULT '0',
  `DataSortu` datetime DEFAULT CURRENT_TIMESTAMP,
  `DataEzabatu` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `langileak`
--

LOCK TABLES `langileak` WRITE;
/*!40000 ALTER TABLE `langileak` DISABLE KEYS */;
INSERT INTO `langileak` VALUES (2,'Ane','Martinez',1,1,'2025-10-11 14:42:10','2025-10-12 16:59:50'),(3,'Iker','Muguruza',0,0,'2025-10-11 14:42:10',NULL),(4,'Test','User',1,0,'2025-10-11 14:42:10',NULL),(12,'Markel','Bergara',0,0,'2025-10-12 16:35:14',NULL),(13,'Miren','Irizabal',0,0,'2025-10-12 17:00:22',NULL);
/*!40000 ALTER TABLE `langileak` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-13 14:20:30
