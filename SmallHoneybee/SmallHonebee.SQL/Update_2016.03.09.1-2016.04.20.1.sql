UPDATE UserLog SET NewValue=REPLACE(NewValue, '结算方式:已激活', '结算方式:购物卡') WHERE NewValue like '%结算方式:已激活%';
UPDATE UserLog SET NewValue=REPLACE(NewValue, '结算方式: 已激活', '结算方式:购物卡') WHERE NewValue like '%结算方式: 已激活%';
UPDATE UserLog SET NewValue=REPLACE(NewValue, '结算方式:未激活', '结算方式:现金') WHERE NewValue like '%结算方式:未激活%';
UPDATE UserLog SET NewValue=REPLACE(NewValue, '结算方式: 未激活', '结算方式:现金') WHERE NewValue like '%结算方式: 未激活%';
UPDATE `systemsetting` SET SettingValue='2016.04.20.1' WHERE SettingCode='1000';