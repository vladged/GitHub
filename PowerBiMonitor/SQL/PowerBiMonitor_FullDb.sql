/****** Object:  Table [dbo].[user_permissions]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_permissions](
	[DisplayName] [varchar](250) NULL,
	[UserPrincipalName] [varchar](250) NOT NULL,
	[UserType] [varchar](50) NULL,
	[workspaceid] [varchar](50) NOT NULL,
	[workspacetype] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](250) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
 CONSTRAINT [PK_user_permissions] PRIMARY KEY CLUSTERED 
(
	[UserPrincipalName] ASC,
	[workspaceid] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[workspaces]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[workspaces](
	[id] [varchar](50) NOT NULL,
	[name] [varchar](500) NULL,
	[isReadOnly] [varchar](50) NULL,
	[IsOnDedicatedCapacity] [varchar](50) NULL,
	[CapacityId] [varchar](50) NULL,
	[Description] [varchar](4000) NULL,
	[type] [varchar](50) NULL,
	[state] [varchar](50) NULL,
	[IsOrphaned] [varchar](50) NULL,
	[Accessible] [int] NULL,
	[MasterAccountAccess] [int] NULL,
	[CallerPrincipalName] [varchar](50) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
 CONSTRAINT [PK_workspaces] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[reports]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reports](
	[id] [varchar](50) NOT NULL,
	[reportType] [varchar](50) NULL,
	[name] [varchar](500) NULL,
	[webUrl] [varchar](500) NULL,
	[embedUrl] [varchar](4000) NULL,
	[datasetId] [varchar](50) NULL,
	[workspaceId] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](250) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
	[isOwnedByMe] [varchar](10) NULL,
	[isFromPbix] [varchar](10) NULL,
 CONSTRAINT [PK_reports] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[datasets]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[datasets](
	[id] [varchar](50) NOT NULL,
	[name] [varchar](500) NULL,
	[addRowsAPIEnabled] [varchar](50) NULL,
	[configuredBy] [varchar](50) NULL,
	[isRefreshable] [varchar](50) NULL,
	[isEffectiveIdentityRequired] [varchar](50) NULL,
	[isEffectiveIdentityRolesRequired] [varchar](50) NULL,
	[isOnPremGatewayRequired] [varchar](50) NULL,
	[workspaceid] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](50) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
 CONSTRAINT [PK_datasets] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_workspaces]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[vw_workspaces]
as
select  
[workspaces].[name] as WorkspaceName,
workspaces.id as id,
[isReadOnly],
[IsOnDedicatedCapacity],
[state] as WorkspaceState,
[CapacityId],
[type] as WorkspaceType,
[IsOrphaned],
[Accessible],
[MasterAccountAccess],

numbOfDatasets=(select COUNT(*) from datasets as d where [workspaces].id=d.workspaceid),
numbOfReports=(select COUNT(*) from reports as r where [workspaces].id=r.workspaceId),
numbOfUsers=(select COUNT(*) from [dbo].[user_permissions] as u_p where [workspaces].id=u_p.workspaceId)

from [dbo].[workspaces]
--inner join [dbo].datasets on [workspaces].id=datasets.workspaceid
--inner join [dbo].reports on datasets.id=reports.datasetid
--left join [dbo].[user_permissions] on [workspaces].id=[user_permissions].workspaceid
where [workspaces].[name] not like 'PersonalWorkspace %'
GO
/****** Object:  View [dbo].[vw_WS_DS_REP]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[vw_WS_DS_REP]
as
select  
[workspaces].workspaceName,
workspaces.id as workspaces_id,
workspaces.WorkspaceType,
workspaces.[isReadOnly],
--workspaces.[CallerPrincipalName],
[IsOnDedicatedCapacity],
[numbOfDatasets],
[numbOfReports],
[numbOfUsers],
[WorkspaceState],
[MasterAccountAccess],
[user_permissions].[UserPrincipalName] as workspaces_user,
[user_permissions].[UserType] as [UserType],

datasets.[name] as [datasets_name],
datasets.id as datasets_id,
datasets.[configuredBy] as datasets_configuredBy,
datasets.[addRowsAPIEnabled],
datasets.[isRefreshable],
iif((addRowsAPIEnabled='true'),'Streaming',iif(isRefreshable='false','Direct Query','Import')) as dataset_type,
reports.[name] as reports_name,
reports.id as reports_id,
[reports].[reportType]
--[vw_NumberOfDatasetsInWs].[NumbOfDatasets]

from [dbo].[vw_workspaces] as workspaces
left join [dbo].datasets on [workspaces].id=datasets.workspaceid
left join [dbo].reports on datasets.id=reports.datasetid
left join [dbo].[user_permissions] on [workspaces].id=[user_permissions].workspaceid
--left join [dbo].[vw_NumberOfDatasetsInWs] on [workspaces].id=[vw_NumberOfDatasetsInWs].workspaceid
where [workspaces].WorkspaceName not like 'PersonalWorkspace%'
GO
/****** Object:  View [dbo].[vw_datasets]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE view [dbo].[vw_datasets]
as
SELECT [id]
      ,[name] as DatasetName
      ,[addRowsAPIEnabled]
      ,[configuredBy]
      ,[isRefreshable]
      ,[isEffectiveIdentityRequired]
      ,[isEffectiveIdentityRolesRequired]
      ,[isOnPremGatewayRequired]
      ,[workspaceid]
      ,[CallerPrincipalName]
      ,[LastTimeUpdated]
     -- ,[Accesible]
	  ,iif((addRowsAPIEnabled='true'),'Streaming',iif(isRefreshable='false','Direct Query','Import')) as DatasetType
	
  FROM [dbo].[datasets]
GO
/****** Object:  Table [dbo].[schedulehistory]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[schedulehistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[requestId] [varchar](50) NULL,
	[datasetId] [varchar](50) NULL,
	[refreshType] [varchar](50) NULL,
	[startTime] [varchar](50) NULL,
	[endTime] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](50) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
	[status] [varchar](50) NULL,
 CONSTRAINT [PK_schedulehistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_schedulehistory]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[vw_schedulehistory]
as
SELECT [schedulehistory].[id]
      ,[requestId]
      ,[datasetId]
      ,[refreshType]
      ,[startTime]
      ,[endTime]
      ,[status]
		--,[vw_datasets].[name] as dataset_name
	 -- ,[vw_datasets].[type] as dataset_type
  FROM [dbo].[schedulehistory]
 
GO
/****** Object:  View [dbo].[vw_NumberOfDatasetsInWs]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[vw_NumberOfDatasetsInWs]
as
select [workspaceid],count(*) as NumbOfDatasets 
from [dbo].[datasets]
group by [workspaceid]
GO
/****** Object:  Table [dbo].[UserActivities]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivities](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[RequestURL] [varchar](1500) NULL,
	[JsonInfo] [varchar](max) NULL,
	[CallerPrincipalName] [varchar](50) NULL,
	[LastTimeUpdated] [datetime] NULL,
	[StartDate] [varchar](50) NULL,
 CONSTRAINT [PK_UserActivities] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[_vw_UserActivities]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE view [dbo].[_vw_UserActivities]
as
SELECT PK, StartDate,Activities.*,[UserActivities].[CallerPrincipalName], [UserActivities].[LastTimeUpdated],
	 RIGHT([UserActivities].[CallerPrincipalName], LEN([UserActivities].[CallerPrincipalName]) - CHARINDEX('@', [UserActivities].[CallerPrincipalName])) Domain 

FROM [UserActivities] 
CROSS APPLY OPENJSON ([UserActivities].[JsonInfo], N'$')
  WITH (
    Id VARCHAR(200) N'$.Id',
	Activity VARCHAR(200) N'$.Activity',
    Operation VARCHAR(200) N'$.Operation', 
    ItemName VARCHAR(200) N'$.ItemName',	
    WorkSpaceName VARCHAR(200) N'$.WorkSpaceName',
	WorkspaceId VARCHAR(200) N'$.WorkspaceId',
    ReportName VARCHAR(200) N'$.ReportName',
	ReportId VARCHAR(200) N'$.ReportId',
	ReportType VARCHAR(200) N'$.ReportType',
	DatasetName VARCHAR(200) N'$.DatasetName',
	DatasetId VARCHAR(200) N'$.DatasetId',
 	DashboardId VARCHAR(200) N'$.DashboardId',
	DashboardName VARCHAR(200) N'$.DashboardName',
	RecordType VARCHAR(200) N'$.RecordType',
    CreationTime VARCHAR(200) N'$.CreationTime',
    OrganizationId VARCHAR(200) N'$.OrganizationId',
    UserType VARCHAR(200) N'$.UserType',
    UserKey VARCHAR(200) N'$.UserKey',
    [Workload] VARCHAR(200) N'$.Workload',
    IdUserId VARCHAR(200) N'$.UserId',
    ClientIP VARCHAR(200) N'$.ClientIP',
    UserAgent VARCHAR(200) N'$.UserAgent',
    ObjectId VARCHAR(200) N'$.ObjectId',
    DataConnectivityMode VARCHAR(200) N'$.DataConnectivityMode',
    IsSuccess VARCHAR(200) N'$.IsSuccess',
	RefreshType VARCHAR(200) N'$.RefreshType',
    Schedules_RefreshFrequency VARCHAR(200) N'$.Schedules.RefreshFrequency',
    Schedules_TimeZone VARCHAR(200) N'$.Schedules.TimeZone',
    ConsumptionMethod VARCHAR(200) N'$.ConsumptionMethod',
	Datasets_DatasetName NVARCHAR(200) '$.Datasets[0].DatasetName', 
	WorkspaceAccessList_WorkspaceId NVARCHAR(200) '$.WorkspaceAccessList[0].WorkspaceId',
	WorkspaceAccessList_UserAccessList_GroupUserAccessRight NVARCHAR(200) '$.WorkspaceAccessList[0].UserAccessList[0].GroupUserAccessRight',
	WorkspaceAccessList_UserAccessList_UserEmailAddress NVARCHAR(200) '$.WorkspaceAccessList[0].UserAccessList[0].UserEmailAddress',
	WorkspaceAccessList_UserAccessList_Identifier NVARCHAR(200) '$.WorkspaceAccessList[0].UserAccessList[0].Identifier',
	WorkspaceAccessList_UserAccessList_PrincipalType NVARCHAR(200) '$.WorkspaceAccessList[0].UserAccessList[0].PrincipalType',
	MembershipInformation_MemberEmail NVARCHAR(200) '$.MembershipInformation[0].MemberEmail'
  ) AS Activities
  where [JsonInfo]<>'{}'
--WHERE JSON_VALUE(Tab.json, '$.Status') = N'Closed'
--ORDER BY JSON_VALUE(Tab.json, '$.Group'), Tab.DateModified;

--select * from vw_UserActivities
GO
/****** Object:  View [dbo].[vw_UserActivities]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE view [dbo].[vw_UserActivities]
as
SELECT [StartDate]
      ,[Id]
      ,[Activity]
      ,[Operation]
      ,[ItemName]
      ,[WorkSpaceName]
      ,[WorkspaceId]
      ,[ReportName]
      ,[ReportId]
      ,[ReportType]
      ,[DatasetName]
      ,[DatasetId]
      ,[DashboardId]
      ,[DashboardName]
      ,[RecordType]
      ,[CreationTime]
      ,[OrganizationId]
      ,[UserType]
      ,[UserKey]
      ,[Workload]
      ,[IdUserId]
      ,[ClientIP]
      ,[UserAgent]
      ,[ObjectId]
      ,[DataConnectivityMode]
      ,[IsSuccess]
      ,[RefreshType]
      ,[Schedules_RefreshFrequency]
      ,[Schedules_TimeZone]
      ,[ConsumptionMethod]
      ,[Datasets_DatasetName]
      ,[WorkspaceAccessList_WorkspaceId]
      ,[WorkspaceAccessList_UserAccessList_GroupUserAccessRight]
      ,[WorkspaceAccessList_UserAccessList_UserEmailAddress]
      ,[WorkspaceAccessList_UserAccessList_Identifier]
      ,[WorkspaceAccessList_UserAccessList_PrincipalType]
      ,[MembershipInformation_MemberEmail]
      ,[CallerPrincipalName]
      ,[LastTimeUpdated]
	  , CONVERT(DATE,[CreationTime]) as  CreationDate
	  ,RIGHT([CallerPrincipalName], LEN([CallerPrincipalName]) - CHARINDEX('@', [CallerPrincipalName])) Domain 
  FROM [dbo].[_vw_UserActivities]
  where Activity not in('ExportActivityEvents','GetGroupsAsAdmin','ChangeGatewayDatasourceUsers','GenerateCustomVisualAADAccessToken')
GO
/****** Object:  Table [dbo].[AdUsers]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdUsers](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[RequestURL] [varchar](1500) NULL,
	[JsonInfo] [varchar](max) NULL,
	[CallerPrincipalName] [varchar](50) NULL,
	[LastTimeUpdated] [datetime] NULL,
 CONSTRAINT [PK_AdUsers] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[_vw_AdUsers]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[_vw_AdUsers]
as
SELECT Users.*,[AdUsers].[CallerPrincipalName], [AdUsers].[LastTimeUpdated]
FROM [AdUsers] 
CROSS APPLY OPENJSON ([AdUsers].[JsonInfo], N'$')
  WITH (
  displayName VARCHAR(200) N'$.displayName',
  userPrincipalName VARCHAR(200) N'$.userPrincipalName',
  userType VARCHAR(200) N'$.userType',

     Mail  VARCHAR(200) N'$.mail',
       MailNickname VARCHAR(200) N'$.mailNickname',
       CreatedDateTime VARCHAR(200) N'$.createdDateTime',
          OtherMail VARCHAR(200) N'$.otherMails[0]'
  ) AS Users
  where [JsonInfo]<>'{}'
GO
/****** Object:  Table [dbo].[AdServicePlans]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdServicePlans](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[RequestURL] [varchar](1500) NULL,
	[JsonInfo] [varchar](max) NULL,
	[CallerPrincipalName] [varchar](50) NULL,
	[LastTimeUpdated] [datetime] NULL,
 CONSTRAINT [PK_AdServicePlans] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[__vw_ServicePlans]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 create view [dbo].[__vw_ServicePlans]
  as
   SELECT ServicePlans.*
    FROM [dbo].[AdServicePlans]
	CROSS APPLY OPENJSON( [AdServicePlans].[JsonInfo], '$.value' ) as ServicePlans
    --WITH (JsonInfo1 NVARCHAR(max) '$.value') as ServicePlans;
GO
/****** Object:  View [dbo].[_vw_ServicePlans]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  CREATE view [dbo].[_vw_ServicePlans]
  as
   SELECT ServicePlan.*,ServicePlan1.*
    FROM __vw_ServicePlans
	CROSS APPLY OPENJSON( __vw_ServicePlans.[value], '$' ) 
	WITH (
	capabilityStatus NVARCHAR(200) '$.capabilityStatus',
	consumedUnits INT '$.consumedUnits',
	objectId NVARCHAR(200) '$.objectId',
	prepaidUnits_enabled NVARCHAR(200) '$.prepaidUnits.enabled',
	prepaidUnits_suspended NVARCHAR(200) '$.prepaidUnits.suspended',
	prepaidUnits_warning NVARCHAR(200) '$.prepaidUnits.warning',
	skuId NVARCHAR(200) '$.skuId',
	skuPartNumber NVARCHAR(200) '$.skuPartNumber',
	appliesTo NVARCHAR(200) '$.appliesTo'
	) as ServicePlan1
	CROSS APPLY OPENJSON( __vw_ServicePlans.[value], '$.servicePlans' )
	WITH (servicePlanId NVARCHAR(200) '$.servicePlanId',
	servicePlanName NVARCHAR(200) '$.servicePlanName',
	provisioningStatus NVARCHAR(200) '$.provisioningStatus',
	appliesTo1 NVARCHAR(200) '$.appliesTo'
	) 	as ServicePlan

GO
/****** Object:  View [dbo].[_vw_AdUserAssignedLicenses]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[_vw_AdUserAssignedLicenses]
as
SELECT AssignedLicenses.SkuId,
    Users.userPrincipalName
	
FROM [dbo].[AdUsers] 
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$' ) 
	WITH (userPrincipalName NVARCHAR(200) '$.userPrincipalName'
) AS Users
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$.assignedLicenses' ) 
	WITH (skuId NVARCHAR(200) '$.skuId'
)as AssignedLicenses
--OUTER APPLY OPENJSON( [AdUsers].[JsonInfo], '$.assignedPlans' ) 
--	WITH (
--	AssignedTimestamp NVARCHAR(200) '$.assignedTimestamp',
--	AapabilityStatus NVARCHAR(200) '$.capabilityStatus',
--	[Service] NVARCHAR(200) '$.service',
--	ServicePlanId NVARCHAR(200) '$.servicePlanId'
--)as AssignedPlans
GO
/****** Object:  View [dbo].[_vw_AdUserProvisionedPlans]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE view [dbo].[_vw_AdUserProvisionedPlans]
as
SELECT ProvisionedPlans.*,
    Users.*,
	[AdUsers].PK
FROM [dbo].[AdUsers] 
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$.provisionedPlans' ) 
	WITH (
	CapabilityStatus NVARCHAR(200) '$.capabilityStatus',
	ProvisioningStatus NVARCHAR(200) '$.provisioningStatus',
	[Service] NVARCHAR(200) '$.service'
) as ProvisionedPlans
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$' ) 
	WITH (UserPrincipalName NVARCHAR(200) '$.userPrincipalName'
) AS Users

GO
/****** Object:  View [dbo].[_vw_AdUserAssignedPlans]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[_vw_AdUserAssignedPlans]
as
SELECT AssignedPlans.*,
    Users.*,
	[AdUsers].PK
FROM [dbo].[AdUsers] 
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$.assignedPlans' ) 
	WITH (
	AssignedTimestamp NVARCHAR(200) '$.assignedTimestamp',
	CapabilityStatus NVARCHAR(200) '$.capabilityStatus',
	[Service] NVARCHAR(200) '$.service',
	servicePlanId NVARCHAR(200) '$.servicePlanId'
) as AssignedPlans
CROSS APPLY OPENJSON( [AdUsers].[JsonInfo], '$' ) 
	WITH (userPrincipalName NVARCHAR(200) '$.userPrincipalName'
) AS Users

GO
/****** Object:  View [dbo].[vw_PowerBiProUsers]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[vw_PowerBiProUsers]
 as
  --select * from [dbo].[_vw_AdUserAssignedLicenses] where [SkuId]='f8a1db68-be16-40ed-86d5-cb42ce701560'
select * from [dbo].[_vw_AdUserAssignedPlans] where service like 'powerbi' and capabilitystatus='enabled'
GO
/****** Object:  View [dbo].[vw_PowerBiStandardUsers]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[vw_PowerBiStandardUsers]
 as
  select * from [dbo].[_vw_AdUserAssignedLicenses] where [SkuId]='a403ebcc-fae0-4ca2-8c8c-7a907fd6c235'
GO
/****** Object:  View [dbo].[vw_AdUsers]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





--select * from [dbo].[vw_PowerBiProUsers]
--select * from [dbo].[vw_PowerBiStandardUsers]

--select * from vw_AdUsers

CREATE view [dbo].[vw_AdUsers]
as
SELECT 
[displayName],
[_vw_AdUsers].[userPrincipalName],
[userType],
[Mail],
_vw_AdUsers.[CreatedDateTime],
Convert(Date,[CreatedDateTime]) as CreatedDate,
[OtherMail],
iif([vw_PowerBiProUsers].userPrincipalName is null, 'Standard','Pro') as LicenseName,
iif([vw_PowerBiStandardUsers].userPrincipalName is null, 'false','true') as PowerBiStandardUser,
iif([vw_PowerBiProUsers].userPrincipalName is null, 'false','true') as PowerBiProUser,
[vw_PowerBiProUsers].assignedTimeStamp as PowerBiPro_AssignedTimestamp,
Convert(Date, assignedTimeStamp) as PowerBiPro_AssignedDate
FROM _vw_AdUsers 
left join [vw_PowerBiProUsers] on _vw_AdUsers.userPrincipalName=[vw_PowerBiProUsers].userPrincipalName
left join [vw_PowerBiStandardUsers] on _vw_AdUsers.userPrincipalName=[vw_PowerBiStandardUsers].userPrincipalName
GO
/****** Object:  Table [dbo].[DateDimension]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DateDimension](
	[Date] [date] NOT NULL,
	[Day] [tinyint] NOT NULL,
	[DaySuffix] [char](2) NOT NULL,
	[Weekday] [tinyint] NOT NULL,
	[WeekDayName] [varchar](10) NOT NULL,
	[IsWeekend] [bit] NOT NULL,
	[IsHoliday] [bit] NOT NULL,
	[HolidayText] [varchar](64) SPARSE  NULL,
	[DOWInMonth] [tinyint] NOT NULL,
	[DayOfYear] [smallint] NOT NULL,
	[WeekOfMonth] [tinyint] NOT NULL,
	[WeekOfYear] [tinyint] NOT NULL,
	[ISOWeekOfYear] [tinyint] NOT NULL,
	[Month] [tinyint] NOT NULL,
	[MonthName] [varchar](10) NOT NULL,
	[Quarter] [tinyint] NOT NULL,
	[QuarterName] [varchar](6) NOT NULL,
	[Year] [int] NOT NULL,
	[MMYYYY] [char](6) NOT NULL,
	[MonthYear] [char](7) NOT NULL,
	[FirstDayOfMonth] [date] NOT NULL,
	[LastDayOfMonth] [date] NOT NULL,
	[FirstDayOfQuarter] [date] NOT NULL,
	[LastDayOfQuarter] [date] NOT NULL,
	[FirstDayOfYear] [date] NOT NULL,
	[LastDayOfYear] [date] NOT NULL,
	[FirstDayOfNextMonth] [date] NOT NULL,
	[FirstDayOfNextYear] [date] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_DateDimension]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_DateDimension]
AS
SELECT        dbo.DateDimension.*
FROM            dbo.DateDimension
GO
/****** Object:  View [dbo].[vw_Reports]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[vw_Reports]
as
SELECT isnull([id],'0') as id
      ,[reportType]
      ,[name] as ReportName
      ,[webUrl]
      ,[embedUrl]
      ,[datasetId]
      ,[workspaceId]
      ,[CallerPrincipalName]
      ,[LastTimeUpdated]
      ,[isOwnedByMe]
      ,[isFromPbix]
  FROM [dbo].[reports]
GO
/****** Object:  View [dbo].[vw_User_Permissions]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[vw_User_Permissions]
as
SELECT [vw_AdUsers].[DisplayName]
      ,isnull([user_permissions].[UserPrincipalName],'') as [UserPrincipalName]
      ,[user_permissions].[UserType]
      ,isnull([workspaceid],'') as [workspaceid]
      ,[workspacetype]
      ,[CallerPrincipalName]
      ,[LastTimeUpdated]
  FROM [dbo].[user_permissions]
  left join [dbo].[vw_AdUsers] on [user_permissions].[UserPrincipalName]=[vw_AdUsers].[UserPrincipalName]
GO
/****** Object:  Table [dbo].[dashboards]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dashboards](
	[id] [varchar](50) NOT NULL,
	[displayName] [varchar](500) NULL,
	[embedUrl] [varchar](4000) NULL,
	[isReadOnly] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](250) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
	[workspaceid] [varchar](50) NULL,
 CONSTRAINT [PK_dashboards] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[t_Security_Log]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_Security_Log](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[Action] [varchar](50) NULL,
	[User] [varchar](250) NULL,
	[RecordsExported] [int] NULL,
	[InsertTimestamp] [datetime2](7) NULL,
	[UserNameWeb] [varchar](100) NOT NULL,
	[Info] [varchar](100) NULL,
	[Message] [varchar](4000) NULL,
 CONSTRAINT [PK_t_Security_Log] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[t_Security_Users]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_Security_Users](
	[UserPrincipalName] [varchar](50) NOT NULL,
	[DisplayName] [varchar](50) NULL,
	[ObjectId] [varchar](50) NOT NULL,
	[UserType] [varchar](50) NULL,
	[IsAdmin] [bit] NULL,
	[AdHocEditor] [bit] NULL,
	[LastModified] [smalldatetime] NULL,
 CONSTRAINT [PK_t_Security_Users] PRIMARY KEY CLUSTERED 
(
	[UserPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserActivities_Archive]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserActivities_Archive](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[RequestURL] [varchar](1500) NULL,
	[JsonInfo] [varchar](max) NULL,
	[CallerPrincipalName] [varchar](50) NULL,
	[LastTimeUpdated] [datetime] NULL,
 CONSTRAINT [PK_UserActivities_1] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 2/27/2020 11:27:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[emailAddress] [varchar](250) NOT NULL,
	[UserState] [varchar](250) NULL,
	[groupUserAccessRight] [varchar](50) NULL,
	[CallerPrincipalName] [varchar](250) NOT NULL,
	[LastTimeUpdated] [smalldatetime] NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[emailAddress] ASC,
	[CallerPrincipalName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
