USE [MyFavorRepos]
GO
/****** Object:  Table [dbo].[GitHubRepos]    Script Date: 2021/6/25 16:29:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GitHubRepos](
	[id] [int] NULL,
	[githubuser] [nvarchar](50) NULL,
	[name] [nvarchar](50) NULL,
	[fullname] [nvarchar](200) NULL,
	[description] [nvarchar](200) NULL,
	[htmlurl] [nvarchar](200) NULL
) ON [PRIMARY]

GO
