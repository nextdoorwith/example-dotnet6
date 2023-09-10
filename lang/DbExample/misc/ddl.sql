SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[m_employee];
DROP INDEX IF EXISTS [dbo].[pk_m_employee];

CREATE TABLE [dbo].[m_employee](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[employee_no] [varchar](10) NOT NULL,
	[name] [nvarchar](256) NOT NULL,
	[address] [nvarchar](2048) NULL,
	[gender] [tinyint] NULL,
	[retired] [bit] NOT NULL DEFAULT 0,
	[birthday] [datetime2](7) NOT NULL,
	[internal_id] [uniqueidentifier] NULL,
	[created_by] [varchar](8) NOT NULL,
	[created_on] [datetime2] NOT NULL,
	[updated_by] [varchar](8) NOT NULL,
	[updated_on] [datetime2] NOT NULL,
	[version] [timestamp] NOT NULL,
	CONSTRAINT [pk_m_employee] PRIMARY KEY CLUSTERED ([user_id])
) ON [PRIMARY]
GO

DROP TABLE IF EXISTS [dbo].[t_sales];
DROP INDEX IF EXISTS [dbo].[pk_t_sales];

CREATE TABLE [dbo].[t_sales](
	[region_id] [tinyint] NOT NULL,
	[year] [smallint] NOT NULL,
	[month] [tinyint] NOT NULL,
	[revenue] [money] default 0 NULL,
	[expense] [money] default 0 NULL,
	[profit] [money] default 0 NULL,
	[created_by] [varchar](8) NOT NULL,
	[created_on] [datetime2] NOT NULL,
	[updated_by] [varchar](8) NOT NULL,
	[updated_on] [datetime2] NOT NULL,
	[version] [timestamp] NOT NULL,
	CONSTRAINT [pk_t_sales] PRIMARY KEY CLUSTERED ([region_id],[year],[month])
) ON [PRIMARY]
GO


-- 注文－注文詳細－製品
-- 外部キー制約のため、削除時はm_order->m_product、作成時は逆
DROP TABLE IF EXISTS [dbo].[m_order_detail];
DROP INDEX IF EXISTS [dbo].[pk_m_order_detail];
DROP INDEX IF EXISTS [dbo].[fk_m_order_detail_order_id];
DROP INDEX IF EXISTS [dbo].[fk_m_order_detail_product_id];
DROP TABLE IF EXISTS [dbo].[m_order];
DROP INDEX IF EXISTS [dbo].[pk_m_order];
DROP TABLE IF EXISTS [dbo].[m_product];
DROP INDEX IF EXISTS [dbo].[pk_m_product];

CREATE TABLE [dbo].[m_product](
	[product_id] [int] NOT NULL,
	[product_code] [varchar](16) NOT NULL,
	[product_name] [nvarchar](256) NOT NULL,
	[price] [money] NOT NULL,
	[remarks] [nvarchar](1024) NULL,
	[created_by] [varchar](8) NOT NULL,
	[created_on] [datetime2] NOT NULL,
	[updated_by] [varchar](8) NOT NULL,
	[updated_on] [datetime2] NOT NULL,
	[version] [timestamp] NOT NULL,
	CONSTRAINT [pk_m_product] PRIMARY KEY CLUSTERED ([product_id])
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[m_order](
	[order_id] [int] IDENTITY(1,1) NOT NULL,
	[order_no] [varchar](16) NOT NULL,
	[created_by] [varchar](8) NOT NULL,
	[created_on] [datetime2] NOT NULL,
	[updated_by] [varchar](8) NOT NULL,
	[updated_on] [datetime2] NOT NULL,
	[version] [timestamp] NOT NULL,
	CONSTRAINT [pk_m_order] PRIMARY KEY CLUSTERED ([order_id])
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[m_order_detail](
	[order_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[created_by] [varchar](8) NOT NULL,
	[created_on] [datetime2] NOT NULL,
	[updated_by] [varchar](8) NOT NULL,
	[updated_on] [datetime2] NOT NULL,
	[version] [timestamp] NOT NULL,
	CONSTRAINT [pk_m_order_detail] PRIMARY KEY CLUSTERED ([order_id], [product_id]),
	CONSTRAINT [fk_m_order_detail_order_id] FOREIGN KEY ([order_id]) REFERENCES [m_order] ([order_id]),
	CONSTRAINT [fk_m_order_detail_product_id] FOREIGN KEY ([product_id]) REFERENCES [m_product] ([product_id])
) ON [PRIMARY]
GO


DROP TABLE IF EXISTS [dbo].[z_test_nopk];

CREATE TABLE [dbo].[z_test_nopk](
	[desc1] [nvarchar](256) NOT NULL,
	[desc2] [nvarchar](256) NOT NULL
) ON [PRIMARY]
GO
