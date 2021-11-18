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

INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-01 08:15:00', 'Teste 1', 'Teste 1');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-01 08:30:00', 'Teste 2', 'Teste 2');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-01 08:50:00', 'Teste 3', 'Teste 3');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-01 13:00:00', 'Teste 4', 'Teste 4');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-02 08:00:00', 'Teste 5', 'Teste 5');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-02 08:15:00', 'Teste 6', 'Teste 6');

COMMIT;

CREATE DATABASE `RS.Log.ProjectB`;

CREATE USER 'ProjectB'@localhost IDENTIFIED BY 'ProjectB@123';
GRANT USAGE ON *.* TO 'ProjectB'@'%' IDENTIFIED BY 'ProjectB@123';
GRANT ALL privileges ON `RS.Log.ProjectB`.* TO 'ProjectB'@'%';
FLUSH PRIVILEGES;


USE `RS.Log.ProjectB`;

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

INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-10 08:15:00', 'Teste 7', 'Teste 7');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-10 08:30:00', 'Teste 8', 'Teste 8');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-10 08:50:00', 'Teste 9', 'Teste 9');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-10 13:00:00', 'Teste 10', 'Teste 10');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-12 08:00:00', 'Teste 11', 'Teste 11');
INSERT INTO Logs (Id, DateCreated, Message, StackTrace) VALUES (null, '2021-11-12 08:15:00', 'Teste 12', 'Teste 12');

COMMIT;

