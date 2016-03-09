INSERT INTO purchaseorder (
	POContractNo,
	PurchaseOrderNo,
	OriginatorId,
	POStatusCategory,
	DateCompleted,
	CreatedBy,
	CreatedOn,
	LastModifiedBy,
	LastModifiedOn
)
VALUES
	(
		'XL11186098',
		'XL11186098',
		1,
		1,
		'2016-03-03',
		'admin',
		NOW(),
		'admin',
		NOW()
	);
UPDATE purchaseorder SET POContractNo='XL11184436', PurchaseOrderNo='XL11184436',Name='', DateCompleted='2016-01-19' WHERE PurchaseOrderId=1;
UPDATE `systemsetting` SET SettingValue='2016.03.09.1' WHERE SettingCode='1000';