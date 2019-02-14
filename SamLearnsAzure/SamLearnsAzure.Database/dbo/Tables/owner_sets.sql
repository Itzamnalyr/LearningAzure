CREATE TABLE [dbo].[owner_sets]
(
	[set_num] VARCHAR(100) NOT NULL , 
	[owner_id] INT NOT NULL,
    [owned] BIT NOT NULL, 
    [wanted] BIT NOT NULL, 
    [owner_set_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    CONSTRAINT [FK_owner_sets_owner] FOREIGN KEY ([owner_id]) REFERENCES [owners]([id]), 
    CONSTRAINT [FK_owner_sets_sets] FOREIGN KEY ([set_num]) REFERENCES [sets]([set_num]) 
)