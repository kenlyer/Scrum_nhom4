use [SkyWaveAirlinesSystem]
GO
/****** Object:  Database [SkyWaveAirlinesSystem]    Script Date: 11/7/2024 11:04:58 PM ******/
CREATE DATABASE [SkyWaveAirlinesSystem]
CONTAINMENT = NONE
ON PRIMARY 
(
    NAME = N'SkyWaveAirlinesSystem', 
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SkyWaveAirlinesSystem.mdf', 
    SIZE = 8192KB, 
    MAXSIZE = UNLIMITED, 
    FILEGROWTH = 65536KB
)
LOG ON 
(
    NAME = N'SkyWaveAirlinesSystem_log', 
    FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SkyWaveAirlinesSystem_log.ldf', 
    SIZE = 8192KB, 
    MAXSIZE = 2048GB, 
    FILEGROWTH = 65536KB
)
WITH CATALOG_COLLATION = DATABASE_DEFAULT;

GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SkyWaveAirlinesSystem].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ARITHABORT OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET  ENABLE_BROKER 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET  MULTI_USER 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET QUERY_STORE = OFF
GO
USE [SkyWaveAirlinesSystem]
GO
/****** Object:  Table [dbo].[AirPort]    Script Date: 12/7/2024 3:41:58 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AirPort](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[code] [nvarchar](255) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_AirPort] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlightSchedules]    Script Date: 12/7/2024 3:50:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlightSchedules](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[plane_id] [int] NOT NULL,
	[from_airport] [int] NOT NULL,
	[to_airport] [int] NOT NULL,
	[departures_at] [datetime] NOT NULL,
	[arrivals_at] [datetime] NOT NULL,
	[cost] [int] NOT NULL,
	[code] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_FlightSchedules] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plane]   Script Date: 12/7/2024 3:50:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plane](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[code] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Plane] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TicketManager]   Script Date: 12/7/2024 3:50:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketManager](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[flight_schedules_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[status] [int] NOT NULL,
	[code] [nchar](255) NOT NULL,
 CONSTRAINT [PK_TicketManager] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/7/2024 3:50:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NULL,
	[email] [nvarchar](255) NOT NULL,
	[cccd] [nvarchar](255) NULL,
	[address] [nvarchar](255) NULL,
	[phone_number] [nvarchar](255) NULL,
	[password] [nvarchar](255) NOT NULL,
	[user_type] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AirPort] ON 
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (1, N'CẢNG HÀNG KHÔNG QUỐC TẾ NỘI BÀI', N'NOI_BAI', N'Cảng hàng không quốc tế Nội Bài ​xã Phú Minh - huyện Sóc Sơn - Thành phố Hà Nội')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (2, N'CẢNG HÀNG KHÔNG QUỐC TẾ TÂN SƠN NHẤT', N'TAN_SON_NHAT', N'Tòa Nhà Cảng Hàng Không Quốc Tế Tân Sơn Nhất - Phường 2, Q. Tân Bình, Tp.Hồ Chí Minh')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (3, N'CẢNG HÀNG KHÔNG QUỐC TẾ ĐÀ NẴNG', N'DA_NANG', N'Phường Hòa Thuận Tây, Quận Hải Châu, Thành phố Đà Nẵng')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (4, N'CẢNG HÀNG KHÔNG QUỐC TẾ PHÚ QUỐC', N'PHU_QUOC', N'Tổ 2, Ấp Dương Tơ, Xã Dương Tơ, H. Phú Quốc, T. Kiên Giang')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (5, N'CẢNG HÀNG KHÔNG QUỐC TẾ CAM RANH', N'CAM_RANH', N'Nguyễn Tất Thành, Cam Hải Đông, Cam Lâm, Khánh Hòa')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (6, N'CẢNG HÀNG KHÔNG QUỐC TẾ VÂN ĐỒN', N'VAN_DON', N'Doàn Kết, Đoàn Kết, Vân Đồn, Quảng Ninh')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (7, N'CẢNG HÀNG KHÔNG QUỐC TẾ CẦN THƠ', N'CAN_THO', N'179B Lê Hồng Phong, Long Hòa, Bình Thủy, Cần Thơ')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (8, N'CẢNG HÀNG KHÔNG QUỐC TẾ CÁT BI', N'CAT_BI', N'Tràng Cát, Hải An, Hải Phòng')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (9, N'CẢNG HÀNG KHÔNG QUỐC TẾ LIÊN KHƯƠNG', N'LIEN_KHUONG', N'Quốc lộ 20, Liên Khương, Đức Trọng, Lâm Đồng')
INSERT [dbo].[AirPort] ([id], [name], [code], [address]) VALUES (10, N'CẢNG HÀNG KHÔNG ĐIỆN BIÊN PHỦ', N'DIEN_BIEN', N'TP. Điện Biên Phủ, Tỉnh Điện Biên')
SET IDENTITY_INSERT [dbo].[AirPort] OFF
GO
SET IDENTITY_INSERT [dbo].[FlightSchedules] ON 

SET IDENTITY_INSERT [dbo].[FlightSchedules] OFF
GO
SET IDENTITY_INSERT [dbo].[Plane] ON 

INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (1, N'Airbus A321-200', N'AIR-200')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (2, N'Boeing 787-9 Dreamliner', N'BOE-787')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (3, N'SAAB 340 Saab 2000', N'SAAB')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (4, N'Airbus A380', N'AIR-380')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (5, N'Boeing 737', N'BOE-737')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (6, N'Boeing 777', N'BOE-777')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (7, N'Airbus A350', N'AIR-350')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (8, N'Boeing 747', N'BOE-747')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (9, N'Embraer E190', N'EMB-190')
INSERT [dbo].[Plane] ([id], [name], [code]) VALUES (10, N'Bombardier CRJ900', N'BOM-900')

SET IDENTITY_INSERT [dbo].[Plane] OFF
GO
SET IDENTITY_INSERT [dbo].[TicketManager] ON 

INSERT [dbo].[TicketManager] ([id], [flight_schedules_id], [user_id], [status], [code]) VALUES (1, 2, 2, 1, N'HN_PQ_200_12371231234')
INSERT [dbo].[TicketManager] ([id], [flight_schedules_id], [user_id], [status], [code]) VALUES (2, 5, 2, 0, N'HN_PQ_200_17121543140')
INSERT [dbo].[TicketManager] ([id], [flight_schedules_id], [user_id], [status], [code]) VALUES (3, 5, 1, 0, N'HN_PQ_200_17121590750')
INSERT [dbo].[TicketManager] ([id], [flight_schedules_id], [user_id], [status], [code]) VALUES (4, 5, 1, 0, N'HN_PQ_200_17122420290')
SET IDENTITY_INSERT [dbo].[TicketManager] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([id], [name], [email], [cccd], [address], [phone_number], [password], [user_type]) VALUES (1, N'Tran Hong Phat', N'asdsaphat@gmail.com', N'123456789', N'Binh Thuan', N'1234565768', N'123456', 0)
INSERT [dbo].[User] ([id], [name], [email], [cccd], [address], [phone_number], [password], [user_type]) VALUES (2, N'Nguyen Dang Khoa', N'khoangu@gmail.com', N'123456789', N'ưe', N'+844999999999', N'123456', 1)

SET IDENTITY_INSERT [dbo].[User] OFF
GO
ALTER TABLE [dbo].[TicketManager]  WITH CHECK ADD  CONSTRAINT [FK_TicketManager_FlightSchedules] FOREIGN KEY([flight_schedules_id])
REFERENCES [dbo].[FlightSchedules] ([id])
GO
ALTER TABLE [dbo].[TicketManager] CHECK CONSTRAINT [FK_TicketManager_FlightSchedules]
GO
ALTER TABLE [dbo].[TicketManager]  WITH CHECK ADD  CONSTRAINT [FK_TicketManager_User] FOREIGN KEY([user_id])
REFERENCES [dbo].[User] ([id])
GO
ALTER TABLE [dbo].[TicketManager] CHECK CONSTRAINT [FK_TicketManager_User]
GO
USE [master]
GO
ALTER DATABASE [SkyWaveAirlinesSystem] SET  READ_WRITE 
GO



--Fix 2.0 
---ALTER TABLE
--flight schedules 
--them so luong cho ngoi 
ALTER TABLE [dbo].[FlightSchedules]
ADD totalSeats INT ;

ALTER TABLE [dbo].[FlightSchedules]
ADD bookedSeats INT ;

ALTER TABLE [dbo].[FlightSchedules]
ADD availableSeats INT ;


ALTER TABLE [dbo].[FlightSchedules]
ADD status_count Nvarchar(50) ;

-- them vi tri cho ngoi
alter table [dbo].[TicketManager]
ADD seat_location INT;

alter table [dbo].[TicketManager]
ADD pay_id Int;



