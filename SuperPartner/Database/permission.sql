CREATE TABLE [dbo].[P_Functions](
	[FuncCode] [nvarchar](50) NOT NULL,
	[FuncName] [nvarchar](200) NULL,
	[AssociateUrls] [nvarchar](4000) NULL,
	[FuncDesc] [ntext] NULL,
	[ExtendProperties] [xml] NULL,
 CONSTRAINT [PK_P_Functions] PRIMARY KEY CLUSTERED 
(
	[FuncCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE TABLE [dbo].[P_User2Functions](
	[UserId] [nvarchar](50) NOT NULL,
	[FuncCode] [nvarchar](50) NOT NULL,
	[AccessLevel] [int] NULL,
 CONSTRAINT [PK_P_User2Functions] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[FuncCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO