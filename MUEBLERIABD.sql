USE [MUEBLERIABD]
GO
/****** Object:  Table [dbo].[VTA_VENTADETALLE]    Script Date: 09/04/2017 14:05:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VTA_VENTADETALLE](
	[FOLIOVTA] [int] IDENTITY(1,1) NOT NULL,
	[IDCLIENTE] [int] NOT NULL,
	[IDARTICULO] [int] NOT NULL,
	[TIPOVENTA] [varchar](20) NOT NULL,
	[FECHA] [date] NOT NULL,
	[CANTIDAD] [nchar](10) NULL,
	[TOTAL] [money] NOT NULL,
	[ACTIVO] [varchar](2) NOT NULL,
 CONSTRAINT [PK_VTA_VENTADETALLE_1] PRIMARY KEY CLUSTERED 
(
	[FOLIOVTA] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VTA_CONFIGURACIONES]    Script Date: 09/04/2017 14:05:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VTA_CONFIGURACIONES](
	[TASAFINANCIAMIENTO] [float] NOT NULL,
	[PORCENTAJEENGANCHE] [int] NOT NULL,
	[PLAZOMAXIMO] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VTA_CLIENTES]    Script Date: 09/04/2017 14:05:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VTA_CLIENTES](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[NOMBRE] [varchar](80) NOT NULL,
	[APELLIDOPATERNO] [varchar](50) NOT NULL,
	[APELLIDOMATERNO] [varchar](50) NULL,
	[RFC] [varchar](16) NOT NULL,
 CONSTRAINT [PK_VTA_CLIENTES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VTA_ARTICULOS]    Script Date: 09/04/2017 14:05:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VTA_ARTICULOS](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DESCRIPCION] [varchar](150) NOT NULL,
	[MODELO] [varchar](50) NOT NULL,
	[PRECIO] [money] NOT NULL,
	[EXISTENCIA] [int] NOT NULL,
 CONSTRAINT [PK_VTA_ARTICULOS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
