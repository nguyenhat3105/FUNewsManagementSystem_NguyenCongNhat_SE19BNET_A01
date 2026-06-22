-- ============================================================
--  FUNewsManagement — Full Database Setup
--  Compatible with AS1 (MVC) & AS2 (Razor Pages)
--  Includes all extended tables + ~10 records per table
--  Image URLs: Unsplash (reliable, always accessible)
-- ============================================================

USE [master]
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'FUNewsManagement')
    DROP DATABASE [FUNewsManagement]
GO

CREATE DATABASE [FUNewsManagement]
GO

USE [FUNewsManagement]
GO

-- ============================================================
--  1. SystemAccount
--  Role: 1=Staff, 2=Lecturer, 3=Reader
--  Note: AccountID dùng INT (khớp với code C#)
-- ============================================================
CREATE TABLE [dbo].[SystemAccount] (
    [AccountID]       INT            NOT NULL IDENTITY(1,1),
    [AccountName]     NVARCHAR(100)  NOT NULL,
    [AccountEmail]    NVARCHAR(100)  NOT NULL,
    [AccountRole]     INT            NOT NULL,  -- 1=Staff, 2=Lecturer, 3=Reader
    [AccountPassword] NVARCHAR(100)  NOT NULL,
    [PhoneNumber]     NVARCHAR(30)   NULL,
    [AvatarUrl]       NVARCHAR(500)  NULL,
    [Bio]             NVARCHAR(500)  NULL,
    CONSTRAINT [PK_SystemAccount] PRIMARY KEY CLUSTERED ([AccountID] ASC)
)
GO

-- ============================================================
--  2. Category
-- ============================================================
CREATE TABLE [dbo].[Category] (
    [CategoryID]          SMALLINT      NOT NULL IDENTITY(1,1),
    [CategoryName]        NVARCHAR(100) NOT NULL,
    [CategoryDescription] NVARCHAR(250) NOT NULL,  -- đúng chính tả (đề bài typo)
    [ParentCategoryID]    SMALLINT      NULL,
    [IsActive]            BIT           NOT NULL DEFAULT(1),
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryID] ASC),
    CONSTRAINT [FK_Category_Category] FOREIGN KEY ([ParentCategoryID])
        REFERENCES [dbo].[Category] ([CategoryID])
)
GO

-- ============================================================
--  3. ArticleState  (bảng mở rộng — đề bài không có)
-- ============================================================
CREATE TABLE [dbo].[ArticleState] (
    [ArticleStateID] INT           NOT NULL,
    [StateName]      NVARCHAR(50)  NOT NULL,
    [Description]    NVARCHAR(200) NULL,
    CONSTRAINT [PK_ArticleState] PRIMARY KEY CLUSTERED ([ArticleStateID] ASC)
)
GO

-- ============================================================
--  4. Tag
-- ============================================================
CREATE TABLE [dbo].[Tag] (
    [TagID]   INT           NOT NULL IDENTITY(1,1),
    [TagName] NVARCHAR(50)  NOT NULL,
    [Note]    NVARCHAR(400) NULL,
    CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED ([TagID] ASC)
)
GO

-- ============================================================
--  5. NewsArticle
-- ============================================================
CREATE TABLE [dbo].[NewsArticle] (
    [NewsArticleID]        NVARCHAR(20)  NOT NULL,
    [NewsTitle]            NVARCHAR(200) NOT NULL,
    [Headline]             NVARCHAR(250) NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL DEFAULT(GETDATE()),
    [NewsContent]          NVARCHAR(MAX) NOT NULL,
    [NewsSource]           NVARCHAR(200) NULL,
    [ImageUrl]             NVARCHAR(500) NULL,
    [CategoryID]           INT           NOT NULL,
    [NewsStatus]           BIT           NOT NULL DEFAULT(1),
    [ArticleStateID]       INT           NOT NULL DEFAULT(3),
    [ScheduledPublishDate] DATETIME      NULL,
    [CreatedByID]          INT           NOT NULL,
    [UpdatedByID]          INT           NULL,
    [ModifiedDate]         DATETIME      NULL,
    CONSTRAINT [PK_NewsArticle] PRIMARY KEY CLUSTERED ([NewsArticleID] ASC),
    CONSTRAINT [FK_NewsArticle_Category] FOREIGN KEY ([CategoryID])
        REFERENCES [dbo].[Category] ([CategoryID]),
    CONSTRAINT [FK_NewsArticle_ArticleState] FOREIGN KEY ([ArticleStateID])
        REFERENCES [dbo].[ArticleState] ([ArticleStateID]),
    CONSTRAINT [FK_NewsArticle_CreatedBy] FOREIGN KEY ([CreatedByID])
        REFERENCES [dbo].[SystemAccount] ([AccountID]),
    CONSTRAINT [FK_NewsArticle_UpdatedBy] FOREIGN KEY ([UpdatedByID])
        REFERENCES [dbo].[SystemAccount] ([AccountID])
)
GO

-- ============================================================
--  6. NewsTag
-- ============================================================
CREATE TABLE [dbo].[NewsTag] (
    [NewsArticleID] NVARCHAR(20) NOT NULL,
    [TagID]         INT          NOT NULL,
    CONSTRAINT [PK_NewsTag] PRIMARY KEY CLUSTERED ([NewsArticleID] ASC, [TagID] ASC),
    CONSTRAINT [FK_NewsTag_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]),
    CONSTRAINT [FK_NewsTag_Tag] FOREIGN KEY ([TagID])
        REFERENCES [dbo].[Tag] ([TagID])
)
GO

