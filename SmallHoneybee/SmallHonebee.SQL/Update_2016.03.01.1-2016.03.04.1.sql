﻿INSERT INTO systemsetting (
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
		'1001',
		'10',
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
		'1002',
		'100',
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
		'1101',
		'北京蜜蜂堂桃园东路店',
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
		'1102',
		'15109248710',
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
		'1103',
		'莲湖区桃园东路店24号',
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
		'1104',
		'SmallHoneybee',
		1,
		'admin',
		NOW(),
		'admin',
		NOW()
	);
UPDATE `systemsetting` SET SettingValue='2016.03.04.1' WHERE SettingCode='1000';