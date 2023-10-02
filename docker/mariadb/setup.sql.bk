CREATE DATABASE `RS.Log.ProjectA`;

--To create a new user for DataBase RS.Log.ProjectA
CREATE USER 'ProjectA'@localhost IDENTIFIED BY 'ProjectA@123';
--Only allow access from localhost (this is the most secure and common configuration you will use for a web application):
--GRANT USAGE ON *.* TO 'ProjectA'@localhost IDENTIFIED BY 'ProjectA@123';
--To allow access to MySQL server from any other computer on the network:
GRANT USAGE ON *.* TO 'ProjectA'@'%' IDENTIFIED BY 'ProjectA@123';
--Grant all privileges to a user on a specific database
--GRANT ALL privileges ON `RS.Log.ProjectA`.* TO 'ProjectA'@localhost;
GRANT ALL privileges ON `RS.Log.ProjectA`.* TO 'ProjectA'@'%';
--To be effective the new assigned permissions you must finish with the following command:
FLUSH PRIVILEGES;

USE `RS.Log.ProjectA`;

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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220214134700_InitialCreate') THEN

    CREATE TABLE `Logs` (
        `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
        `IdProcess` bigint unsigned NOT NULL,
        `LogLevel` SMALLINT NOT NULL,
        `Message` VARCHAR(255) COLLATE utf8_general_ci NOT NULL,
        `Info` longtext COLLATE utf8_general_ci NOT NULL,
        `DateCreated` datetime(6) NOT NULL DEFAULT NOW(),
        CONSTRAINT `PK_Logs` PRIMARY KEY (`Id`, `IdProcess`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220214134700_InitialCreate') THEN

    CREATE INDEX `idx_Logs_DateCreated` ON `Logs` (`DateCreated`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220214134700_InitialCreate') THEN

    CREATE INDEX `idx_Logs_DateCreated-IdProcess` ON `Logs` (`DateCreated`, `IdProcess`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220214134700_InitialCreate') THEN

    CREATE INDEX `idx_Logs_DateCreated-LogLevel` ON `Logs` (`DateCreated`, `LogLevel`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20220214134700_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20220214134700_InitialCreate', '6.0.1');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

