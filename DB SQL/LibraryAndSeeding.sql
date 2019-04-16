--------------------------------------------
-- CREATE DOCUMENT LIBRARY TABLE
-----------------------------------------------

USE [Dev.Angelo.Connect]
GO

/****** Object:  Table [cms].[DocumentLibrary]    Script Date: 8/16/2018 12:41:55 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [cms].[DocumentLibrary](
	[Id] [nvarchar](450) NOT NULL,
	[LibraryType] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[OwnerId] [nvarchar](max) NULL,
 CONSTRAINT [PK_DocumentLibrary] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

--------------------------------------------
-- UPDATE FOLDERS TABLE
-----------------------------------------------
ALTER TABLE [cms].[Folder]
ADD 
	[DocumentLibraryId] nvarchar(450) Not Null,
	IsDeleted Bit Not Null Default(0),
	IsSystemFolder Bit Not Null Default(0);

Go

ALTER TABLE [cms].[Folder]  WITH CHECK ADD  CONSTRAINT [FK_Folder_DocumentLibrary_DocumentLibraryId] FOREIGN KEY([DocumentLibraryId])
REFERENCES [cms].[DocumentLibrary] ([Id])
GO

ALTER TABLE [cms].[Folder] CHECK CONSTRAINT [FK_Folder_DocumentLibrary_DocumentLibraryId]
GO


--------------------------------------------
-- UPDATE FOLDERITEM TABLE
-----------------------------------------------
ALTER TABLE [cms].[FolderItem]
ADD 
	IsDeleted Bit Not Null Default(0);

Go



----------------------------------------------
-- DATA MANIPULATION
-----------------------------------------------

DECLARE @Users TABLE (
	rowId int IDENTITY(1,1) PRIMARY KEY,
	UserId varchar(450) not null,
	ClientId varchar(450))

DECLARE @Clients TABLE (
	rowId int IDENTITY(1,1) PRIMARY KEY,
	ClientId varchar(450))

DECLARE @Sites TABLE (
	rowId int IDENTITY(1,1) PRIMARY KEY,
	SiteId varchar(450) not null,
	ClientId varchar(450))

DECLARE @RowsToProcess int
DECLARE @CurrentRow int
DECLARE @UserIdValue varchar(450)
DECLARE @SiteIdValue varchar(450)
DECLARE @ClientIdValue varchar(450)

--****************** Get users, clients, sites to start creating document libraries ***********
--***  SITES
--*******************
SET @CurrentRow = 0

Insert into @Sites (siteid, clientid)
select id, ClientId
from cms.Site

set @RowsToProcess = @@ROWCOUNT

WHILE @CurrentRow<@RowsToProcess
BEGIN
    SET @CurrentRow=@CurrentRow+1
    
	
	SELECT 
        @SiteIdValue = s.SiteId, @ClientIdValue = s.ClientId
    FROM  @Sites S
    WHERE RowID= @CurrentRow

    --do your thing here--
	--insert into document library
	IF NOT EXISTS (SELECT * FROM cms.DocumentLibrary WHERE OwnerId = @SiteIdValue and LibraryType = 'Site')
			INSERT INTO [cms].[DocumentLibrary]
				   ([Id]
				   ,[LibraryType]
				   ,[Location]
				   ,[OwnerId])
			 VALUES
				   (NEWID()
				   ,'Site'
				   ,'clients/' + @ClientIdValue + '/sites/' + @SiteIdValue + '/'
				   ,@SiteIdValue)

END


--****************** Get users, clients, sites to start creating document libraries ***********
--***  CLIENTS
--*******************

SET @CurrentRow = 0

Insert into @Clients (clientid)
select id
from cms.Client

set @RowsToProcess = @@ROWCOUNT

WHILE @CurrentRow<@RowsToProcess
BEGIN
    SET @CurrentRow=@CurrentRow+1
    
	
	SELECT 
        @ClientIdValue = c.ClientId
    FROM  @Clients c
    WHERE RowID= @CurrentRow

    --do your thing here--
	--insert into document library
	IF NOT EXISTS (SELECT * FROM cms.DocumentLibrary WHERE OwnerId = @ClientIdValue and LibraryType = 'Client')
			INSERT INTO [cms].[DocumentLibrary]
				   ([Id]
				   ,[LibraryType]
				   ,[Location]
				   ,[OwnerId])
			 VALUES
				   (NEWID()
				   ,'Client'
				   ,'clients/' + @ClientIdValue + '/'
				   ,@ClientIdValue)

END




--****************** Get users, clients, sites to start creating document libraries ***********
--***  USERS
--*******************

SET @CurrentRow = 0

Insert into @Users (UserId, ClientId)
select u.id, d.TenantId
from [Dev.Angelo.Identity].auth.[User] U
	INNER JOIN  [Dev.Angelo.Identity].auth.Directory D ON D.Id = U.DirectoryId

set @RowsToProcess = @@ROWCOUNT

WHILE @CurrentRow<@RowsToProcess
BEGIN
    SET @CurrentRow=@CurrentRow+1
    
	
	SELECT 
        @ClientIdValue = u.ClientId, @UserIdValue = u.UserId
    FROM  @Users u
    WHERE RowID= @CurrentRow

    --do your thing here--
	--insert into document library
	IF NOT EXISTS (SELECT * FROM cms.DocumentLibrary WHERE OwnerId = @UserIdValue and LibraryType = 'User')
			INSERT INTO [cms].[DocumentLibrary]
				   ([Id]
				   ,[LibraryType]
				   ,[Location]
				   ,[OwnerId])
			 VALUES
				   (NEWID()
				   ,'User'
				   ,'clients/' + @ClientIdValue + '/users/' + @UserIdValue + '/'
				   ,@UserIdValue)

END



--***************************************************-
--  UPDATE FOLDERS FK TO LIBRARIES
--*****************************************************

UPDATE F
SET F.DocumentLibraryId = d.Id
--SELECT f.DocumentLibraryId, f.Title, d.OwnerId, d.id, d.librarytype
FROM [Dev.Angelo.Connect].cms.Folder F
	INNER JOIN [Dev.Angelo.Connect].cms.DocumentLibrary D on d.OwnerId = f.OwnerId



GO

--***************************************************-
--  ENSURES TRASH FOLDER IS CREATED FOR EACH LIBRARY
--*****************************************************
DECLARE @totalrootfolders int
DECLARE @ROW INT
DECLARE @TITLE VARCHAR(450)
DECLARE @PARENTID VARCHAR(450)
DECLARE @LIBRARYID VARCHAR(450)

DECLARE @RootsForLibraries TABLE (
	NumId int identity(1,1) primary key not null,
	[Id] [nvarchar](450) NOT NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[DocumentLibraryId] [nvarchar](450) NULL,
	[DocumentType] [nvarchar](max) NULL,
	[FolderFlags] [int] NOT NULL,
	[FolderType] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsSystemFolder] [bit] NOT NULL,
	[OwnerId] [nvarchar](max) NULL,
	[OwnerLevel] [int] NOT NULL,
	[ParentId] [nvarchar](450) NULL,
	[Title] [nvarchar](max) NULL
)


INSERT INTO @RootsForLibraries 
           ([Id]
           ,[CreatedBy]
           ,[DocumentLibraryId]
           ,[DocumentType]
           ,[FolderFlags]
           ,[FolderType]
           ,[IsDeleted]
           ,[IsSystemFolder]
           ,[OwnerId]
           ,[OwnerLevel]
           ,[ParentId]
           ,[Title])
SELECT [Id]
      ,[CreatedBy]
      ,[DocumentLibraryId]
      ,[DocumentType]
      ,[FolderFlags]
      ,[FolderType]
      ,[IsDeleted]
      ,[IsSystemFolder]
      ,[OwnerId]
      ,[OwnerLevel]
      ,[ParentId]
      ,[Title]
FROM [cms].[Folder]
WHERE ParentId IS NULL AND TITLE = ''

SET @totalrootfolders = @@ROWCOUNT

set @row = 0

WHILE @ROW<@totalrootfolders
BEGIN
    SET @ROW=@ROW+1
   
		SELECT @TITLE = 'Trash', @PARENTID = Id, @LIBRARYID = DocumentLibraryId
		from @RootsForLibraries where NumId = @ROW

    if not exists (select * from [cms].[Folder] where DocumentLibraryId = @LIBRARYID and ParentId = @PARENTID and Title = 'Trash')
	begin
		
			INSERT INTO [cms].[Folder]
				   ([Id]
				   ,[CreatedBy]
				   ,[DocumentLibraryId]
				   ,[DocumentType]
				   ,[FolderFlags]
				   ,[FolderType]
				   ,[IsDeleted]
				   ,[IsSystemFolder]
				   ,[OwnerId]
				   ,[OwnerLevel]
				   ,[ParentId]
				   ,[Title])
			 select NEWID()
				   ,[CreatedBy]
				   ,[DocumentLibraryId]
				   ,[DocumentType]
				   ,[FolderFlags]
				   ,[FolderType]
				   ,[IsDeleted]
				   ,1
				   ,[OwnerId]
				   ,[OwnerLevel]
				   ,ID
				   ,'Trash' 
			 from @RootsForLibraries where NumId = @ROW

	end
		
    
End





--**********************************************************************
--    SCRIPT FOR MOVING PHYSICAL FILES TO THE NEW LIBRARY STRUCTURE
--*********************************************************************


select FI.DocumentId as [FileName],
	  'wwwroot\usersfolder\' + FI.DocumentId as [CurrentDriveLocation],
	  'wwwroot\' + L.Location + FI.DocumentId as [NewDriveLocation]
from [Dev.Angelo.Connect].[cms].[DocumentLibrary] L
	inner join [Dev.Angelo.Connect].[cms].[Folder] F on f.DocumentLibraryId = L.Id
	inner join [Dev.Angelo.Connect].[cms].[FolderItem] FI on FI.FolderId = F.Id