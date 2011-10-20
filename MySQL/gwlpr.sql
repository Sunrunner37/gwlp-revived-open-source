/*
SQLyog Community Edition- MySQL GUI v8.14 
MySQL - 5.0.51a : Database - gwlpr
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`gwlpr` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `gwlpr`;

/*Table structure for table `accounts_masterdata` */

DROP TABLE IF EXISTS `accounts_masterdata`;

CREATE TABLE `accounts_masterdata` (
  `AccountID` int(11) NOT NULL auto_increment,
  `Email` char(64) NOT NULL,
  `Password` char(20) NOT NULL,
  `GroupID` int(11) NOT NULL default '0',
  `IsBanned` tinyint(1) NOT NULL default '0',
  `CharID` int(11) default NULL COMMENT 'The CharID of the last played Char.',
  `LuxonPtsFree` int(11) default '0',
  `LuxonPtsTotal` int(11) default '0',
  `KurzickPtsFree` int(11) default '0',
  `KurzickPtsTotal` int(11) default '0',
  `BalthPtsFree` int(11) default '0',
  `BalthPtsTotal` int(11) default '0',
  `ImperialPtsFree` int(11) default '0',
  `ImperialPtsTotal` int(11) default '0',
  `GuiSettings` blob NOT NULL,
  PRIMARY KEY  (`AccountID`,`Email`)
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;

/*Table structure for table `chars_masterdata` */

DROP TABLE IF EXISTS `chars_masterdata`;

CREATE TABLE `chars_masterdata` (
  `CharID` int(11) NOT NULL auto_increment,
  `CharName` char(20) NOT NULL,
  `AccountID` int(11) NOT NULL,
  `MapID` int(11) NOT NULL,
  `Level` int(11) NOT NULL default '1',
  `ProfessionPrimary` tinyint(4) NOT NULL,
  `ProfessionSecondary` tinyint(4) NOT NULL,
  `IsPvP` tinyint(1) NOT NULL default '0',
  `InventoryGold` int(11) NOT NULL default '0',
  `ExperiencePts` int(11) NOT NULL default '0',
  `AttrPtsFree` int(11) NOT NULL default '0',
  `AttrPtsTotal` int(11) NOT NULL default '0',
  `SkillPtsFree` int(11) NOT NULL default '0',
  `SkillPtsTotal` int(11) NOT NULL default '0',
  `SkillBar` blob,
  `SkillsAvailable` blob,
  `LookCampaign` int(11) NOT NULL,
  `LookFace` int(11) NOT NULL,
  `LookHairColor` int(11) NOT NULL,
  `LookHairStyle` int(11) NOT NULL,
  `LookHeight` int(11) NOT NULL,
  `LookSex` int(11) NOT NULL,
  `LookSkinColor` int(11) NOT NULL,
  `ShowHelm` tinyint(1) NOT NULL default '0',
  `ArmorHead` blob,
  `ArmorChest` blob,
  `ArmorArms` blob,
  `ArmorLegs` blob,
  `ArmorFeet` blob,
  `WeaponPrimary` blob,
  `WeaponSecondary` blob,
  PRIMARY KEY  (`CharID`,`CharName`)
) ENGINE=MyISAM AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;

/*Table structure for table `groups_commands` */

DROP TABLE IF EXISTS `groups_commands`;

CREATE TABLE `groups_commands` (
  `CommandID` int(11) NOT NULL auto_increment,
  `CommandName` char(16) NOT NULL,
  `GroupID` int(11) NOT NULL,
  PRIMARY KEY  (`CommandID`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

/*Table structure for table `groups_masterdata` */

DROP TABLE IF EXISTS `groups_masterdata`;

CREATE TABLE `groups_masterdata` (
  `GroupID` int(11) NOT NULL,
  `GroupName` char(64) NOT NULL,
  `GroupPrefix` char(8) NOT NULL,
  `GroupChatColor` int(11) NOT NULL default '4',
  PRIMARY KEY  (`GroupID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `items_masterdata` */

DROP TABLE IF EXISTS `items_masterdata`;

CREATE TABLE `items_masterdata` (
  `ItemID` int(11) NOT NULL auto_increment,
  `GameItemID` int(11) NOT NULL,
  `GameItemFileID` int(11) NOT NULL,
  `Name` char(28) NOT NULL default 'Unknown Item',
  `ItemType` int(11) NOT NULL default '11',
  PRIMARY KEY  (`ItemID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `items_personaldata` */

DROP TABLE IF EXISTS `items_personaldata`;

CREATE TABLE `items_personaldata` (
  `PersonalItemID` bigint(20) NOT NULL auto_increment,
  `ItemID` int(11) NOT NULL,
  `AccountID` int(11) NOT NULL,
  `CharID` int(11) NOT NULL,
  `DyeColor` int(11) NOT NULL,
  `Flags` int(11) NOT NULL,
  `Quantity` int(11) NOT NULL default '1',
  `Storage` int(11) NOT NULL default '6',
  `Slot` int(11) NOT NULL default '1',
  `CreatorCharID` int(11) NOT NULL,
  `CreatorName` char(20) NOT NULL,
  `Stats` blob,
  PRIMARY KEY  (`PersonalItemID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `maps_masterdata` */

DROP TABLE IF EXISTS `maps_masterdata`;

CREATE TABLE `maps_masterdata` (
  `MapID` int(11) NOT NULL auto_increment COMMENT 'This is not connected to GW!',
  `GameMapID` int(11) NOT NULL,
  `GameMapFileID` int(11) NOT NULL,
  `GameMapName` char(64) NOT NULL default 'UnknownMap',
  PRIMARY KEY  (`MapID`,`GameMapID`,`GameMapFileID`)
) ENGINE=MyISAM AUTO_INCREMENT=729 DEFAULT CHARSET=latin1;

/*Table structure for table `maps_spawns` */

DROP TABLE IF EXISTS `maps_spawns`;

CREATE TABLE `maps_spawns` (
  `SpawnID` int(11) NOT NULL auto_increment,
  `MapID` int(11) NOT NULL,
  `IsOutpost` tinyint(1) NOT NULL default '1',
  `IsPvE` tinyint(1) NOT NULL default '1',
  `TeamSpawnNumber` int(11) default NULL,
  `SpawnX` float NOT NULL,
  `SpawnY` float NOT NULL,
  `SpawnPlane` int(11) NOT NULL default '0',
  `SpawnRadius` float NOT NULL default '0',
  PRIMARY KEY  (`SpawnID`)
) ENGINE=MyISAM AUTO_INCREMENT=426 DEFAULT CHARSET=latin1;

/*Table structure for table `npcs_masterdata` */

DROP TABLE IF EXISTS `npcs_masterdata`;

CREATE TABLE `npcs_masterdata` (
  `NpcID` int(11) NOT NULL,
  `NpcName` char(128) default NULL,
  `NpcFileID` int(11) NOT NULL,
  `ModelHash` blob NOT NULL,
  `Appearance` blob NOT NULL,
  `Scale` int(11) NOT NULL,
  `ProfessionFlags` int(11) NOT NULL,
  PRIMARY KEY  (`NpcID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `npcs_names` */

DROP TABLE IF EXISTS `npcs_names`;

CREATE TABLE `npcs_names` (
  `NameID` int(11) NOT NULL auto_increment,
  `NameHash` blob NOT NULL,
  `Name` char(128) NOT NULL,
  PRIMARY KEY  (`NameID`,`Name`),
  UNIQUE KEY `NameID` (`NameID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*Table structure for table `npcs_spawns` */

DROP TABLE IF EXISTS `npcs_spawns`;

CREATE TABLE `npcs_spawns` (
  `NpcSpawnID` int(11) NOT NULL auto_increment,
  `NpcID` int(11) NOT NULL,
  `NameID` int(11) NOT NULL default '0',
  `MapID` int(11) NOT NULL,
  `SpawnX` float NOT NULL,
  `SpawnY` float NOT NULL,
  `Plane` int(11) NOT NULL,
  `Rotation` float NOT NULL,
  `Speed` float NOT NULL,
  `Level` int(11) NOT NULL,
  `Profession` int(11) NOT NULL,
  `AtOutpost` tinyint(1) NOT NULL,
  `AtPvE` tinyint(1) NOT NULL,
  `TeamID` int(11) NOT NULL,
  `GroupSize` int(11) NOT NULL,
  PRIMARY KEY  (`NpcSpawnID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
