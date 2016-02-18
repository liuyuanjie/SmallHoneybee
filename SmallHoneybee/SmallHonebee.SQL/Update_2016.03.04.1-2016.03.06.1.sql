ALTER TABLE `saleorder` ADD UserRealPrice float(10,2) NOT NULL, ADD UserReturnedPrice float(10,2) NOT NULL;

CREATE TABLE `membercard` (
  `MemberCardId` int(11) NOT NULL,
  `MemberCardNo` varchar(20) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `MemberType` tinyint(4) NOT NULL,
  `MemberMoney` float NOT NULL,
  `DispatchDate` datetime NOT NULL,
  `DispatchUser` int(11) DEFAULT NULL,
  `SurplusMoney` float NOT NULL,
  `IsEnable` bit(1) NOT NULL,
  `PasswordSalt` varchar(255) DEFAULT NULL,
  `PasswordHash` varchar(255) DEFAULT NULL,
  `MemberCardStatus` tinyint(4) NOT NULL,
  `Descript` varchar(255) DEFAULT NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` varchar(50) NOT NULL,
  `LastModifiedOn` datetime NOT NULL,
  `LastModifiedBy` varchar(50) NOT NULL,
  `RowVersion` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`MemberCardId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `usermembercard` (
  `UserMemberCardId` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `MemberCardId` int(11) NOT NULL,
  PRIMARY KEY (`UserMemberCardId`),
  KEY `FK_UserMemberCard_User_UserId_UserId` (`UserId`),
  KEY `FK_UserMemberCard_MemberCard_MemberCardId_MemberCardId` (`MemberCardId`),
  CONSTRAINT `FK_UserMemberCard_MemberCard_MemberCardId_MemberCardId` FOREIGN KEY (`MemberCardId`) REFERENCES `membercard` (`MemberCardId`) ON DELETE NO ACTION,
  CONSTRAINT `FK_UserMemberCard_User_UserId_UserId` FOREIGN KEY (`UserId`) REFERENCES `user` (`UserId`) ON DELETE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO systemsetting (
	SettingCode,
	SettingValue,
	IsEnable,
	CreatedBy,
	CreatedOn,
	LastModifiedBy,
	LastModifiedOn
)
VALUES
	(
		'1105',
		'400-700-5788',
		1,
		'admin',
		NOW(),
		'admin',
		NOW()
	);

INSERT INTO systemsetting (
	SettingCode,
	SettingValue,
	IsEnable,
	CreatedBy,
	CreatedOn,
	LastModifiedBy,
	LastModifiedOn
)
VALUES
	(
		'1106',
		'www.beehall.com',
		1,
		'admin',
		NOW(),
		'admin',
		NOW()
	);

UPDATE `systemsetting` SET SettingValue=1 WHERE SettingCode='1001';
UPDATE `user` SET MemberPoints=MemberPoints/10;
UPDATE `systemsetting` SET SettingValue='2016.03.06.1' WHERE SettingCode='1000';