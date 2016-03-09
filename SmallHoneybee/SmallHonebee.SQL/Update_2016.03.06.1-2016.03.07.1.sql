
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
		'1003',
		'111111',
		1,
		'admin',
		NOW(),
		'admin',
		NOW()
	);
UPDATE `systemsetting` SET SettingValue='2016.03.07.1' WHERE SettingCode='1000';