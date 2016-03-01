/* init the mysql userid=root;psw=12345;*/
/*
Navicat MySQL Data Transfer

Source Server         : SmallHoneybee
Source Server Version : 50548
Source Host           : localhost:3306
Source Database       : smallhoneybee

Target Server Type    : MYSQL
Target Server Version : 50548
File Encoding         : 65001

Date: 2016-03-01 21:50:58
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for category
-- ----------------------------
DROP TABLE IF EXISTS `category`;
CREATE TABLE `category` (
  `CategoryId` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `CategoryNo` varchar(20) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`CategoryId`),
  UNIQUE KEY `Index_Category_Name` (`Name`),
  KEY `Index_Category_CategoryNo` (`CategoryNo`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for daybook
-- ----------------------------
DROP TABLE IF EXISTS `daybook`;
CREATE TABLE `daybook` (
  `DayBookId` int(11) NOT NULL AUTO_INCREMENT,
  `DayBookDate` datetime NOT NULL,
  `DayBookType` tinyint(4) NOT NULL,
  `HowManey` float DEFAULT NULL,
  `Description` text,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `SettlementMen` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`DayBookId`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for poitem
-- ----------------------------
DROP TABLE IF EXISTS `poitem`;
CREATE TABLE `poitem` (
  `POItemId` int(11) NOT NULL AUTO_INCREMENT,
  `PurchaseOrderId` int(11) NOT NULL,
  `ProduceId` int(11) NOT NULL,
  `POStatusCategory` tinyint(4) NOT NULL,
  `PriceOrdered` float(10,2) DEFAULT '0.00',
  `PriceReceived` float(10,2) DEFAULT '0.00',
  `QuantityOrdered` float(10,2) DEFAULT '0.00',
  `QuantityReceived` float(10,2) DEFAULT '0.00',
  `Description` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`POItemId`),
  KEY `Index_POItem_ProduceId` (`ProduceId`) USING BTREE,
  KEY `Index_POItem_PurchaseOrderId` (`PurchaseOrderId`) USING BTREE,
  CONSTRAINT `FK_POItem_Produce_ProduceId_ProduceId` FOREIGN KEY (`ProduceId`) REFERENCES `produce` (`ProduceId`) ON DELETE NO ACTION,
  CONSTRAINT `FK_POItem_PurchaseOrder_PurchaseOrderId_PurchaseOrderId` FOREIGN KEY (`PurchaseOrderId`) REFERENCES `purchaseorder` (`PurchaseOrderId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=625 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for produce
-- ----------------------------
DROP TABLE IF EXISTS `produce`;
CREATE TABLE `produce` (
  `ProduceId` int(11) NOT NULL AUTO_INCREMENT,
  `ProduceNo` varchar(15) NOT NULL,
  `BarCode` varchar(20) DEFAULT NULL,
  `CategoryId` int(2) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Alias` varchar(100) DEFAULT NULL,
  `PackSize` varchar(30) DEFAULT NULL,
  `RetailPrice` float(10,2) NOT NULL DEFAULT '0.00',
  `DiscountRate` float NOT NULL DEFAULT '1',
  `LowestDiscountRate` float DEFAULT NULL,
  `FactoryPrice` float(10,2) NOT NULL DEFAULT '0.00',
  `OrderSize` varchar(30) DEFAULT NULL,
  `PackFee` float(10,2) DEFAULT '0.00',
  `ProcessFee` float(10,2) DEFAULT '0.00',
  `Description` varchar(255) DEFAULT NULL,
  `Quantity` float(10,2) NOT NULL,
  `Enable` bit(1) NOT NULL,
  `HowUsed` tinyint(2) DEFAULT NULL,
  `HowPurchased` tinyint(2) DEFAULT NULL,
  `PurchasedUsedRatio` float DEFAULT '1',
  `LastOrderDate` datetime DEFAULT NULL,
  `ExpirationDate` float DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ProduceId`),
  UNIQUE KEY `Index_Produce_Unique_ProduceNo` (`ProduceNo`) USING BTREE,
  UNIQUE KEY `Index_Produce_Unique_BarCode` (`BarCode`) USING BTREE,
  KEY `Index_Produce_Name` (`Name`),
  KEY `Index_Produce_CategoryId` (`CategoryId`) USING BTREE,
  CONSTRAINT `FK_Produce_Category_CategoryId_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `category` (`CategoryId`) ON DELETE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=159 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for producelog
-- ----------------------------
DROP TABLE IF EXISTS `producelog`;
CREATE TABLE `producelog` (
  `ProduceLogId` bigint(20) NOT NULL AUTO_INCREMENT,
  `ProduceId` int(11) NOT NULL,
  `OldValue` text,
  `NewValue` text,
  `DateChanged` datetime NOT NULL,
  `ChangedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ProduceLogId`),
  KEY `Index_ProduceLog_ProduceId` (`ProduceId`) USING BTREE,
  KEY `Index_ProduceLog_DateChanged` (`DateChanged`) USING BTREE,
  CONSTRAINT `FK_ProduceLog_Produce_ProduceId_produceId` FOREIGN KEY (`ProduceId`) REFERENCES `produce` (`ProduceId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for purchaseorder
-- ----------------------------
DROP TABLE IF EXISTS `purchaseorder`;
CREATE TABLE `purchaseorder` (
  `PurchaseOrderId` int(11) NOT NULL AUTO_INCREMENT,
  `POContractNo` varchar(20) NOT NULL,
  `PurchaseOrderNo` varchar(20) NOT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `OriginatorId` int(11) NOT NULL,
  `POStatusCategory` tinyint(2) NOT NULL,
  `DateOriginated` date DEFAULT NULL,
  `DateReceived` date DEFAULT NULL,
  `DateCompleted` date DEFAULT NULL,
  `TotalAmount` float(10,2) DEFAULT '0.00',
  `TotalTax` float(10,2) DEFAULT '0.00',
  `TotalShipping` float(10,2) DEFAULT '0.00',
  `TotalOther` float(10,2) DEFAULT '0.00',
  `GrandTotal` float(10,2) DEFAULT '0.00',
  `Description` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PurchaseOrderId`),
  UNIQUE KEY `Index_PurchaseOrder_PurchaseOrderNo` (`PurchaseOrderNo`) USING BTREE,
  UNIQUE KEY `Index_PurchaseOrder_POConractNo` (`POContractNo`),
  KEY `Index_PurchaseOrder_DateOriginated` (`DateOriginated`) USING BTREE,
  KEY `Index_PurchaseOrder_OriginatorId` (`OriginatorId`),
  CONSTRAINT `FK_PurchaseOrder_User_OriginatorId_UserId` FOREIGN KEY (`OriginatorId`) REFERENCES `user` (`UserId`) ON DELETE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for saleorder
-- ----------------------------
DROP TABLE IF EXISTS `saleorder`;
CREATE TABLE `saleorder` (
  `SaleOrderId` int(11) NOT NULL AUTO_INCREMENT,
  `SaleOrderNo` varchar(20) NOT NULL,
  `Name` varchar(100) DEFAULT NULL,
  `SaleOrderStatus` tinyint(4) NOT NULL,
  `OriginUserId` int(11) DEFAULT NULL,
  `PurchaseOrderUserId` int(11) DEFAULT NULL,
  `HowBalance` tinyint(4) NOT NULL DEFAULT '0',
  `DateOriginated` datetime DEFAULT NULL,
  `DateCompleted` datetime DEFAULT NULL,
  `ProduceCost` float(10,2) DEFAULT '0.00',
  `FavorableCost` float(10,2) DEFAULT '0.00',
  `OtherCost` float(10,2) DEFAULT '0.00',
  `TotalCost` float(10,2) DEFAULT '0.00',
  `Description` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `ProduceTotalCount` float(10,2) DEFAULT '0.00',
  `ProduceTotalOriginal` float(10,2) DEFAULT '0.00',
  `ProduceTotalDiscount` float(10,2) DEFAULT '0.00',
  PRIMARY KEY (`SaleOrderId`),
  UNIQUE KEY `Index_SaleOrder_SaleOrderNo` (`SaleOrderNo`),
  KEY `Index_SaleOrder_DateOriginated` (`DateOriginated`),
  KEY `Index_SaleOrder_OriginUserId` (`OriginUserId`) USING BTREE,
  KEY `Index_SaleOrder_PurchaseOrderUserId` (`PurchaseOrderUserId`) USING BTREE,
  CONSTRAINT `FK_Produce_SaleOrder_OriginUserId_UserId` FOREIGN KEY (`OriginUserId`) REFERENCES `user` (`UserId`) ON DELETE NO ACTION,
  CONSTRAINT `FK_Produce_SaleOrder_PurchaseOrderUserId_UserId` FOREIGN KEY (`PurchaseOrderUserId`) REFERENCES `user` (`UserId`) ON DELETE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for solog
-- ----------------------------
DROP TABLE IF EXISTS `solog`;
CREATE TABLE `solog` (
  `SOLogId` bigint(20) NOT NULL AUTO_INCREMENT,
  `SaleOrderId` int(11) NOT NULL,
  `OldValue` text,
  `NewValue` text,
  `DateChanged` datetime NOT NULL,
  `ChangedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SOLogId`),
  KEY `Index_SaleOrder_SaleOrderId` (`SaleOrderId`) USING BTREE,
  KEY `Index_SOLog_DateChanged` (`DateChanged`) USING BTREE,
  CONSTRAINT `FK_SaleOrder_SOLog_SaleOrderId_SaleOrderId` FOREIGN KEY (`SaleOrderId`) REFERENCES `saleorder` (`SaleOrderId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for soproduce
-- ----------------------------
DROP TABLE IF EXISTS `soproduce`;
CREATE TABLE `soproduce` (
  `SOProduceId` int(11) NOT NULL AUTO_INCREMENT,
  `SaleOrderId` int(11) NOT NULL,
  `ProduceId` int(11) DEFAULT NULL,
  `DiscountRate` float NOT NULL DEFAULT '1',
  `DateSaled` datetime DEFAULT NULL,
  `RetailPrice` float DEFAULT NULL,
  `CostPerUnit` float DEFAULT NULL,
  `Quantity` float DEFAULT NULL,
  `SOProduceStatusCategory` tinyint(4) NOT NULL DEFAULT '1',
  `Description` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SOProduceId`),
  KEY `Index_SOProduce_SaleOrderId` (`SaleOrderId`),
  KEY `Index_SOProduce_ProduceId` (`ProduceId`) USING BTREE,
  CONSTRAINT `FK_SOProduce_Produce_ProduceId_ProduceId` FOREIGN KEY (`ProduceId`) REFERENCES `produce` (`ProduceId`) ON DELETE NO ACTION,
  CONSTRAINT `Fk_SOProduce_SaleOrder_SaleOrderId_SaleOrderId` FOREIGN KEY (`SaleOrderId`) REFERENCES `saleorder` (`SaleOrderId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for systemsetting
-- ----------------------------
DROP TABLE IF EXISTS `systemsetting`;
CREATE TABLE `systemsetting` (
  `SettingId` int(11) NOT NULL AUTO_INCREMENT,
  `SettingCode` char(4) NOT NULL,
  `SettingValue` varchar(255) NOT NULL,
  `IsEnable` bit(1) NOT NULL,
  `Description` text,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`SettingId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `UserType` tinyint(4) NOT NULL,
  `UserNo` varchar(50) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Login` varchar(255) NOT NULL,
  `PasswordSalt` varchar(255) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `CashBalance` float(10,0) NOT NULL DEFAULT '0',
  `CashFee` float NOT NULL DEFAULT '0',
  `CashTotal` float NOT NULL DEFAULT '0',
  `MemberPoints` float NOT NULL DEFAULT '0',
  `Phone` varchar(20) NOT NULL,
  `Address` varchar(100) DEFAULT NULL,
  `Weichat` varchar(50) DEFAULT NULL,
  `QqNo` varchar(50) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Enable` bit(1) NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(100) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(100) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Sex` tinyint(4) NOT NULL,
  `DateOfBirth` datetime DEFAULT NULL,
  `MemberType` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`UserId`),
  UNIQUE KEY `Index_User_Unique_UserNo` (`UserNo`) USING BTREE,
  UNIQUE KEY `Index_User_Unique_Phone` (`Phone`) USING BTREE,
  KEY `Index_User_Name` (`Name`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for userlog
-- ----------------------------
DROP TABLE IF EXISTS `userlog`;
CREATE TABLE `userlog` (
  `UserLogId` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `OldValue` text,
  `NewValue` text,
  `DateChanged` datetime NOT NULL,
  `ChangedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserLogId`),
  KEY `Index_User_UserId` (`UserId`),
  KEY `Index_User_DateChanged` (`DateChanged`),
  CONSTRAINT `FK_UserLog_User_UserId_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
