SET NOCOUNT ON

DECLARE @CURRENT_VERSION_KEY NVARCHAR(150) = 'Jobs.Schema';
DECLARE @CURRENT_VERSION_VALUE NVARCHAR(32);
DECLARE @TARGET_VERSION_VALUE NVARCHAR(32);

SET @TARGET_VERSION_VALUE = '${version}';

PRINT 'Installing Jobs SQL objects...';


-- Create the database schema if it doesn't exists
IF NOT EXISTS (SELECT [schema_id] FROM [sys].[schemas] WHERE [name] = '${schema}')
BEGIN
	EXEC (N'CREATE SCHEMA [${schema}]');
	PRINT 'Created database schema [${schema}]';
END


DECLARE @SCHEMA_ID int;
SELECT @SCHEMA_ID = [schema_id] FROM [sys].[schemas] WHERE [name] = '${schema}';


-- Create the version meta table if not exists
IF NOT EXISTS(
	SELECT [object_id] FROM [sys].[tables]
	WHERE [name] = '${keystore}' AND [schema_id] = @SCHEMA_ID
)
BEGIN
	CREATE TABLE [${schema}].[${keystore}](
		[Key] NVARCHAR(150) NOT NULL,
		[Value] NVARCHAR(4000) NULL,
		CONSTRAINT [PK_${keystore}] PRIMARY KEY CLUSTERED ([Key] ASC)
	);

	PRINT 'Created table [${schema}].[${keystore}]';
END



SELECT @CURRENT_VERSION_VALUE = CAST([Value] AS NVARCHAR(32)) FROM [${schema}].[${keystore}] 
WHERE [Key] = @CURRENT_VERSION_KEY



PRINT 'Registered Jobs schema version: ' + 
	CASE @CURRENT_VERSION_VALUE 
		WHEN NULL THEN 'none' 
		ELSE @CURRENT_VERSION_VALUE 
	END;




IF @CURRENT_VERSION_VALUE IS NULL
BEGIN
	PRINT 'Installing schema version 1';

	-- Create job tables
	CREATE TABLE [${schema}].[DelayedJobs] (
		[Id]   NVARCHAR (128) NOT NULL,
		[Data] NVARCHAR (MAX) NULL,
		[Due]  DATETIME       NULL,

		CONSTRAINT [PK_DelayedJobs] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
	PRINT 'Created table [${schema}].[DelayedJobs]';


	CREATE TABLE [${schema}].[CronJobs] (
		[Id]       NVARCHAR (128) NOT NULL,
		[Name]     NVARCHAR (128) NOT NULL UNIQUE,
		[TypeName] NVARCHAR (MAX) NULL,
		[Cron]     NVARCHAR (MAX) NULL,
		[LastRun]  DATETIME       NOT NULL,

		CONSTRAINT [PK_CronJobs] PRIMARY KEY CLUSTERED ([Id] ASC)
	);
	PRINT 'Created table [${schema}].[CronJobs]';


	-- Register version code in meta keystore
	SET @CURRENT_VERSION_VALUE = '1.0.0';


	UPDATE [${schema}].[${keystore}] SET [Value] = @CURRENT_VERSION_VALUE
	WHERE [Key] = @CURRENT_VERSION_KEY

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [${schema}].[${keystore}] ([Key], [Value]) 
		VALUES (@CURRENT_VERSION_KEY, @CURRENT_VERSION_VALUE)
	END

	PRINT 'Jobs storage schema initialized';

END