-- ============================================================
--  7. AuditLog  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[AuditLog] (
    [AuditLogID]  INT           NOT NULL IDENTITY(1,1),
    [ActorEmail]  NVARCHAR(80)  NOT NULL,
    [ActorRole]   NVARCHAR(30)  NOT NULL,
    [Action]      NVARCHAR(40)  NOT NULL,
    [EntityName]  NVARCHAR(80)  NOT NULL,
    [EntityKey]   NVARCHAR(80)  NULL,
    [Description] NVARCHAR(500) NULL,
    [CreatedAt]   DATETIME      NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED ([AuditLogID] ASC)
)
GO

-- ============================================================
--  8. ApprovalHistory  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[ApprovalHistory] (
    [HistoryID]     INT           NOT NULL IDENTITY(1,1),
    [NewsArticleID] NVARCHAR(20)  NOT NULL,
    [AccountID]     INT           NULL,
    [Action]        NVARCHAR(30)  NOT NULL,
    [Note]          NVARCHAR(500) NULL,
    [Timestamp]     DATETIME      NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_ApprovalHistory] PRIMARY KEY CLUSTERED ([HistoryID] ASC),
    CONSTRAINT [FK_ApprovalHistory_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ApprovalHistory_Account] FOREIGN KEY ([AccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID])
)
GO

-- ============================================================
--  9. ArticleLike  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[ArticleLike] (
    [ArticleLikeID] INT          NOT NULL IDENTITY(1,1),
    [NewsArticleID] NVARCHAR(20) NOT NULL,
    [AccountID]     INT          NOT NULL,
    [CreatedAt]     DATETIME     NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_ArticleLike] PRIMARY KEY CLUSTERED ([ArticleLikeID] ASC),
    CONSTRAINT [UQ_ArticleLike] UNIQUE ([NewsArticleID], [AccountID]),
    CONSTRAINT [FK_ArticleLike_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ArticleLike_Account] FOREIGN KEY ([AccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID])
)
GO

-- ============================================================
--  10. ArticleBookmark  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[ArticleBookmark] (
    [ArticleBookmarkID] INT          NOT NULL IDENTITY(1,1),
    [NewsArticleID]     NVARCHAR(20) NOT NULL,
    [AccountID]         INT          NOT NULL,
    [CreatedAt]         DATETIME     NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_ArticleBookmark] PRIMARY KEY CLUSTERED ([ArticleBookmarkID] ASC),
    CONSTRAINT [UQ_ArticleBookmark] UNIQUE ([NewsArticleID], [AccountID]),
    CONSTRAINT [FK_ArticleBookmark_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ArticleBookmark_Account] FOREIGN KEY ([AccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID])
)
GO

-- ============================================================
--  11. ArticleComment  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[ArticleComment] (
    [ArticleCommentID] INT           NOT NULL IDENTITY(1,1),
    [NewsArticleID]    NVARCHAR(20)  NOT NULL,
    [AccountID]        INT           NULL,
    [DisplayName]      NVARCHAR(100) NOT NULL,
    [Content]          NVARCHAR(1000) NOT NULL,
    [ParentCommentID]  INT           NULL,
    [CreatedAt]        DATETIME      NOT NULL DEFAULT(GETDATE()),
    [IsDeleted]        BIT           NOT NULL DEFAULT(0),
    CONSTRAINT [PK_ArticleComment] PRIMARY KEY CLUSTERED ([ArticleCommentID] ASC),
    CONSTRAINT [FK_ArticleComment_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ArticleComment_Account] FOREIGN KEY ([AccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID]),
    CONSTRAINT [FK_ArticleComment_Parent] FOREIGN KEY ([ParentCommentID])
        REFERENCES [dbo].[ArticleComment] ([ArticleCommentID])
)
GO

