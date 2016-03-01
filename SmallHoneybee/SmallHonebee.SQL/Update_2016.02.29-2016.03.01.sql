UPDATE `user` SET UserType=88 WHERE NAME='admin';
UPDATE `user` SET UserType=30 WHERE login='caij';
UPDATE `user` SET UserType=60,PasswordSalt='z5uCU+',PasswordHash='ejV1Q1UrQWExMjM0NQ==' WHERE login='tangqy';
ALTER TABLE `saleorder` ADD ProduceTotalCount FLOAT(10,2) DEFAULT '0.00', ADD ProduceTotalOriginal FLOAT(10,2) DEFAULT '0.00',ADD ProduceTotalDiscount FLOAT(10,2) DEFAULT '0.00';  
ALTER TABLE `user` ADD Sex TINYINT(4) NOT NULL, ADD DateOfBirth datetime, ADD MemberType TINYINT(4);
ALTER TABLE `daybook` ADD SettlementMen VARCHAR(20);

UPDATE saleorder SET ProduceTotalCount = (select count(*) FROM soproduce where soproduce.SaleOrderId=saleorder.SaleOrderId);
UPDATE soproduce SET RetailPrice = (select RetailPrice FROM produce where produce.ProduceId=soproduce.ProduceId);
UPDATE saleorder SET ProduceTotalOriginal = (select sum(soproduce.RetailPrice*soproduce.Quantity) FROM soproduce where soproduce.SaleOrderId=saleorder.SaleOrderId);
UPDATE saleorder SET ProduceTotalDiscount = (select sum((soproduce.RetailPrice-soproduce.CostPerUnit)*soproduce.Quantity) FROM soproduce where soproduce.SaleOrderId=saleorder.SaleOrderId);

