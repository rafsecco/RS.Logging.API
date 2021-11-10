CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET utf8mb4;

START TRANSACTION;

CREATE TABLE `Logs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `DateCreated` datetime(6) NOT NULL,
    `Message` VARCHAR(255) COLLATE utf8_general_ci NOT NULL,
    `StackTrace` VARCHAR(255) COLLATE utf8_general_ci NULL,
    CONSTRAINT `PK_Logs` PRIMARY KEY (`Id`)
) COLLATE utf8_general_ci;

CREATE INDEX `idx_Logs_DateCreated` ON `Logs` (`DateCreated`);

CREATE INDEX `idx_Logs_DateCreated-Message` ON `Logs` (`DateCreated`, `Message`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211109175814_CreateDB', '5.0.11');

COMMIT;

