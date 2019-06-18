CREATE TABLE [dbo].[set_images]
(
	[set_image_id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[set_num] VARCHAR(100) NOT NULL,
	[set_image] VARCHAR(500) NULL, 
    CONSTRAINT [FK_set_images_sets] FOREIGN KEY ([set_num]) REFERENCES [sets]([set_num]) 
)
