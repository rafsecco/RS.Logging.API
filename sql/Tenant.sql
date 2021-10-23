CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET utf8mb4;

START TRANSACTION;

ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Logs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `TenantId` VARCHAR(100) CHARACTER SET utf8mb4 NOT NULL,
    `DateCreated` datetime(6) NOT NULL,
    `Message` VARCHAR(255) CHARACTER SET utf8mb4 NULL,
    `StackTrace` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Logs` PRIMARY KEY (`Id`)
) CHARACTER SET utf8mb4;

INSERT INTO `Logs` (`Id`, `DateCreated`, `Message`, `StackTrace`, `TenantId`)
VALUES (1, '2021-10-23 07:51:47', 'teste 1', NULL, 'tenant-1');

INSERT INTO `Logs` (`Id`, `DateCreated`, `Message`, `StackTrace`, `TenantId`)
VALUES (2, '2021-10-23 07:51:47', 'teste 2', NULL, 'tenant-2');

INSERT INTO `Logs` (`Id`, `DateCreated`, `Message`, `StackTrace`, `TenantId`)
VALUES (3, '2021-10-23 07:51:47', 'teste 3', NULL, 'tenant-2');

CREATE INDEX `idx_tenantid` ON `Logs` (`DateCreated`, `TenantId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211023105148_Tenant', '5.0.11');

COMMIT;