-- ============================================================
--  12. Notification  (bảng mở rộng)
-- ============================================================
CREATE TABLE [dbo].[Notification] (
    [NotificationID]     INT           NOT NULL IDENTITY(1,1),
    [RecipientAccountID] INT           NOT NULL,
    [NewsArticleID]      NVARCHAR(20)  NOT NULL,
    [Type]               NVARCHAR(50)  NOT NULL,  -- Like | Bookmark | Comment
    [ActorAccountID]     INT           NULL,
    [ActorName]          NVARCHAR(100) NOT NULL DEFAULT(''),
    [Excerpt]            NVARCHAR(500) NULL,
    [IsRead]             BIT           NOT NULL DEFAULT(0),
    [CreatedAt]          DATETIME      NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([NotificationID] ASC),
    CONSTRAINT [FK_Notification_NewsArticle] FOREIGN KEY ([NewsArticleID])
        REFERENCES [dbo].[NewsArticle] ([NewsArticleID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Notification_Recipient] FOREIGN KEY ([RecipientAccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID]),
    CONSTRAINT [FK_Notification_Actor] FOREIGN KEY ([ActorAccountID])
        REFERENCES [dbo].[SystemAccount] ([AccountID])
)
GO


-- ============================================================
--  DATA: ArticleState (cố định, không thay đổi)
-- ============================================================
INSERT INTO [dbo].[ArticleState] ([ArticleStateID], [StateName], [Description]) VALUES
(1, N'Draft',          N'Article is being written.'),
(2, N'Pending Review', N'Article is waiting for approval.'),
(3, N'Published',      N'Article is visible when scheduled time is due.'),
(4, N'Archived',       N'Article is hidden but retained.')
GO

-- ============================================================
--  DATA: SystemAccount
--  Role 1 = Staff (tạo/sửa bài), Role 2 = Lecturer (chỉ xem)
--  Password: @@abc123@@ (đủ dài >= 6 ký tự)
-- ============================================================
SET IDENTITY_INSERT [dbo].[SystemAccount] ON
GO
INSERT INTO [dbo].[SystemAccount] ([AccountID],[AccountName],[AccountEmail],[AccountRole],[AccountPassword],[PhoneNumber],[AvatarUrl],[Bio]) VALUES
(1,  N'Isabella David',      N'IsabellaDavid@FUNewsManagement.org',    1, N'@@abc123@@', N'0901111001',
 N'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=120&h=120&fit=crop&crop=face',
 N'Senior news editor with 5 years at FU. Passionate about academic journalism.'),

(2,  N'Michael Charlotte',   N'MichaelCharlotte@FUNewsManagement.org', 1, N'@@abc123@@', N'0901111002',
 N'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=120&h=120&fit=crop&crop=face',
 N'Content writer specializing in technology and campus events.'),

(3,  N'Steve Paris',         N'SteveParis@FUNewsManagement.org',       1, N'@@abc123@@', N'0901111003',
 N'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=120&h=120&fit=crop&crop=face',
 N'Staff journalist covering student affairs and campus safety.'),

(4,  N'Emma William',        N'EmmaWilliam@FUNewsManagement.org',      2, N'@@abc123@@', N'0902222001',
 N'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=120&h=120&fit=crop&crop=face',
 N'Lecturer in Computer Science department. Research focus: AI and Machine Learning.'),

(5,  N'Olivia James',        N'OliviaJames@FUNewsManagement.org',      2, N'@@abc123@@', N'0902222002',
 N'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=120&h=120&fit=crop&crop=face',
 N'Lecturer in Business Administration. Passionate about entrepreneurship education.'),

(6,  N'James Anderson',      N'JamesAnderson@FUNewsManagement.org',    1, N'@@abc123@@', N'0901111004',
 N'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=120&h=120&fit=crop&crop=face',
 N'Sports and campus life news writer.'),

(7,  N'Sophia Martinez',     N'SophiaMartinez@FUNewsManagement.org',   2, N'@@abc123@@', N'0902222003',
 N'https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?w=120&h=120&fit=crop&crop=face',
 N'Associate professor in the Faculty of Information Technology.'),

(8,  N'Lucas Thompson',      N'LucasThompson@FUNewsManagement.org',    1, N'@@abc123@@', N'0901111005',
 N'https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=120&h=120&fit=crop&crop=face',
 N'Research and academic news reporter.'),

(9,  N'Mia Robinson',        N'MiaRobinson@FUNewsManagement.org',      2, N'@@abc123@@', N'0902222004',
 N'https://images.unsplash.com/photo-1520813792240-56fc4a3765a7?w=120&h=120&fit=crop&crop=face',
 N'Lecturer in Software Engineering. Enthusiast of open-source development.'),

(10, N'Ethan Walker',        N'EthanWalker@FUNewsManagement.org',      1, N'@@abc123@@', N'0901111006',
 N'https://images.unsplash.com/photo-1463453091185-61582044d556?w=120&h=120&fit=crop&crop=face',
 N'Chief editor overseeing all published articles at FU News.')
GO
SET IDENTITY_INSERT [dbo].[SystemAccount] OFF
GO

-- ============================================================
--  DATA: Category
-- ============================================================
SET IDENTITY_INSERT [dbo].[Category] ON
GO
INSERT INTO [dbo].[Category] ([CategoryID],[CategoryName],[CategoryDescription],[ParentCategoryID],[IsActive]) VALUES
(1, N'Academic News',     N'Research findings, faculty appointments, program updates and academic announcements.', NULL, 1),
(2, N'Student Affairs',   N'Student activities, clubs, organizations, sports and campus initiatives.',            NULL, 1),
(3, N'Campus Safety',     N'Incidents, safety measures and emergency protocols implemented on campus.',           NULL, 1),
(4, N'Alumni News',       N'Achievements, careers, reunions and contributions from FU graduates.',               NULL, 1),
(5, N'Capstone Projects', N'Highlights of final-year student capstone projects and presentations.',               1,    1),
(6, N'Research & Labs',   N'Lab innovations, published papers and research department spotlights.',               1,    1),
(7, N'Sports & Events',   N'Intercollegiate sports, competitions and campus cultural events.',                    2,    1),
(8, N'Scholarships',      N'Scholarship announcements, financial aid updates and student awards.',                2,    1),
(9, N'Cybersecurity',     N'Campus IT security news, alerts and best-practice guidelines.',                      3,    1),
(10,N'Career Center',     N'Job fairs, internship opportunities and career development resources.',               4,    0)
GO
SET IDENTITY_INSERT [dbo].[Category] OFF
GO

-- ============================================================
--  DATA: Tag
-- ============================================================
SET IDENTITY_INSERT [dbo].[Tag] ON
GO
INSERT INTO [dbo].[Tag] ([TagID],[TagName],[Note]) VALUES
(1,  N'Education',    N'Topics related to academic education and learning.'),
(2,  N'Technology',   N'Technology news, innovations and digital trends.'),
(3,  N'Research',     N'Academic research, lab findings and publications.'),
(4,  N'Innovation',   N'Creative solutions and breakthrough ideas.'),
(5,  N'Campus Life',  N'Day-to-day campus life, culture and community.'),
(6,  N'Faculty',      N'Faculty achievements, appointments and publications.'),
(7,  N'Alumni',       N'Alumni network, success stories and reunions.'),
(8,  N'Events',       N'Upcoming and past university events.'),
(9,  N'Resources',    N'Campus resources, libraries and support services.'),
(10, N'AI & ML',      N'Artificial Intelligence and Machine Learning related news.'),
(11, N'Sustainability',N'Green campus, eco-initiatives and sustainability programs.'),
(12, N'Internship',   N'Internship opportunities and industry partnerships.')
GO
SET IDENTITY_INSERT [dbo].[Tag] OFF
GO

-- ============================================================
--  DATA: NewsArticle
--  ImageUrl: Unsplash ảnh đề tài đại học, kích thước 800x450
--  ArticleStateID: 3 = Published (hiển thị public)
--  NewsStatus: 1 = Active
-- ============================================================
INSERT INTO [dbo].[NewsArticle]
    ([NewsArticleID],[NewsTitle],[Headline],[CreatedDate],[NewsContent],[NewsSource],[ImageUrl],[CategoryID],[NewsStatus],[ArticleStateID],[CreatedByID],[UpdatedByID],[ModifiedDate])
VALUES
(
  N'N000000001',
  N'FU Celebrates Outstanding Alumni Achievements in 2024',
  N'University FU honors graduates who have excelled in tech, arts and public service.',
  '2024-05-01 08:00:00',
  N'FU recently hosted its Annual Alumni Excellence Gala, recognizing over 50 graduates who have made significant contributions across various industries.

The event highlighted stories from software engineers now leading Fortune 500 teams, social entrepreneurs building schools in rural Vietnam, and artists whose works have been exhibited internationally.

"Our alumni are our greatest pride," said Rector Nguyen Minh Duc. "Their success is the clearest evidence of the quality of education we deliver."

The Alumni Association also announced a new scholarship fund in memory of Professor Tran Van An, available to underprivileged students starting next academic year.',
  N'FU News Office',
  N'https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=800&h=450&fit=crop',
  4, 1, 3, 1, 1, '2024-05-02 10:00:00'
),
(
  N'N000000002',
  N'Alumni Mentorship Program Launches for Class of 2024 Graduates',
  N'New program pairs fresh graduates with experienced alumni for career guidance.',
  '2024-05-03 09:00:00',
  N'The FU Alumni Association unveiled a structured mentorship program designed to support the Class of 2024 graduates as they transition into the workforce.

The program matches each mentee with an alumni mentor based on their field of study and career goals. Weekly check-ins, resume reviews, and mock interviews are among the key activities planned.

"I wish I had something like this when I graduated," said Nguyen Thi Lan, a 2015 alumna now working as a product manager at a leading tech company. "I will definitely give back."

Applications are open until June 30, with the program officially kicking off in July.',
  N'Alumni Affairs Office',
  N'https://images.unsplash.com/photo-1531482615713-2afd69097998?w=800&h=450&fit=crop',
  4, 1, 3, 1, NULL, NULL
),
(
  N'N000000003',
  N'Software Engineering Department Unveils New Curriculum for 2024–2025',
  N'Updated curriculum focuses on cloud computing, DevOps and AI-driven development.',
  '2024-05-05 08:30:00',
  N'The Software Engineering Department at FU has officially announced a major curriculum overhaul effective from the 2024–2025 academic year.

Key additions include:
- Cloud Computing with AWS and Azure
- DevOps pipelines and CI/CD practices
- AI-assisted software development
- Cybersecurity fundamentals for developers

Head of Department Dr. Pham Quoc Viet stated that the changes align with feedback from over 120 industry partners. "We want our students to be job-ready from day one," he said.

Final-year students will also benefit from a new industry project module where they collaborate directly with partner companies on real products.',
  N'N/A',
  N'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800&h=450&fit=crop',
  1, 1, 3, 2, 2, '2024-05-06 09:00:00'
),
(
  N'N000000004',
  N'Dr. David Nitzevet Appointed Head of AI Department at FU',
  N'Renowned AI researcher brings global expertise to lead FU AI faculty.',
  '2024-05-07 10:00:00',
  N'FU proudly announces the appointment of Dr. David Nitzevet, a distinguished researcher in deep learning, as the new Head of the Artificial Intelligence Department.

Dr. Nitzevet brings over 15 years of experience, including leading AI research teams at MIT and publishing more than 60 peer-reviewed papers. His specialties include reinforcement learning, neural architecture search, and AI ethics.

In his first address to students, he said: "AI is not just a tool — it is a new way of thinking. I want FU students to be pioneers, not just users."

The department plans to open a dedicated AI Research Lab in the new semester, equipped with GPU clusters for deep learning training.',
  N'FU Communications',
  N'https://images.unsplash.com/photo-1485827404703-89b55fcc595e?w=800&h=450&fit=crop',
  1, 1, 3, 2, 2, '2024-05-08 11:00:00'
),
(
  N'N000000005',
  N'Groundbreaking STEM Research Published by FU Faculty Team',
  N'FU researchers publish findings that could revolutionize materials engineering.',
  '2024-05-09 08:00:00',
  N'A multidisciplinary team from FU''s Science and Engineering faculties has published a landmark paper in the journal Nature Materials, presenting new findings on bio-inspired composite materials.

The research demonstrates a method to produce lightweight, ultra-strong materials by mimicking the structural properties of abalone shells. Potential applications span aerospace, automotive and medical devices.

Lead researcher Associate Professor Le Thi Hoa said the work was the result of five years of collaboration with labs in Japan and Germany. "Science is always global," she noted. "FU students who participated in this project have gained an experience that no classroom can replicate."

The paper has already been cited 40 times in the two weeks since publication.',
  N'Research Office',
  N'https://images.unsplash.com/photo-1532094349884-543bc11b234d?w=800&h=450&fit=crop',
  1, 1, 3, 2, NULL, NULL
),
(
  N'N000000006',
  N'FU Annual Sports Festival 2024 Kicks Off with Record Participation',
  N'Over 3,000 students join the largest campus sports festival in FU history.',
  '2024-05-11 07:00:00',
  N'The 2024 FU Annual Sports Festival officially opened on Saturday morning with a colorful opening ceremony attended by faculty members, staff and over 3,000 student participants — a record high.

Sports on offer this year include football, badminton, swimming, e-sports, chess, and a new addition: drone racing.

Student Union President Tran Minh Khoa said: "Sports are more than competition — they build character, teamwork and resilience. We are proud of the enthusiasm this year."

The festival runs for two weeks and culminates in an awards ceremony on May 25. Live streaming of major events is available on the FU YouTube channel.',
  N'Student Union',
  N'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800&h=450&fit=crop',
  2, 1, 3, 3, NULL, NULL
),
(
  N'N000000007',
  N'Campus Safety Committee Issues New Emergency Response Guidelines',
  N'Updated protocols cover fire, medical and cybersecurity emergencies.',
  '2024-05-13 09:00:00',
  N'The FU Campus Safety Committee has released a comprehensive update to its emergency response guidelines, effective immediately.

Major updates include:
- A new two-minute evacuation drill protocol for all academic buildings
- A dedicated campus safety app available on iOS and Android
- Enhanced lighting and CCTV coverage in parking areas and corridors
- A 24/7 hotline staffed by trained security personnel

Committee Chair Dr. Bui Van Nam stated: "Safety is our highest priority. These updates reflect lessons learned from drills and feedback from students and staff."

All students and employees are encouraged to download the new safety app and review the updated guidelines on the campus portal.',
  N'Campus Safety Office',
  N'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&h=450&fit=crop',
  3, 1, 3, 3, 3, '2024-05-14 08:00:00'
),
(
  N'N000000008',
  N'FU Students Win Gold at National Robotics Championship',
  N'Team CodeBlue brings home first-place trophy beating 45 universities nationwide.',
  '2024-05-15 10:00:00',
  N'Team CodeBlue from FU has clinched the gold medal at the National Robotics Championship held in Hanoi, competing against teams from 45 universities across the country.

The team''s robot, "Phoenix V3", completed all four challenge stages in record time and demonstrated superior precision in the sorting and obstacle-navigation tasks.

Team captain Nguyen Van Hai, a third-year AI student, said: "We spent six months in the lab on this. Winning feels surreal but also very validating."

Faculty advisor Dr. Tran Duc Manh congratulated the team: "This win is a testament to FU''s investment in engineering education and student innovation culture."

The team will represent Vietnam at the ASEAN Robotics Competition in Singapore this August.',
  N'FU Engineering Faculty',
  N'https://images.unsplash.com/photo-1561144257-e32e8506541b?w=800&h=450&fit=crop',
  2, 1, 3, 8, NULL, NULL
),
(
  N'N000000009',
  N'New Scholarship Fund Opens for Low-Income IT Students',
  N'FU launches VieTech Scholarship supporting 50 students per year in IT fields.',
  '2024-05-17 08:00:00',
  N'FU''s Financial Aid Office, in partnership with VieTech Corporation, has launched the VieTech Scholarship Fund, offering full tuition coverage to 50 low-income students annually in Information Technology and Software Engineering programs.

Eligibility criteria include a GPA of 3.2 or higher, demonstrated financial need, and a written essay on how technology can improve lives in Vietnam.

"We believe talent should never be limited by financial circumstances," said Ms. Hoang Thi Mai, VieTech''s CSR Director.

Applications open June 1 and close July 31. Awardees will also receive internship opportunities at VieTech upon graduation.',
  N'Financial Aid Office',
  N'https://images.unsplash.com/photo-1434030216411-0b793f4b4173?w=800&h=450&fit=crop',
  2, 1, 3, 1, NULL, NULL
),
(
  N'N000000010',
  N'FU Research Lab Partners with Google for Cloud AI Projects',
  N'Partnership enables students and faculty to access Google Cloud credits and mentorship.',
  '2024-05-19 09:00:00',
  N'FU''s AI Research Lab has signed a formal partnership agreement with Google Vietnam, providing faculty and students with access to Google Cloud Platform credits, exclusive training, and mentorship from Google engineers.

Under the agreement, three FU research projects per semester will be shortlisted for potential co-funding and publication support.

Dean of the Faculty of IT, Professor Nguyen Huu Long, stated: "This partnership places FU among a select group of Asian universities collaborating directly with Google. It opens extraordinary doors for our researchers."

An inaugural joint workshop on Large Language Models is scheduled for June, open to all registered FU students and faculty.',
  N'FU IT Faculty',
  N'https://images.unsplash.com/photo-1573804633927-bfcbcd909acd?w=800&h=450&fit=crop',
  6, 1, 3, 10, 10, '2024-05-20 10:00:00'
),
(
  N'N000000011',
  N'Cybersecurity Awareness Week: What Every Student Should Know',
  N'FU''s IT Security team shares essential tips to protect your digital identity.',
  '2024-05-21 08:00:00',
  N'As part of Cybersecurity Awareness Week, FU''s Information Security team hosted a series of workshops, demonstrations and a capture-the-flag competition drawing over 500 participants.

Key tips shared during the event:
1. Use unique, strong passwords and a password manager
2. Enable two-factor authentication on all academic accounts
3. Never connect to unverified public Wi-Fi without a VPN
4. Be cautious of phishing emails — always verify sender addresses
5. Report suspicious activity to security@fu.edu.vn immediately

A new phishing simulation campaign will test staff and student awareness monthly. Results will be shared anonymously to help the community learn.',
  N'IT Security Team',
  N'https://images.unsplash.com/photo-1550751827-4bd374c3f58b?w=800&h=450&fit=crop',
  9, 1, 3, 3, NULL, NULL
),
(
  N'N000000012',
  N'Career Fair 2024: 80 Companies, 2,000 Job Openings for FU Students',
  N'Biggest career fair in FU history connects students with top employers.',
  '2024-05-23 09:00:00',
  N'FU Career Center hosted the largest Career Fair in the university''s history on May 22, with 80 companies from sectors including technology, finance, healthcare and manufacturing.

Over 2,000 job and internship positions were available, and more than 1,500 students attended, submitting resumes and sitting for on-the-spot interviews.

"We saw extremely high quality from FU students this year," said Mr. Pham Anh Tuan, HR Director at TechVision Vietnam. "We extended offers to 12 candidates on the spot."

The Career Center has also launched an online portal where students can browse all participating company profiles and apply directly to remaining vacancies until June 15.',
  N'Career Center',
  N'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=800&h=450&fit=crop',
  10, 0, 4, 6, 6, '2024-05-24 09:00:00'  -- Inactive, Archived (demo)
)
GO

-- ============================================================
--  DATA: NewsTag  (phân loại tags cho từng bài)
-- ============================================================
INSERT INTO [dbo].[NewsTag] ([NewsArticleID],[TagID]) VALUES
-- N000000001 Alumni Achievements
(N'N000000001', 7), (N'N000000001', 5), (N'N000000001', 8),
-- N000000002 Mentorship
(N'N000000002', 7), (N'N000000002', 1), (N'N000000002', 9),
-- N000000003 Curriculum
(N'N000000003', 1), (N'N000000003', 2), (N'N000000003', 4),
-- N000000004 AI Department Head
(N'N000000004', 6), (N'N000000004', 10), (N'N000000004', 3),
-- N000000005 STEM Research
(N'N000000005', 3), (N'N000000005', 4), (N'N000000005', 6),
-- N000000006 Sports Festival
(N'N000000006', 5), (N'N000000006', 8),
-- N000000007 Campus Safety
(N'N000000007', 9), (N'N000000007', 5),
-- N000000008 Robotics Win
(N'N000000008', 4), (N'N000000008', 2), (N'N000000008', 8),
-- N000000009 Scholarship
(N'N000000009', 1), (N'N000000009', 9), (N'N000000009', 12),
-- N000000010 Google Partnership
(N'N000000010', 10), (N'N000000010', 2), (N'N000000010', 3), (N'N000000010', 4),
-- N000000011 Cybersecurity
(N'N000000011', 2), (N'N000000011', 9),
-- N000000012 Career Fair
(N'N000000012', 12), (N'N000000012', 8), (N'N000000012', 5)
GO

-- ============================================================
--  DATA: AuditLog  (lịch sử hành động mẫu)
-- ============================================================
INSERT INTO [dbo].[AuditLog] ([ActorEmail],[ActorRole],[Action],[EntityName],[EntityKey],[Description],[CreatedAt]) VALUES
(N'IsabellaDavid@FUNewsManagement.org', N'Staff', N'Create', N'NewsArticle', N'N000000001', N'Created: FU Celebrates Outstanding Alumni Achievements in 2024', '2024-05-01 08:05:00'),
(N'IsabellaDavid@FUNewsManagement.org', N'Staff', N'Create', N'NewsArticle', N'N000000002', N'Created: Alumni Mentorship Program Launches for Class of 2024', '2024-05-03 09:05:00'),
(N'OliviaJames@FUNewsManagement.org',   N'Staff', N'Create', N'NewsArticle', N'N000000003', N'Created: Software Engineering Department Unveils New Curriculum', '2024-05-05 08:35:00'),
(N'OliviaJames@FUNewsManagement.org',   N'Staff', N'Update', N'NewsArticle', N'N000000003', N'Updated article content and headline', '2024-05-06 09:05:00'),
(N'OliviaJames@FUNewsManagement.org',   N'Staff', N'Create', N'NewsArticle', N'N000000004', N'Created: Dr. David Nitzevet Appointed Head of AI Department', '2024-05-07 10:05:00'),
(N'SteveParis@FUNewsManagement.org',    N'Staff', N'Create', N'NewsArticle', N'N000000006', N'Created: FU Annual Sports Festival 2024 Kicks Off', '2024-05-11 07:05:00'),
(N'SteveParis@FUNewsManagement.org',    N'Staff', N'Create', N'NewsArticle', N'N000000007', N'Created: Campus Safety Committee Issues New Emergency Response Guidelines', '2024-05-13 09:05:00'),
(N'admin@FUNewsManagementSystem.org',   N'Admin', N'Delete', N'SystemAccount', N'11', N'Removed test account', '2024-05-14 15:00:00'),
(N'EthanWalker@FUNewsManagement.org',   N'Staff', N'Create', N'NewsArticle', N'N000000010', N'Created: FU Research Lab Partners with Google for Cloud AI Projects', '2024-05-19 09:05:00'),
(N'EthanWalker@FUNewsManagement.org',   N'Staff', N'Update', N'NewsArticle', N'N000000010', N'Added image URL and updated content', '2024-05-20 10:05:00')
GO

-- ============================================================
--  DATA: ApprovalHistory
-- ============================================================
INSERT INTO [dbo].[ApprovalHistory] ([NewsArticleID],[AccountID],[Action],[Note],[Timestamp]) VALUES
(N'N000000001', 10, N'Publish',  N'Content approved. Good quality article.', '2024-05-01 10:00:00'),
(N'N000000002', 10, N'Publish',  N'Approved after minor edits.', '2024-05-03 11:00:00'),
(N'N000000003', 10, N'Review',   N'Sent back for additional technical details.', '2024-05-05 12:00:00'),
(N'N000000003', 10, N'Publish',  N'Approved after revision.', '2024-05-06 10:00:00'),
(N'N000000004', 10, N'Publish',  N'Approved. Excellent profile article.', '2024-05-07 11:30:00'),
(N'N000000005', 10, N'Publish',  N'Research article approved by editorial board.', '2024-05-09 09:30:00'),
(N'N000000006', 10, N'Publish',  N'Event coverage approved.', '2024-05-11 08:00:00'),
(N'N000000007', 10, N'Publish',  N'Safety guideline article approved as urgent.', '2024-05-13 10:00:00'),
(N'N000000008', 10, N'Publish',  N'Competition result confirmed by Sports Office.', '2024-05-15 11:00:00'),
(N'N000000012', 10, N'Archive',  N'Career Fair ended. Archiving post.', '2024-05-24 10:00:00')
GO

-- ============================================================
--  DATA: ArticleComment
-- ============================================================
INSERT INTO [dbo].[ArticleComment] ([NewsArticleID],[AccountID],[DisplayName],[Content],[ParentCommentID],[CreatedAt],[IsDeleted]) VALUES
-- Comments on N000000001
(N'N000000001', 4, N'Emma William',   N'Wonderful to see FU alumni making such an impact globally!', NULL, '2024-05-01 12:00:00', 0),
(N'N000000001', 5, N'Olivia James',   N'Proud to be part of this institution. Great article!', NULL, '2024-05-01 13:00:00', 0),
(N'N000000001', NULL, N'Anonymous Reader', N'This is really inspiring. Hope to be featured someday!', NULL, '2024-05-01 14:00:00', 0),
-- Comments on N000000003
(N'N000000003', 4, N'Emma William',   N'The cloud computing addition is long overdue. Great move!', NULL, '2024-05-05 10:00:00', 0),
(N'N000000003', 9, N'Mia Robinson',   N'Looking forward to the new DevOps curriculum!', NULL, '2024-05-05 11:00:00', 0),
-- Comments on N000000004
(N'N000000004', 7, N'Sophia Martinez',N'Dr. Nitzevet is a fantastic choice. His papers are groundbreaking.', NULL, '2024-05-07 12:00:00', 0),
(N'N000000004', 4, N'Emma William',   N'AI at FU will reach new heights with this appointment!', NULL, '2024-05-07 13:00:00', 0),
-- Comments on N000000008
(N'N000000008', 5, N'Olivia James',   N'Congratulations Team CodeBlue! You made FU proud!', NULL, '2024-05-15 11:00:00', 0),
(N'N000000008', NULL,N'Anonymous',    N'Amazing achievement! Rooting for you at the ASEAN comp!', NULL, '2024-05-15 12:00:00', 0),
-- Comments on N000000010
(N'N000000010', 7, N'Sophia Martinez',N'A Google partnership is a huge milestone. Congratulations FU!', NULL, '2024-05-19 10:00:00', 0),
(N'N000000010', 9, N'Mia Robinson',   N'This opens amazing research doors for students like mine.', NULL, '2024-05-19 11:00:00', 0),
(N'N000000010', 4, N'Emma William',   N'Can''t wait for the LLM workshop in June!', 10, '2024-05-19 12:00:00', 0)
GO

-- ============================================================
--  DATA: ArticleLike
-- ============================================================
INSERT INTO [dbo].[ArticleLike] ([NewsArticleID],[AccountID],[CreatedAt]) VALUES
(N'N000000001', 4, '2024-05-01 12:30:00'),
(N'N000000001', 5, '2024-05-01 13:30:00'),
(N'N000000001', 7, '2024-05-01 14:30:00'),
(N'N000000003', 4, '2024-05-05 10:30:00'),
(N'N000000003', 9, '2024-05-05 11:30:00'),
(N'N000000004', 7, '2024-05-07 12:30:00'),
(N'N000000004', 4, '2024-05-07 13:30:00'),
(N'N000000005', 7, '2024-05-09 09:30:00'),
(N'N000000008', 5, '2024-05-15 11:30:00'),
(N'N000000010', 7, '2024-05-19 10:30:00'),
(N'N000000010', 9, '2024-05-19 11:30:00'),
(N'N000000010', 4, '2024-05-19 12:30:00')
GO

-- ============================================================
--  DATA: ArticleBookmark
-- ============================================================
INSERT INTO [dbo].[ArticleBookmark] ([NewsArticleID],[AccountID],[CreatedAt]) VALUES
(N'N000000001', 4, '2024-05-01 15:00:00'),
(N'N000000003', 9, '2024-05-05 12:00:00'),
(N'N000000004', 7, '2024-05-07 14:00:00'),
(N'N000000005', 7, '2024-05-09 10:00:00'),
(N'N000000005', 9, '2024-05-09 11:00:00'),
(N'N000000008', 4, '2024-05-15 13:00:00'),
(N'N000000009', 9, '2024-05-17 09:00:00'),
(N'N000000010', 7, '2024-05-19 13:00:00'),
(N'N000000010', 4, '2024-05-19 14:00:00'),
(N'N000000011', 9, '2024-05-21 09:00:00')
GO

-- ============================================================
--  DATA: Notification
-- ============================================================
INSERT INTO [dbo].[Notification] ([RecipientAccountID],[NewsArticleID],[Type],[ActorAccountID],[ActorName],[Excerpt],[IsRead],[CreatedAt]) VALUES
-- Notify author of N000000001 (AccountID=1) about likes/comments
(1, N'N000000001', N'Like',    4, N'Emma William',    NULL,                              0, '2024-05-01 12:30:00'),
(1, N'N000000001', N'Like',    5, N'Olivia James',    NULL,                              0, '2024-05-01 13:30:00'),
(1, N'N000000001', N'Comment', 4, N'Emma William',    N'Wonderful to see FU alumni...',  0, '2024-05-01 12:00:00'),
-- Notify author of N000000003 (AccountID=2) about likes/comments
(2, N'N000000003', N'Like',    4, N'Emma William',    NULL,                              1, '2024-05-05 10:30:00'),
(2, N'N000000003', N'Comment', 4, N'Emma William',    N'The cloud computing addition...', 1, '2024-05-05 10:00:00'),
-- Notify author of N000000004 (AccountID=2)
(2, N'N000000004', N'Like',    7, N'Sophia Martinez', NULL,                              0, '2024-05-07 12:30:00'),
(2, N'N000000004', N'Comment', 7, N'Sophia Martinez', N'Dr. Nitzevet is a fantastic...', 0, '2024-05-07 12:00:00'),
-- Notify author of N000000008 (AccountID=8)
(8, N'N000000008', N'Like',    5, N'Olivia James',    NULL,                              0, '2024-05-15 11:30:00'),
(8, N'N000000008', N'Comment', 5, N'Olivia James',    N'Congratulations Team CodeBlue!', 0, '2024-05-15 11:00:00'),
-- Notify author of N000000010 (AccountID=10)
(10,N'N000000010', N'Like',    7, N'Sophia Martinez', NULL,                              1, '2024-05-19 10:30:00'),
(10,N'N000000010', N'Bookmark',4, N'Emma William',    NULL,                              0, '2024-05-19 14:00:00')
GO

-- ============================================================
--  VERIFY
-- ============================================================
SELECT 'SystemAccount'  AS TableName, COUNT(*) AS Records FROM [dbo].[SystemAccount]
UNION ALL
SELECT 'Category',       COUNT(*) FROM [dbo].[Category]
UNION ALL
SELECT 'ArticleState',   COUNT(*) FROM [dbo].[ArticleState]
UNION ALL
SELECT 'Tag',            COUNT(*) FROM [dbo].[Tag]
UNION ALL
SELECT 'NewsArticle',    COUNT(*) FROM [dbo].[NewsArticle]
UNION ALL
SELECT 'NewsTag',        COUNT(*) FROM [dbo].[NewsTag]
UNION ALL
SELECT 'AuditLog',       COUNT(*) FROM [dbo].[AuditLog]
UNION ALL
SELECT 'ApprovalHistory',COUNT(*) FROM [dbo].[ApprovalHistory]
UNION ALL
SELECT 'ArticleComment', COUNT(*) FROM [dbo].[ArticleComment]
UNION ALL
SELECT 'ArticleLike',    COUNT(*) FROM [dbo].[ArticleLike]
UNION ALL
SELECT 'ArticleBookmark',COUNT(*) FROM [dbo].[ArticleBookmark]
UNION ALL
SELECT 'Notification',   COUNT(*) FROM [dbo].[Notification]
ORDER BY TableName
GO

PRINT '✅ FUNewsManagement database setup completed successfully!'
GO
