SET IDENTITY_INSERT [dbo].[People] ON 

INSERT [dbo].[People] ([Id], [Name]) VALUES (1, N'Julia')
INSERT [dbo].[People] ([Id], [Name]) VALUES (2, N'Igor')
INSERT [dbo].[People] ([Id], [Name]) VALUES (3, N'Koshka')
SET IDENTITY_INSERT [dbo].[People] OFF
--SET IDENTITY_INSERT [dbo].[Books] ON 

--INSERT [dbo].[Books] ([Id], [Name], [PersonId]) VALUES (1, N'UX Design', 1)
--INSERT [dbo].[Books] ([Id], [Name], [PersonId]) VALUES (2, N'Cook book', 1)
--INSERT [dbo].[Books] ([Id], [Name], [PersonId]) VALUES (3, N'Design Patterns', 2)
--INSERT [dbo].[Books] ([Id], [Name], [PersonId]) VALUES (4, N'Docker', 2)
--SET IDENTITY_INSERT [dbo].[Books] OFF
