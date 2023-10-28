CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230930224026_InitialCreate') THEN

    CREATE TABLE `TB_Log` (
        `id_Log` bigint unsigned NOT NULL AUTO_INCREMENT,
        `ie_LogLevel` SMALLINT NOT NULL,
        `dt_CreatedAt` datetime(6) NOT NULL DEFAULT NOW(),
        `ds_Message` VARCHAR(255) COLLATE utf8_general_ci NOT NULL,
        `ds_StackTrace` longtext COLLATE utf8_general_ci NULL,
        CONSTRAINT `PK_TB_Log` PRIMARY KEY (`id_Log`)
    ) COLLATE=utf8_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230930224026_InitialCreate') THEN

    CREATE INDEX `idx_Logs_CreatedAt` ON `TB_Log` (`dt_CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230930224026_InitialCreate') THEN

    CREATE INDEX `idx_Logs_CreatedAt-LogLevel` ON `TB_Log` (`dt_CreatedAt`, `ie_LogLevel`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230930224026_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230930224026_InitialCreate', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    ALTER TABLE `TB_Log` DROP INDEX `idx_Logs_CreatedAt-LogLevel`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx_TB_Log_dt_CreatedAt-ie_LogLevel` ON `TB_Log` (`dt_CreatedAt`, `ie_LogLevel`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    ALTER TABLE `TB_Log` DROP INDEX `idx_Logs_CreatedAt`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx_TB_Log_dt_CreatedAt` ON `TB_Log` (`dt_CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    ALTER TABLE `TB_Log` MODIFY COLUMN `ie_LogLevel` SMALLINT UNSIGNED NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    ALTER TABLE `TB_Log` MODIFY COLUMN `id_Log` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE TABLE `TB_LogProcess` (
        `id_LogProcess` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
        `id_Process` INT UNSIGNED NOT NULL,
        `dt_CreatedAt` datetime(6) NOT NULL DEFAULT NOW(),
        `nm_Process` VARCHAR(255) COLLATE utf8_general_ci NULL,
        CONSTRAINT `PK_TB_LogProcess` PRIMARY KEY (`id_LogProcess`)
    ) COLLATE=utf8_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE TABLE `TB_LogProcessDetail` (
        `id_LogProcessDetails` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
        `cd_Process` BIGINT UNSIGNED NOT NULL,
        `ie_LogLevel` SMALLINT UNSIGNED NOT NULL,
        `dt_CreatedAt` datetime(6) NOT NULL DEFAULT NOW(),
        `ds_Message` VARCHAR(255) COLLATE utf8_general_ci NOT NULL,
        `ds_StackTrace` longtext COLLATE utf8_general_ci NULL,
        CONSTRAINT `PK_TB_LogProcessDetail` PRIMARY KEY (`id_LogProcessDetails`),
        CONSTRAINT `FK_TB_LogProcessDetail_TB_LogProcess_cd_Process` FOREIGN KEY (`cd_Process`) REFERENCES `TB_LogProcess` (`id_LogProcess`) ON DELETE CASCADE
    ) COLLATE=utf8_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx-TB_LogProcess_dt_CreatedAt` ON `TB_LogProcess` (`dt_CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx-TB_LogProcess_dt_CreatedAt-id_Process` ON `TB_LogProcess` (`dt_CreatedAt`, `id_Process`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx-TB_LogProcess_dt_CreatedAt1` ON `TB_LogProcessDetail` (`dt_CreatedAt`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `idx-TB_LogProcessDetail_dt_CreatedAt-cd_Process` ON `TB_LogProcessDetail` (`dt_CreatedAt`, `cd_Process`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    CREATE INDEX `IX_TB_LogProcessDetail_cd_Process` ON `TB_LogProcessDetail` (`cd_Process`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20231028132620_LogProcess') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20231028132620_LogProcess', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

