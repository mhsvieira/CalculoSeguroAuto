USE [SeguroAutomovel]
GO

/****** Object:  Table [dbo].[Apolice]    Script Date: 19/06/2020 13:24:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Apolice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Cpf] [varchar](15) NOT NULL,
	[DataNascimento] [datetime] NOT NULL,
	[DataSeguro] [datetime] NOT NULL,
	[InicioVigencia] [datetime] NOT NULL,
	[FinalVigencia] [datetime] NOT NULL,
	[CepPernoite] [varchar](15) NOT NULL,
	[Placa] [varchar](15) NOT NULL,
	[Modelo] [varchar](100) NOT NULL,
	[ValorIS] [decimal](18, 2) NOT NULL,
	[PremioTotal] [decimal](18, 2) NOT NULL,
	[Status] [varchar](1) NOT NULL,
 CONSTRAINT [PK_Apolice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


