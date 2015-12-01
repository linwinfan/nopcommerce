-- Script Date: 2015/11/11 22:28  - ErikEJ.SqlCeScripting version 3.5.2.56
-- Adding as column with NOT NULL is not allowed, set a default value or allow NULL
--ALTER TABLE [Affiliate] ADD [Level] int ;
ALTER TABLE [Affiliate] ADD [ParentAffiliateId] int;
GO
