CREATE TABLE [dbo].[lego_owner_sets]
(
	[set_num] VARCHAR(100) NOT NULL , 
	[owner_id] INT NOT NULL,
    [owned] BIT NOT NULL, 
    [wanted] BIT NOT NULL, 
    [owner_set_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    CONSTRAINT [FK_lego_owner_sets_lego_owner] FOREIGN KEY ([owner_id]) REFERENCES [lego_owners]([id]), 
    CONSTRAINT [FK_lego_owner_sets_lego_sets] FOREIGN KEY ([set_num]) REFERENCES [lego_sets]([set_num]) 
)